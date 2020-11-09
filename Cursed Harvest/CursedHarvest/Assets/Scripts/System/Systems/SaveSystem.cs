using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.IO;

/// <summary>
/// Notifies listeners to save the game.
/// Obtains saved data from callback, and writes it to a file.
/// </summary>

[AddComponentMenu("Farming Kit/Systems/Save System")]
public class SaveSystem : GameSystem
{
    [SerializeField]
    private SaveGame cachedSaveData;

    [SerializeField]
    private SaveEvent onGameSave;

    [SerializeField]
    private SaveEvent onGameLoad;

    [SerializeField]
    private GameEvent onNewGameStarted;

    [SerializeField]
    private StringEvent onSceneWarp;

    [SerializeField]
    private FloatEvent onWarpStart;

    [SerializeField]
    private FloatEvent onWarpEnd;

    [SerializeField]
    private IntReference saveSlot;

    [SerializeField]
    private StringReference playerName;

    [SerializeField]
    private StringReference farmName;

    [System.NonSerialized]
    private bool isNewGame;

    public override void OnLoadSystem()
    {
        cachedSaveData = SaveUtility.LoadSave(saveSlot.Value);

        if (cachedSaveData == null)
        {
            CreateNewSave();
            isNewGame = true;
        }

        onSceneWarp?.AddListener(OnSceneWarp);
        onWarpStart?.AddListener(OnWarpStart);
        onWarpEnd?.AddListener(OnWarpEnd);
    }

    private void CreateNewSave()
    {
        cachedSaveData = new SaveGame();

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene getScene = SceneManager.GetSceneAt(i);

            if (getScene.name != "Core")
            {
                cachedSaveData.lastScene = getScene.name;
                cachedSaveData.playerName = playerName.Value;
                cachedSaveData.farmName = farmName.Value;
            }
        }

        WriteSaveToFile();
    }

    private void Start()
    {
        onGameLoad?.Invoke(cachedSaveData);

        if (isNewGame)
        {
            onNewGameStarted.Invoke();
            onGameSave?.Invoke(cachedSaveData);
        }
    }

    private void OnSceneWarp(string scene)
    {
        cachedSaveData.lastScene = scene;
        onGameSave?.Invoke(cachedSaveData);
        WriteSaveToFile();
    }

    private void OnWarpEnd(float obj)
    {
        onGameLoad?.Invoke(cachedSaveData);
        Pauze(false);
    }

    private void OnWarpStart(float obj)
    {
        Pauze(true);
    }


    public override void OnTick()
    {
        // Nofify all listeners for onGameSave that game is saved
        onGameSave?.Invoke(cachedSaveData);

#if UNITY_WEBGL && !UNITY_EDITOR
        WriteSaveToFile();
#endif
    }

    private void OnDestroy()
    {
        onGameSave?.Invoke(cachedSaveData);

        WriteSaveToFile();
    }

    private void WriteSaveToFile()
    {
        TimeSpan currentTimePlayed = DateTime.Now - cachedSaveData.saveDate;
        TimeSpan allTimePlayed = cachedSaveData.timePlayed;
        cachedSaveData.timePlayed = allTimePlayed + currentTimePlayed;

        SaveUtility.WriteSave(cachedSaveData, saveSlot.Value);
    }
}
