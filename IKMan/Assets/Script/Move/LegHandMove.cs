using UnityEngine;

public class LegHandMove : Move
{
    public LegHandMove(string name) : base(name) {
    }
    public override void init() {
        normalizedTime = 0;
        state = 0;
    }

    public LegControllerType2 legController() {
        if (targetController is LegControllerType2) {
            return (LegControllerType2)targetController;
        }
        return null;
    }
    
    public HandController handController() {
        if (targetController is HandController) {
            return (HandController)targetController;
        }
        return null;
    }

}