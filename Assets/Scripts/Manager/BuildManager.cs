using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("BuildManager");
                _instance = obj.AddComponent<BuildManager>();
                DontDestroyOnLoad(obj);
            }

            return _instance;
        }
    }

    private static BuildManager _instance;

    private BuildData[] _buildDatas;
    public BuildData[] BuildDatas => _buildDatas;

    private void Awake()
    {
        if(_instance != null )
        {
            Destroy(gameObject);
            return;
        }

        _buildDatas = Resources.LoadAll<BuildData>("BuildDatas/");
    }


}
