using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Data")]
    private SoundData _soundData;
    public SoundData Data => _soundData;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _bgmSource;       // BGM용
    [SerializeField] private AudioSource _globalSfxSource; // 일반적인 짧은 2D 효과음용 (UI 등)

    [Header("Volume Settings (0.0 ~ 1.0)")]
    [Range(0f, 1f)] public static float MasterVolume = 1.0f;
    [Range(0f, 1f)] public static float BgmVolume = 1.0f;
    [Range(0f, 1f)] public static float SfxVolume = 1.0f;

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
    // 볼륨 조절
    // ====================================================
    public void SetMasterVolume(float volume)
    {
        MasterVolume = Mathf.Clamp01(volume);
        UpdateBgmVolume();
    }

    public void SetBgmVolume(float volume)
    {
        BgmVolume = Mathf.Clamp01(volume);
        UpdateBgmVolume();
    }

    public void SetSfxVolume(float volume)
    {
        SfxVolume = Mathf.Clamp01(volume);
    }

    private void UpdateBgmVolume()
    {
        if (_bgmSource != null)
        {
            _bgmSource.volume = MasterVolume * BgmVolume;
        }
    }

    // ====================================================
    // BGM 관리
    // ====================================================
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        if (_bgmSource.clip == clip && _bgmSource.isPlaying) return;

        _bgmSource.clip = clip;
        _bgmSource.volume = MasterVolume * BgmVolume;
        _bgmSource.Play();
    }

    public void StopBGM()
    {
        _bgmSource.Stop();
    }

    // ====================================================
    // 1. 일반 2D 효과음 (멈출 필요 없는 짧은 소리)
    // ====================================================
    public void PlayGlobalSFX(AudioClip clip)
    {
        if (clip == null) return;
        float finalVolume = MasterVolume * SfxVolume;
        _globalSfxSource.PlayOneShot(clip, finalVolume);
    }

    /// <summary>
    /// [요청 1] 현재 재생 중인 모든 2D 효과음(UI 등)을 즉시 멈춥니다.
    /// </summary>
    public void StopAllGlobalSFX()
    {
        _globalSfxSource.Stop();
    }

    // ====================================================
    // 2. 제어 가능한 2D 효과음 (특정 소리 멈추기 가능)
    // 용도: 알람 소리, 심장 박동 등 멈춰야 하는 2D 소리
    // ====================================================
    /// <summary>
    /// [요청 3] 중간에 멈춰야 하는 2D 소리를 재생하고 AudioSource를 반환합니다.
    /// </summary>
    public AudioSource PlayStoppable2DSFX(AudioClip clip, bool loop = false)
    {
        if (clip == null) return null;

        GameObject audioObj = new GameObject("Temp_2D_Stoppable");
        audioObj.transform.SetParent(this.transform); // 매니저 자식으로 정리

        AudioSource audioSource = audioObj.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = MasterVolume * SfxVolume;
        audioSource.spatialBlend = 0f; // 2D 사운드
        audioSource.loop = loop;

        audioSource.Play();

        // 반복 재생이 아닐 경우에만 소리 끝나면 자동 삭제
        if (!loop)
        {
            Destroy(audioObj, clip.length);
        }

        return audioSource; // 제어권 반환
    }

    // ====================================================
    // 3. 3D 효과음 (특정 소리 멈추기 가능)
    // ====================================================
    /// <summary>
    /// [요청 2] 3D 소리를 재생하고 제어할 수 있는 AudioSource를 반환합니다.
    /// </summary>
    public AudioSource Play3DSFX(AudioClip clip, Vector3 position, float maxDistance = 20.0f, bool loop = false)
    {
        if (clip == null) return null;

        GameObject audioObj = new GameObject("Temp_3DSFX");
        audioObj.transform.position = position;

        AudioSource audioSource = audioObj.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = MasterVolume * SfxVolume;
        audioSource.spatialBlend = 1.0f; // 3D
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.minDistance = 1.0f;
        audioSource.maxDistance = maxDistance;
        audioSource.loop = loop;

        audioSource.Play();

        // 반복 재생이 아닐 경우에만 자동 삭제
        if (!loop)
        {
            Destroy(audioObj, clip.length);
        }

        return audioSource; // 제어권 반환
    }

    // ====================================================
    // 공통: 특정 소리 멈추기 도우미 함수
    // ====================================================
    /// <summary>
    /// 반환받았던 AudioSource를 넣어주면 소리를 끄고 삭제합니다.
    /// </summary>
    public void StopSFX(AudioSource source)
    {
        if (source != null)
        {
            source.Stop();
            Destroy(source.gameObject);
        }
    }
}