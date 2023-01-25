using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LocomotionState : AnyState {

    public override string getName() {
        return "LocomotionState";
    }

    public override ActionStateMachine run() {
        return null;
    }

    public override void handleEvent(Event e) {

    }

    public override void handleInput(SMInput smInput) {

    }
}