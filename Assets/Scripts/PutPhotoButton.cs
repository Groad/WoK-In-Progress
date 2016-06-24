using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;

public class PutPhotoButton : EditorButton
{
    [DllImport("user32.dll")]
    private static extern void OpenFileDialog();
    [DllImport("user32.dll")]
    private static extern void ShowDialog();
    [DllImport("user32.dll")]
    private static extern void SaveFileDialog();

    private FileBrowser browser;
    private string fileFormat = ".png";
    private string folderName = "images";
    private bool isImageBrowsing = false;
    public static string photoPath;
    public Collider inputKiller;
    private string browserDefaultDirectory;

	void Start ()
    {
        browser = GetComponent<FileBrowser>();
        browserDefaultDirectory = "C:/Users/" + System.Environment.UserName + "/Pictures";
        if (File.Exists(browserDefaultDirectory))
        {
            if (File.Exists(browserDefaultDirectory + "/Camera Roll"))
            {
                browserDefaultDirectory += "/Camera Roll";
            }
        }
        else
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            browserDefaultDirectory = System.Environment.CurrentDirectory;
#elif UNITY_ANDROID
            browserDefaultDirectory = "/";
#endif
            //browserDefaultDirectory = Application.persistentDataPath;
        }
        
        if (inputKiller != null)
        {
            TabletServerSide.inputKiller = inputKiller;
        }
        browser.DefaultDirectory = browserDefaultDirectory;
	}

    public override void press()
    {
#if UNITY_STANDALONE_WIN
        System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
        openFileDialog.InitialDirectory = browserDefaultDirectory;
        var sel = "JPG Files (*.jpg)|*.jpg";
        openFileDialog.Filter = sel;
        openFileDialog.FilterIndex = 1;
        openFileDialog.RestoreDirectory = false;
        if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            string imagePath = openFileDialog.FileName;
            OnImageSelect(imagePath);
        }
#elif UNITY_ANDROID
        if (browser.isShowing == false)
        {
            fileFormat = ".jpg";
            folderName = "images";
            isImageBrowsing = true;
            browser.Show(browserDefaultDirectory, "*.jpg", this, FileSelectMode.File);
            TabletServerSide.fileBrowserOpen = true;
        }
#endif
	}

    void OnFileSelected(FileInfo info)
    {
        OnImageSelect(info.path);
        TabletServerSide.fileBrowserOpen = false;
    }

    void OnBrowseCancel()
    {
        TabletServerSide.fileBrowserOpen = false;
    }

    void OnImageSelect(string imgPath)
    {
        if (imgPath == "" || imgPath == null) return;

        photoPath = imgPath;
        Debug.Log("Selected file path is " + imgPath);
        FileStream fs = new FileStream(imgPath, FileMode.Open);
        long end = fs.Seek(0, SeekOrigin.End);
        fs.Seek(0, SeekOrigin.Begin);
        byte[] bytes = new byte[end];
        fs.Read(bytes, 0, (int)end);
        fs.Close();

        GameObject stickyToPutPhotoTo = null;
        if (SceneManager.isDesktopScene)
        {
            if (ftlGatherer.ActiveNotes != null && ftlGatherer.ActiveNotes.Count > 0 && ftlGatherer.ActiveNotes[0] != null)
            {
                stickyToPutPhotoTo = ftlGatherer.ActiveNotes[0];
            }
            else
            {
                if (NewStickyButton.thisSticky == null)
                {
                    NewStickyButton.instance.ForceMouseDown();
                }
                stickyToPutPhotoTo = NewStickyButton.thisSticky;
            }
            StickyScript ss = stickyToPutPhotoTo.GetComponent<StickyScript>();
            if (imgPath.Contains(SaveLoadManager.saveLoadMediaFolder))
            {
                ss.imagePath = imgPath.Remove(0, (SaveLoadManager.saveLoadMediaFolder).Length);
            }
            else
            {
                ss.imagePath = Random.Range(0, System.Int32.MaxValue) + ".jpg";
                File.Copy(imgPath, SaveLoadManager.saveLoadMediaFolder + ss.imagePath, true);
            }
        }
        else
        {
            if (NewStickyButton.thisSticky == null)
            {
                NewStickyButton.instance.ForceMouseDown();
            }
            stickyToPutPhotoTo = NewStickyButton.thisSticky;
        }
        
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(bytes);
        stickyToPutPhotoTo.GetComponent<StickyScript>().putPhoto(tex);
    }
}
