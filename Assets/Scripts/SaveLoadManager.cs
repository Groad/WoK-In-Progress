using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System;

public class SaveLoadManager : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern void OpenFileDialog();
    [DllImport("user32.dll")]
    private static extern void ShowDialog();
    [DllImport("user32.dll")]
    private static extern void SaveFileDialog();

    public static SaveLoadManager instance;
    public Transform connections;
    public Transform stickiesInPalette;
    public Transform handles;
    private static int currentId;
    public ToolbarButton saveButton;
    public ToolbarButton exportAsImageButton;
    public ToolbarButton saveAsButton;
    public ToolbarButton clearButton;
    public ToolbarButton loadButton;
    public ToolbarButton selectedSaveButton;
    public ToolbarButton mergeLoadButton;
    public ToolbarButton quitButton;
    private static string saveLoadPath = "";
    private static string saveLoadFolder = "";
    public static string saveLoadMediaFolder = "";
    private static string dataPathLastPart = "";

    public GameObject handlePrefab;
    public GameObject linePrefab;
    public Camera screenshotCamera;

    public static string LoadedText;
    private static string[] undoStrings;
    private const int NoOfUndos = 205;
    private static int undoIndex;
    private static bool undoRedoInProgress;
    private static string TempSavePath;

    private static int tempSaveMoveCounter;
    private static float tempSaveTimer;

    public string loadedText;
    public string[] UndoStrings;

    public static string lastSaveString;
    void Awake()
    {
        instance = this;
        ResetUndoMoves();
        TempSavePath = Application.dataPath + "/tempSystemSave.wok";
        //init for saveLoadMediaFolder is done in SendToPaletteButton.cs
    }

    void Start()
    {

    }

    void OnApplicationQuit()
    {
        if (DesktopServerSide.instance != null && DesktopServerSide.instance.isConnectedToServer)
        {
            TempSave();
        }
    }

    public static void TempSave()
    {
#if !UNITY_EDITOR
        string tempLoadPath = saveLoadPath;
        saveLoadPath = TempSavePath;
        if (File.Exists(saveLoadPath))
        {
            File.Delete(saveLoadPath);
        }
        instance.Save();
        saveLoadPath = tempLoadPath;
        Debug.Log("TempSave");
#else
        //Debug.Log("UNITY_EDITOR");
#endif
    }

    public static void TempLoad()
    {
#if !UNITY_EDITOR
        if (File.Exists(TempSavePath))
        {
            var reader = new StreamReader(TempSavePath);
            LoadedText = reader.ReadToEnd();
            Reset();
            Load();
            Debug.Log("TempLoad");
        }
#else
        //Debug.Log("UNITY_EDITOR");
#endif
    }

    public static void SavePress()
    {
        if (saveLoadPath == "")
        {
            instance.SaveDialog();
        }
        else
        {
            //overwrite file
            if (File.Exists(saveLoadPath))
            {
                File.Delete(saveLoadPath);
                instance.Save();
            }
            else
            {
                instance.SaveDialog();
            }
        }
    }

    public static void ExportAsImagePress()
    {
        instance.ExportAsImageDialog();
    }

    public static void SaveAsPress()
    {
        instance.SaveDialog();
    }

    public static void ClearPress()
    {
        Reset();
    }

    public static void LoadPress()
    {
        instance.LoadDialog();
    }

    public static void SelectedSavePress()
    {
        if (ftlGatherer.ActiveNotes != null && ftlGatherer.ActiveNotes.Count > 0 && ftlGatherer.ActiveNotes[0].tag == "StickyNote")
        {
            instance.SaveDialog(ftlGatherer.ActiveNotes);
        }
    }

    public static void MergeLoadPress()
    {
        instance.LoadDialog(true);
    }

    public static void QuitPress()
    {
        Application.Quit();
    }
	
	void Update ()
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                bool pressedUp = Input.GetKeyDown(KeyCode.UpArrow);
                if (ftlGatherer.ActiveNotes != null)
                {
                    for (int i = 0; i < ftlGatherer.ActiveNotes.Count; i++)
                    {
                        if (ftlGatherer.ActiveNotes[i] != null)
                        {
                            StickyScript ss = ftlGatherer.ActiveNotes[i].GetComponent<StickyScript>();
                            if (ss != null)
                            {
                                ss.layerNo += (pressedUp ? 1 : -1);
                                if (ss.layerNo < -StickyScript.MaxLayerNo)
                                {
                                    ss.layerNo = -StickyScript.MaxLayerNo;
                                }
                                if (ss.layerNo > StickyScript.MaxLayerNo)
                                {
                                    ss.layerNo = StickyScript.MaxLayerNo;
                                }
                            }
                        }
                    }
                    GridSnapper.audioSource.Play();
                    MakeAMove("sticky layer change (SaveLoadManager)");
                }
            }
        }

        float z = -30.9f;
        float zDiff = -0.3f;
        foreach (Transform child in transform)
        {
            StickyScript stickyScript = child.GetComponent<StickyScript>();
            child.position = new Vector3(child.position.x, child.position.y, z + stickyScript.layerNo * 5f);
            if (z < -22f)
            {
                z -= zDiff;
            }
        }
        
        if (UDPManager.isMain)
        {
            if (saveButton.pressed)
            {
                SavePress();
            }
            if (exportAsImageButton.pressed)
            {
                ExportAsImagePress();
            }
            if (saveAsButton.pressed)
            {
                SaveAsPress();
            }
            if (clearButton.pressed)
            {
                ClearPress();
            }
            if (loadButton.pressed)
            {
                LoadPress();
            }
            if (selectedSaveButton.pressed)
            {
                SelectedSavePress();
            }
            if (mergeLoadButton.pressed)
            {
                MergeLoadPress();
            }
        }
        if (quitButton.pressed)
        {
            QuitPress();
        }

        if (tempSaveMoveCounter > 9)
        {
            tempSaveTimer += Time.deltaTime;
            if (tempSaveTimer > 3f)
            {
                TempSave();
                tempSaveTimer = 0f;
                tempSaveMoveCounter = 0;
            }
        }
    }

    void SaveDialog(List<GameObject> selectedStickies = null)
    {
        System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
        saveFileDialog.InitialDirectory = UnityEngine.Application.dataPath;
        var sel = "WOK Files (*.wok)|*.wok";
        saveFileDialog.Filter = sel;
        saveFileDialog.RestoreDirectory = true;
        if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            saveLoadPath = saveFileDialog.FileName;
            setSaveLoadFolder();
            Save(selectedStickies);
        }
    }

    void ExportAsImageDialog()
    {
        System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
        saveFileDialog.InitialDirectory = UnityEngine.Application.dataPath;
        var sel = "PNG Files (*.png)|*.png";
        saveFileDialog.Filter = sel;
        saveFileDialog.RestoreDirectory = true;
        if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            RenderTexture rt = new RenderTexture(screenshotCamera.pixelWidth, screenshotCamera.pixelHeight, 24);
            screenshotCamera.targetTexture = rt;
            Texture2D screenshotTexture = new Texture2D(screenshotCamera.pixelWidth, screenshotCamera.pixelHeight, TextureFormat.RGB24, false);
            screenshotCamera.Render();
            RenderTexture.active = rt;
            screenshotTexture.ReadPixels(new Rect(0, 0, screenshotCamera.pixelWidth, screenshotCamera.pixelHeight), 0, 0);

            byte[] bytes = screenshotTexture.EncodeToPNG();
            File.WriteAllBytes(saveFileDialog.FileName, bytes);
        }
    }

    void Save(List<GameObject> selectedStickies = null)//save file doesn't exist. It's deleted for overwriting if it existed.
    {
        var tw = File.CreateText(saveLoadPath);
        tw.Write(SaveString(true, selectedStickies));
        tw.Close();
        //photos
        for (int i = 0; i < photoPaths.Count; i++)
        {
            if (!File.Exists(saveLoadMediaFolder + photoPaths[i]))
            {
                File.Copy(Application.dataPath + "/" + photoPaths[i], saveLoadMediaFolder + photoPaths[i]);
            }
        }
    }

    void LoadDialog(bool merge = false)
    {
        LoadedText = "";
        System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
        openFileDialog.InitialDirectory = Application.dataPath;
        var sel = "WOK Files (*.wok)|*.wok";
        openFileDialog.Filter = sel;
        openFileDialog.FilterIndex = 1;
        openFileDialog.RestoreDirectory = false;
        if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            saveLoadPath = openFileDialog.FileName;
            setSaveLoadFolder();
            var reader = new StreamReader(openFileDialog.FileName);
            LoadedText = reader.ReadToEnd();
            reader.Close();
            if (!merge)
            {
                Reset();
            }
            Load();
            if (merge)
            {
                //get the nodes to the right by ViewControl.stickieMinX
                float maxX = 0f;
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (nodes[i].tag == "StickyNote")
                    {
                        float x = nodes[i].GetComponent<StickyScript>().maxX();
                        if (x > maxX)
                        {
                            maxX = x;
                        }
                    }
                    else //handle
                    {
                        if (nodes[i].transform.position.x > maxX)
                        {
                            maxX = nodes[i].transform.position.x;
                        }
                    }
                }

                for (int i = 0; i < nodes.Count; i++)
                {
                    nodes[i].transform.position += (ViewControl.stickieMinX - maxX - 2f) * Vector3.right;
                }
            }
            ResetUndoMoves();
        }
    }

    public static void Reset()
    {
        foreach (Transform child in instance.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in instance.connections)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in instance.handles)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in instance.stickiesInPalette)
        {
            Destroy(child.gameObject);
        }
        StickyPalette.instance.stickiesInPalette = new List<GameObject>();
        CopyPasteManager.noOfPastesInARowCount = 0;
    }

    private static List<string> photoPaths;
    public static string SaveString(bool savePhotos = true, List<GameObject> stickiesOverride = null)
    {
        photoPaths = new List<string>();
        int noOfStickiesInMain = 0;
        int noOfStickiesInPalette = 0;
        string toReturn = "";
        List<GameObject> nodes = new List<GameObject>();
        List<GameObject> stickies = new List<GameObject>();
        List<GameObject> handles = new List<GameObject>();
        if (stickiesOverride != null)
        {
            stickies = stickiesOverride;
            handles = ftlGatherer.getConnectedHandles(stickies);
            for (int i = 0; i < stickies.Count; i++)
            {
                if (stickies[i].GetComponent<StickyInPanel>() != null)
                {
                    noOfStickiesInPalette++;
                }
                else
                {
                    noOfStickiesInMain++;
                    nodes.Add(stickies[i]);
                }
            }
            for (int i = 0; i < handles.Count; i++)
            {
                nodes.Add(handles[i]);
            }
        }
        else
        {
            foreach (Transform child in instance.transform)
            {
                stickies.Add(child.gameObject);
                nodes.Add(child.gameObject);
                noOfStickiesInMain++;
            }
            foreach (Transform child in instance.handles)
            {
                handles.Add(child.gameObject);
                nodes.Add(child.gameObject);
            }
            for (int i = 0; i < StickyPalette.instance.stickiesInPalette.Count; i++)
            {
                if (StickyPalette.instance.stickiesInPalette[i] != null)
                {
                    stickies.Add(StickyPalette.instance.stickiesInPalette[i]);
                    noOfStickiesInPalette++;
                }
            }
        }
        toReturn += "noOfStickiesInMain=" + noOfStickiesInMain;
        toReturn += "\n";
        toReturn += "noOfStickiesInPalette=" + noOfStickiesInPalette;
        toReturn += "\n";
        for (int i = 0; i < stickies.Count; i++)
        {
            bool isMain = (i < noOfStickiesInMain);
            toReturn += "<sticky>";
            toReturn += "\n";
            StickyScript ss = stickies[i].GetComponent<StickyScript>();
            toReturn += "onMain=" + (isMain ? 1 : 0);
            toReturn += "\n";
            toReturn += "imagePath=" + ss.imagePath;
            toReturn += "\n";
            if (savePhotos && ss.imagePath.Length > 0 && File.Exists(Application.dataPath + "/" + ss.imagePath))
            {
                if (!photoPaths.Contains(ss.imagePath))
                {
                    photoPaths.Add(ss.imagePath);
                }
            }

            if (isMain)
            {
                toReturn += "position=" + ss.transform.position.x + "," + ss.transform.position.y + "," + ss.transform.position.z;
                toReturn += "\n";
            }
            toReturn += "<stickyInfo>";
            toReturn += "\n";
            toReturn += ss.stickyInfoToString();
            toReturn += "\n";
            toReturn += "</stickyInfo>";
            toReturn += "\n";
            toReturn += "</sticky>";
            toReturn += "\n";
        }
        for (int i = 0; i < handles.Count; i++)
        {
            Transform h = handles[i].transform;
            toReturn += "handle=" + h.position.x + "," + h.position.y + "," + h.position.z;
            toReturn += "\n";
        }
        toReturn += "endOfHandles\n";
        //save their connections
        int noOfConnections = 0;
        List<int[]> connections = new List<int[]>();
        foreach (Transform child in instance.connections)
        {
            LineScript lineScript = child.GetComponent<LineScript>();
            int endpoint0Found = -1;
            int endpoint1Found = -1;
            for (int i = 0; i < nodes.Count; i++)
            {
                if (endpoint0Found == -1 && endpoint1Found == -1)
                {
                    if (lineScript.Endpoint0 == nodes[i])
                    {
                        endpoint0Found = i;
                    }
                    if (lineScript.Endpoint1 == nodes[i])
                    {
                        endpoint1Found = i;
                    }
                }
                else if (endpoint0Found != -1 && endpoint1Found == -1)
                {
                    if (lineScript.Endpoint1 == nodes[i])
                    {
                        endpoint1Found = i;
                        break;
                    }
                }
                else if (endpoint0Found == -1 && endpoint1Found != -1)
                {
                    if (lineScript.Endpoint0 == nodes[i])
                    {
                        endpoint0Found = i;
                        break;
                    }
                }
                else
                {
                    break;
                }

            }
            if (endpoint0Found != -1 && endpoint1Found != -1)
            {
                noOfConnections++;
                connections.Add(new int[] { noOfConnections, endpoint0Found, endpoint1Found });
            }
        }
        toReturn += "noOfConnections=" + noOfConnections;
        toReturn += "\n";
        for (int i = 0; i < connections.Count; i++)
        {
            toReturn += "connection=" + connections[i][0] + "," + connections[i][1] + "," + connections[i][2];
            toReturn += "\n";
        }
        lastSaveString = toReturn;
        return toReturn;
    }

    private enum LoadReadState { None, InSticky, Handles, Connections };
    public static void Load(bool selectStickies = false)
    {
        List<GameObject> stickiesToSelect = new List<GameObject>();

        nodes = new List<GameObject>();

        string[] lines = LoadedText.Split('\n');
        int noOfStickiesInMain = 0;
        int noOfStickiesInPalette = 0;
        int noOfConnections = 0;
        LoadReadState state = LoadReadState.None;
        string stickyData = "";
        int currentSticky = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            if (state == LoadReadState.None)
            {
                if (line.Contains("noOfStickiesInMain="))
                {
                    string intString = line.Remove(0, "noOfStickiesInMain=".Length);
                    noOfStickiesInMain = int.Parse(intString);
                }
                else if (line.Contains("noOfStickiesInPalette="))
                {
                    string intString = line.Remove(0, "noOfStickiesInPalette=".Length);
                    noOfStickiesInPalette = int.Parse(intString);
                }
                else if (line.Contains("<sticky>"))
                {
                    state = LoadReadState.InSticky;
                    stickyData = "";
                }
            }
            else if (state == LoadReadState.InSticky)
            {
                if (line.Contains("</sticky>"))
                {
                    GameObject sticky = LoadSticky(stickyData, currentSticky < noOfStickiesInMain);
                    if (selectStickies)
                    {
                        stickiesToSelect.Add(sticky);
                        Debug.Log(sticky.name + " " + UnityEngine.Random.value);
                    }
                    currentSticky++;
                    if (currentSticky >= noOfStickiesInMain + noOfStickiesInPalette)
                    {
                        state = LoadReadState.Handles;
                    }
                    else
                    {
                        state = LoadReadState.None;
                    }
                }
                else
                {
                    stickyData += line + "\n";
                }
            }
            else if (state == LoadReadState.Handles)
            {
                if (line.Contains("handle="))
                {
                    Vector3 pos = Vector3.zero;
                    string[] floatStrings = line.Remove(0, "handle=".Length).Split(',');
                    pos.x = float.Parse(floatStrings[0]);
                    pos.y = float.Parse(floatStrings[1]);
                    pos.z = float.Parse(floatStrings[2]);
                    GameObject h = Instantiate(instance.handlePrefab, pos, Quaternion.identity) as GameObject;
                    h.transform.parent = instance.handles;
                    nodes.Add(h);
                }
                else if (line.Contains("endOfHandles"))
                {
                    state = LoadReadState.Connections;
                }

            }
            else if (state == LoadReadState.Connections)
            {
                if (line.Contains("noOfConnections="))
                {
                    string intString = line.Remove(0, "noOfConnections=".Length);
                    noOfConnections = int.Parse(intString);
                }
                else if (line.Contains("connection="))
                {
                    string[] intStrings = line.Remove(0, "connection=".Length).Split(',');
                    int endpoint0Found = int.Parse(intStrings[1]);
                    int endpoint1Found = int.Parse(intStrings[2]);
                    var lineObject = Instantiate(instance.linePrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    LineScript lineScript = lineObject.GetComponent<LineScript>();
                    lineScript.transform.SetParent(SaveLoadManager.instance.connections);
                    lineScript.Endpoint0 = nodes[endpoint0Found];
                    lineScript.Endpoint1 = nodes[endpoint1Found];
                    if (lineScript.Endpoint0.GetComponent<LineScript>() != null)
                    {
                        lineScript.Endpoint0.GetComponent<LineScript>().Endpoint1 = lineScript.gameObject;
                    }
                    if (lineScript.Endpoint1.GetComponent<LineScript>() != null)
                    {
                        lineScript.Endpoint1.GetComponent<LineScript>().Endpoint0 = lineScript.gameObject;
                    }
                    lineScript.Go = true;
                }
            }
        }
        ConnectionManager.Reload();

        ftlGatherer.ActiveNotes = new List<GameObject>();
        for (int i = 0; i < stickiesToSelect.Count; i++)
        {
            ftlGatherer.ActiveNotes.Add(stickiesToSelect[i]);
        }
    }

    private static List<GameObject> nodes;
    private static GameObject LoadSticky(string stickyData, bool isMain)
    {
        bool inStickyInfo = false;
        string[] lines = stickyData.Split('\n');
        string stickyInfo = "";
        string imagePath = "";
        Vector3 pos = Vector3.zero;
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            if (inStickyInfo)
            {
                if (line.Contains("</stickyInfo>"))
                {
                    inStickyInfo = false;
                }
                else
                {
                    stickyInfo += line + "\n";
                }
            }
            else
            {
                if (line.Contains("<stickyInfo>"))
                {
                    stickyInfo = "";
                    inStickyInfo = true;
                }
                else if (line.Contains("imagePath="))
                {
                    imagePath = line.Remove(0, "imagePath=".Length);
                }
                else if (line.Contains("onMain="))
                {
                    string intString = line.Remove(0, "onMain=".Length);
                    if (isMain != (int.Parse(intString) == 1))
                    {
                        Debug.LogWarning("There's a problem.");
                    }
                }
                else if (line.Contains("position="))
                {
                    string[] floatStrings = line.Remove(0, "position=".Length).Split(',');
                    pos.x = float.Parse(floatStrings[0]);
                    pos.y = float.Parse(floatStrings[1]);
                    pos.z = float.Parse(floatStrings[2]);
                }
            }
        }

        if (isMain)
        {
            GameObject sticky = StickySender.instance.AddSticky(pos + CopyPasteManager.noOfPastesInARowCount * CopyPasteManager.offset);
            StickyScript lastCreatedSticky = sticky.GetComponent<StickyScript>();
            lastCreatedSticky.readStickyInfo(stickyInfo);
            if (imagePath != "")
            {
                lastCreatedSticky.imagePath = imagePath;
                Texture2D texture = DesktopDownloader.LoadPNG(saveLoadMediaFolder + imagePath);
                lastCreatedSticky.putPhoto(texture, true);
            }
            nodes.Add(sticky);
            return sticky;
        }
        else
        {
            GameObject sticky = NewStickyButton.instance.CreateSticky();
            StickyScript lastCreatedSticky = sticky.GetComponent<StickyScript>();
            lastCreatedSticky.readStickyInfo(stickyInfo);
            lastCreatedSticky.GetComponent<StickyInPanel>().sendToPalette(GameObject.Find("StickyPalette").GetComponent<StickyPalette>());
            if (imagePath != "")
            {
                lastCreatedSticky.imagePath = imagePath;
                Texture2D texture = DesktopDownloader.LoadPNG(saveLoadMediaFolder + "/" + imagePath);
                lastCreatedSticky.putPhoto(texture, true);
            }
            return sticky;
        }
    }

    void setSaveLoadFolder()
    {
        saveLoadFolder = "";
        char[] delimiters = new char[] { '\\', '/' };
        string[] parts = saveLoadPath.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < parts.Length - 1; i++)
        {
            saveLoadFolder += parts[i] + "/";
        }
        saveLoadMediaFolder = saveLoadFolder + parts[parts.Length - 1] + "_Media/";
        if (!Directory.Exists(saveLoadMediaFolder))
        {
            Directory.CreateDirectory(saveLoadMediaFolder);
        }
    }

    public static string getDataPathLastPart()
    {
        if (dataPathLastPart == "")
        {
            char[] delimiters = new char[] { '\\', '/' };
            string[] parts = Application.dataPath.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 1)
            {
                dataPathLastPart = parts[parts.Length - 2] + "/";
            }
        }
        return dataPathLastPart;
    }

    void ResetUndoMoves()
    {
        undoStrings = new string[NoOfUndos];
        for (int i = 0; i < NoOfUndos; i++)
        {
            undoStrings[i] = "";
        }
        UndoStrings = new string[NoOfUndos];
        for (int i = 0; i < NoOfUndos; i++)
        {
            UndoStrings[i] = "";
        }
    }

    public static void MakeAMove(string debugMessage = "")
    {
        if (!undoRedoInProgress && SceneManager.isDesktopScene)
        {
            if (undoStrings == null)
            {
                Debug.Log("undoStrings == null");
            }
            if (undoStrings[undoIndex] == null)
            {
                Debug.Log("undoStrings[undoIndex] == null");
            }
            undoIndex++;
            if (undoIndex >= NoOfUndos)
            {
                undoIndex = 0;
            }
            undoStrings[undoIndex] = SaveString(false);
            instance.UndoStrings[undoIndex] = undoStrings[undoIndex];
            int tempUndoIndex = undoIndex;
            for (int i = 0; i < 5; i++)
            {
                tempUndoIndex++;
                if (tempUndoIndex >= NoOfUndos)
                {
                    tempUndoIndex = 0;
                }
                undoStrings[tempUndoIndex] = "";
                instance.UndoStrings[tempUndoIndex] = "";
            }

            tempSaveMoveCounter++;

            //Debug.Log("MakeAMove " + undoIndex + " " + Time.realtimeSinceStartup + " " + debugMessage);

            UDPManager.send(lastSaveString, true);
        }
    }

    public static void Undo()
    {
        undoRedoInProgress = true;
        int tempUndoIndex = undoIndex;
        tempUndoIndex--;
        if (tempUndoIndex < 0)
        {
            tempUndoIndex = NoOfUndos - 1;
        }
        if (undoStrings[tempUndoIndex] != "")
        {
            undoIndex = tempUndoIndex;
            LoadedText = undoStrings[undoIndex];
            instance.loadedText = undoStrings[undoIndex];
            Reset();
            Load();
            Debug.Log("Undo " + undoIndex + " " + Time.realtimeSinceStartup);
        }
        undoRedoInProgress = false;
    }

    public static void Redo()
    {
        undoRedoInProgress = true;
        int tempUndoIndex = undoIndex + 1;
        if (tempUndoIndex >= NoOfUndos)
        {
            tempUndoIndex = 0;
        }
        if (undoStrings[tempUndoIndex] != "")
        {
            undoIndex = tempUndoIndex;
            LoadedText = undoStrings[undoIndex];
            instance.loadedText = undoStrings[undoIndex];
            Reset();
            Load();
            Debug.Log("Redo " + undoIndex + " " + Time.realtimeSinceStartup);
        }
        undoRedoInProgress = false;
    }
}
