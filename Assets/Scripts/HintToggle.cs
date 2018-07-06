using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.SceneManagement;

public class HintToggle: MonoBehaviour  {
	public float blinkTransitionSpeed = 1.0f;
	public List<GameObject> listHints = new List<GameObject>();
	protected VRTK_HeadsetFade headsetFade = null;
    public AudioClip clipOn = null;
    public AudioClip clipOff = null;
    public bool enableHintTesting = false;
    protected const float timeMaxClick = 0.75f;  // for 'warp' to next level, max interval between clicks
    protected const int numMinClick = 3;    // for 'warp', min number of clicks 
    protected float timeLastWarpClick = 0.0f;   // for 'warp', track last time of click
    protected int numWarpClick = 0; // for 'warp', track number of clicks

	void Start() 
	{
		headsetFade = GetComponent<VRTK_HeadsetFade>();
        if (headsetFade) 
        {
    		headsetFade.HeadsetFadeComplete += new HeadsetFadeEventHandler(ActivateHints);
        }
	}

	public void RediscoverHints(GameObject objParent, bool bForgetPrior=true)
	{
        if (bForgetPrior) 
        {
            listHints.Clear();
        }
        if (listHints.Count==0 && objParent != null)
		{
            for (int i=0; i < objParent.transform.childCount; i++) 
            {
                listHints.Add(objParent.transform.GetChild(i).gameObject);
            }   //end scan of individual object
		}
		Debug.Log(string.Format("[HintToggle]: Found a total of {0} hint objects", listHints.Count));
		if (enableHintTesting) 
        {
            //test mode places you into normal state and makes first object in hint list (e.g. the ball) non-kinematic
            GameManager.instance.state = GameManager.GAME_STATE.STATE_NORMAL;
            if (listHints.Count > 0) 
            {
                GameObject objStartBall = listHints[0];
                Rigidbody rb = objStartBall.GetComponent<Rigidbody>();
                if (rb != null) 
                {
                    rb.isKinematic = false;
                }
            } 
        }
        else 
        {
            DisableHints(false);
        }
	}

    public void WarpClick() 
    {
        if ((Time.fixedTime - timeLastWarpClick) < timeMaxClick)
        {
            numWarpClick++;
            if (numWarpClick >=(numMinClick - 1)) 
            {
                Debug.Log(string.Format("[HintToggle]: Trigger Level Warp, clicks {0} ", numWarpClick));
                numWarpClick = 0;
                GameManager.instance.LoadNewScene(null);
            }
        }
        else 
        {
            numWarpClick = 0;
        }
        //Debug.Log(string.Format("[HintToggle]: Warp (now {0}, then {1}), clicks {2} ", Time.fixedTime, timeLastWarpClick, numWarpClick));
        timeLastWarpClick = Time.fixedTime;
    }

	public void EnableHints() 
	{
        GameManager.instance.state = GameManager.GAME_STATE.STATE_HINTS;
        if (clipOn != null) 
        {
            AudioSource.PlayClipAtPoint(clipOn, Camera.main.transform.position);
        }
		Fade();
	}

	public void DisableHints(bool bRestoreCamera) 
	{
		if (bRestoreCamera && headsetFade) 
		{
			headsetFade.Unfade(blinkTransitionSpeed);
            if (clipOff != null) 
            {
                AudioSource.PlayClipAtPoint(clipOff, Camera.main.transform.position);
            }
            GameManager.instance.state = GameManager.GAME_STATE.STATE_RETURN_TO_LAST;
            WarpClick();
		}
		foreach (GameObject objHint in listHints) 
		{
			objHint.SetActive(false);
		}	
    }

	protected virtual void Fade()
	{
		if (!headsetFade)
			return;		
		headsetFade.Fade(new Color(1.0f, 1.0f, 1.0f, 0.1f), blinkTransitionSpeed);
	}

	protected void ActivateHints(object sender, HeadsetFadeEventArgs args) 
    {
		foreach (GameObject objHint in listHints) 
		{
			objHint.SetActive(true);
		}
	}


}
