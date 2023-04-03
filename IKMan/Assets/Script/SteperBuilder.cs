using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteperBuilder
{
    private Vector3 forward;
    private Vector3 right;
    private float duration;
    private int lerpFunction;

    private Transform body;
    private Vector3 realForward;
    private Vector3 realRight;
    int mode;
    private Transform target;
    private Vector3[] points;

    public SteperBuilder WithForward(Vector3 forward)
    {
        this.forward = forward;
        return this;
    }

    public SteperBuilder WithRight(Vector3 right)
    {
        this.right = right;
        return this;
    }

    public SteperBuilder WithDuration(float duration)
    {
        this.duration = duration;
        return this;
    }

    public SteperBuilder WithLerpFunction(int lerpFunction)
    {
        this.lerpFunction = lerpFunction;
        return this;
    }

    public SteperBuilder WithBody(Transform body)
    {
        this.body = body;
        return this;
    }

    public SteperBuilder WithTarget(Transform target)
    {
        this.target = target;
        return this;
    }

    public SteperBuilder WithPoints(Vector3[] points)
    {
        this.points = points;
        return this;
    }

    public SteperBuilder WithRealForward(Vector3 realForward)
    {
        this.realForward = realForward;
        return this;
    }

    public SteperBuilder WithRealRight(Vector3 realRight)
    {
        this.realRight = realRight;
        return this;
    }

    public SteperBuilder WithMode(int mode)
    {
        this.mode = mode;
        return this;
    }

    public Steper Build()
    {
        return new Steper(forward,
                          right,
                          duration,
                          lerpFunction,
                          body,
                          mode,
                          target,
                          points);
    }
}
