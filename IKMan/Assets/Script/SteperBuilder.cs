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

    public Steper Build()
    {
        return new Steper(forward, right, duration, lerpFunction, body, target, points);
    }
}
