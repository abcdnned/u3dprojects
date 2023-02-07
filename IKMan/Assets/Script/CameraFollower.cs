using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public LegControllerType2 leftLeg;
    public LegControllerType2 rightLeg;
    public Transform follower;
    public Transform camera;

    public bool lockdir = false;
    [SerializeField] float trackSpeed = 5;

    bool waiting = false;
    private int llsc;

    private int rlsc;
    // Start is called before the first frame update

    private HumanIKController humanIKController;
    public float maxBodyAngel = 20;
    private Vector2 lastm = Vector2.zero;

    private Vector3 targetDir = Vector3.zero;

    // private ReadTrigger step = new ReadTrigger(false);
    // private ReadTrigger movementChange = new ReadTrigger(false);

    private void Awake() {
        humanIKController = GetComponent<HumanIKController>();
        llsc = 0;
        rlsc = 0;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!humanIKController.walking) return;
        // updateMovementChange();
        transfer();
    }

    // private void updateMovementChange() {
    //     Vector2 m = humanIKController.getMovement();
    //     if (!m.Equals(lastm)) {
    //         movementChange.set();
    //         Debug.Log(this.GetType().Name + " movement change set ");
    //     }
    //     lastm = new Vector2(m.x, m.y);
    // }

    private Transform getMainFoot() {
        if (leftLeg.Moving && !rightLeg.Moving) {
            return leftLeg.transform;
        } else if (rightLeg.Moving && !leftLeg.Moving) {
            return rightLeg.transform;
        } else if (leftLeg.Moving && rightLeg.Moving) {
            return leftLeg.normalizedTime > rightLeg.normalizedTime
                                ? leftLeg.transform
                                : rightLeg.transform;
        }
        return null;
    }

    // public void acceptMoveStepTrigger() {
    //     updateMovementChange();
    //     if (movementChange.peek()) {
    //         step.set();
    //         Debug.Log(this.GetType().Name + " step " + step.peek());
    //     }
    // }

    public void setDir(Vector3 v) {
        // Debug.Log(this.GetType().Name + " set dir " + v);
        targetDir = v;
    }

    private void transfer() {
        // if (movementChange.read()) {
        //     if (!step.read()) {
        //         return;
        //     }
        // }
        // Debug.Log(this.GetType().Name + " targetDir " + targetDir);

        // if (targetDir.magnitude == 0) return; 
        Vector2 m = humanIKController.getMovement();
        Vector3 dir = Utils.forward(camera) * m.y + Utils.right(camera) * m.x;
        // Vector3 dir = Utils.forward(camera);
        Quaternion tr = Quaternion.LookRotation(dir);       
        Quaternion r = Quaternion.Slerp(
            follower.rotation,
            tr, 
            1 - Mathf.Exp(-trackSpeed * Time.deltaTime)
        );
        leftLeg.transform.SetParent(follower);
        rightLeg.transform.SetParent(follower);
        follower.rotation = r;
        leftLeg.transform.SetParent(null);
        rightLeg.transform.SetParent(null);
        // if (mainFoot != null) {
        //     float angel = Vector3.Angle(targetDir, f);
        //     if (angel > maxBodyAngel) {
        //         Debug.Log(this.GetType().Name + " angel > maxBodyAngel ");
        //         Vector3 canditeDir = Vector3.RotateTowards(targetDir, f, angel - maxBodyAngel, 0);
        //         if (Vector3.Angle(Utils.forward(follower.transform), targetDir) > 10 ) {
        //             targetDir = canditeDir;
        //         }
        //     }
        // }
        // Vector3 llDir = Utils.forward(leftLeg.transform);
        // Vector3 rlDir = Utils.forward(rightLeg.transform);
        // Vector3 standDir = Utils.customTowards(llDir, rlDir, 0.5f, 0);
        // float bodyFootAngel = Vector3.Angle(standDir, Utils.forward(follower.transform));
        // float targetAngel = Vector3.Angle(targetDir, Utils.forward(follower.transform));
        // if (bodyFootAngel < maxBodyAngel || targetAngel < maxBodyAngel) {
        // } 
    }
}
