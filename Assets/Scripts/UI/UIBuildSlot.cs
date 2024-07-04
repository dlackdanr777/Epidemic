using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIBuildSlot : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _ingredientText;

    private string _craftName;
    private string _itemName;
    private int _itemAmount;
    private string _itemID;

    public Button Button;

    public void OnEnable()
    {
        int itemCount = GameManager.Instance.Player.Inventory.GetItemCountByID(_itemID);

        itemCount = itemCount <= 0 ? 0 : itemCount;

        _ingredientText.text = _itemName;
        _ingredientText.text += "  " + itemCount + "/" + _itemAmount;
    }



    public void UpdateUI(Craft craft)
    {
        _iconImage.sprite = craft.Sprite;
        _craftName = craft.Name;
        _itemName = ItemManager.Instance.GetItemByID(craft.NecessaryItemId).Data.Name;
        _itemAmount = craft.NecessaryItemAmount;
        _itemID = craft.NecessaryItemId;

        _nameText.text = _craftName;
        _ingredientText.text = _itemName;
        _ingredientText.text += "  0/" + _itemAmount;
    }

}
