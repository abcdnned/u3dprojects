using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Timer {
    private float time;

    private ReadTrigger on = new ReadTrigger(false);

    public void setTimer(float time) {
        on.clear();
        this.time = time;
        // Debug.Log(this.GetType().Name + " timer " + time);
    }

    public void reset() {
        on.clear();
        this.time = 0;
    }

    public void countDown(float dt) {
        if (time > 0 && !on.peek()) {
            time -= dt;
            // Debug.Log(this.GetType().Name + " count down " + time);
            if (time <= 0) {
                on.set();
                // Debug.Log(this.GetType().Name + " on.set ");
            }
        }
    }

    public bool check() {
        return on.read();
    }


}