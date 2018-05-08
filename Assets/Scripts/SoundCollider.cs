﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCollider : MonoBehaviour {
	public AudioClip soundHit = null;
	public AudioClip soundRub = null;


	protected void PlayClip(AudioSource audioSrc, AudioClip audioClip) {
		if (audioClip==null || audioSrc==null) {
			return;
		}
		//Valve.VR.InteractionSystem.Player.instance.audioListener	
		audioSrc.PlayOneShot(audioClip);
	}

	protected virtual void OnHit(AudioSource audioSrc, GameObject objOther) {
		PlayClip(audioSrc, soundHit);
	}

	void OnTriggerEnter(Collider other) {
		AudioSource audioSource = other.GetComponent<AudioSource>();
		if (audioSource==null) {
			return;
		}
	    if (soundHit != null) {
            Debug.Log(string.Format("This object ({0}) is a collectable from other ({1})", gameObject.name, other.gameObject.name));
            OnHit(audioSource, other.gameObject);
		}
	}
	
}