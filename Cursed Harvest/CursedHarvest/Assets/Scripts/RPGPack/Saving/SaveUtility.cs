using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.InteropServices;

public class SaveUtility
{
#if UNITY_EDITOR

    [UnityEditor.MenuItem(itemName: "Tools/Open Save Location")]
    public static void OpenSaveLocation()
    {
        string path = Application.persistentDataPath;
        
#if UNITY_EDITOR_WIN

        path = path.Replace(@"/", @"\");   // explorer doesn't like front slashes
        System.Diagnostics.Process.Start("explorer.exe", "/select," + path);

#elif UNITY_EDITOR_OSX

        string macPath = path.Replace("\\", "/"); 
        bool openInsidesOfFolder = false;

        if (System.IO.Directory.Exists(macPath)) 
        {
            openInsidesOfFolder = true;
        }

        if (!macPath.StartsWith("\""))
        {
            macPath = "\"" + macPath;
        }

        if (!macPath.EndsWith("\""))
        {
            macPath = macPath + "\"";
        }

        string arguments = (openInsidesOfFolder ? "" : "-R ") + macPath;
        System.Diagnostics.Process.Start("open", arguments);
    
#endif
    }

#endif


#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void SyncFiles();

    [DllImport("__Internal")]
    private static extern void WindowAlert(string message);
#endif

    private static readonly string fileExtentionName = ".gamesave";
    private static readonly string gameFileName = "slot";
    private static readonly string configFileName = "config";
    private static string dataPath => Application.persistentDataPath;

    private static bool debugMode = false;

    private static void Log(string text)
    {
        if (debugMode)
        {
            Debug.Log(text);
        }
    }

    private static SaveConfig cachedSaveConfig;

    private static Dictionary<int, SaveGame> cachedSaveGames;

    public static Dictionary<int, SaveGame> ObtainAllSaveGames()
    {
        if (cachedSaveGames != null)
        {
            return cachedSaveGames;
        }

        string[] filePaths = Directory.GetFiles(dataPath);

        string[] savePaths = filePaths.Where(path => path.EndsWith(fileExtentionName)).ToArray();

        Dictionary<int, SaveGame> gameSaves = new Dictionary<int, SaveGame>();

        for (int i = 0; i < savePaths.Length; i++)
        {
            Log($"Found save at: {savePaths[i]}");

            using (var reader = new BinaryReader(File.Open(savePaths[i], FileMode.Open)))
            {
                string dataString = reader.ReadString();

                if (!string.IsNullOrEmpty(dataString))
                {
                    LoadSaveFromPath(savePaths[i], ref gameSaves, dataString);
                }
                else
                {
                    Log($"Save file empty: {savePaths[i]}");
                }
            }
        }

        cachedSaveGames = gameSaves;

        return gameSaves;
    }

    private static void LoadSaveFromPath(string savePath, ref Dictionary<int, SaveGame> gameSaves, string dataString)
    {
        SaveGame getSave = JsonUtility.FromJson<SaveGame>(dataString);

        if (getSave != null)
        {
            getSave.lastKnownFilePath = savePath;

            int getSlotNumber;

            string fileName = savePath.Substring(dataPath.Length + gameFileName.Length + 1);

            if (int.TryParse(fileName.Substring(0, fileName.LastIndexOf(".")), out getSlotNumber))
            {
                gameSaves.Add(getSlotNumber, getSave);
            }
        }
        else
        {
            Log($"Save file corrupted: {savePath}");
        }
    }

    public static SaveGame LoadSave(int slot)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
                SyncFiles();
#endif

        Log($"Attempting to load game at slot: {slot}");

        SaveGame getSave;

        if (SaveUtility.ObtainAllSaveGames().TryGetValue(slot, out getSave))
        {
            Log($"Succesful load at slot: {slot}");
            return getSave;
        }
        else
        {
            Log($"Could not load game at slot {slot}");
            return null;
        }
    }

    public static void WriteSave(SaveGame saveGame, int saveSlot)
    {
        if (!cachedSaveGames.ContainsKey(saveSlot))
        {
            cachedSaveGames.Add(saveSlot, saveGame);
        }

        saveGame.saveDate = DateTime.Now;

        string savePath = $"{Application.persistentDataPath}/{gameFileName}{saveSlot.ToString()}{fileExtentionName}";

        Log($"Saving game slot {saveSlot.ToString()} to : {savePath}");

        using (var writer = new BinaryWriter(File.Open(savePath, FileMode.Create)))
        {
            writer.Write(JsonUtility.ToJson(saveGame));
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        SyncFiles();
#endif

    }

    public static void WriteConfig(SaveConfig config)
    {
        string savePath = $"{Application.persistentDataPath}/{configFileName}{fileExtentionName}";

        Log($"Saving game configuration to : {savePath}");

        using (var writer = new BinaryWriter(File.Open(savePath, FileMode.Create)))
        {
            writer.Write(JsonUtility.ToJson(config));
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        SyncFiles();
#endif

    }

    public static void DeleteSave(SaveGame saveGame)
    {
        int? removeIndex = ObtainAllSaveGames().FirstOrDefault(save => save.Value == saveGame).Key;

        if (removeIndex.HasValue)
        {
            DeleteSave(removeIndex.Value);
        }
    }

    public static void DeleteSave(int slot)
    {
        SaveGame getSave = LoadSave(slot);

        if (getSave != null)
        {

            if (File.Exists(getSave.lastKnownFilePath))
            {
                File.Delete(getSave.lastKnownFilePath);

                if (cachedSaveGames.Count > 0)
                {
                    cachedSaveGames.Remove(slot);
                }

#if UNITY_WEBGL && !UNITY_EDITOR
        SyncFiles();
#endif
            }
            else
            {
                Log($"Save file removal at path: {getSave.lastKnownFilePath}  failed.");
            }
        }
    }

    public static SaveConfig LoadOrCreateConfig()
    {

#if UNITY_WEBGL && !UNITY_EDITOR
                SyncFiles();
#endif

        if (cachedSaveConfig != null)
        {
            return cachedSaveConfig;
        }

        string[] filePaths = Directory.GetFiles(Application.persistentDataPath);

        string configPath = filePaths.FirstOrDefault(path => path.EndsWith($"{configFileName}{fileExtentionName}"));

        if (!string.IsNullOrEmpty(configPath))
        {
            using (var reader = new BinaryReader(File.Open(configPath, FileMode.Open)))
            {
                SaveConfig getSaveConfig = JsonUtility.FromJson<SaveConfig>(reader.ReadString());

                if (getSaveConfig != null)
                {
                    Log($"Loaded configuration from {configPath}");
                    cachedSaveConfig = getSaveConfig;
                }
                else
                {
                    getSaveConfig = new SaveConfig();
                }

                return getSaveConfig;
            }
        }
        else
        {
            cachedSaveConfig = new SaveConfig();

            return cachedSaveConfig;
        }
    }

    public static int GetAvailableSaveSlot()
    {
        for (int i = 0; i < 300; i++)
        {
            if (!ObtainAllSaveGames().ContainsKey(i))
            {
                return i;
            }
        }

        return -1;
    }
}
