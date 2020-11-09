using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class SaveGame : ISerializationCallbackReceiver
{
    public int lastKnownSlotID;
    public string lastKnownFilePath;
    public string lastScene = "";
    public System.DateTime saveDate = DateTime.Now;
    public TimePlayed timePlayed = new TimePlayed();
    public string playerName = "";
    public string farmName = "";

    [System.Serializable]
    public struct SavedData
    {
        public string guid;
        public string data;
    }

    [SerializeField]
    private List<SavedData> savedData = new List<SavedData>();

    // A dictionary provides consistent lookup performance, but is not serialized by Unity, or turned into JSON
    [System.NonSerialized]
    private Dictionary<string, int> saveDataCache = new Dictionary<string, int>(StringComparer.Ordinal);

    [System.NonSerialized]
    private int saveIndexOffset;

    public void Remove(string _guid)
    {
        int saveIndex;

        if (saveDataCache.TryGetValue(_guid, out saveIndex))
        {
            // Zero out the string data, it will be wiped on next cache initialization.
            savedData[saveIndex] = new SavedData();
        }
        else
        {
            Debug.Log("Attempted to remove empty save data");
        }
    }

    public void Set(string _guid, string _data)
    {
        int saveIndex;
        if (saveDataCache.TryGetValue(_guid, out saveIndex))
        {
            savedData[saveIndex] = new SavedData() { guid = _guid, data = _data };
        }
        else
        {
            SavedData newSaveData = new SavedData() { guid = _guid, data = _data };

            savedData.Add(newSaveData);
            saveDataCache.Add(_guid, savedData.Count - 1);
        }
    }

    public string Get(string _guid)
    {
        int saveIndex;
        if (saveDataCache.TryGetValue(_guid, out saveIndex))
        {
            return savedData[saveIndex].data;
        }
        else
        {
            return null;
        }
    }

    public void OnAfterDeserialize()
    {
        if (savedData.Count > 0)
        {
            savedData.RemoveAll(s => string.IsNullOrEmpty(s.data));

            for (int i = 0; i < savedData.Count; i++)
            {
                saveDataCache.Add(savedData[i].guid, i);
            }
        }
    }

    public void OnBeforeSerialize()
    {

    }
}