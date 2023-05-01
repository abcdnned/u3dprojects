using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HeadController : TargetController
{

    public Vector3 transferDir;
    public Transform head;

    public Transform cam;

    // Head track mode, 0 = Lock with direction, 1 = Slerp with body, 2 = Do nothing
    // 3 = Slerp with direction
    public int mode = 2;

    [SerializeField] float trackSpeed = 5;
    void Update()
    {
        headTransfer(0);
    }

    private void updateTransferDirection(Vector3 d) {
        transferDir = d;
    }

    public void setMode(int m) {
        mode = m;
        Debug.Log(this.GetType().Name + " Set head track mode " + mode );
    }

    private void headTransfer(float angelOffset) {
        if (mode == 1) {
            updateTransferDirection(Utils.forwardFlat(body.transform));
        } else {
            updateTransferDirection(Utils.forwardFlat(walkPointer.transform));
        }
        Quaternion tr = Quaternion.LookRotation(transferDir);       
        Quaternion offset = Quaternion.AngleAxis(angelOffset, Vector3.up);
        tr *= offset;
        if (mode == 1 || mode == 3) {
            Quaternion r = Quaternion.Slerp(
                head.rotation,
                tr, 
                1 - Mathf.Exp(-trackSpeed * Time.deltaTime)
            );
            Utils.deltaRotate(head, r);
        } else if (mode == 0) {
            Utils.deltaRotate(head, tr);
        } else if (mode == 2){
        } else {
            throw new Exception("Head track mode not supported");
        }
    }
}