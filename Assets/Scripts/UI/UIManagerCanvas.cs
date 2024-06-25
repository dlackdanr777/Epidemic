using System.Collections;
using TMPro;
using UnityEngine;

public class UIManagerCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _centerText;
    [SerializeField] private TextMeshProUGUI _aimRightText;
    private Coroutine ShowCenterTextRoutine;

    public void Start()
    {
        _centerText.alpha = 0;
        _aimRightText.alpha = 0;
    }

    public void ShowRightText(string textContent)
    {
        _aimRightText.alpha = 1;
        _aimRightText.text = textContent;
    }

    public void HiddenRightText()
    {
        _aimRightText.alpha = 0;
    }


    public void ShowCenterText(string textContent)
    {
        if (ShowCenterTextRoutine != null)
            StopCoroutine(ShowCenterTextRoutine);

        ShowCenterTextRoutine = StartCoroutine(ShowText(_centerText, textContent));
    }

    private IEnumerator ShowText(TextMeshProUGUI tmpText, string textContent)
    {
        tmpText.alpha = 1.0f;
        tmpText.text = textContent;

        yield return YieldCache.WaitForSeconds(2);

        while (tmpText.alpha > 0)
        {
            tmpText.alpha -= 0.02f;
            yield return YieldCache.WaitForSeconds(0.02f);
        }
    }
}
