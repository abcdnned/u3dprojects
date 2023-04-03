using UnityEngine;

public class HipWalkPoseMove : Move
{
    // [SerializeField] bool walkPosing = false;
    public HipWalkPoseMove() : base(MoveNameConstants.HipWalkPose)
    {
        
    }

    public override Move move(float dt)
    {
        return this;
    }
}
