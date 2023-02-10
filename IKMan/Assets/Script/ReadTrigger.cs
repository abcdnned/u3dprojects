using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ReadTrigger {
    private bool value = false;

    public ReadTrigger(bool initValue) {
        value = initValue;
    }

    public void set() {
        value = true;
        // Debug.Log(this.GetType().Name + " set " + value);
    }
    
    public bool peek() {
        return value;
    }

    public void clear() {
        value = false;
    }

    public bool read() {
        // Debug.Log(this.GetType().Name + " read " + value);
        bool result = value;
        if (value) {
            value = false;
        }
        return result;
    }
}