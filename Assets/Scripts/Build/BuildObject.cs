using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildObject : MonoBehaviour
{
    private BuildData _buildData;
    public BuildData BuildData => _buildData;

    public void SetData(BuildData buildData)
    {
        _buildData = buildData;
    }
}
