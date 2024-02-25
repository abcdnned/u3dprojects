using UnityEngine;

/*
[Future Target] All hand foot hip head controllers should be conbine into a one big HumanController
Here is what HumanController should offer
1. one center face api for launch complex moves
2. inside HumanController, different controller move can corrodinate
3. center state api check
4. natrual moves should provider a general recovery method
5. all geomatory change should be warped into a move
*/

/*
state 0 = init
state 1 = prestart
state 2 = active
state 3 = recover
state 4 = finish
*/
public class Move
{
    public static int RUNING = 0;
    public static int FINISH = 1;
    public string name;
    internal int state;

    public float normalizedTime;

    public float duration;

    public Move nextMove;

    public TargetController targetController;

    public HumanIKController hic;

    public MoveManager moveManager;





    public virtual void beReady() {

    }

    public virtual string getMoveType() {
        return AdvanceIKController.IK;
    }

    public Move(string n) {
        this.name = n;
    }
    public bool IsHandMoving()
    {
        return this.name == MoveNameConstants.HandMoving;
    }

    public bool IsHandIdle()
    {
        return this.name == MoveNameConstants.HandIdle;
    }

    public bool IsLegMoving()
    {
        return this.name == MoveNameConstants.LegMoving;
    }

    public bool IsLegIdle()
    {
        return this.name == MoveNameConstants.LegIdle;
    }

    public bool isMoving() {
        return this.name.Contains("moving");
    }

    public virtual Move transfer() {
        return this;
    }
    public virtual Move move(float dt) {
        return this;
    }

    public virtual void init() {
        normalizedTime = 0;
        state = 0;
    }

    public virtual void finish() {
    }

    public virtual bool stateFinish() {
        return state > 100;
    }

    public bool isIdle() {
        return name.Contains("idle") || stateFinish();
    }


}
