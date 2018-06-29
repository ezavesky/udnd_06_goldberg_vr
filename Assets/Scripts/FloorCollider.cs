using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCollider : SoundCollider {

	protected override void OnHit(AudioSource audioSrc, GameObject objOther) {
		base.OnHit(audioSrc, objOther);
        GameManager.instance.ResetCollectables();       //floor will aways reset collectables
	}


}
