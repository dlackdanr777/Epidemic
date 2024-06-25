using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public int ID => _id;
    public string Name => _name;
    public string Description => _description;
    public Sprite Icon => _icon;

    [SerializeField] protected int _id;
    [SerializeField] protected string _name;
    [TextArea]
    [SerializeField] protected string _description;
    [SerializeField] protected Sprite _icon;

    public abstract Item CreateItem();

}
