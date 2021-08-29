using UnityEngine;
using System.Collections;

public class get_head : MonoBehaviour {
    Transform hmdTrackedObject = null;//头盔
                                    
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //找到头盔的设备
        if (hmdTrackedObject == null)
        {
            SteamVR_TrackedObject[] trackedObjects = FindObjectsOfType<SteamVR_TrackedObject>();
            foreach (SteamVR_TrackedObject tracked in trackedObjects)
            {
                if (tracked.index == SteamVR_TrackedObject.EIndex.Hmd)
                {
                    hmdTrackedObject = tracked.transform;
                    break;
                }
            }
        }

        if (hmdTrackedObject)
        {

        }
    }
}
