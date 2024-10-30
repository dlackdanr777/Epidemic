using UnityEngine;

public class BasicData : ScriptableObject
{
    [SerializeField] protected string _id;
    public string Id => _id;
    [SerializeField] protected string _name;
    public string Name => _name;

    [TextArea][SerializeField] protected string _description;
    public string Description => _description;

    [SerializeField] protected Sprite _sprite;
    public Sprite Sprite => _sprite;
}
