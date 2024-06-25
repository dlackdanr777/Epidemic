using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Bake된 애니메이션 메쉬를 저장하는 스크립터블 오브젝트</summary>
public class AnimatedMeshScriptableObject : ScriptableObject
{
    public int AnimationFPS;
    public List<Animation> Animations = new List<Animation>();

    [Serializable]
    public struct Animation
    {
        public string Name;
        public bool LoopTime;
        public List<MeshData> MeshDataList;
    }

    [Serializable]
    public struct MeshData
    {
        public Mesh Mesh;
        public List<Vector3> VertexList;
        public List<Vector3> NomalList;
    }

}
