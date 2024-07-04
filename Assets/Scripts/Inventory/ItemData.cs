using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    [SerializeField] protected string _id;
    public string ID => _id;
    [SerializeField] protected string _name;
    public string Name => _name;

    [TextArea][SerializeField] protected string _description;
    public string Description => _description;

    [SerializeField] protected Sprite _sprite;
    public Sprite Sprite => _sprite;

    [Range(1, 5)] [SerializeField] protected int _width = 1;
    public int Width => _width;
    [Range(1, 5)][SerializeField] protected int _height = 1;
    public int Height => _height;

    public abstract Item CreateItem();

}
