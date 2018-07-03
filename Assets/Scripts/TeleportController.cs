using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class TeleportController : SoundCollider {
    public VRTK_InteractableObject interactObj;
    protected TeleportController teleportClosest = null;

    protected GameObject objRings = null;
    protected bool isEchoing = false;
    public AudioClip soundInactive = null;
    public float durationEchoPulse = 1.0f;
    
	// Use this for initialization
	void Start () {
        if (gameObject.transform.childCount != 0)   // attempt to get rotating ring child
        {
            objRings = gameObject.transform.GetChild(0).gameObject;
            objRings.SetActive(false);
            isEchoing = false;
        }
        if (interactObj == null)
        {
            interactObj = GetComponent<VRTK_InteractableObject>();
        }
        if (interactObj) 
        {
            interactObj.InteractableObjectGrabbed += new InteractableObjectEventHandler(OnInteractableObjectGrabbed);
            interactObj.InteractableObjectUngrabbed += new InteractableObjectEventHandler(OnInteractableObjectUngrabbed);
        }
        FindClosestTeleport();
	}
	
	// turn on or off the echo
	protected void ToggleEcho(bool bNewState = false) 
    {
        if (objRings != null)
        {
            objRings.SetActive(bNewState);
        }
	}

    //turn on echo and turn off after a fixed amount of time
    protected IEnumerator PulseEcho(GameObject objOther) 
    {
        Physics.IgnoreCollision(objOther.GetComponent<Collider>(), 
                                gameObject.GetComponent<Collider>(), true);
        ToggleEcho(true);
        yield return new WaitForSeconds(durationEchoPulse);

        Physics.IgnoreCollision(objOther.GetComponent<Collider>(), 
                                gameObject.GetComponent<Collider>(), false);
        ToggleEcho(false);
    }

    protected void FindClosestTeleport()
    {
        teleportClosest = null;
        float fDistMin = 0f;
        float fDistLocal = 0f;
        foreach (TeleportController teleportCompare in interactObj.transform.parent.GetComponentsInChildren<TeleportController>()) 
        {
            fDistLocal = Vector3.Distance(teleportCompare.gameObject.transform.position, gameObject.transform.position);
            if ((teleportClosest==null || fDistLocal < fDistMin)  // closest teleport
                && (teleportCompare.gameObject.GetInstanceID()!=gameObject.GetInstanceID())) // and not self
            {
                fDistMin = fDistLocal;
                teleportClosest = teleportCompare;
            }
        }
    }

    protected void OnInteractableObjectGrabbed(object sender, InteractableObjectEventArgs args) 
    {
        FindClosestTeleport();  //when grabbed, search for closest teleport object within same parent
        if (teleportClosest != null)            //if one found, trigger it's 'rotating' state
        {
            teleportClosest.ToggleEcho(true);
        }
        ToggleEcho(true);
    }

    protected void OnInteractableObjectUngrabbed(object sender, InteractableObjectEventArgs args) 
    {
        if (teleportClosest != null)            //release/revert state of other rotating object
        {
            teleportClosest.ToggleEcho(false);
        }
        ToggleEcho(false);
    }

	protected override void OnHit(AudioSource audioSrc, GameObject objOther) {
        if (teleportClosest != null) 
        {
            Rigidbody rb = objOther.GetComponent<Rigidbody>();
            if (rb != null && rb.velocity.magnitude > 0.1f)     // don't teleport if just on object! 
            {
                StartCoroutine(teleportClosest.PulseEcho(objOther));

                //reverse velocity for new forward direction
                //rb.velocity = teleportClosest.gameObject.transform.forward * rb.velocity.magnitude;

                //  http://answers.unity.com/comments/427002/view.html
                Vector3 velocity = rb.velocity;
                velocity = Vector3.Reflect(velocity, teleportClosest.transform.forward);
                velocity = transform.InverseTransformDirection(velocity);
                velocity = teleportClosest.gameObject.transform.TransformDirection(velocity);
                Debug.Log(string.Format("[TeleportController]: Velocity before {0}, after {1}", rb.velocity, velocity));
                rb.velocity = velocity;

                //get position of destination
                objOther.transform.position = teleportClosest.gameObject.transform.position; //objOther.transform.position - transform.position + teleportClosest.gameObject.transform.position;

                base.OnHit(audioSrc, objOther); //next apply good hit sound
                StartCoroutine(PulseEcho(objOther));
                return;
            }
        }
        PlayClip(audioSrc, soundInactive);      // didn't do anything, play dud sound
	}

}
