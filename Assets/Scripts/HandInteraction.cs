using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandInteraction : MonoBehaviour {
    protected SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;
    public float throwForce = 1.5f;

    //Swipe
    protected float swipeSum;
    protected float touchLast = 0;
    protected float touchCurrent;
    protected float distance;
    protected bool hasSwipedLeft;
    protected bool hasSwipedRight;
    public ObjectMenuManager objectMenuManager = null;

	// Use this for initialization
	void Start () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        device = SteamVR_Controller.Input((int)trackedObj.index);
    }

    // Update is called once per frame
    void Update () {
        if (objectMenuManager) {
            if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad))
            {
                //SteamVR_LoadLevel.Begin("Scene1");
                touchLast = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
            }
            else if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad))
            {
                swipeSum = 0;
                touchCurrent = 0;
                touchLast = 0;
                hasSwipedLeft = false;
                hasSwipedRight = false;
            }
            else if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
            {
                //Spawn object currently selected by menu
                SpawnObject();
            }
            else if (device.GetTouch(SteamVR_Controller.ButtonMask.Touchpad))
            {         
                touchCurrent = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
                distance = touchCurrent - touchLast;
                touchLast = touchCurrent;
                swipeSum += distance;
                if (!hasSwipedRight)
                {
                    if (swipeSum > 0.5f)
                    {
                        swipeSum = 0;
                        SwipeRight();
                        hasSwipedRight = true;
                        hasSwipedLeft = false;
                    }
                }
                if (!hasSwipedLeft)
                {
                    if (swipeSum < -0.5f)
                    {
                        swipeSum = 0;
                        SwipeLeft();
                        hasSwipedLeft = true;
                        hasSwipedRight = false;
                    }
                }            
            }            
        }
	}

    void SpawnObject()
    {
        objectMenuManager.SpawnCurrentObject();
    }

    void SwipeLeft()
    {
        objectMenuManager.MenuLeft();
        Debug.Log("SwipeLeft");
    }
    void SwipeRight()
    {
        objectMenuManager.MenuRight();
        Debug.Log("SwipeRight");
    }
    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.CompareTag("Throwable") || col.gameObject.CompareTag("Collectable"))
        {
            if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                ThrowObject(col);    
            }
            else if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                GrabObject(col);
            }
        }
    }
    
    void GrabObject(Collider coli)
    {
        coli.transform.SetParent(gameObject.transform);
        coli.GetComponent<Rigidbody>().isKinematic = true;
        device.TriggerHapticPulse(2000);
        Debug.Log("You are touching down the trigger on an object");
    }

    void ThrowObject(Collider coli)
    {
        coli.transform.SetParent(null);
        Rigidbody rigidBody = coli.GetComponent<Rigidbody>();
        bool isStatic = !coli.gameObject.CompareTag("Throwable");
        rigidBody.isKinematic = isStatic;
        rigidBody.useGravity = !isStatic;
        rigidBody.velocity = device.velocity * throwForce;
        rigidBody.angularVelocity = device.angularVelocity;
        Debug.Log("You have released the trigger");
    }
}
