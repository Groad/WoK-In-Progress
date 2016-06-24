using UnityEngine;
using System.Collections;
using System.IO;
using ImageVideoContactPicker;
using TNet;

public class FileSender : MonoBehaviour {

	public bool isAndroid = false;

//#if !UNITY_ANDROID
	private FileBrowser browser;
//#endif

	private StreamWriter fileWriter = null;
	private string fileFormat = ".png";
	private string folderName = "images";
	private bool isImageBrowsing = false;

	void Awake()
	{
		isAndroid = Application.platform == RuntimePlatform.Android;
		ServerConnection.ConnectionStatus += CheckConnection;
		Debug.Log ("Run on android: " + isAndroid);

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
			if (browser.isShowing) browser.Hide();
		}
	}

	public void SendTxt()
	{
		if (isAndroid)
		{
			#if UNITY_ANDROID

			#endif
		}
		else
		{
			if (browser.isShowing == false) 
			{
				fileFormat = ".txt";
				folderName = "texts";
				isImageBrowsing = false;
                browser.Show(@"", "*.txt", this, FileSelectMode.File);
                TabletServerSide.fileBrowserOpen = true;
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
				fileFormat = ".png";
				folderName = "images";
				isImageBrowsing = true;
                browser.Show(@"", "*.png", this, FileSelectMode.File);
                TabletServerSide.fileBrowserOpen = true;
			}
			//savedFileFinder.Show();
		}
	}

	void OnFileSelected(FileInfo info)
	{
		OnImageSelect (info.path);
	}

	void OnImageSelect(string imgPath)
	{
		if (imgPath == "" || imgPath == null) return;
		Debug.Log ("Selected file path is " + imgPath);
		FileStream fs = new FileStream(imgPath, FileMode.Open);
		long end = fs.Seek(0, SeekOrigin.End);
		fs.Seek(0, SeekOrigin.Begin);
		byte[] bytes = new byte[end];
		fs.Read(bytes, 0, (int)end);
		fs.Close();

		Debug.Log ("Bytes length: "+bytes.Length);

		BinaryWriter writer = TNManager.BeginSend(Packet.RequestSaveFile);
        string path = Random.Range(0, System.Int32.MaxValue).ToString() + fileFormat;
		writer.Write(path);
		writer.Write(bytes.Length);
		writer.Write(bytes);
		TNManager.EndSend();
		ServerConnection.Instance.SendRFC ("SavedFile", path, isImageBrowsing);
		ServerConnection.ImageAlreadySent ();
	}
}
