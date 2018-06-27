using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMenuManager : MonoBehaviour {
    public List<GameObject> objectPrefabList; //set manually in inspector 
    public float scaleMenu = 0.3f;
    public GameObject objectRemoveHolder = null;   //place holder to remove an object
    protected int currentObject = 0;
    protected int idxTest = 0;  //index of test object
    protected List<GameObject> objectList; //handled automatically at start

	// Use this for initialization
	void Start () {
        objectList = new List<GameObject>();
        bool bFoundFirst = false;

        foreach (GameObject obj in objectPrefabList)
        {
            GameObject objNew = Instantiate(obj, gameObject.transform); //add with menu as parent
            Rigidbody rigid = objNew.GetComponent<Rigidbody>();
            if (rigid)
            {
                rigid.useGravity = false;
                rigid.isKinematic = true;
                objNew.tag = "Untagged"; // prevent grabbing of object
            }
            objNew.transform.localPosition = Vector3.zero;
            //objNew.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);

            Collider collider = objNew.GetComponent<Collider>();
            if (collider) 
            {
                collider.enabled = false;
            }

            if (bFoundFirst) {
                objNew.SetActive(false);
            }
            bFoundFirst = true;
            //objNew.transform.localScale = new Vector3(scaleMenu, scaleMenu, scaleMenu);
            objectList.Add(objNew); //save for use later
        }
        MenuHide();
	}

    public void MenuShow(GameObject objTarget=null) 
    {
        if (objTarget != null) 
        {
            //do nothing for now, but don't show full menu
            //StartCoroutine (FadeOut3D(objTarget.transform, 0.2f, false, 0.5f));
            return;
        }
        gameObject.SetActive(true);
    }

    public void MenuHide(GameObject objTarget=null) 
    {
        if (objTarget != null) 
        {
            //do nothing for now because other object wasn't dimmed
            // StartCoroutine (FadeOut3D(objTarget.transform, 1.0f, false, 0.25f));
            return;
        }
        gameObject.SetActive(false);
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
        if (!gameObject.activeSelf) {
            return;
        }
        if (objectPrefabList[currentObject].tag == "Hints")
        {
            Debug.Log("HintS");

            //have a special type of release that may trigger other items!
            //normalPlay
        }
        else
        {
            Instantiate(objectPrefabList[currentObject], 
                objectList[currentObject].transform.position, 
                objectList[currentObject].transform.rotation);
        }

    }

    public void DeleteCurrentObject(GameObject objTarget) 
    {
        if (!objTarget) 
        {
            return;
        }
        Destroy(objTarget);
    }
    
    protected static IEnumerator FadeOut3D (Transform t, float targetAlpha, bool isVanish, float duration)
    {
        //https://answers.unity.com/questions/992672/fade-gameobject-alpha-with-standard-shader.html#
        Renderer sr = t.GetComponent<Renderer> ();
        float diffAlpha = (targetAlpha - sr.material.color.a);
 
        float counter = 0;
        while (counter < duration) 
        {
            float alphaAmount = sr.material.color.a + (Time.deltaTime * diffAlpha) / duration;
            sr.material.color = new Color (sr.material.color.r, sr.material.color.g, sr.material.color.b, alphaAmount);
              counter += Time.deltaTime;
            yield return null;
        }
        sr.material.color = new Color (sr.material.color.r, sr.material.color.g, sr.material.color.b, targetAlpha);
        if (isVanish) 
        {
            sr.transform.gameObject.SetActive (false);
        }
    }
}
