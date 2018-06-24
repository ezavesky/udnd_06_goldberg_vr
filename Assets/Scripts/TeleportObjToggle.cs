using UnityEngine;
using System.Collections.Generic;

public class TeleportObjToggle : MonoBehaviour
{
    public GameObject[] objects = new GameObject[0];
    public string[] retoggleTags = new string[0];
    public AudioClip clipStart = null;
    public AudioClip clipHold = null;

    void Start() 
    {
        List<GameObject > listObjs = new List<GameObject>();
        for (int i=0; i<retoggleTags.Length; i++) {
            GameObject[] addObjs = GameObject.FindGameObjectsWithTag(retoggleTags[i]);
            listObjs.AddRange(addObjs);
        }
        objects = listObjs.ToArray();
        ToggleObjects(false);
    }

    public virtual void ToggleObjects(bool newState)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] != null)
            {
                objects[i].SetActive(newState);
            }
        }
        if (newState) {
            if (clipStart) 
            {
            

            }
        }

    }

    
}
