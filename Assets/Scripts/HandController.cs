using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
public class HandController : MonoBehaviour {
    public float throwForce = 1.5f;

    //Swipe
    protected float swipeSum;
    protected float distance;
    protected bool hasSwipedLeft;
    protected bool hasSwipedRight;

    protected bool isTouched;
    private Vector2 touchAxis;
    private Vector2 touchAxisLast = Vector2.zero;
    private VRTK_InteractGrab grabController = null;

    public ObjectMenuManager objectMenuManager = null;

	// Use this for initialization
	void Start () {
        touchAxis = touchAxisLast = Vector2.zero;
        hasSwipedLeft = hasSwipedRight = isTouched = false;

        grabController = GetComponent<VRTK_InteractGrab>();
        VRTK_ControllerEvents eventListener = GetComponent<VRTK_ControllerEvents>();
        eventListener.TouchpadAxisChanged += new ControllerInteractionEventHandler(DoTouchpadAxisChanged);

        // eventListener.TriggerReleased += new ControllerInteractionEventHandler(DoTriggerReleased);
        eventListener.TouchpadTouchEnd += new ControllerInteractionEventHandler(DoTouchpadTouchEnd);
        eventListener.TouchpadTouchStart += new ControllerInteractionEventHandler(DoTouchpadTouchStart);
        eventListener.TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadPress);

        eventListener.GripPressed += new ControllerInteractionEventHandler(DoGripPress);
    }

    // catch events for touch and controller
    public void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
    {
        ReleaseTouch();
    }

    public void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
    {
        SetTouchAxis(e.touchpadAxis);
    }

    public void DoTouchpadTouchStart(object sender, ControllerInteractionEventArgs e)
    {
        SetTouchAxis(Vector2.zero);
    }

    public void DoGripPress(object sender, ControllerInteractionEventArgs e)
    {
        SwipeRight();
    }

    public void DoTouchpadPress(object sender, ControllerInteractionEventArgs e)
    {
        if (grabController) 
        {
            GameObject objDelete = grabController.GetGrabbedObject();
            if (objDelete) 
            {
                DeleteCurrentObject(objDelete);
                return;
            }
        }
        //if we didn't have an object in hand, skip
        SpawnCurrentObject();
    }

    // protected events to process changes in touch pad
    private void SetTouchAxis(Vector2 data)
    {
        touchAxis = data;
        isTouched = true;
        GameObject objGrabbed = null;
        if (grabController) 
        {
            objGrabbed = grabController.GetGrabbedObject();
        }
        objectMenuManager.MenuShow(objGrabbed);
    }   
    
    private void ReleaseTouch()
    {
        isTouched = false;
        swipeSum = 0;
        hasSwipedLeft = false;
        hasSwipedRight = false;
        SetTouchAxis(Vector2.zero);
        objectMenuManager.MenuHide();
    }

    private void SpawnCurrentObject()
    {
        objectMenuManager.SpawnCurrentObject();
    }
    private void DeleteCurrentObject(GameObject objTarget)
    {
        objectMenuManager.DeleteCurrentObject(objTarget);
    }

    private void SwipeLeft()
    {
        objectMenuManager.MenuLeft();
        hasSwipedLeft = true;
        // Debug.Log("SwipeLeft");
    }
    private void SwipeRight()
    {
        objectMenuManager.MenuRight();
        hasSwipedRight = true;
        // Debug.Log("SwipeRight");
    }
    private void SwipeCompute()
    {
        distance = touchAxis.x - touchAxisLast.x;
        touchAxisLast = touchAxis;
        swipeSum += distance;
        //Debug.Log(string.Format("D:{0}, sum:{1}, left:{2}, right:{3}", distance, swipeSum, hasSwipedLeft, hasSwipedRight));
        if (!hasSwipedLeft && !hasSwipedRight) 
        {
            if (swipeSum > 0.5f)
            {
                SwipeRight();
            }
            else if (swipeSum < -0.5f)
            {
                SwipeLeft();
            }
        }
        else if (swipeSum < 0.1 && swipeSum > -0.1) { //return to center
            hasSwipedLeft = hasSwipedRight = false;
        }
    }

    // Update is called once per frame
    void Update () {
        if (isTouched) 
        {
            SwipeCompute();
        }
	}

    /*
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
    */
}
