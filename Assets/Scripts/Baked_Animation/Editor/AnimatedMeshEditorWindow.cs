using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.EditorCoroutines.Editor; //유니티 에디터 상에서 코루틴을 쓸 수 있게 해주는 namespace
using UnityEditor;
using UnityEngine;


/// <summary>애니메이션을 굽는 툴 클래스</summary>
public class AnimatedMeshEditorWindow : EditorWindow
{
    private const string BASE_PATH = "Assets/Animated Models/";

    [MenuItem("Tools/Animated Mesh Creator")]
    //에디터 창을 열 수 있게 하는 함수
    public static void CreateEditorWindow()
    {
        EditorWindow window = GetWindow<AnimatedMeshEditorWindow>();
        window.titleContent = new GUIContent("Animated Mesh Editor");
    }

    private GameObject _animatedModel;
    private int _animationFPS = 30;
    private string _name;
    private bool _optimize = false;
    private bool _dryRun = false;

    private void OnGUI()
    {
        GameObject newModel = EditorGUILayout.ObjectField("Animated Model", _animatedModel, typeof(GameObject), true) as GameObject;
        if (newModel != _animatedModel)
            _name = newModel.name + "animations";

        Animator animator = newModel == null ? null : newModel.GetComponentInChildren<Animator>();
        _animatedModel = newModel;

        _name = EditorGUILayout.TextField("Name", _name);
        _animationFPS = EditorGUILayout.IntSlider("Animation FPS", _animationFPS, 1, 100);
        _optimize = EditorGUILayout.Toggle("Optimize", _optimize);
        _dryRun = EditorGUILayout.Toggle("Dry Run", _dryRun);

        GUI.enabled = newModel != null && animator.runtimeAnimatorController != null;
        if (GUILayout.Button("Generate ScriptableObjects"))
        {
            if (newModel == null)
                return;

            //저장 경로를 생성한다.
            AssetDatabase.GenerateUniqueAssetPath(BASE_PATH + _name);

            //애니메이터의 클립들을 구워 스크립터블 오브젝트 형태로 저장하는 코루틴 실행
            EditorCoroutineUtility.StartCoroutine(GenerateModel(animator, _dryRun), this);
        }

        GUI.enabled = true;
        if (GUILayout.Button("Clear progress bar"))
            EditorUtility.ClearProgressBar();

    }


    /// <summary>애니메이션들을 구워 스크립터블 오브젝트 형태로 저장시키는 함수</summary>
    private IEnumerator GenerateModel(Animator animator, bool dryRun)
    {

        string parentFolder = BASE_PATH + _name + "/";

        foreach (SkinnedMeshRenderer skinnedMeshRenderer in _animatedModel.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            int clipIndex = 1;

            //스크립터블 오브젝트를 생성한다.
            AnimatedMeshScriptableObject scriptableObject = CreateInstance<AnimatedMeshScriptableObject>();
            scriptableObject.AnimationFPS = _animationFPS;

            //애니메이터에 등록된 클립들을 반복한다.
            foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            {
                //진행 바를 보여준다.
                EditorUtility.DisplayProgressBar("Processing Animations", $"Processing animation {clip.name} ({clipIndex / animator.runtimeAnimatorController.animationClips.Length})", clipIndex / (float)animator.runtimeAnimatorController.animationClips.Length);
                AnimatedMeshScriptableObject.Animation animation = new AnimatedMeshScriptableObject.Animation();
                List<AnimatedMeshScriptableObject.MeshData> meshDatas = new List<AnimatedMeshScriptableObject.MeshData>();

                animation.Name = clip.name;
                float increment = 1f / _animationFPS;
                animator.Play(clip.name);

                //fps *  clip.length 만큼 반복한다.
                for (float time = increment, length = clip.length; time < length; time += increment)
                {
                    //애니메이터를 수동으로 실행한다.
                    animator.Update(increment);

                    if (_dryRun)
                        yield return new WaitForSeconds(increment);

                    AnimatedMeshScriptableObject.MeshData meshData = new AnimatedMeshScriptableObject.MeshData();
                    meshData.VertexList = new List<Vector3>();
                    meshData.NomalList = new List<Vector3>();

                    //현재 스킨 메시 렌더러의 상태를 고정된 메시로 변환
                    meshData.Mesh = new Mesh();
                    skinnedMeshRenderer.BakeMesh(meshData.Mesh);

                    if (_optimize)
                        meshData.Mesh.Optimize();

                    if (!_dryRun)
                    {
                        //만약 해당 경로에 폴더가 존재하지 않으면?
                        if (!AssetDatabase.IsValidFolder(parentFolder + clip.name + $"/{skinnedMeshRenderer.name}"))
                            System.IO.Directory.CreateDirectory(parentFolder + clip.name + $"/{skinnedMeshRenderer.name}");//폴더를 생성한다.

                        AssetDatabase.CreateAsset(meshData.Mesh, parentFolder + clip.name + $"/{skinnedMeshRenderer.name}/{time:N4}.asset");
                    }

                    meshData.VertexList = meshData.Mesh.vertices.ToList();
                    meshData.NomalList = meshData.Mesh.normals.ToList();
                    meshDatas.Add( meshData );
                }

                Debug.Log($"Setting {clip.name} to have {meshDatas.Count} meshes");
                animation.MeshDataList = meshDatas;
                scriptableObject.Animations.Add(animation);
                clipIndex++;
            }

            if (!_dryRun)
            {
                Debug.Log($"Creating asset with {scriptableObject.Animations.Count} animations and {scriptableObject.Animations.Sum((item) => item.MeshDataList.Count)} meshes");
                EditorUtility.SetDirty(scriptableObject);
                AssetDatabase.CreateAsset(scriptableObject, BASE_PATH + skinnedMeshRenderer.name +" " + _name + ".asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

        }

        EditorUtility.ClearProgressBar();
    }


}
