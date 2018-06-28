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
    public enum GAME_STATE { STATE_INITIAL, STATE_NORMAL, STATE_TESTING, STATE_HINTS, STATE_FINISHED, STATE_RETURN_TO_LAST=1000 };
    protected GAME_STATE _state = GAME_STATE.STATE_INITIAL;
    protected GAME_STATE _stateLast = GAME_STATE.STATE_INITIAL;
    protected GameObject objUniqueBall = null;  //created instance of unique object

    // validate star/collectable possiblity when teleported from a valid location
    public bool normalPlay { 
        get 
        { 
            return _state == GAME_STATE.STATE_NORMAL; 
        } 
        private set {} 
    }

    public GAME_STATE state {
        set
        {
            //TODO: other game mechanics when state changes?
            if (value == GAME_STATE.STATE_RETURN_TO_LAST) {
                GAME_STATE stateTemp = _state;
                _state = _stateLast;    
            }
            else {
                // guarantee that we have only one unique object (e.g. a ball)
                ValidateSingletonBall();
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

    // turn on or off the next stage teleport when completed

    

}
