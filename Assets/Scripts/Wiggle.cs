using UnityEngine;

public class Wiggle
{
    private float time = 0f;
    private float startTime = 0f;
    public float frequency = 1f;
    public float strength = 10f;
    public Vector2 direction = Vector2.zero;
    private float randomAngle = 0f;
    private float secondaryRandomAngle = 0f;
    private float lastAbsSin = 0f;
    private float lastSinDir = 0f;
    private float pi = 0f;

    public Wiggle(float str, float freq, float tim, Vector2 dir)
    {
        direction = dir;
        strength = str;
        frequency = freq;
        startTime = time = tim;
    }

    public bool IsDone()
    {
        return time == 0f;
    }

    public Quaternion GetRotation(AnimationCurve strengthOverLifetime = null)
    {
        time = Mathf.MoveTowards(time, 0f, Time.deltaTime);
        pi += Time.deltaTime * frequency;
        if (pi > Mathf.PI) pi -= Mathf.PI;
        float sin = Mathf.Sin(pi * frequency);
        Vector2 result = Vector2.zero;
        if (direction != Vector2.zero)
        {
            Vector3 cross = Vector3.Cross(direction, Vector3.forward);
            float angle = strength * sin;
            if (strengthOverLifetime != null) angle *= strengthOverLifetime.Evaluate(1f - (time / startTime));
            return Quaternion.AngleAxis(angle, cross);
        }
        else
        {
            float absSin = Mathf.Abs(sin);
            if (lastAbsSin > absSin)
            {
                int sinDir = sin > 0f ? 1 : -1;
                if (lastSinDir != sinDir)
                {
                    randomAngle += Random.Range(-120f, 120f);
                    lastSinDir = sin > 0f ? 1 : -1;
                }
            }
            lastAbsSin = absSin;

            Vector2 dir = Quaternion.AngleAxis(randomAngle, Vector3.forward) * Vector2.up;
            Vector3 cross = Vector3.Cross(dir, Vector3.forward);
            float angle = strength * sin;
            if (strengthOverLifetime != null) angle *= strengthOverLifetime.Evaluate(1f - (time / startTime));
            return Quaternion.AngleAxis(angle, cross);
        }
    }


    public Vector3 GetPosition(AnimationCurve strengthOverLifetime = null)
    {
        time = Mathf.MoveTowards(time, 0f, Time.deltaTime);
        pi += Time.deltaTime * frequency;
        if (pi > Mathf.PI * 2f) pi -= Mathf.PI * 2f;
        float sin = Mathf.Sin(pi);
        Vector2 result = Vector2.zero;
        if (direction != Vector2.zero)
        {
            float offset = strength * sin;
            if (strengthOverLifetime != null) strength *= strengthOverLifetime.Evaluate(1f - (time / startTime));
            return direction * offset;
        }
        else
        {
            float absSin = Mathf.Abs(sin);
            if (lastAbsSin >= absSin && absSin > 0.5f)
            {
                int sinDir = sin > 0f ? 1 : -1;
                if (lastSinDir != sinDir)
                {
                    if (sin > 0f)
                    {
                        randomAngle += Random.Range(-180f, 180f);
                        if (Mathf.Abs(randomAngle - secondaryRandomAngle) < 90f) randomAngle += Random.Range(50f, 180f);
                    }
                    else
                    {
                        secondaryRandomAngle += Random.Range(-180f, 180f);
                        if (Mathf.Abs(randomAngle - secondaryRandomAngle) < 90f) secondaryRandomAngle += Random.Range(50f, 180f);
                    }
                    lastSinDir = sin > 0f ? 1 : -1;
                }
            }
            lastAbsSin = absSin;

            Vector2 fromDir = Quaternion.AngleAxis(randomAngle, Vector3.forward) * Vector2.up;
            Vector2 toDir = Quaternion.AngleAxis(secondaryRandomAngle, Vector3.forward) * Vector2.up;
            float offset = strength * sin;
            if (strengthOverLifetime != null) offset *= strengthOverLifetime.Evaluate(1f - (time / startTime));
            return Vector3.Lerp(fromDir * strength, toDir * strength, Mathf.InverseLerp(-1f, 1f, sin));
        }
    }

    public void SetDecay(float t)
    {
        startTime = time = t;
    }

}