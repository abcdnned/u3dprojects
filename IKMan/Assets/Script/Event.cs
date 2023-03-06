using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Author: Sergio Abreu García | https://sergioabreu.me

/// <summary> Tells the ActiveRagdoll what it should do. Input can be external (like the
/// one from the player or from another script) and internal (kind of like sensors, such as
/// detecting if it's on floor). </summary>
public class Event {
    // ---------- EXTERNAL INPUT ----------

    internal string eventId;

    public Event() {

    }

    public Event(string eventId) {
        this.eventId = eventId;
    }

}