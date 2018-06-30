using UnityEngine;
using System.Collections.Generic;
using VRTK;

public class TeleportObjToggle : MonoBehaviour
{
    public List<VRTK_DestinationPoint> listTeleporters = new List<VRTK_DestinationPoint>();
    public AudioClip clipStart = null;
    public AudioClip clipHold = null;

    public bool RediscoverTeleporters(GameObject objParent) 
    {
        VRTK_DestinationPoint[] addObjs = objParent.GetComponentsInChildren<VRTK_DestinationPoint>();  //  .FindGameObjectsWithTag(retoggleTags[i]);
        if ((addObjs == null) || (addObjs.Length == 0)) 
        {
            return false;
        }
        listTeleporters.Clear();
        listTeleporters.AddRange(addObjs);
        ToggleObjects(false);
        return true;
    }

    public virtual void ToggleObjects(bool newState)
    {
        foreach (VRTK_DestinationPoint objTeleport in listTeleporters)
        {
            if (objTeleport != null)
            {
                objTeleport.gameObject.SetActive(newState);
            }
        }
        if (newState) {
            if (clipStart) 
            {
                //start playing back a 'pending' loop

            }
        }

    }

    
}
