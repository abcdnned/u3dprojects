using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseManager : MonoBehaviour
{
    internal Dictionary<string, HandDelayLooker> handDelayLookerMap
     = new Dictionary<string, HandDelayLooker>();

    // Start is called before the first frame update
    void Start()
    {
        HandDelayLooker[] handDelayLookers = GetComponentsInChildren<HandDelayLooker>();
        foreach (HandDelayLooker t in handDelayLookers) {
            string name = null;
            if (t.name.ToLower().Contains(IKSampleNames.SHOULDER)) {
                name = IKSampleNames.SHOULDER + "_" + t.transform.parent.gameObject.name;
            } else if (t.name.ToLower().Contains(IKSampleNames.ELBOW)) {
                name = IKSampleNames.ELBOW + "_" + t.transform.parent.gameObject.name;
            } else if (t.name.ToLower().Contains(IKSampleNames.HAND)) {
                name = IKSampleNames.HAND+ "_" + t.transform.parent.gameObject.name;
            }
            handDelayLookerMap[name] = t;
            // Debug.Log(" regist " + handDelayLookerMap[t.name] + " " + t);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
