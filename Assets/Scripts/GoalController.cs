using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class GoalController : SoundCollider {
    public bool BallSticks = true;
    public GameObject objParentCollectables = null;
    public VRTK_DestinationPoint objTeleportFinal = null;
    public float delayTeleport = 2.0f;

    void Start() {
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

    protected void TeleportFinal() {
        //valid task, enable final teleport!
        if (objTeleportFinal != null) {
            // get destination point from editor/other - VRTK_DestinationPoint objTeleportFinal
            VRTK_BasicTeleport teleportBasic = VRTK_ObjectCache.registeredTeleporters.Count > 0 ? VRTK_ObjectCache.registeredTeleporters[0] : null;
            if (teleportBasic) {
                //we will capture the orientation for position
                //   for more complex snap operations (or none) check source - https://github.com/thestonefox/VRTK/blob/92ae954f9cdea93fb3687be8a3f9f23336f94784/Assets/VRTK/Prefabs/DestinationPoint/VRTK_DestinationPoint.cs#L448
                Quaternion? destinationRotation = Quaternion.Euler(0f, objTeleportFinal.gameObject.transform.eulerAngles.y, 0f);
                teleportBasic.Teleport(objTeleportFinal.gameObject.transform, objTeleportFinal.gameObject.transform.position, destinationRotation);
            }
        }
    }

}
