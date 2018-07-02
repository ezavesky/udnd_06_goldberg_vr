using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class FanController : SoundCollider {
    protected VRTK_InteractableObject grabInteraction = null;
    public GameObject objBlade = null;
    public Collider colliderWind = null;
    public GameObject objBody = null;
    public float smooth = 300.0f;
    public float fanMultiplier = 2f;

	// Use this for initialization
	void Start () {
        grabInteraction = GetComponent<VRTK_InteractableObject>();
		if (grabInteraction != null) {  
            grabInteraction.InteractableObjectUngrabbed += new InteractableObjectEventHandler(OnObjectUngrab);
        }
        triggerStay = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (objBlade != null)   // spin the blade
        {
            objBlade.transform.Rotate(Vector3.forward * Time.deltaTime * smooth, Space.Self);
        }
	}

	protected override void OnHit(AudioSource audioSrc, GameObject objOther) {
		base.OnHit(audioSrc, objOther);
        if (objBody != null && colliderWind != null) 
        {
            float fDistance = Vector3.Distance(objOther.transform.position, objBody.transform.position);
            float fSize = colliderWind.bounds.size.magnitude;
            float fMagnitude = Mathf.Max(0.0f, fSize-fDistance);
            Vector3 vectForce = objBody.transform.forward * fMagnitude * fanMultiplier;
            //Debug.Log(string.Format("[FanController]: Dist {0}, Size {1}, Multiplier {2}, Diff {3}, Force {4}", fDistance, fSize, fanMultiplier, fMagnitude, vectForce));
            Rigidbody rb = objOther.GetComponent<Rigidbody>();
            if (rb != null) 
            {
                rb.AddForce(vectForce);
            }
        }
        //GameManager.instance.CaptureColletable(gameObject);
	}

    protected void OnObjectUngrab(object sender, InteractableObjectEventArgs e) {
        GameManager.instance.state = GameManager.GAME_STATE.STATE_NORMAL;  // pulled the object we care about, normal state!
        VRTK_InteractableObject objInteract = (VRTK_InteractableObject)sender;
    }
}
