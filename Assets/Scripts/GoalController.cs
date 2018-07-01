﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class GoalController : SoundCollider {
    public bool BallSticks = true;
    public GameObject objParentCollectables = null;
    public VRTK_DestinationPoint objTeleportFinal = null;
    public VRTK_DestinationPoint objTeleportInitial = null;
    public string nameSceneNext = "";
    protected float delayTeleport = 5.0f;

    public void RediscoverCollectables() {
        GameManager.instance.RediscoverCollectables(objParentCollectables);
    }

	protected override void OnHit(AudioSource audioSrc, GameObject objOther) {
        if (!GameManager.instance.finishedLevel) 
            return;

		base.OnHit(audioSrc, objOther);
		Rigidbody rb = objOther.GetComponent<Rigidbody>();
		if (rb != null && BallSticks) {
			rb.isKinematic = true;
			rb.angularVelocity = Vector3.zero;
			rb.velocity = Vector3.zero;
            Invoke("TeleportFinal", delayTeleport);
		}
	}

    public void TeleportUser(bool bInitial) {
        //valid task, enable final teleport!
        VRTK_DestinationPoint teleportDestination = bInitial ? objTeleportInitial : objTeleportFinal;
        if (teleportDestination == null) {
            Debug.LogWarning(string.Format("[GoalController]: Attmpted to teleport (initial: {0}), but no value set.", bInitial));
            return;
        }
        DoTeleport(teleportDestination);
    }

    protected void TeleportFinal() 
    {
        TeleportUser(false);        // teleport directly
        GameManager.instance.LoadNewScene(nameSceneNext);       //load next scene
    }

    protected void DoTeleport(VRTK_DestinationPoint teleportDestination) {
        // get destination point from editor/other - VRTK_DestinationPoint objTeleportFinal
        VRTK_BasicTeleport teleportBasic = VRTK_ObjectCache.registeredTeleporters.Count > 0 ? VRTK_ObjectCache.registeredTeleporters[0] : null;
        if (teleportBasic) {
            //we will capture the orientation for position
            //   for more complex snap operations (or none) check source - https://github.com/thestonefox/VRTK/blob/92ae954f9cdea93fb3687be8a3f9f23336f94784/Assets/VRTK/Prefabs/DestinationPoint/VRTK_DestinationPoint.cs#L448
            Quaternion? destinationRotation = Quaternion.Euler(0f, teleportDestination.gameObject.transform.eulerAngles.y, 0f);
            teleportBasic.Teleport(teleportDestination.gameObject.transform, 
                                    teleportDestination.gameObject.transform.position, destinationRotation);
        }
    }

}
