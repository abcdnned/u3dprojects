using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

// file: EnhanceCharacterJoint.cs
[RequireComponent (typeof (HandDelayLooker))]
public class EnhanceHandDelayLooker : MonoBehaviour {
}
 
// file: EnhanceCharacterJointEditor.cs
[CustomEditor(typeof(EnhanceHandDelayLooker))]
class EnhanceHandDelayLookerEditor : Editor
{
    void OnSceneGUI () {
       
        EnhanceHandDelayLooker script = (EnhanceHandDelayLooker) this.target;
        if (script != null)
        {
            HandDelayLooker looker = (HandDelayLooker) script.gameObject.GetComponent(typeof(HandDelayLooker));
            Transform sun = looker.sun.transform;
            float size = HandleUtility.GetHandleSize(looker.transform.position);
           
            Handles.color = new Color(0,1f,0) ; // green
            Handles.DrawLine(sun.position, looker.transform.position);
        }
    }
}