using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class OpenHelpWindow : MonoBehaviour
{
    static OpenHelpWindow()
    {
        if (!EditorPrefs.GetBool("RPGFarmingKit_DisplayedWelcomeMessage", false) == false)
        {
            HelpWindow.Display();
            EditorPrefs.SetBool("RPGFarmingKit_DisplayedWelcomeMessage", true);
        }
    }
}
