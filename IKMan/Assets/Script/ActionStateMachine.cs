using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Author: Sergio Abreu García | https://sergioabreu.me

/// <summary> Tells the ActiveRagdoll what it should do. Input can be external (like the
/// one from the player or from another script) and internal (kind of like sensors, such as
/// detecting if it's on floor). </summary>
public class ActionStateMachine {
    // ---------- EXTERNAL INPUT ----------
    protected ActionStateMachine previousState;

    public void setPreviousState(ActionStateMachine previousState) {
        this.previousState = previousState;
    }

    public virtual string getName() {
        return "ActionStateMachine";
    }

    public virtual ActionStateMachine run() {
        return null;
    }

    public virtual void handleEvent(Event e) {

    }

    public virtual void handleInput(SMInput smInput) {

    }

}