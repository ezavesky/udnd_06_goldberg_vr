using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class HintToggle: MonoBehaviour  {
	public float blinkTransitionSpeed = 1.0f;
	public string[] tagSet = new string[0];
	public GameObject[] objsHint = null;
	protected VRTK_HeadsetFade headsetFade = null;

	void Start() 
	{
		RediscoverHints(true);
        //GameManager.instance.state = GameManager.GAME_STATE.STATE_NORMAL;
		headsetFade = GetComponent<VRTK_HeadsetFade>();
	}

	public void RediscoverHints(bool bForgetPrior) 
	{
        if (bForgetPrior || (objsHint==null || objsHint.Length==0))
		{
			List<GameObject> listObjs = new List<GameObject>();
			foreach (string tagSearch in tagSet)
			{
				listObjs.AddRange(GameObject.FindGameObjectsWithTag(tagSearch));
			}
			objsHint = listObjs.ToArray();
		}
		Debug.Log(string.Format("[HintToggle]: Found a total of {0} hint objects", objsHint.Length));
		DisableHints(false);
	}

	public void EnableHints() 
	{
        GameManager.instance.state = GameManager.GAME_STATE.STATE_HINTS;
		Fade();
	}

	public void DisableHints(bool bRestoreCamera) 
	{
		if (bRestoreCamera && headsetFade) 
		{
			headsetFade.Unfade(blinkTransitionSpeed);
		}
		foreach (GameObject objHint in objsHint) 
		{
			objHint.SetActive(false);
		}	
        GameManager.instance.state = GameManager.GAME_STATE.STATE_RETURN_TO_LAST;
    }

	protected virtual void Fade()
	{
		if (!headsetFade)
			return;		
		headsetFade.HeadsetFadeComplete += new HeadsetFadeEventHandler(ActivateHints);
		headsetFade.Fade(new Color(1.0f, 1.0f, 1.0f, 0.1f), blinkTransitionSpeed);
	}

	protected void ActivateHints(object sender, HeadsetFadeEventArgs args) {
		foreach (GameObject objHint in objsHint) 
		{
			objHint.SetActive(true);
		}
		headsetFade.HeadsetFadeComplete -= new HeadsetFadeEventHandler(ActivateHints);
	}


}
