using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMenuManager : MonoBehaviour {
    public List<GameObject> objectPrefabList; //set manually in inspector 
    public float scaleMenu = 0.3f;

    protected int currentObject = 0;                                       
    protected List<GameObject> objectList; //handled automatically at start
	// Use this for initialization
	void Start () {
		foreach(GameObject obj in objectPrefabList)
        {
            GameObject objNew = Instantiate(obj, gameObject.transform); //add with menu as parent
            objNew.transform.localScale = new Vector3(scaleMenu, scaleMenu, scaleMenu);
            objectList.Add(objNew); //save for use later
        }
	}

    public void MenuLeft()
    {
        objectList[currentObject].SetActive(false);
        currentObject--;
        if(currentObject < 0)
        {
            currentObject = objectList.Count - 1;
        }
        objectList[currentObject].SetActive(true);
    }
    public void MenuRight()
    {
        objectList[currentObject].SetActive(false);
        currentObject++;
        if (currentObject > objectList.Count - 1)
        {
            currentObject = 0;
        }
        objectList[currentObject].SetActive(true);
    }
    public void SpawnCurrentObject()
    {
        Instantiate(objectPrefabList[currentObject], 
            objectList[currentObject].transform.position, 
            objectList[currentObject].transform.rotation);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
