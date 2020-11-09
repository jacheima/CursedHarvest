using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Attach this to the root of an object that you want to save
/// </summary>
[DisallowMultipleComponent]
public class Saveable : MonoBehaviour
{
    [SerializeField]
    public SaveEvent onGameSaved;

    [SerializeField]
    public SaveEvent onGameLoaded;

    [System.Serializable]
    public class SaveableComponent
    {
        public StringReference identifier = new StringReference();
        public MonoBehaviour monoBehaviour;
    }

    [SerializeField, HideInInspector]
    private List<SaveableComponent> saveableComponents = new List<SaveableComponent>();

    private Dictionary<string, ISaveable> saveableComponentDictionary = new Dictionary<string, ISaveable>();

    public StringReference saveIdentification = new StringReference();

    [SerializeField]
    private bool loadOnce;
    private bool hasLoaded;

    [System.Serializable]
    public struct SaveStructure
    {
        public string identifier;
        public string data;
    }

    [System.Serializable]
    public class SaveContainer
    {
        public List<SaveStructure> saveStructures = new List<SaveStructure>();
    }

    // Used to know if specific data is already contained within the save structure array
    private Dictionary<string, int> iSaveableDataIdentifiers = new Dictionary<string, int>();

    private SaveContainer iSaveableData = new SaveContainer();

    private SaveGame cachedSaveGame;

#if UNITY_EDITOR

    private string identification;
    private static string identificationData;
    private static string lastSelectedGUID;

    // Used to check if the object is selected
    private void OnDrawGizmosSelected()
    {
        if (string.IsNullOrEmpty(identification))
        {
            identification = ($"{this.GetInstanceID()}{this.name}{this.transform.position}");
        }

        if (identificationData != identification)
        {
            if (saveIdentification.UseConstant)
            {
                if (!string.IsNullOrEmpty(lastSelectedGUID) && lastSelectedGUID == saveIdentification.ConstantValue)
                {
                    Debug.Log("Saveable Component: A new save identification has been made for duplicated object");
                    saveIdentification.ConstantValue = string.Empty;
                    OnValidate();
                }

                lastSelectedGUID = saveIdentification.ConstantValue;
            }

            identificationData = identification;
        }
    }

    private void OnValidate()
    {
        // Set a new save identification if it is not a prefab at the moment.
        if (this.gameObject.scene.name != null)
        {
            if (saveIdentification.UseConstant && string.IsNullOrEmpty(saveIdentification.ConstantValue))
            {
                saveIdentification.ConstantValue = (System.Guid.NewGuid().ToString());
            }


        }
        else
        {
            // If the prefab has a string of length 32, we can assume it is a GUID
            if (saveIdentification.ConstantValue.Length == 36)
            {
                saveIdentification.ConstantValue = string.Empty;
                EditorUtility.SetDirty(this.gameObject);
            }
        }

        ISaveable[] saveables = GetComponentsInChildren<ISaveable>(true);

        if (saveables.Length != saveableComponents.Count)
        {
            if (saveableComponents.Count > saveables.Length)
            {
                for (int i = saveableComponents.Count - 1; i >= saveables.Length; i--)
                {
                    saveableComponents.RemoveAt(i);
                }
            }

            saveableComponents.RemoveAll(s => s.monoBehaviour == null);

            ISaveable[] cachedSaveables = new ISaveable[saveableComponents.Count];
            for (int i = 0; i < cachedSaveables.Length; i++)
            {
                cachedSaveables[i] = saveableComponents[i].monoBehaviour as ISaveable;
            }

            ISaveable[] missingElements = saveables.Except(cachedSaveables).ToArray();

            for (int i = 0; i < missingElements.Length; i++)
            {
                SaveableComponent newSaveableComponent = new SaveableComponent()
                {
                    monoBehaviour = missingElements[i] as MonoBehaviour
                };

                if (newSaveableComponent.identifier.UseConstant == true)
                {
                    string typeString = newSaveableComponent.monoBehaviour.GetType().ToString();
                    string guidString = System.Guid.NewGuid().ToString().Substring(0, 5);

                    newSaveableComponent.identifier.ConstantValue = $"{typeString} {guidString}";
                }

                saveableComponents.Add(newSaveableComponent);
            }
        }
    }

