using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkPointer : MonoBehaviour
{

    public Transform cam;

    public Transform player;

    public CameraModule cameraModule;

    public HumanIKController humanIKController;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // bool turn = false;
        if ((humanIKController.currentStatus.getName() == LocomotionState.NAME
            && humanIKController.currentStatus.cs.name == LocomotionState.STATE_MOVE)
            || humanIKController.currentStatus.legMovingCheck() ) {
            transform.position = new Vector3(player.position.x, transform.position.y, player.position.z);
            Vector2 m = humanIKController.getMovement();
            if (Mathf.Abs(m.y) > 0 || Mathf.Abs(m.x) > 0) {
                Vector3 targetDir = Utils.forward(cam) * m.y + Utils.right(cam) * m.x;
                Quaternion tr = Quaternion.LookRotation(targetDir);       
                transform.rotation = tr;
            }
        }
    }

    public void lookCamera() {
        Vector3 targetDir = Utils.forward(cam);
        Quaternion tr = Quaternion.LookRotation(targetDir);       
        transform.rotation = tr;
    }
}
