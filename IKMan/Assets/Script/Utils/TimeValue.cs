using System;
using UnityEngine;

public class TimeValue<T>
{
    T initValue;

    T targetValue;

    Func<T, T, float, T> lerp;

    float initTime;

    float duration;

    internal void init(T v, T t, float duration, Func<T, T, float, T> lerp) {
        initValue = v;
        targetValue = t;
        this.duration = duration;
        initTime = Time.time;
        this.lerp = lerp;
    }

    internal T getValue() {
        // Debug.Log(" Time " + (Time.time - initTime) / duration);
        return lerp(initValue, targetValue, (Time.time - initTime) / duration);
    }

    internal void reset() {
        initTime = Time.time;
    }

    internal bool overdue() {
        return initTime + duration < Time.time;
    }

}
