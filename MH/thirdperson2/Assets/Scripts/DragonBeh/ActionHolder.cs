using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TY {
public class ActionHolder {
    List<ActionCandidate> actions = new List<ActionCandidate>();

    public void AddAction(ActionCandidate ac) {
        actions.Add(ac);
    }

    public ActionCandidate PickAction(float dis, float angel, out bool needAdjustPosition) {
        actions.Shuffle();
        foreach (ActionCandidate action in actions) {
            float[] er = action.EffectiveRange();
            float[] ea = action.EffectiveAngle();
            if (dis >= er[0] && dis <= er[1] && angel >= ea[0] && angel <= ea[1]) {
                needAdjustPosition = false;
                return action;
            }
        }
        // Debug.Log(this.GetType().Name + " repick ");
        int index = Random.Range(0, actions.Count);
        needAdjustPosition = true;
        return actions[index];
    }
}
}