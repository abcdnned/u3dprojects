using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public Transform follower;
    public Transform camera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetDir = Vector3.zero;
        targetDir = camera.forward;
        targetDir.y = 0;
        Quaternion tr = Quaternion.LookRotation(targetDir);       
        follower.rotation = tr;
    }
}
