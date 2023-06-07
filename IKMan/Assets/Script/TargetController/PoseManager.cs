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
            handDelayLookerMap[t.name] = t;
            // Debug.Log(" regist " + handDelayLookerMap[t.name] + " " + t);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
