﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalController : SoundCollider {
    public bool BallSticks = true;

	protected override void OnHit(AudioSource audioSrc, GameObject objOther) {
		base.OnHit(audioSrc, objOther);
		Rigidbody rb = objOther.GetComponent<Rigidbody>();
		if (rb != null && BallSticks) {
			rb.isKinematic = true;
			rb.angularVelocity = Vector3.zero;
			rb.velocity = Vector3.zero;

            //valid task, enable final teleport!
		}
	}

}
