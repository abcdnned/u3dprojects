using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{

    private int grab = 0;
    public float speed = 10;

    public Transform handle;

    private Vector3 mouse_position = Vector3.zero;
    private readonly float mouseSpeed = 1f;
    private readonly int GRAB_CNT = 3;

    private float max_mouse_move = 2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate() {
        //mouse move
        Vector3 m = Input.mousePosition - mouse_position;
        mouse_position = Input.mousePosition;
        if (grab > 0) {
            float x = mouseSpeed * m.x * Time.fixedDeltaTime;
            float y = mouseSpeed * m.y * Time.fixedDeltaTime;
            if (grab == 1) {
                Vector3 d = new Vector3(x, y, 0);
                d = Math.Min(d.magnitude, max_mouse_move) * d.normalized;
                handle.transform.position = handle.transform.position + d;
            } else {
                Vector3 d = new Vector3(x, 0, y);
                d = Math.Min(d.magnitude, max_mouse_move) * d.normalized;
                handle.transform.position = handle.transform.position + d;
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W)) {
            transform.position = transform.position + transform.forward * speed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.S)) {
            transform.position = transform.position - transform.forward * speed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.LeftArrow)) {
            transform.position = transform.position - transform.right * speed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.RightArrow)) {
            transform.position = transform.position + transform.right * speed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.UpArrow)) {
            transform.position = transform.position + transform.up * speed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.DownArrow)) {
            transform.position = transform.position - transform.up * speed * Time.deltaTime;
        // } else if (Input.GetKey(KeyCode.A)) {
        //     Rotate(transform, -1);
        // } else if (Input.GetKey(KeyCode.D)) {
        //     Rotate(transform, 1);
        } else if (Input.GetKey(KeyCode.R)) {
            RotateHorizontal(transform, -1);
        } else if (Input.GetKey(KeyCode.F)) {
            RotateHorizontal(transform, 1);
        }

        //mouse click
        if (Input.GetMouseButtonUp(0)) {
            grab = (grab + 1) % GRAB_CNT;
            Debug.Log(this.GetType().Name + " Grab: " + grab);
        } else if (Input.GetMouseButtonUp(1)) {
        }
    }

    private void RotateHorizontal(Transform transform, float horizontal)
    {
        // Vector3 targetDir = transform.up * horizontal;
        // Debug.Log(this.GetType().Name + " transform.up " + transform.up);
        // targetDir.Normalize();
        // Quaternion tr = Quaternion.LookRotation(targetDir);
        // Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, speed * 0.1f * Time.deltaTime);
        // transform.rotation = targetRotation;
        transform.Rotate(Vector3.right * horizontal, Space.World);
        // transform.RotateAroundLocal(Vector3.right, speed * 0.1f * Time.deltaTime);
    }

    private void Rotate(Transform transform, float vertical) {
        // Vector3 targetDir = transform.right * vertical;
        // targetDir.Normalize();
        // Quaternion tr = Quaternion.LookRotation(targetDir);
        // Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, speed * 0.1f * Time.deltaTime);
        // transform.rotation = targetRotation;
        transform.Rotate(Vector3.up * vertical, Space.World);
        // transform.RotateAroundLocal(Vector3.up, speed * 0.1f * Time.deltaTime);
    }
}
