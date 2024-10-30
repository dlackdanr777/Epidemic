using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildData", menuName = "BuildData")]
public class BuildData : BasicData
{
    [SerializeField] private int _hp;
    public int Hp => _hp;

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