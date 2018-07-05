using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Source: https://forum.unity.com/threads/creating-a-proper-game-manager.378606/#post-2480596
public class Singleton<T> : MonoBehaviour where T:Singleton<T>
{
    private static volatile T _instance = null;
    private static object _lock = new object();

    public static T instance
    {
        get
        {
            if(_instance == null)
            {
                lock(_lock)
                {
                    if(_instance == null)
                    {
                        GameObject go = new GameObject();
                        _instance = go.AddComponent<T>();
                        go.name = typeof(T).ToString() + " singleton";
                        DontDestroyOnLoad(go);
                    }
                }
            }
            return _instance;
        }
    }
}

public class GameManager : Singleton<GameManager> 
{
    public enum GAME_STATE { STATE_INITIAL, STATE_NORMAL, STATE_TESTING, STATE_HINTS, STATE_FINISHED, STATE_NEXT_LEVEL, STATE_RETURN_TO_LAST=1000 };
    protected GAME_STATE _state = GAME_STATE.STATE_INITIAL;
    protected GAME_STATE _stateLast = GAME_STATE.STATE_INITIAL;
    protected GameObject objUniqueBall = null;  //created instance of unique object
    protected SceneController sceneController = null;  //allow manipulation of the scene
    public Transform toolParentTransform = null;  // transform to use for created tools

    protected Dictionary<int, GameObject> collectableDict = new Dictionary<int, GameObject>();

    // validate star/collectable possiblity when teleported from a valid location
    public bool normalPlay { 
        get 
        { 
            return _state == GAME_STATE.STATE_NORMAL; 
        } 
        private set {} 
    }
    public bool finishedLevel { 
        get 
        { 
            // state jump if we're in normal but no collectables (like in training level)
            if ((_state == GAME_STATE.STATE_FINISHED) || (_state == GAME_STATE.STATE_NORMAL && collectableDict.Count==0)) 
            {
                state = GAME_STATE.STATE_FINISHED;
                return true;
            }
            return false;
        } 
        private set {} 
    }

    public GAME_STATE state {
        set
        {
            //TODO: other game mechanics when state changes?
            if (value == GAME_STATE.STATE_RETURN_TO_LAST) {
                _state = _stateLast;    
            }
            else if (value == GAME_STATE.STATE_NEXT_LEVEL) {
                Debug.Log(string.Format("[GameManager] Switching to next scene with scene manager {0}", sceneController));
                _stateLast = _state;
                _state = GAME_STATE.STATE_INITIAL;
            }
            else {
                // guarantee that we have only one unique object (e.g. a ball)
                Debug.Log(string.Format("[GameManager]: New state {0}, previous state {1}", value, _state));
                _stateLast = _state;
                _state = value;
            }
        }
        get 
        {
            return _state;
        }
    }

    // helper methods for scene/level controll

    public void RegisterSceneController(SceneController sceneControllerNew) 
    {
         sceneController = sceneControllerNew;
    }

    public void StageNewScene(string nameSceneNext) 
    {
        sceneController.SceneStage(nameSceneNext);
    }

    public bool LoadNewScene(string nameSceneNext) 
    {
        if (state != GAME_STATE.STATE_FINISHED && !string.IsNullOrEmpty(nameSceneNext))
        {
            Debug.LogWarning(string.Format("[GameManager] Attempting to load new scene '{0}' but not in finished state", nameSceneNext));
            return false;
        }
        state = GAME_STATE.STATE_NEXT_LEVEL;
        sceneController.SceneLoad(nameSceneNext);
        return true;
    }

    // methods to control single ball for final run or testing

    public void ValidateSingletonBall() {
        if (objUniqueBall != null) {
            Destroy(objUniqueBall);
            objUniqueBall = null;
        }
    }
    
    public void RegisterSingletonBall(GameObject objNew) {
        ValidateSingletonBall();
        objUniqueBall = objNew;
    }

    /// methods for managing collectables in a level

    public void CaptureColletable(GameObject objNew) 
    {
        if (normalPlay) {
            if (!collectableDict.ContainsKey(objNew.GetInstanceID())) 
            {
                Debug.LogError(string.Format("[GameManager]: Error, object {0}, instance {1} not found in collectable list, but marked as capture",
                                objNew.name, objNew.GetInstanceID()));
                return;
            }
    		//set captured in our final list...
            objNew.SetActive(false);
            bool bRemainCapture = false;
            foreach (KeyValuePair<int, GameObject> pairC in collectableDict) 
            {
                bRemainCapture |= pairC.Value.activeSelf;
            }
            if (!bRemainCapture) 
            {
                state = GAME_STATE.STATE_FINISHED;
                Debug.Log(string.Format("[GameManager]: Setting state to finished with {0} collectables.", collectableDict.Count));
            }
        }
    }

    public bool RediscoverCollectables(GameObject objLevelParent) 
    {
        StarController[] addObjs = objLevelParent.GetComponentsInChildren<StarController>();  //  .FindGameObjectsWithTag(retoggleTags[i]);
        if ((addObjs == null) || (addObjs.Length == 0)) 
        {
            return false;
        }
        //walk through all examples to discover the collectable objects
        collectableDict.Clear();
        foreach (StarController objCollectable in addObjs) 
        {
            GameObject objChild = objCollectable.gameObject;
            collectableDict.Add(objChild.GetInstanceID(), objChild);
        }
        Debug.Log(string.Format("[GameManager]: Discovered {0} collectables.", collectableDict.Count));
        return true;
    }

    public void ResetCollectables(bool bClearAll=false) 
    {
        //reset collectables for a level because the ball fell
        if (collectableDict.Count == 0) 
        {
            return;
        }
        else if (bClearAll) 
        {
            collectableDict.Clear();
            return;
        }
        foreach (KeyValuePair<int, GameObject> pairC in collectableDict) 
        {
            pairC.Value.SetActive(true);
        }
        Debug.Log(string.Format("[GameManager]: Reset {0} collectables.", collectableDict.Count));
    }

}
