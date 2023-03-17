using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Banner {

    public static string STOP_WALKING = "stop_walking";
    public static string WALKING_TO_TRANSFER = "walking_to_transfer";
    private int count;
    private int sc = 0;

    internal string id;

    public static float TIMEOUT = 2;
    object[] subs;

    private ReadTrigger checkedTrigger;

    public static int DEFAULT_CONDITION_COUNT = 0;

    private float timeout = 0;
    private int DEFAULT_BANNER_SIZE = 20;

    public Banner(string id) {
        this.count = 0;
        checkedTrigger = new ReadTrigger(false);
        subs = new object[DEFAULT_BANNER_SIZE];
        sc = 0;
        timeout = 0;
        this.id = id;
    }
    public void refresh() {
        this.count = 0;
        checkedTrigger.clear();
        subs = new object[DEFAULT_BANNER_SIZE];
        sc = 0;
        timeout = 0;
    }

    public bool available() {
        return sc > 0;
    }

    public void Finish()
    {
        if (count >= sc)
        {
            throw new Exception("Condition reached.");
        } else {
            count++;
            if (count == sc) {
                checkedTrigger.set();
            }
        }
    }

    public void registerSub(object o) {
        subs[sc] = o;
        sc++;
    }

    public void Reset() {
        count = 0;
        checkedTrigger.clear();
        subs = new object[DEFAULT_BANNER_SIZE];
        sc = 0;
    }

    public bool Check() {
        return available() && checkedTrigger.read();
    }
}
