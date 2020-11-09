using UnityEngine;
using System.Collections;

[AddComponentMenu("")]
public class SaveableInstance : MonoBehaviour
{
    public SaveablePrefabInstanceManager prefabInstanceManager;
    public SaveablePrefab saveablePrefab;
    public Saveable saveable;

    public void SetSaveablePrefabInstanceManager(SaveablePrefabInstanceManager reference, Saveable saveable, SaveablePrefab saveablePrefab)
    {
        prefabInstanceManager = reference;
        this.saveable = saveable;
        this.saveablePrefab = saveablePrefab;
    }

    public void OnDisable()
    {
        prefabInstanceManager.RemoveListener(saveable, saveablePrefab);
    }
}
