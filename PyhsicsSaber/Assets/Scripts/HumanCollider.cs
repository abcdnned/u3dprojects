using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TY {

public class HumanCollider : MonoBehaviour
{
    private static string[] COLLIDER_TARGETS = new string[] {
        "neck", "shoudler", "bicep", "forarm", "hip", "shin"};

    void Awake()
    {
        Transform[] transforms = GetComponentsInChildren<Transform>();
        foreach (Transform t in transforms) {
            string path = GetFullName(t, transform);
            string boneName = t.name.ToLower();
            Collider c = null;
            if (t.GetComponent<Collider>() != null) {
                // Debug.Log("escap bone for component alread exists " + boneName);
                continue;
            }
            c = handleSpecialCollider(t, boneName);
            if (c == null && MatchLinkTarget(t, boneName)) {
                // Debug.Log("found collider target " + path);
                Vector3 deltaVec = t.GetChild(0).position - t.position;
                // Debug.Log(this.GetType().Name + " deltaVec " + deltaVec);
                float boneLength = deltaVec.magnitude * 10;
                // Debug.Log("found child " + GetFullName(t.GetChild(0), transform));
                // Debug.Log(this.GetType().Name + " length " + boneLength);
                CapsuleCollider capsuleCollider = t.gameObject.AddComponent<CapsuleCollider>();
                capsuleCollider.radius = LinkRadius();
                capsuleCollider.height = boneLength;
                capsuleCollider.center = new Vector3(0, boneLength / 2, 0);
                c = capsuleCollider;
            }
            if (c != null) {
                // c.tag = "PlayerBody";
                // c.isTrigger = true;
                // Debug.Log("found collider target " + path);
            }
        }
    }

    protected float LinkRadius() {
        return 1f;
    }

    protected bool MatchLinkTarget(Transform t, string boneName) {
        bool nameMatch = ArrayMatch(COLLIDER_TARGETS, boneName);
        return nameMatch && (t.childCount > 0 && t.GetChild(0) != null);
    }

    protected bool ArrayMatch(string[] matcher, string name) {
        bool nameMatch = false;
        foreach (string s in ReturnMatchPattern()) {
            if (MatchBoneName(s, name)) {
                nameMatch = true;
                break;
            }
        }
        return nameMatch;
    }

    protected string GetFullName(Transform component, Transform relativeTo)
    {
        string name = component.name;
        Transform tmp = component;
       
        while(tmp.parent != null && tmp.parent != relativeTo)
        {
            name = tmp.parent.name + "/" + name;
            tmp = tmp.parent;
        }
       
        return name;
    }

    protected string[] ReturnMatchPattern() {
        return COLLIDER_TARGETS;
    }

    protected bool MatchBoneName(string s, string t) {
        return s.Equals(t) || t.Equals(s + ".r") || t.Equals(s + ".l");
    }

    protected bool MatchBoneNameNumber(string s, string t, params int[] except) {
        bool isNumeric = int.TryParse(t.Substring(t.Length - 1), out int n);
        for (int i = 0; i < except.Length; ++i) {
            if (n == except[i]) return false;
        }
        string head = t.Substring(0, t.Length - 1);
        return head.Equals(s) && isNumeric;
    }

    protected bool IsColliderTarget(string boneName) {
        return false;
    }

    protected Collider handleSpecialCollider(Transform t, string boneName) {
        if (MatchBoneName("head", boneName)) {
            BoxCollider boxCollider = t.gameObject.AddComponent<BoxCollider>();
            boxCollider.center = new Vector3(0.0f, 1.78f, 0);
            boxCollider.size = new Vector3(3f, 3f, 3f);
            return boxCollider;
        } else if (MatchBoneName("hand", boneName)) {
            BoxCollider boxCollider = t.gameObject.AddComponent<BoxCollider>();
            boxCollider.center = new Vector3(0.0f, 2.0f, 0);
            boxCollider.size = new Vector3(1f, 2f, 1f);
            return boxCollider;
        } else if (MatchBoneName("foot", boneName)) {
            BoxCollider boxCollider = t.gameObject.AddComponent<BoxCollider>();
            boxCollider.center = new Vector3(0.0f, 1.0f, 0);
            boxCollider.size = new Vector3(1f, 3f, 1f);
            return boxCollider;
        } else if (MatchBoneNameNumber("spin", boneName, 0, 3)) {
            CapsuleCollider capsuleCollider = t.gameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.radius = 1f;
            capsuleCollider.height = 2f;
            capsuleCollider.center = new Vector3(0, capsuleCollider.height / 2, 0);
            return capsuleCollider;
        }
        return null;
    }
}

}