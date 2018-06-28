using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using VRTK;

public class StartingBallController : MonoBehaviour {

    public VRTK_InteractableObject ballInteraction = null;
    protected GameObject objPrefab = null;

	// Use this for initialization
	void Start () {
		if (ballInteraction != null) {  
            objPrefab = ballInteraction.gameObject;  //make this our prefab object
            objPrefab.GetComponent<Rigidbody>().isKinematic = true;
            objPrefab.SetActive(false);   
            CloneNewStart();    //replace with new "enhanced" object
        }
	}
	
    protected GameObject CloneNewStart() 
    {
        if (objPrefab != null) // user just took our existing ball, clone another in its place (and enable it)
        {
            GameObject objNew = Instantiate(objPrefab, objPrefab.transform.parent);
            objNew.GetComponent<VRTK_InteractableObject>().InteractableObjectUngrabbed += new InteractableObjectEventHandler(OnObjectUngrab);
            objNew.GetComponent<VRTK_InteractableObject>().InteractableObjectGrabbed += new InteractableObjectEventHandler(OnObjectGrab);
            objNew.GetComponent<Rigidbody>().isKinematic = true;
            objNew.SetActive(true);
            return objNew;
        }
        return null;
    }
    
    protected void OnObjectGrab(object sender, InteractableObjectEventArgs e) {
        VRTK_InteractableObject objInteract = (VRTK_InteractableObject)sender;
        // a little bit of fix to make our grabbed object non-kinematic in our hand
        Transform previousParent;
        bool previousGrabbable;
        bool previousKinematic;
        objInteract.GetPreviousState(out previousParent, out previousKinematic, out previousGrabbable);
        objInteract.OverridePreviousState(previousParent, false, previousGrabbable);
        objInteract.isKinematic = false;
        objInteract.gameObject.GetComponent<VRTK_InteractableObject>().InteractableObjectGrabbed -= new InteractableObjectEventHandler(OnObjectGrab);
    }

    protected void OnObjectUngrab(object sender, InteractableObjectEventArgs e) {
        GameManager.instance.state = GameManager.GAME_STATE.STATE_NORMAL;  // pulled the object we care about, normal state!
        VRTK_InteractableObject objInteract = (VRTK_InteractableObject)sender;
        CloneNewStart();
        // now remove components that make this ball "special"
        objInteract.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        objInteract.gameObject.GetComponent<VRTK_InteractableObject>().InteractableObjectUngrabbed -= new InteractableObjectEventHandler(OnObjectUngrab);
        GameManager.instance.RegisterSingletonBall(objInteract.gameObject);  // register the new "valid" ball as the singleton
    }

}
