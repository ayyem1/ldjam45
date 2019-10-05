using System;
using UnityEngine;

public class Sentry : MonoBehaviour
{
    [SerializeField] private Transform sentryTransform = null;
    [SerializeField] private Camera mainCamera = null;
    [SerializeField] private float sentryRadius = 0.0F;
    [SerializeField] private Vector3 sentryCenter = Vector3.zero;

    private Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);

    private void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = sentryTransform.position.z - mainCamera.transform.position.z;
        mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);

        Vector3 vectorToMouse = mousePosition - sentryCenter;
        // Clamp the sentry arc to the top half of the circle.
        // This translates to all y coordinates greater than
        // the center
        vectorToMouse.y = Mathf.Max(sentryCenter.y, vectorToMouse.y);
        // This addition is so that the sentry travels the whole
        // path to the other side of the semi-circle instead of
        // jumping there.
        vectorToMouse.y += 1.0F;
        float sqrmag = vectorToMouse.sqrMagnitude;
        float magnitude = (float) Math.Sqrt(sqrmag);
        float normalizedX = vectorToMouse.x / magnitude;
        float normalizedY = vectorToMouse.y / magnitude;
        vectorToMouse.x = normalizedX * sentryRadius;
        vectorToMouse.y = normalizedY * sentryRadius;

        sentryTransform.position = sentryCenter + vectorToMouse;
    }
}
