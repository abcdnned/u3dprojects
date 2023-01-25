using UnityEngine;

public static class CustomEasingFunction
{
    public enum Ease
    {
        EaseFootWalk,
    }

    //the rotation of IK foot target when walking
    public static float EaseFootWalk(float time, float value1)
    {
        if (time < 0.25) {

        } else if (time > 0.75) {

        }
        return 0;
    }
    public delegate float Function(float v, float v2);

    public static Function GetEasingFunction(Ease easingFunction)
    {
        if (easingFunction == Ease.EaseFootWalk)
        {
            return EaseFootWalk;
        }
        return null;
    }
}
