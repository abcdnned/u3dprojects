using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointTrouq : MonoBehaviour
{
    public float x;
    public float y;
    public float z;


    CharacterJoint joint;
    Rigidbody rgbd;
    // Start is called before the first frame update
    void Start()
    {
        joint = GetComponent<CharacterJoint>();
        rgbd = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 torque = new Vector3(x * Time.deltaTime,
                                     y * Time.deltaTime,
                                     z * Time.deltaTime);
        rgbd.AddTorque(torque, ForceMode.VelocityChange);
    }
}
