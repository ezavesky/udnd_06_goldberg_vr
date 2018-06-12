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
    protected enum GAME_STATE { STATE_INITIAL, STATE_NORMAL, STATE_HINTS, STATE_FINISHED };
    protected GAME_STATE state = GAME_STATE.STATE_INITIAL;

    // validate star/collectable possiblity when teleported from a valid location
    public bool normalPlay { 
        get 
        { 
            return state == GAME_STATE.STATE_NORMAL; 
        } 
        private set {} 
    }
    
    // determine when you have teleported to a location
    // turn on or off the next stage teleport when completed
    // hints to show/hide objects with tag "hint"

	/*
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
 	*/
	// static access to the one and only instance of game manager

    

}
