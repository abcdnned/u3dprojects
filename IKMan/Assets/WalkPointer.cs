using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkPointer : MonoBehaviour
{

    public Transform camera;

    public Transform player;

    public CameraModule cameraModule;

    public HumanIKController controller;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.position.x, transform.position.y, player.position.z);
        Vector2 m = controller.getMovement();
        if (Mathf.Abs(m.y) > 0 || Mathf.Abs(m.x) > 0) {
            Vector3 targetDir = Utils.forward(camera) * m.y + Utils.right(camera) * m.x;
            Quaternion tr = Quaternion.LookRotation(targetDir);       
            transform.rotation = tr;
        }
    }
}
