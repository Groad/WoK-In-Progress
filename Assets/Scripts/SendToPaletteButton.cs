using UnityEngine;
using System.Collections;
using System.IO;

public class SendToPaletteButton : EditorButton
{
    public StickyPalette palette;
    public TextMesh noOfClientsText;
    public static string sentStickyName;

    void Start()
    {
        if (SaveLoadManager.saveLoadMediaFolder == "")
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            SaveLoadManager.saveLoadMediaFolder = Application.dataPath + "/";
#elif UNITY_ANDROID
            SaveLoadManager.saveLoadMediaFolder = "/sdcard/Qnowledge/";
            if (!Directory.Exists(SaveLoadManager.saveLoadMediaFolder))
            {
                Directory.CreateDirectory(SaveLoadManager.saveLoadMediaFolder);
            }
#endif
        }
        if (noOfClientsText != null)
        {
            DesktopServerSide.UpdateClientCount += UpdateClients;
        }
    }

    public override void press()
    {
        if (NewStickyButton.thisSticky != null)
        {
            if (SceneManager.isDesktopScene)
            {
                NewStickyButton.thisSticky.GetComponent<StickyInPanel>().sendToPalette(palette);
                ftlGatherer.ActiveNotes = null;
                StickyInPanel.mode = StickyInPanel.Mode.drag;
                SaveLoadManager.MakeAMove("SendToPaletteButton");
            }
            else
            {
                TabletFileSending.SendTxtButton();
            }
        }
    }

    void UpdateClients(string v)
    {
        if (noOfClientsText != null)
        {
            noOfClientsText.text = v;
        }
    }
}
