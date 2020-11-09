using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class UISaveSlotDisplayer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playerNameText;

    [SerializeField]
    private TextMeshProUGUI farmNameText;

    [SerializeField]
    private TextMeshProUGUI dateText;

    [SerializeField]
    private RawImage rawImage;

    [SerializeField]
    private GameObject characterRendererPrefab;

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private GameObject slotAvailableObjects;

    [SerializeField]
    private GameObject slotUsedObjects;

    [SerializeField]
    private Button buttonRequestRemove;

    [SerializeField]
    private ScriptableReference confirmationWindow;

    [SerializeField]
    private UnityEvent OnRemoveRequested;

    // Apply data to saveables, to display specific state data.
    [SerializeField]
    private List<Saveable> saveable = new List<Saveable>();

    [SerializeField]
    private UnityEventInt onPressedSlot = new UnityEventInt();

    private SaveGame saveGame;
    private int slot;

    private bool isInitialized;

    public void LoadSave()
    {
        if (slot == -1)
        {
            return;
        }

        if (string.IsNullOrEmpty(saveGame.lastScene) || !Application.CanStreamedLevelBeLoaded(saveGame.lastScene))
        {
            confirmationWindow.Reference.GetComponent<ConfirmationWindow>().Configure(new ConfirmationWindow.Configuration()
            {
                acceptOnly = true,
                question = "Load Error \n (Scene File Is Not Valid)",
                answerYes = "Ok"
            });

            return;
        }

        onPressedSlot.Invoke(slot);
    }

    private void Initialize()
    {
        isInitialized = true;

        if (characterRendererPrefab != null)
        {
            GameObject characterRenderer = GameObject.Instantiate(characterRendererPrefab);
            characterRenderer.transform.position = new Vector3((Camera.allCamerasCount) * 5, 0, 0);

            if (rawImage != null)
            {
                rawImage.texture = characterRenderer.GetComponent<UICharacterRenderer>().GetTexture();
                rawImage.SetNativeSize();
                rawImage.rectTransform.sizeDelta = rawImage.rectTransform.sizeDelta * 2;
            }

            Saveable getSaveable = characterRenderer.GetComponentInChildren<Saveable>();

            if (getSaveable != null)
            {
                saveable.Add(getSaveable);
            }
        }

        buttonRequestRemove.onClick.AddListener(RequestRemoveSlot);
    }

    private void RequestRemoveSlot()
    {
        ConfirmationWindow getConfirmationWindow = confirmationWindow?.Reference?.GetComponent<ConfirmationWindow>();

        if (getConfirmationWindow != null)
        {
            getConfirmationWindow.Configure(new ConfirmationWindow.Configuration()
            {
                question = "Do you want to remove this save slot?",
                answerYes = "Yes",
                answerNo = "No",
                actionYes = OnActionRemove
            });
        }
    }

    private void OnActionRemove()
    {
        SaveUtility.DeleteSave(saveGame);
        LoadSlot(null, slot);
        OnRemoveRequested.Invoke();
    }

    public void LoadSlot(SaveGame saveGame, int slot)
    {
        this.saveGame = saveGame;
        this.slot = slot;

        if (!isInitialized)
        {
            Initialize();
        }

        if (saveGame == null)
        {
            SetEmpty();
        }
        else
        {
            canvasGroup.alpha = 1f;

            farmNameText?.SetText($"{saveGame.farmName}");
            playerNameText?.SetText($"{saveGame.playerName}");
            dateText?.SetText($"{saveGame.timePlayed.hours.ToString("00")}:{saveGame.timePlayed.minutes.ToString("00")}");

            slotAvailableObjects.gameObject.SetActive(false);
            slotUsedObjects.gameObject.SetActive(true);

            for (int i = 0; i < saveable.Count; i++)
            {
                saveable[i].gameObject.SetActive(true);
                saveable[i].OnLoadRequest(saveGame);
            }
        }
    }

    public void SetEmpty()
    {
        this.saveGame = null;
        this.slot = -1;

        canvasGroup.alpha = 0.5f;

        slotAvailableObjects.gameObject.SetActive(true);
        slotUsedObjects.gameObject.SetActive(false);

        for (int i = 0; i < saveable.Count; i++)
        {
            saveable[i].gameObject.SetActive(false);
        }
    }
}
