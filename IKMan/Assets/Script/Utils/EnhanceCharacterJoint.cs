using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

// file: EnhanceCharacterJoint.cs
[RequireComponent (typeof (CharacterJoint))]
public class EnhanceCharacterJoint : MonoBehaviour {
}
 
// file: EnhanceCharacterJointEditor.cs
[CustomEditor(typeof(EnhanceCharacterJoint))]
class EnhanceCharacterJointEditor : Editor
{
    void OnSceneGUI () {
       
        EnhanceCharacterJoint script = (EnhanceCharacterJoint) this.target;
        if (script != null)
        {
            CharacterJoint joint = (CharacterJoint) script.gameObject.GetComponent(typeof(CharacterJoint));
            Transform t = joint.transform;
            Vector3 twistAxis = joint.transform.TransformDirection(joint.axis).normalized;
            Vector3 swingAxis = joint.transform.TransformDirection(joint.swingAxis).normalized;
            Vector3 normal1 = Vector3.Cross(twistAxis, swingAxis);
           
            float size = HandleUtility.GetHandleSize(joint.transform.position);
            float angle = 30;
           
            Handles.color = new Color(1,0,0) ; // red
            Handles.DrawWireArc(joint.transform.position, twistAxis,  normal1, angle, size);
            Handles.DrawWireArc(joint.transform.position, twistAxis,  normal1, -angle, size);
            Handles.DrawLine(joint.transform.position, joint.transform.position + Quaternion.AngleAxis(-angle, twistAxis) * normal1 * size);
            Handles.DrawLine(joint.transform.position, joint.transform.position + Quaternion.AngleAxis(angle, twistAxis) * normal1 * size);
           
            Handles.color = new Color(0,1f,0) ; // green
            Handles.DrawWireArc(joint.transform.position, swingAxis, normal1, angle, size);
            Handles.DrawWireArc(joint.transform.position, swingAxis, normal1, -angle, size);          
            Handles.DrawLine(joint.transform.position, joint.transform.position + Quaternion.AngleAxis(-angle, swingAxis) * normal1 * size);
            Handles.DrawLine(joint.transform.position, joint.transform.position + Quaternion.AngleAxis(angle, swingAxis) * normal1 * size);
           
            Handles.color = new Color(0,0,1f); // blue
            Handles.DrawWireArc(joint.transform.position, normal1, swingAxis, angle, size);
            Handles.DrawWireArc(joint.transform.position, normal1, swingAxis, -angle, size);          
            Handles.DrawLine(joint.transform.position, joint.transform.position + Quaternion.AngleAxis(-angle, normal1) * swingAxis * size);
            Handles.DrawLine(joint.transform.position, joint.transform.position + Quaternion.AngleAxis(angle, normal1) * swingAxis * size);
        }
    }
}