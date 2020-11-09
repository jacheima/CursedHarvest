using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Responsible for saving SaveablePrefabs. Once a Scriptable Prefab is spawned this manager will store the reference to the prefab.
/// So it can be instantiated again when the scene loads.
/// 
/// This class works indivudually from the save system, to prevent any execution order issues.
/// </summary>
[RequireComponent(typeof(Saveable)),]
public class SaveablePrefabInstanceManager : MonoBehaviour, ISaveable
{

    [SerializeField]
    private IntReference currentSaveSlot;

    private SaveGame currentSaveGame;

    private bool isSaveable;

    public Dictionary<SaveablePrefab, SaveablePrefabData> saveReferences = new Dictionary<SaveablePrefab, SaveablePrefabData>();

    [System.Serializable]
    public class SaveablePrefabData
    {
        public string trimmedguid;
        public string prefabGUID;
        public Dictionary<Saveable, string> saveableGUIDS = new Dictionary<Saveable, string>();
    }

    private bool destroyingScene = false;
    private bool quittingGame = false;

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += SceneChange;
        SceneManager.sceneUnloaded += SceneUnload;
        Application.quitting += IsQuittingGame;
    }

    private void OnDestroy()
    {
        foreach (SaveablePrefabData data in saveReferences.Values)
        {
            data.saveableGUIDS.Clear();
        }

        saveReferences.Clear();

        SceneManager.activeSceneChanged -= SceneChange;
        SceneManager.sceneUnloaded -= SceneUnload;
        Application.quitting -= IsQuittingGame;
    }

    private void OnApplicationQuit()
    {
        quittingGame = true;
    }

    private void IsQuittingGame()
    {
        quittingGame = true;
    }

    private void SceneUnload(Scene arg0)
    {
        if (arg0 == this.gameObject.scene)
        {
            destroyingScene = true;
        }
    }

    private void SceneChange(Scene arg0, Scene arg1)
    {
        if (arg0 == this.gameObject.scene)
        {
            destroyingScene = true;
        }
    }

    public void AddListener(Saveable instance, SaveablePrefab scriptablePrefab, string identification)
    {
        SaveablePrefabData prefabData;

        // Is there no data yet for this scriptable prefab? Create it.
        if (!saveReferences.TryGetValue(scriptablePrefab, out prefabData))
        {
            prefabData = new SaveablePrefabData();

            prefabData.prefabGUID = scriptablePrefab.GetGuid();
            prefabData.trimmedguid = $"{this.gameObject.scene.name}{prefabData.prefabGUID.Substring(0, 4)}";

            saveReferences.Add(scriptablePrefab, prefabData);
        }

        // Add identification keys for each saveable object.
        if (!prefabData.saveableGUIDS.ContainsKey(instance))
        {
            string saveableGUID = (string.IsNullOrEmpty(identification) ? $"I{prefabData.trimmedguid}{prefabData.saveableGUIDS.Count}" : identification);

            instance.saveIdentification.UseConstant = true;
            instance.saveIdentification.ConstantValue = saveableGUID;

            isSaveable = true;

            prefabData.saveableGUIDS.Add(instance, saveableGUID);
        }
    }

    public void RemoveListener(Saveable instance, SaveablePrefab scriptablePrefab)
    {

#if UNITY_EDITOR
        if (!Application.isPlaying)
            return;
#endif
        // We only remove the saved state of a prefab when it is destroyed by code.
        // Not while the scene is being destroyed or when the game is quitting.
        if (destroyingScene || quittingGame)
        {
            return;
        }

        SaveablePrefabData data;

        if (saveReferences.TryGetValue(scriptablePrefab, out data))
        {
            instance.RemoveFromSaveData();

            if (data.saveableGUIDS.Remove(instance))
            {
                isSaveable = true;
            }
            else
            {
                Debug.Log("Tried to remove listener that was never added.");
            }
        }
    }

    #region Saving

    [System.Serializable]
    public class SaveData
    {
        [System.Serializable]
        public struct GuidData
        {
            public string prefabGUID;
            public List<string> saveableGUIDs;
        }

        public List<GuidData> data = new List<GuidData>();
        public int totalSpawnedInstances;
    }

    public string OnSave()
    {
        isSaveable = false;

        SaveData saveData = new SaveData();

        foreach (SaveablePrefabData cachedData in saveReferences.Values)
        {
            SaveData.GuidData guidData;

            List<string> saveables = new List<string>();

            foreach (var item in cachedData.saveableGUIDS.Values)
            {
                saveables.Add(item);
            }

            guidData = new SaveData.GuidData()
            {
                prefabGUID = cachedData.prefabGUID,
                saveableGUIDs = saveables
            };

            saveData.data.Add(guidData);
        }

        return JsonUtility.ToJson(saveData);
    }

    public void OnLoad(string data)
    {
        if (string.IsNullOrEmpty(data))
            return;

        currentSaveGame = SaveUtility.LoadSave(currentSaveSlot.Value);

        if (currentSaveGame == null)
        {
            Debug.Log("Could not find current save");
            return;
        }

        SaveData saveData = JsonUtility.FromJson<SaveData>(data);

        if (saveData.data != null && saveData.data.Count > 0)
        {
            for (int i = 0; i < saveData.data.Count; i++)
            {
                SaveablePrefab saveablePrefab = ScriptableAssetDatabase.GetAsset(saveData.data[i].prefabGUID) as SaveablePrefab;

                if (saveablePrefab == null)
                {
                    Debug.Log($"Could not find reference in ScriptableAssetDatabase for Saveable Prefab : {saveData.data[i].prefabGUID}");
                    continue;
                }

                for (int i2 = 0; i2 < saveData.data[i].saveableGUIDs.Count; i2++)
                {
                    Saveable getSaveable = saveablePrefab.Retrieve<Saveable>(saveData.data[i].saveableGUIDs[i2]);
                    getSaveable.OnLoadRequest(currentSaveGame);

#if UNITY_EDITOR
                    getSaveable.transform.SetParent(this.transform, true);
#endif
                }
            }
        }
    }

    public bool OnSaveCondition()
    {
        return isSaveable;
    }

    #endregion
}
