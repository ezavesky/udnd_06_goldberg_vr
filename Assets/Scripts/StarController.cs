using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarController : SoundCollider {
    public float smooth = 75.0f;

	// Use this for initialization
	void Start () {
		//randomize rotation in global Y
		Vector3 euler = transform.eulerAngles;
		euler.y = Random.Range(0f, 360f);
		transform.eulerAngles = euler;
	}
	
	// Update is called once per frame
	void Update () {
		// ...also rotate around the World's Y axis
        transform.Rotate(Vector3.up * Time.deltaTime * smooth, Space.World);
		//Debug.Log(string.Format("Rotate: {0}", transform.eulerAngles));
	}

	protected override void OnHit(AudioSource audioSrc, GameObject objOther) {
		base.OnHit(audioSrc, objOther);
		gameObject.SetActive(false);
	}


}
