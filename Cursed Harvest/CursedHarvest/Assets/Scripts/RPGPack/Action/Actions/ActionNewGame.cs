using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Actions/New Game" )]
public class ActionNewGame : ScriptableObject
{
    [SerializeField]
    private IntVariable saveSlot;

    [SerializeField]
    private StringVariable firstScene;

    [SerializeField]
    private StringVariable playerScene;

    public void Execute()
    {
        int availableSaveSlot = SaveUtility.GetAvailableSaveSlot();

        saveSlot.Value = availableSaveSlot;

        SceneManager.LoadScene(firstScene.Value);
        SceneManager.LoadScene(playerScene.Value, LoadSceneMode.Additive);
    }
}
