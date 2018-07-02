using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class FanController : SoundCollider {
    public GameObject objBlade = null;
    public Collider colliderWind = null;
    public GameObject objBody = null;
    public float smooth = 300.0f;
    public float forceMultiplier = 80f;

	// Use this for initialization
	void Start () {
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
            Vector3 vectForce = objBody.transform.forward * fMagnitude * forceMultiplier;
            //Debug.Log(string.Format("[FanController]: Dist {0}, Size {1}, Multiplier {2}, Diff {3}, Force {4}", fDistance, fSize, forceMultiplier, fMagnitude, vectForce));
            Rigidbody rb = objOther.GetComponent<Rigidbody>();
            if (rb != null) 
            {
                rb.AddForce(vectForce);
            }
        }
	}

}
