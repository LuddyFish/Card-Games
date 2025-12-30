using System.Collections;
using UnityEngine;

public static class AnimationUtilities
{
    // --- Coroutine Runner ---
    private class CoroutineRunner : MonoBehaviour { }
    
    private static CoroutineRunner Instance;

    private static CoroutineRunner Runner
    {
        get
        {
            if (Instance == null)
            {
                var go = new GameObject("AnimationUtilities_CoroutineRunner");
                Object.DontDestroyOnLoad(go);
                Instance = go.AddComponent<CoroutineRunner>();
            }
            return Instance;
        }
    }

    // --- Animation Utilities --- 
    /// <summary>
    /// Move <paramref name="obj"/> along a linear path
    /// </summary>
    /// <param name="obj">The object to move</param>
    /// <param name="origin">Start position</param>
    /// <param name="target">End position</param>
    /// <param name="duration">Length of time to perform action</param>
    public static Coroutine Lerp(Transform obj, Vector2 origin, Vector2 target, float duration)
        => Runner.StartCoroutine(LerpLinear(obj, origin, target, duration));

    private static IEnumerator LerpLinear(Transform obj, Vector2 origin, Vector2 target, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            obj.position = Vector2.Lerp(origin, target, t / duration);
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        obj.position = target;
    }

    /// <summary>
    /// Move <paramref name="obj"/> along an arc around <paramref name="point"/>
    /// </summary>
    /// <param name="obj">The object to move</param>
    /// <param name="origin">Start position</param>
    /// <param name="target">End position</param>
    /// <param name="point">The position to arc around</param>
    /// <param name="duration">Length of time to perform action</param>
    /// <returns></returns>
    public static Coroutine SlerpAroundPoint(Transform obj, Vector3 origin, Vector3 target, Vector3 point, float duration)
        => Runner.StartCoroutine(LerpAround(obj, origin, target, point, duration));

    private static IEnumerator LerpAround(Transform obj, Vector3 origin, Vector3 target, Vector3 point, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            obj.position = SlerpAroundPoint(origin, target, point, t / duration);
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        obj.position = target;
    }

    /// <summary>
    /// Move a position around a specified point
    /// </summary>
    /// <param name="start">Origin position</param>
    /// <param name="end">Target position</param>
    /// <param name="centreOffset">Point to rotate</param>
    /// <param name="t">
    /// Time in rotation between <paramref name="start"/> and <paramref name="end"/>
    /// <br/>0 = start; 1 = finish
    /// </param>
    /// <returns>The current <c>Vector3</c> position at <paramref name="t"/> time</returns>
    public static Vector3 SlerpAroundPoint(Vector3 start, Vector3 end, Vector3 centreOffset, float t)
    {
        var centrePivot = (start + end) / 2 + centreOffset;
        var startCenter = start - centrePivot;
        var endCenter = end - centrePivot;

        return Vector3.Slerp(startCenter, endCenter, t) + centrePivot;
    }

    /// <summary>
    /// Move <paramref name="obj"/> in a circular motion inwards from <paramref name="distance"/> to <paramref name="center"/>
    /// </summary>
    /// <param name="obj">The object to move</param>
    /// <param name="distance">Origin/Start position</param>
    /// <param name="center">Center of sprial / End position</param>
    /// <param name="revolutions">Number of revolutions</param>
    /// <param name="duration">Duration of action</param>
    /// <returns></returns>
    public static Coroutine SpiralIn(Transform obj, Vector3 distance, Vector3 center, int revolutions, float duration)
        => Runner.StartCoroutine(Spiral(obj, distance, center, revolutions, duration, true));

    /// <summary>
    /// Move <paramref name="obj"/> in a circular motion outwards from <paramref name="center"/> to <paramref name="distance"/>
    /// </summary>
    /// <param name="obj">The object to move</param>
    /// <param name="distance">End position</param>
    /// <param name="center">Center of sprial / Start position</param>
    /// <param name="revolutions">Number of revolutions</param>
    /// <param name="duration">Duration of action</param>
    /// <returns></returns>
    public static Coroutine SpiralOut(Transform obj, Vector3 distance, Vector3 center, int revolutions, float duration)
        => Runner.StartCoroutine(Spiral(obj, distance, center, revolutions, duration, false));

    private static IEnumerator Spiral(Transform obj, Vector3 distance, Vector3 center, int revolutions, float duration, bool inwards)
    {
        float t = 0;
        float d = duration;
        float r = (distance - center).magnitude;
        Vector3 dir = (distance - center).normalized;
        float startAngle = Vector3.SignedAngle(Vector3.right, dir, Vector3.forward);

        while (t < d)
        {
            t += Time.deltaTime;
            float p = inwards ? (t / d) : (1 - t / d);
            float angle = revolutions * Mathf.PI * 2 * p + startAngle;
            Vector3 offset = new Vector3(
                Mathf.Cos(angle) * r * (1f - p),
                Mathf.Sin(angle) * r * (1f - p)
            );

            obj.position = center + offset;

            yield return new WaitForEndOfFrame();
        }
        obj.position = inwards ? center : distance;
    }
}
