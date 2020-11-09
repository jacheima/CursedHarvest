using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Actions/Load Game")]
public class ActionLoadGame : ScriptableObject
{
    [SerializeField]
    private IntVariable saveSlot;

    [SerializeField]
    private StringVariable playerScene;

    public void Execute(int slotNumber)
    {
        saveSlot.Value = slotNumber;

        SaveGame getSaveGame = SaveUtility.LoadSave(slotNumber);

        SceneManager.LoadScene(getSaveGame.lastScene);
        SceneManager.LoadScene(playerScene.Value, LoadSceneMode.Additive);
    }
}