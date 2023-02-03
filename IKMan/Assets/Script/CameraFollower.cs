using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public Transform follower;
    public Transform camera;
    [SerializeField] float trackSpeed = 5;
    // Start is called before the first frame update

    private HumanIKController humanIKController;

    private void Awake() {
        humanIKController = GetComponent<HumanIKController>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector2 m = humanIKController.getMovement();
        if (humanIKController.walking) {
            Vector3 targetDir = Utils.forward(camera) * m.y + Utils.right(camera) * m.x;
            Quaternion tr = Quaternion.LookRotation(targetDir);       
            Quaternion r = Quaternion.Slerp(
                follower.rotation,
                tr, 
                1 - Mathf.Exp(-trackSpeed * Time.deltaTime)
            );
            follower.rotation = r;
        }
    }
}
