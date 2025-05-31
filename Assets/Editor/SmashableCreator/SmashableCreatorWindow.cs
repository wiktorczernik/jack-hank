using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;

public class SmashableCreatorWindow : EditorWindow
{
    string smashableName;
    GameObject modelPrefab;
    SmashableType smashableType;

    [MenuItem("JackHank/Smashable Creator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SmashableCreatorWindow));
    }
    void OnGUI()
    {
        bool isSmashableBeingEdited = false;

        var currentPrefab = PrefabStageUtility.GetCurrentPrefabStage();
        SmashableEntity smashable;
        if (currentPrefab != null)
        {
            smashable = currentPrefab.prefabContentsRoot.GetComponentInChildren<SmashableEntity>();
            isSmashableBeingEdited = smashable != null;
        }

        if (isSmashableBeingEdited)
        {
            GUILayout.Label("Opened prefab");
        }
        else
        {
            GUILayout.Label("Name");
            smashableName = EditorGUILayout.TextField(smashableName);
            GUILayout.Label("Model");
            modelPrefab = (GameObject)EditorGUILayout.ObjectField(modelPrefab, typeof(GameObject), false);
            GUILayout.Label("Type");
            smashableType = (SmashableType)EditorGUILayout.EnumPopup(smashableType);

            Texture2D modelPreview = AssetPreview.GetAssetPreview(modelPrefab);
            GUILayout.Label(modelPreview);
            if (GUILayout.Button("Create"))
            {
                GameObject smashableGameObject = new GameObject();
                smashableGameObject.name = smashableName;
                smashableGameObject.tag = "Smashable";
                smashableGameObject.transform.position = Vector3.zero;
                smashableGameObject.transform.eulerAngles = Vector3.zero;
                smashableGameObject.transform.localScale = Vector3.one;
                smashable = smashableGameObject.AddComponent<SmashableEntity>();

                GameObject modelParent = new GameObject();
                modelParent.name = "Model";
                modelParent.tag = "Smashable";
                modelParent.transform.SetParent(smashableGameObject.transform);
                modelParent.transform.position = Vector3.zero;
                modelParent.transform.eulerAngles = Vector3.zero;
                modelParent.transform.localScale = Vector3.one;

                var lodGroup = modelParent.AddComponent<LODGroup>();
                var rigidbody = modelParent.AddComponent<Rigidbody>();
                var collisionEvents = modelParent.AddComponent<CollisionEventEmitter>();

                GameObject modelInstance = Instantiate(modelPrefab, modelParent.transform);
                modelInstance.tag = "Smashable";

                MeshRenderer[] meshRenderers = modelInstance.GetComponentsInChildren<MeshRenderer>();
                MeshFilter[] meshFilters = modelInstance.GetComponentsInChildren<MeshFilter>();

                GameObject audioParent = new GameObject();
                audioParent.name = "Audio Source";
                audioParent.tag = "Smashable";
                audioParent.transform.SetParent(smashableGameObject.transform);
                audioParent.transform.position = Vector3.zero;
                audioParent.transform.eulerAngles = Vector3.zero;
                audioParent.transform.localScale = Vector3.one;

                var audioSource = audioParent.AddComponent<AudioSource>();

                smashable.usedRigidbody = rigidbody;
                smashable.model = modelParent;
                smashable.audioSource = audioSource;

                List<Collider> colliders = new();
                foreach(MeshFilter mf in meshFilters)
                {
                    var collider = mf.gameObject.AddComponent<MeshCollider>();
                    collider.convex = true;
                    colliders.Add(collider);
                }
                smashable.usedColliders = colliders.ToArray();

                LOD[] lods = new LOD[1];
                lods[0] = new LOD(0.1f, meshRenderers);
                lodGroup.SetLODs(lods);
                lodGroup.RecalculateBounds();

                rigidbody.constraints = RigidbodyConstraints.FreezeAll;

                string prefabPath = $"Assets/Prefabs/Smashables/{smashableName}.prefab";
                PrefabUtility.SaveAsPrefabAsset(smashableGameObject, prefabPath);
                GameObject.DestroyImmediate(smashableGameObject);
                AssetDatabase.Refresh();
                PrefabStageUtility.OpenPrefab(prefabPath);
            }
        }
    }
}