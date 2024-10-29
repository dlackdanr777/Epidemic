using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildData", menuName = "BuildData")]
public class BuildData : ScriptableObject
{
    [SerializeField] private string _id;
    public string Id => _id;

    [SerializeField] private Sprite _sprite;
    public Sprite Sprite => _sprite;

    [SerializeField] private string _name;
    public string Name => _name;

    [SerializeField] private NeedItemData[] _needItemData;
    public NeedItemData[] NeedItemData => _needItemData;

    [SerializeField] private BuildObject _buildObjectPrefab;
    public BuildObject BuildObjectPrefab => _buildObjectPrefab;

    [SerializeField] private PreviewObject _previewPrefab;
    public PreviewObject PreviewPrefab => _previewPrefab;
}


[Serializable]
public class NeedItemData
{
    [SerializeField] private string _needItemId;
    public string NeedItemId => _needItemId;

    [SerializeField] private int _needItemAmount;
    public int NeedItemAmount => _needItemAmount;
}