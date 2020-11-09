using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This scriptable object is used to obtain unique references to prefabs.
/// </summary>
[CreateAssetMenu(fileName = "Saveable Prefab", menuName = "Referencing/Saveable prefab")]
public class SaveablePrefab : ScriptableAsset
{
    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    private ScriptableReference instanceManagerReference;

    [System.NonSerialized]
    private SaveablePrefabInstanceManager instanceManager;

    public T Retrieve<T>(string identification = "") where T : UnityEngine.Object
    {
        GameObject prefabInstance = GameObject.Instantiate(prefab.gameObject);

        Saveable getSaveable = prefabInstance.GetComponent<Saveable>();

        if (getSaveable != null)
        {
            if (instanceManager == null)
            {
                GameObject getManagerObject = instanceManagerReference?.Reference;
                if (getManagerObject != null)
                {
                    instanceManager = getManagerObject.GetComponent<SaveablePrefabInstanceManager>();
                }
                else
                {
                    Debug.Log("No instance manager found within this scene. This means that a prefab will not save.");
                }
            }

            if (instanceManager != null)
            {
                SaveableInstance instanceController = prefabInstance.AddComponent<SaveableInstance>();
                instanceController.SetSaveablePrefabInstanceManager(instanceManager, getSaveable, this);

                instanceManager.AddListener(getSaveable, this, identification);
            }
        }

        if (getSaveable != null && typeof(T) == typeof(Saveable))
        {
            return getSaveable as T;
        }

        if (typeof(T) == typeof(GameObject))
        {
            return prefabInstance as T;
        }

        return prefabInstance.GetComponent<T>();
    }

    public GameObject GetPrefab()
    {
        return prefab;
    }
}