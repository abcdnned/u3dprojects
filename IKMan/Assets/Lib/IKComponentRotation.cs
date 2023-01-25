using UnityEngine;

public static class IKComponentRotation
{
    public static float mappingFootRotation(float time, float maxAngel)
    {
        if (time <= 0.45) {
            float v = Mathf.Clamp(time, 0, 0.45f);
            v /= 0.45f;
            return Bezier(0, maxAngel * 2, 0, v);
        } else if (time >= 0.55) {
            float v = Mathf.Clamp(time, 0.55f, 1);
            v -= 0.55f;
            v /= 0.45f;
            return Bezier(0, -maxAngel, 0, v);
        }
        return 0;
    }

    public static float Bezier(float start, float center, float end, float cur) {
        float value =
        Mathf.Lerp(
            Mathf.Lerp(start, center, cur),
            Mathf.Lerp(center, end, cur),
            cur
        );
        return value;
    }
}
