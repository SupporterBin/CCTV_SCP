using DG.Tweening;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyPad : MonoBehaviour
{
    private int currentKeyCode = 0;
    [SerializeField, Header("글자를 표시할 텍스트")]
    private TextMeshProUGUI currentKeyCodeText;

    [SerializeField, Header("상태를 표시할 이미지")]
    private Image lodingPad_Image;

    [SerializeField]
    private GameObject loding_Obj;
    [SerializeField]
    private GameObject successTextObj;
    [SerializeField]
    private GameObject failTextObj;

    private bool isEnter = false;
    private bool isSuccess = false;

    private void Update()
    {
        //if (currentKeyCode == 0) currentKeyCodeText.text = "_ _ _ _";

        // 1. 현재 코드를 문자열로 변환합니다.
        //    (currentKeyCode가 0이면 빈 문자열로 처리)
        string codeString = (currentKeyCode == 0) ? "" : currentKeyCode.ToString();

        // 2. 문자열   의 오른쪽에 "_" 문자를 채워 총 길이를 4로 만듭니다.
        //    예: "12" -> "12__"
        string paddedCode = codeString.PadRight(4, '_');

        // 3. 각 문자 사이에 공백을 추가합니다.
        //    예: "12__" -> "1 2 _ _ "
        StringBuilder displayText = new StringBuilder();
        foreach (char c in paddedCode)
        {
            displayText.Append(c).Append(' ');
        }

        // 4. 맨 마지막의 불필요한 공백을 제거하고 텍스트에 적용합니다.
        //    예: "1 2 _ _ " -> "1 2 _ _"
        currentKeyCodeText.text = displayText.ToString().TrimEnd();



        if(lodingPad_Image)
        {
            lodingPad_Image.GetComponent<RectTransform>().Rotate(0, 0, -60 * Time.deltaTime, Space.Self);
        }
    }

    /// <summary>
    /// 키패드 입력을 받아 숫자를 누적합니다. (최대 4자리)
    /// </summary>
    /// <param name="num">입력된 한 자리 숫자 (0-9)</param>
    public void InputKey(int num)
    {
        // 1. 입력된 숫자가 한 자리(0-9)인지 확인합니다.
        if (num < 0 || num > 9)
        {
            return; // 한 자리 숫자가 아니면 무시
        }
        if (currentKeyCode < 1000)
        {
            // 3. 기존 숫자에 10을 곱해 자릿수를 올리고 새 숫자를 더합니다.
            // 예: (12 * 10) + 5  =>  120 + 5 = 125
            currentKeyCode = (currentKeyCode * 10) + num;
        }
        // Debug.Log("현재 코드: " + currentKeyCode);
    }

    /// <summary>
    /// 현재까지 입력된 코드를 0으로 초기화합니다.
    /// </summary>
    public void ClearKey()
    {
        currentKeyCode = 0;
    }

    public IEnumerator Enter()
    {
        if (isEnter) yield break;

        isEnter = true;

        successTextObj.SetActive(false);
        failTextObj.SetActive(false);

        loding_Obj.SetActive(true);

        SoundManager.Instance.PlayGlobalSFX(SoundManager.Instance.Data.protocolLoadingLoop);

        yield return new WaitForSecondsRealtime(2.0f);

        loding_Obj.SetActive(false);

        if (GameManager.Instance.protocolNum == currentKeyCode)
        {
            TrueEnter();
        }
        else
        {
            FalseEnter();
        }
    }

    private void TrueEnter()
    {
        SoundManager.Instance.PlayGlobalSFX(SoundManager.Instance.Data.protocolSuccess);
        GameManager.Instance.anomalySystem.specialObjects[1].GetComponent<Animator>().Play("Off");

        for (int i = 0; i < 3; i++)
        {
            StabilityManager.Instance.StabilizationUp(-StabilityManager.Instance.CurrentStability[i] + 15f, i);
        }

        isSuccess = true;
        GameManager.Instance.isDeadWarring = false;
        successTextObj.SetActive(true);
        Debug.Log("맞음");
    }

    private void FalseEnter()
    {
        SoundManager.Instance.PlayGlobalSFX(SoundManager.Instance.Data.protocolFail);
        ClearKey();
        failTextObj.SetActive(true);
        isEnter = false;
        Debug.Log("틀림");
    }

    public bool GetSucess()
    {
        return isSuccess;
    }
}
