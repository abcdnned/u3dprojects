using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

// file: EnhanceCharacterJoint.cs
public class TwoNodeGizmo : MonoBehaviour {
    public Transform shoulder;
    public Transform elbow;
    public Transform hand;

}

[CustomEditor(typeof(TwoNodeGizmo))]
class TwoNodeGizmoEditor : Editor
{
    void OnSceneGUI () {
       
        TwoNodeGizmo script = (TwoNodeGizmo) this.target;
        if (script != null)
        {
            if (script.shoulder != null && script.elbow != null) {
                Handles.color = new Color(1,0,0) ; // red
                Handles.DrawLine(script.shoulder.position, script.elbow.position);
            }
           
            if (script.hand != null && script.elbow != null) {
                Handles.color = new Color(0,1f,0) ; // green
                Handles.DrawLine(script.elbow.position, script.hand.position);
            }
           
        }
    }
}