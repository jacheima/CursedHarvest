using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Contains all objects that are marked as ScriptableAsset
/// </summary>
[System.Serializable, DefaultExecutionOrder(-900)]
public class ScriptableAssetDatabase : MonoBehaviour
{
    private static ScriptableAssetDatabase instance;

    public List<string> keys = new List<string>();
    public List<ScriptableObject> values = new List<ScriptableObject>();

    private Dictionary<string, ScriptableObject> items = new Dictionary<string, ScriptableObject>();
    private bool initializedDictionary = false;

    private void Awake()
    {
        instance = this;
        GameObject.DontDestroyOnLoad(this.gameObject);
    }

    private void InitializeDictionary()
    {
        for (int i = 0; i < keys.Count; i++)
        {
            if (!items.ContainsKey(keys[i]))
            {
                items.Add(keys[i], values[i]);
            }
            else
            {
                Debug.Log($"The GUID for: {values[i].name} has already been added to the Asset Database.");
            }
        }
    }

    public static IReferenceableAsset GetAsset(string guid)
    {
        if (!instance.initializedDictionary)
        {
            instance.InitializeDictionary();
            instance.initializedDictionary = true;
        }

        ScriptableObject data;
        instance.items.TryGetValue(guid, out data);
        return data as IReferenceableAsset;
    }
}