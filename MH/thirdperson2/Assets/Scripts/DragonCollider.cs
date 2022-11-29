using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TY {

public class DragonCollider : MonoBehaviour
{
    // Start is called before the first frame update
    private static string[] COLLIDER_TARGETS = new string[] {"neck", "spine", "tail", "leg", "wing"};

    // Collider[] bites = new Collider[20];

    public Dictionary<string, Collider> colliderDict = new Dictionary<string, Collider>();

    private DragonTarget dragonTarget;

    // int bitesCount = 0;

    // Collider[] tails = new Collider[20];

    // int tailsCount = 0;
    void Awake()
    {
        // bites = new Collider[20];
        // tails = new Collider[20];
        dragonTarget = GetComponent<DragonTarget>();
        Transform[] transforms = GetComponentsInChildren<Transform>();
        foreach (Transform t in transforms) {
            // Debug.Log(transform.name);
            // Debug.Log(t.name);
            string boneName = t.name.ToLower();
            Collider c = null;
            if (t.GetComponent<Collider>() != null) {
                // Debug.Log("escap bone for component alread exists " + boneName);
                continue;
            }
            if (boneName.Equals("head")) {
                BoxCollider boxCollider = t.gameObject.AddComponent<BoxCollider>();
                boxCollider.center = new Vector3(0.14f, 0.82f, 0);
                boxCollider.size = new Vector3(1.1f, 1.6f, 1.0f);
                c = boxCollider;
                // Debug.Log(this.GetType().Name + " bitesCount " + bitesCount);
                // bites[bitesCount] = c;
                // bitesCount++;
            } else if (boneName.Equals("foot.r") || boneName.Equals("foot.l")) {
                SphereCollider sphereCollider = t.gameObject.AddComponent<SphereCollider>();
                sphereCollider.radius = 0.2f;
                c = sphereCollider;
            } else if (boneName.Equals("tail8")) {
                BoxCollider boxCollider = t.gameObject.AddComponent<BoxCollider>();
                boxCollider.center = new Vector3(0, 1.5f, 0);
                boxCollider.size = new Vector3(1f, 3f, 1f);
                c = boxCollider;
            } else if (boneName.Equals("wingbone1_3.l") || boneName.Equals("wingbone1_3.r")) {
                BoxCollider boxCollider = t.gameObject.AddComponent<BoxCollider>();
                boxCollider.center = new Vector3(0, 1.5f, 0);
                boxCollider.size = new Vector3(0.5f, 3f, 0.8f);
                c = boxCollider;
            } else {
                foreach (string matcher in COLLIDER_TARGETS) {
                    if (t.name.ToLower().Contains(matcher) && !t.name.ToLower().Contains("ik")
                    && !boneName.Contains("pole") && !boneName.Contains("root")) {
                        if (t.childCount > 0 && t.GetChild(0) != null) {
                            Vector3 deltaVec = t.GetChild(0).position - t.position;
                            float boneLength = deltaVec.magnitude;
                            CapsuleCollider capsuleCollider = t.gameObject.AddComponent<CapsuleCollider>();
                            capsuleCollider.radius = 0.5f;
                            capsuleCollider.height = boneLength;
                            capsuleCollider.center = new Vector3(0, boneLength / 2, 0);
                            c = capsuleCollider;
                            // if (boneName.Contains("tail")) {
                            //     tails[tailsCount] = c;
                            //     tailsCount++;
                            // }
                            break;
                        }
                    }
                }
            }
            if (c != null) {
                c.tag = "Hittable";
                c.isTrigger = true;
                c.enabled = true;
                string path = GetFullName(t, transform);
                colliderDict[boneName] = c;
                // Debug.Log("found collider target " + path);
            }
        }
    //    for(var t:Transform in GetComponentsInChildren<Transform>())
    //    {
    //        // full name of armor piece, relative to root
    //        var path:String = GetFullName(t, armorRoot);
       
    //        // find this piece in our character rig
    //        var match:Transform = characterRoot.Find(path);
       
    //        // parent it, store it for later movement matching, or whatever
    //        if(match)
    //        {
    //            // do something
    //        }
    //    }
  
    }

    string GetFullName(Transform component, Transform relativeTo)
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
 

    // Update is called once per frame
    void Update()
    {
        
    }

    // private void OnTriggerEnter(Collider collision) {
    //     if (collision.tag == "PlayerBody" && dragonTarget.attackType != DragonTarget.ATTACK_NONE) {
    //         HandlePlayerHit(collision, dragonTarget.attackType);
    //     }
    // }

    // private void HandlePlayerHit(Collider collider, string attackType) {
    //     Debug.Log(this.GetType().Name + " Dragon attack hit " + collider.gameObject.name);
    // }

}

}