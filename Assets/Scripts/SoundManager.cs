using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Data")]
    private SoundData _soundData;
    public SoundData Data => _soundData;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _bgmSource;       // BGM용
    [SerializeField] private AudioSource _globalSfxSource; // 2D 효과음용

    [Header("Volume Settings (0.0 ~ 1.0)")]
    [Range(0f, 1f)] public static float MasterVolume = 1.0f; // 전체 볼륨
    [Range(0f, 1f)] public static float BgmVolume = 1.0f;    // 배경음악 볼륨
    [Range(0f, 1f)] public static float SfxVolume = 1.0f;    // 효과음 볼륨 (2D + 3D 공통)

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        _soundData = Resources.Load<SoundData>("Sound/SoundData");

        // 소스 생성 로직은 동일
        if (_bgmSource == null)
        {
            GameObject bgmObj = new GameObject("Channel_BGM");
            bgmObj.transform.SetParent(this.transform);
            _bgmSource = bgmObj.AddComponent<AudioSource>();
            _bgmSource.loop = true;
            _bgmSource.spatialBlend = 0f;
        }

        if (_globalSfxSource == null)
        {
            GameObject globalObj = new GameObject("Channel_GlobalSFX");
            globalObj.transform.SetParent(this.transform);
            _globalSfxSource = globalObj.AddComponent<AudioSource>();
            _globalSfxSource.spatialBlend = 0f;
        }
    }

    // ====================================================
    // 볼륨 조절 메서드 (UI 슬라이더 등에서 호출)
    // ====================================================

    /// <summary>
    /// 전체 볼륨 조절 후 BGM에 즉시 반영
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        MasterVolume = Mathf.Clamp01(volume);
        UpdateBgmVolume(); // 마스터 볼륨이 바뀌면 현재 재생 중인 BGM 크기도 바껴야 함
    }

    /// <summary>
    /// BGM 볼륨 조절 후 즉시 반영
    /// </summary>
    public void SetBgmVolume(float volume)
    {
        BgmVolume = Mathf.Clamp01(volume);
        UpdateBgmVolume();
    }

    /// <summary>
    /// SFX 볼륨 조절 (재생될 소리부터 적용됨)
    /// </summary>
    public void SetSfxVolume(float volume)
    {
        SfxVolume = Mathf.Clamp01(volume);
    }

    // 현재 재생 중인 BGM 소스에 실시간으로 볼륨 적용하는 내부 함수
    private void UpdateBgmVolume()
    {
        if (_bgmSource != null)
        {
            _bgmSource.volume = MasterVolume * BgmVolume;
        }
    }

    // ====================================================
    // 재생 기능 (볼륨 계산 적용)
    // ====================================================

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        if (_bgmSource.clip == clip && _bgmSource.isPlaying) return;

        _bgmSource.clip = clip;
        // 최종 볼륨 = 마스터 * BGM설정
        _bgmSource.volume = MasterVolume * BgmVolume;
        _bgmSource.Play();
    }

    public void StopBGM()
    {
        _bgmSource.Stop();
    }

    public void PlayGlobalSFX(AudioClip clip)
    {
        if (clip == null) return;

        // 최종 볼륨 = 마스터 * SFX설정
        float finalVolume = MasterVolume * SfxVolume;
        _globalSfxSource.PlayOneShot(clip, finalVolume);
    }

    public void Play3DSFX(AudioClip clip, Vector3 position, float maxDistance = 20.0f)
    {
        if (clip == null) return;

        GameObject audioObj = new GameObject("Temp_3DSFX");
        audioObj.transform.position = position;

        AudioSource audioSource = audioObj.AddComponent<AudioSource>();
        audioSource.clip = clip;

        // 최종 볼륨 = 마스터 * SFX설정
        audioSource.volume = MasterVolume * SfxVolume;

        audioSource.spatialBlend = 1.0f;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.minDistance = 1.0f;
        audioSource.maxDistance = maxDistance;

        audioSource.Play();
        Destroy(audioObj, clip.length);
    }
}