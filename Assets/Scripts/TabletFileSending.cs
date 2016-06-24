using UnityEngine;
using System.Collections;
using System.IO;
using ImageVideoContactPicker;
using TNet;

public class TabletFileSending : MonoBehaviour {

    public bool isAndroid = false;

//#if !UNITY_ANDROID
    private FileBrowser browser;
//#endif

    private StreamWriter fileWriter = null;
    private string fileFormat = ".txt";
    private string folderName = "images";
    private bool isImageBrowsing = false;

    private static TabletFileSending Instance;

    void Awake()
    {
        Instance = this;
        isAndroid = Application.platform == RuntimePlatform.Android;
        TabletServerSide.ConnectionStatus += CheckConnection;
        Debug.Log("Run on android: " + isAndroid);

        if (isAndroid)
        {
#if UNITY_ANDROID
				PickerEventListener.onImageSelect += OnImageSelect;	
#endif
        }
    }

    void Start()
    {
#if !UNITY_ANDROID
        browser = GetComponent<FileBrowser>();
        //savedFileFinder = GetComponent<SavedFileFinder>();
        //savedFileFinder.GetPath += OnImageSelect;
#endif
    }

    private void CheckConnection(string connection)
    {
        if (connection == "False")
        {
            //if (browser.isShowing) browser.Hide();
        }
    }

    public void SendTxt()
    {
//        if (isAndroid)
//        {
//#if UNITY_ANDROID

//#endif
//        }
//        else 
            if (NewStickyButton.thisSticky != null)
        {
            //create txt file
            fileFormat = ".txt";
            SendToPaletteButton.sentStickyName = "" + Random.Range(0, System.Int32.MaxValue);
            string path = SaveLoadManager.saveLoadMediaFolder + SendToPaletteButton.sentStickyName + fileFormat;
            TextWriter tw = new StreamWriter(path, true);
            tw.Write(NewStickyButton.thisSticky.GetComponent<StickyScript>().stickyInfoToString());
            tw.Close();
            //call OnImageSelect with txt file's path
            isImageBrowsing = false;
            OnImageSelect(path);
            //send photo if there's one
            StickyScript ss = NewStickyButton.thisSticky.GetComponent<StickyScript>();
            if (ss.isTherePhoto)
            {
                fileFormat = ".jpg";
                path = SaveLoadManager.saveLoadMediaFolder + SendToPaletteButton.sentStickyName + fileFormat;
                File.Copy(PutPhotoButton.photoPath, path, true);
                isImageBrowsing = true;
                OnImageSelect(path);
            }
            //destroy the sent sticky
            if (NewStickyButton.thisSticky != null)
            {
                Destroy(NewStickyButton.thisSticky);
                ftlGatherer.ActiveNotes = null;
            }
        }
    }

    public void SendImage()
    {
        if (isAndroid)
        {
#if UNITY_ANDROID
				AndroidPicker.BrowseImage ();
#endif
        }
        else
        {
            if (browser.isShowing == false)
            {
                fileFormat = ".jpg";
                folderName = "images";
                isImageBrowsing = true;
                browser.Show(/*@""*/Application.dataPath, "*.jpg", this, FileSelectMode.File);
                TabletServerSide.fileBrowserOpen = true;
            }
            //savedFileFinder.Show();
        }
    }

    void OnFileSelected(FileInfo info)
    {
        OnImageSelect(info.path);
    }

    void OnImageSelect(string imgPath)
    {
        if (imgPath == "" || imgPath == null) return;
        Debug.Log("Selected file path is " + imgPath);
        FileStream fs = new FileStream(imgPath, FileMode.Open);
        long end = fs.Seek(0, SeekOrigin.End);
        fs.Seek(0, SeekOrigin.Begin);
        byte[] bytes = new byte[end];
        fs.Read(bytes, 0, (int)end);
        fs.Close();

        Debug.Log("Bytes length: " + bytes.Length);

        BinaryWriter writer = TNManager.BeginSend(Packet.RequestSaveFile);
        string path = "./" + SendToPaletteButton.sentStickyName + fileFormat;
        writer.Write(path);
        writer.Write(bytes.Length);
        writer.Write(bytes);
        TNManager.EndSend();
        TabletServerSide.Instance.SendRFC("SavedFile", path, isImageBrowsing);
        TabletServerSide.ImageAlreadySent();
    }

    public static void SendTxtButton()
    {
        Instance.SendTxt();
    }
}