    public void Refresh()
    {
        OnValidate();
    }

#endif

    private bool clearSaveData;

    public void RemoveFromSaveData()
    {
        if (cachedSaveGame != null)
        {
            cachedSaveGame.Remove(saveIdentification.Value);

            onGameSaved?.RemoveListener(OnSaveRequest);
            onGameLoaded?.RemoveListener(OnLoadRequest);
        }
    }

    private void Awake()
    {
        // Store the data into a dictionary for performant retrieval of identifiers.
        for (int i = 0; i < saveableComponents.Count; i++)
        {
            saveableComponentDictionary.Add(saveableComponents[i].identifier.Value, saveableComponents[i].monoBehaviour as ISaveable);
        }

        onGameSaved?.AddListener(OnSaveRequest);
        onGameLoaded?.AddListener(OnLoadRequest);
    }

    private void OnDestroy()
    {
        onGameSaved?.RemoveListener(OnSaveRequest);
        onGameLoaded?.RemoveListener(OnLoadRequest);
    }

    // Request is sent by the Save System
    public void OnSaveRequest(SaveGame saveGame)
    {
        if (cachedSaveGame != saveGame)
        {
            cachedSaveGame = saveGame;
        }

        if (string.IsNullOrEmpty(saveIdentification.Value))
        {
            return;
        }

        foreach (KeyValuePair<string, ISaveable> item in saveableComponentDictionary)
        {
            int getSavedIndex = -1;

            // Skip if the component does not want to be saved
            if (item.Value.OnSaveCondition() == false)
            {
                continue;
            }

            SaveStructure saveStructure = new SaveStructure()
            {
                identifier = item.Key,
                data = item.Value.OnSave()
            };

            // Does the array location for the identifier already exist? Then overwrite it instead.
            if (!iSaveableDataIdentifiers.TryGetValue(item.Key, out getSavedIndex))
            {
                iSaveableData.saveStructures.Add(saveStructure);
                iSaveableDataIdentifiers.Add(item.Key, iSaveableData.saveStructures.Count - 1);
            }
            else
            {
                iSaveableData.saveStructures[getSavedIndex] = saveStructure;
            }
        }

        saveGame.Set(saveIdentification.Value, JsonUtility.ToJson(iSaveableData));
    }

    // Request is sent by the Save System
    public void OnLoadRequest(SaveGame saveGame)
    {
        if (cachedSaveGame != saveGame)
        {
            cachedSaveGame = saveGame;
        }

        if (saveGame == null)
        {
            Debug.Log("Invalid save game request");
            return;
        }

        if (loadOnce && hasLoaded)
        {
            return;
        }

        if (string.IsNullOrEmpty(saveIdentification.Value))
        {
            Debug.Log($"Save identification is empty on {this.gameObject.name}");
            return;
        }

        iSaveableData = JsonUtility.FromJson<SaveContainer>(saveGame.Get(saveIdentification.Value));

        if (iSaveableData != null)
        {
            for (int i = 0; i < iSaveableData.saveStructures.Count; i++)
            {
                // Try to get a saveable component by it's unique identifier.
                ISaveable getSaveable;
                saveableComponentDictionary.TryGetValue(iSaveableData.saveStructures[i].identifier, out getSaveable);

                if (getSaveable != null)
                {
                    getSaveable.OnLoad(iSaveableData.saveStructures[i].data);

                    if (!iSaveableDataIdentifiers.ContainsKey(iSaveableData.saveStructures[i].identifier))
                    {
                        iSaveableDataIdentifiers.Add(iSaveableData.saveStructures[i].identifier, i);
                    }
                }
            }
        }
        else
        {
            iSaveableData = new SaveContainer();
        }

        hasLoaded = true;
    }
}