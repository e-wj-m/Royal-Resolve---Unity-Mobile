using System.Collections;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class MeleeSwipeTrail : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GestureDetector gestureDetector;
    [SerializeField] private Transform pivot;                 

    [Header("Swing Feel")]
    [SerializeField] private float swingDuration = 0.15f;     // seconds for the arc
    [SerializeField] private float swingArcDegrees = 140f;    // total arc angle
    [SerializeField] private float swingRadius = 1.1f;        // how far from pivot the blade tip is
    [SerializeField] private float trailZLocal = 0f;          // keep consistent in 2D

    private TrailRenderer tr;
    private Coroutine swingRoutine;

    private void Awake()
    {
        tr = GetComponent<TrailRenderer>();
        tr.enabled = false;
        tr.Clear();

        if (pivot == null && transform.parent != null)
            pivot = transform.parent;
    }

    private void OnEnable()
    {
        if (gestureDetector != null)
            gestureDetector.OnSwipeDetected += HandleSwipe;
    }

    private void OnDisable()
    {
        if (gestureDetector != null)
            gestureDetector.OnSwipeDetected -= HandleSwipe;
    }

    private void HandleSwipe(SwipeDirection dir)
    {
        if (pivot == null) return;

        if (swingRoutine != null)
            StopCoroutine(swingRoutine);

        swingRoutine = StartCoroutine(Swing(dir));
    }

    private IEnumerator Swing(SwipeDirection dir)
    {
        // Decide swing plane angle

        float centerAngle = DirectionToAngle(dir);

        // Arc from (center - halfArc) to (center + halfArc)
        float halfArc = swingArcDegrees * 0.5f;
        float startAngle = centerAngle - halfArc;
        float endAngle = centerAngle + halfArc;

        // Enable trail and clear so it starts crisp
        tr.Clear();
        tr.enabled = true;

        float t = 0f;
        while (t < swingDuration)
        {
            float u = t / swingDuration;
            float angle = Mathf.Lerp(startAngle, endAngle, EaseOutCubic(u));

            // Place the trail tip at a radius around the pivot
            Vector3 localPos = AngleToLocalOffset(angle) * swingRadius;
            localPos.z = trailZLocal;

            transform.localPosition = localPos;

            t += Time.deltaTime;
            yield return null;
        }

        // Ensure final position
        {
            Vector3 localPos = AngleToLocalOffset(endAngle) * swingRadius;
            localPos.z = trailZLocal;
            transform.localPosition = localPos;
        }

        // Small delay to let the trail finish drawing then disable
        yield return new WaitForSeconds(0.03f);
        tr.enabled = false;
        swingRoutine = null;
    }

    private float DirectionToAngle(SwipeDirection dir)
    {
        switch (dir)
        {
            case SwipeDirection.Right: return 0f;
            case SwipeDirection.Up: return 90f;
            case SwipeDirection.Left: return 180f;
            case SwipeDirection.Down: return 270f;
            default: return 0f;
        }
    }

    // Angle in degrees 
    private Vector3 AngleToLocalOffset(float deg)
    {
        float rad = deg * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);
    }

    private float EaseOutCubic(float x)
    {
        x = Mathf.Clamp01(x);
        float oneMinus = 1f - x;
        return 1f - (oneMinus * oneMinus * oneMinus);
    }
}