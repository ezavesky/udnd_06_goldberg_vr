using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class TrampolineController : SoundCollider {
    public float forceMultiplier = 80f;

	protected override void OnHit(AudioSource audioSrc, GameObject objOther) {
		base.OnHit(audioSrc, objOther);

        /*
        float fDistance = Vector3.Distance(objOther.transform.position, gameObject.transform.position);
        float fSize = colliderWind.bounds.size.magnitude;
        float fMagnitude = Mathf.Max(0.0f, fSize-fDistance);
        Vector3 vectForce = gameObject.transform.forward * fMagnitude * forceMultiplier;
        //Debug.Log(string.Format("[FanController]: Dist {0}, Size {1}, Multiplier {2}, Diff {3}, Force {4}", fDistance, fSize, fanMultiplier, fMagnitude, vectForce));
        Rigidbody rb = objOther.GetComponent<Rigidbody>();
        if (rb != null) 
        {
            rb.AddForce(vectForce);
        }
        */
	}


}
