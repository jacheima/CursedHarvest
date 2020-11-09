using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public interface ISaveable
{
    /// <param name="saveSystem"> JSON data</param>
    string OnSave();

    /// <param name="saveSystem"> JSON data</param>
    void OnLoad(string data);

    /// <summary>
    /// Returning true will allow the save to occur, else it will skip the save
    /// </summary>
    bool OnSaveCondition();
}