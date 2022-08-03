using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class Witality_InstanceMaker : MonoBehaviour
{
    public bool executeScript;
    //[SerializeField]  public string prefabPath = "prefabs";
    public GameObject prefabObject;

    void Start()
    {
        
    }


    void Update()
    {
        if (executeScript)
        {
            executeScript = false;
            CreateInstances();
        }

    }

    void CreateInstances()
    {
        Transform[] allChildren = transform.GetComponentsInChildren<Transform>();

        IDictionary<string, GameObject> foundItems = new Dictionary<string, GameObject>();
        
        foreach (Transform child in allChildren)
        {
            GameObject go = child.gameObject;

            //string[] subs = go.name.Split('_');
            //if (subs[subs.Length - 1] == "inst" )
            //{
            Transform old_parent = child.parent;
            //if (foundItems.ContainsKey(subs[0]))
            //{

            GameObject foundPrefab = prefabObject;// foundItems[subs[0]];
            Vector3 pos = child.localPosition;
            Quaternion rot = child.localRotation;
            Vector3 scl = child.localScale;

            GameObject newInstance = PrefabUtility.InstantiatePrefab(foundPrefab) as GameObject;// new Vector3(0, 0, 0), Quaternion.identity);
            // newInstance.name = subs[0] + "_" + subs[1];
            newInstance.transform.parent = old_parent;
            newInstance.transform.localPosition = pos;
            newInstance.transform.localRotation = rot;
            newInstance.transform.localScale = scl;
            //DestroyImmediate(go);
                    
            //}
            //else
            //{
            //    string localPath = "Assets/" + prefabPath + "/" + subs[0] + ".prefab";
            //    Debug.Log(localPath);
            //    //localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
            //    GameObject prefab_go = PrefabUtility.SaveAsPrefabAssetAndConnect(go, localPath, InteractionMode.UserAction);
                //   foundItems[subs[0]] = prefab_go;
                //}
            //}
            
        }

    }
}
