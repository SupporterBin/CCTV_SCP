using System.Collections;
using UnityEngine;

public enum KeyOption
{
    None,
    Delete,
    Call
}

public class KeyWord : MonoBehaviour
{
    private KeyPad keyPad;

    [Header("특수 키로 사용할꺼야?")]
    public bool isKeyType;
    [Header("이 키가 어떤 숫자 담당? (일의 자리)")]
    public int keyValue;
    [Header("어떤 특수 역할 담당?")]
    public KeyOption keyOption;

    private bool isClicking;

    private Renderer myRenderer;
    private Material mat; // 머티리얼을 저장할 변수
    private Color originalEmissionColor; // 원래 색상을 저장할 변수

    [Header("누를 때 색깔 달라지는 설정")]
    public Color blinkColor = Color.white; // 켜질 때의 기본 색상
    public float blinkIntensity = -10;    // 켜질 때의 강도

    private void Awake()
    {
        keyPad = gameObject.GetComponentInParent<KeyPad>();

        // 1. Mesh Renderer나 Skinned Mesh Renderer를 가져옵니다.
        myRenderer = GetComponent<Renderer>();

        // 2. 렌더러에서 실제 머티리얼 인스턴스를 가져옵니다.
        mat = myRenderer.material;

        // 3. 원래의 Emission 색상을 저장해 둡니다. (꺼진 상태는 보통 Color.black)
        originalEmissionColor = mat.GetColor("_EmissionColor");
    }

    private void KeyOptionGet()
    {
        switch(keyOption)
        {
            case KeyOption.None:
                Debug.Log("에러 설정 잘못됨");
                break;
            case KeyOption.Delete:
                keyPad.ClearKey();
                break;
            case KeyOption.Call:
                break;
        }
    }

    private IEnumerator KeyWordClick()
    {
        isClicking = true;

        if (isKeyType) KeyOptionGet();
        else keyPad.InputKey(keyValue);

        // 1. 머티리얼의 Emission을 활성화하고 원하는 강도로 설정
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", blinkColor * blinkIntensity);

        Debug.Log("누름");
        yield return new WaitForSeconds(0.3f);

        // 3. 0.3초가 지나면, 원래 저장해둔 Emission 색상으로 되돌립니다.
        mat.SetColor("_EmissionColor", originalEmissionColor);

        // 4. 만약 원래 색상이 검은색(꺼진 상태)이었다면 키워드도 꺼줍니다.
        if (originalEmissionColor == Color.black)
        {
            mat.DisableKeyword("_EMISSION");
        }

        Debug.Log("딜레이 종료");
        isClicking = false;
    }

    private void OnMouseDown()
    {
        if (isClicking) return;

        StartCoroutine(KeyWordClick());
    }
}
