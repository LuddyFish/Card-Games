using System.Collections;
using UnityEngine;

public static class AnimationUtilities
{
    /// <summary>
    /// Move <paramref name="origin"/> along a linear path
    /// </summary>
    /// <param name="obj">The object to move</param>
    /// <param name="origin">Start position</param>
    /// <param name="target">End position</param>
    /// <param name="duration">Length of time to perform rotation</param>
    public static IEnumerator Lerp(Transform obj, Vector2 origin, Vector2 target, float duration)
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
    /// <param name="duration">Length of time to perform rotation</param>
    /// <returns></returns>
    public static IEnumerator SlerpAroundPoint(Transform obj, Vector3 origin, Vector3 target, Vector3 point, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            obj.position = SlerpAroundPoint(origin, target, point.y, t / duration);
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
    /// <param name="centreOffset">Point to rotate around along the y axis</param>
    /// <param name="t">
    /// Time in rotation between <paramref name="start"/> and <paramref name="end"/>
    /// <br/>0 = start; 1 = finish
    /// </param>
    /// <returns>The current <c>Vector3</c> position at <paramref name="t"/> time</returns>
    public static Vector3 SlerpAroundPoint(Vector3 start, Vector3 end, float centreOffset, float t)
    {
        var centrePivot = (start + end) / 2 - new Vector3(0, -centreOffset);
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
    public static IEnumerator SpiralIn(Transform obj, Vector3 distance, Vector3 center, int revolutions, float duration)
    {
        float t = 0;
        float r = (distance - center).magnitude;
        Vector3 dir = (distance - center).normalized;
        float startAngle = Vector3.SignedAngle(Vector3.right, dir, Vector3.forward);

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;
            float angle = revolutions * Mathf.PI * 2 * p + startAngle;
            Vector3 offset = new Vector3(
                Mathf.Cos(angle) * r * (1f - p),
                Mathf.Sin(angle) * r * (1f - p)
            );

            obj.position = center + offset;

            yield return new WaitForEndOfFrame();
        }
        obj.position = center;
    }

    /// <summary>
    /// Move <paramref name="obj"/> in a circular motion outwards from <paramref name="center"/> to <paramref name="distance"/>
    /// </summary>
    /// <param name="obj">The object to move</param>
    /// <param name="distance">End position</param>
    /// <param name="center">Center of sprial / Start position</param>
    /// <param name="revolutions">Number of revolutions</param>
    /// <param name="duration">Duration of action</param>
    /// <returns></returns>
    public static IEnumerator SpiralOut(Transform obj, Vector3 distance, Vector3 center, int revolutions, float duration)
    {
        float t = duration;
        float r = (distance - center).magnitude;
        Vector3 dir = (distance - center).normalized;
        float startAngle = Vector3.SignedAngle(Vector3.right, dir, Vector3.forward);

        while (t > 0)
        {
            t -= Time.deltaTime;
            float p = t / duration;
            float angle = revolutions * Mathf.PI * 2 * p + startAngle;
            Vector3 offset = new Vector3(
                Mathf.Cos(angle) * r * (1f - p),
                Mathf.Sin(angle) * r * (1f - p)
            );

            obj.position = center + offset;

            yield return new WaitForEndOfFrame();
        }
        obj.position = distance;
    }
}
