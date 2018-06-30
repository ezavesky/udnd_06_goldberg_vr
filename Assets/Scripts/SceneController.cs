﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {
    public string[] nameLevels = new string[0];
    public TeleportObjToggle teleporterController = null;

	// Use this for initialization
	void Start () {
        // load scene / operation
        if (nameLevels.Length > 0) {
            // alternate is fade in to new temporary stage
            SceneLoad(nameLevels[0]);
        }
	}
	
    void SceneLoad(string strName) {
        StartCoroutine(LoadYourAsyncScene(strName));
    }

    IEnumerator LoadYourAsyncScene(string strName)
    {
        // load the scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(strName, LoadSceneMode.Additive);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;

            // on complete, find all of the collectables under new scene
            Scene sceneNew = SceneManager.GetSceneByName(strName); 
            foreach (GameObject objRoot in sceneNew.GetRootGameObjects()) 
            {
                // start teleporter rediscover
                if (teleporterController != null) 
                {
                    teleporterController.RediscoverTeleporters(objRoot);
                }

                GoalController sceneGoal = objRoot.GetComponent<GoalController>();
                if (sceneGoal != null) 
                {
                    //reset game manager to find collectables
                    sceneGoal.RediscoverCollectables();                    
                    // teleport user to spawn within new scene
                    sceneGoal.TeleportUser(true);
                }
                
            }   //end search of goal


        }   //end wait for scene load
    }   //end async scene load
}