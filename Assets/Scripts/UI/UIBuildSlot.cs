using Muks.DataBind;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIBuildSlot : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private TextMeshProUGUI _ingredientText;

    private string _craftName;
    private BuildData _buildData;

    public Button Button;

    public void OnEnable()
    {
        if (_buildData == null)
            return;

        _ingredientText.text = string.Empty;
        for (int i = 0, cnt = _buildData.NeedItemData.Length; i < cnt; ++i)
        {
            int itemCount = GameManager.Instance.Player.Inventory.GetItemCountByID(_buildData.NeedItemData[i].NeedItemId);
            int needItemCount = _buildData.NeedItemData[i].NeedItemAmount;
            string itemCountStr = itemCount < needItemCount ? Utility.SetStringColor(Color.red, itemCount.ToString()) : Utility.SetStringColor(Color.white, itemCount.ToString());

            _ingredientText.text += ItemManager.Instance.GetItemDataByID(_buildData.NeedItemData[i].NeedItemId).Name;
            _ingredientText.text += "  " + itemCountStr + "/" + _buildData.NeedItemData[i].NeedItemAmount + "\n";
        }
    }



    public void UpdateUI(BuildData buildData)
    {
        _buildData = buildData;
        _iconImage.sprite = buildData.Sprite;
        _craftName = buildData.Name;
        _nameText.text = _craftName;

        _ingredientText.text = string.Empty;
        _hpText.text = "������: " + buildData.Hp;

        for (int i = 0, cnt = buildData.NeedItemData.Length; i < cnt; ++i)
        {
            int itemCount = GameManager.Instance.Player.Inventory.GetItemCountByID(buildData.NeedItemData[i].NeedItemId);
            int needItemCount = buildData.NeedItemData[i].NeedItemAmount;
            string itemCountStr = itemCount < needItemCount ? Utility.SetStringColor(Color.red, itemCount.ToString()) : Utility.SetStringColor(Color.white, itemCount.ToString());

            _ingredientText.text += ItemManager.Instance.GetItemDataByID(buildData.NeedItemData[i].NeedItemId).Name;
            _ingredientText.text += "  " + itemCountStr + "/" + buildData.NeedItemData[i].NeedItemAmount + "\n";
        }



    }

}
