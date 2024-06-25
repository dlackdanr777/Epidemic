using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIStatus : MonoBehaviour
{
    [SerializeField] private Image _amountImage;
    private float _maxAmount;

    public void Init(float maxAmount)
    {
        _maxAmount = maxAmount;
    }

    public void AmountChanged(float amount)
    {
        float amountPercentage = amount / _maxAmount;
        if (amountPercentage < 0.0001f)
            amountPercentage = 0;
        else if(amountPercentage > 1)
            amountPercentage = 1;

        _amountImage.fillAmount = amountPercentage;
    }
}
