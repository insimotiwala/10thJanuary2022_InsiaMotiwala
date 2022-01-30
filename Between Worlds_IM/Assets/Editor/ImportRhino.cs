using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ImportRhino : EditorWindow
{
    private UnityEngine.TextAsset csvFile = null;

    [MenuItem("Window/Rhino/Import")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<ImportRhino>("Import Rhino");
    }

    private void OnGUI()
    {
        //Window Code
        GUILayout.Label("RhinoToUnity *.csv File", EditorStyles.boldLabel);

        //csvFile = EditorGUILayout.TextField("File", csvFile);
        csvFile = EditorGUILayout.ObjectField(csvFile, typeof(UnityEngine.TextAsset), false) as UnityEngine.TextAsset;

        if (GUILayout.Button("Import"))
        {
            Debug.Log("Import Started");
            //Debug.Log(file.ToString());
            ImportRhinoCSV(csvFile);
            Debug.Log("Import Complete!");
        }
    }

    private void ImportRhinoCSV(TextAsset textAsset)
    {
        //make sure the text asset isn't empty
        if (textAsset == null) { return; }

        //get lines and make sure there are lines
        string text = textAsset.text;
        string[] lines = text.Split(new string[] { System.Environment.NewLine }, System.StringSplitOptions.None);
        if (lines.Length <= 1) { return; } //make sure there are lines

        //make sure all the expected headers exist
        //Layer, PositionX, PositionY, PositionZ, ForwardX, ForwardY, ForwardZ, NormalX, NormalY, NormalZ
        int d = -1;
        int layer, posX, posY, posZ, fwdX, fwdY, fwdZ, upX, upY, upZ; //csv column indices
        layer = posX = posY = posZ = fwdX = fwdY = fwdZ = upX = upY = upZ = d;
        string[] headers = lines[0].Split(',');
        for (int i = 0; i < headers.Length; i++)
        {
            string header = headers[i];
            if (header == "Layer") { layer = i; }
            else if (header == "PositionX") { posX = i; }
            else if (header == "PositionY") { posY = i; }
            else if (header == "PositionZ") { posZ = i; }
            else if (header == "ForwardX") { fwdX = i; }
            else if (header == "ForwardY") { fwdY = i; }
            else if (header == "ForwardZ") { fwdZ = i; }
            else if (header == "NormalX") { upX = i; }
            else if (header == "NormalY") { upY = i; }
            else if (header == "NormalZ") { upZ = i; }
        }
        if (layer == d || posX == d || posY == d || posZ == d || fwdX == d || fwdY == d || fwdZ == d || upX == d || upY == d || upZ == d)
        { Debug.Log("Incorrect Headers"); return; }

        //get all prefabs form the project
        Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();
        string[] prefabGuids = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets" });
        foreach (string guid in prefabGuids)
        {
            //Debug.Log(guid);
            var path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefabs.ContainsKey(go.name)) { continue; }
            prefabs.Add(go.name, go);
        }

        //loop through each line of the csv and instantiate the game object
        //the layer path creates the parent hierarchy with the last layer name matching that of the prefab to instantiate
        Dictionary<string, GameObject> parents = new Dictionary<string, GameObject>();
        for (int i = 1; i < lines.Length; i++)
        {
            //get data
            string line = lines[i];
            string[] obj = line.Split(',');
            if (obj.Length < headers.Length) { continue; }

            //get/create parents
            GameObject parent = null;
            string path = obj[layer];
            string[] pathParents = path.Split(':');
            if (pathParents.Length >= 3) //if there is at least two layers
            {
                string parentPath = "";

                for (int p = 0; p < pathParents.Length; p++)
                {
                    //Debug.Log(parentPath);
                    string parentName = pathParents[p];
                    if (string.IsNullOrEmpty(parentName)) { continue; }
                    parentPath += parentName;

                    //if parent already exists
                    if (parents.ContainsKey(parentPath))
                    {
                        parent = parents[parentPath];
                        parentPath += "::";
                        continue;
                    }

                    //instantiate new parent and set tree
                    GameObject go = new GameObject(parentName);
                    if (parent != null) { go.transform.parent = parent.transform; }
                    parents.Add(parentPath, go);
                    parent = go;
                    parentPath += "::";
                }
            }

            //get prefab or make empty game object
            GameObject prefab = null;
            string name = pathParents[pathParents.Length - 1];
            if (prefabs.ContainsKey(name)) { prefab = prefabs[name]; }

            //get vectors from parsed data
            try
            {
                Vector3 position = new Vector3(float.Parse(obj[posX]), float.Parse(obj[posY]), float.Parse(obj[posZ]));
                Vector3 forward = new Vector3(float.Parse(obj[fwdX]), float.Parse(obj[fwdY]), float.Parse(obj[fwdZ]));
                Vector3 up = new Vector3(float.Parse(obj[upX]), float.Parse(obj[upY]), float.Parse(obj[upZ]));
                InstantiateGameObject(prefab, position, forward, up, parent, name);
            }
            catch
            {
                Debug.Log("Import Rhino Error");
            }
        }
    }

    private GameObject InstantiateGameObject(GameObject prefab, Vector3 position, Vector3 forward, Vector3 up, GameObject parent, string name)
    {
        //instantiate gameobject
        GameObject obj = null;
        if (prefab == null)
        {
            obj = new GameObject(name);
        }
        else
        {
            //obj = Instantiate(prefab, position, Quaternion.identity);
            obj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        }

        //set object position
        obj.transform.position = position;
        if (parent != null) { obj.transform.parent = parent.transform; }
        obj.transform.rotation.SetLookRotation(forward, up);
        return obj;
    }
}