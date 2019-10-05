using System;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] private Transform shieldTransform = null;
    [SerializeField] private Camera mainCamera = null;
    [SerializeField] private float shieldRadius = 0.0F;
    [SerializeField] private Vector3 shieldCenter = Vector3.zero;
    [SerializeField] private Renderer shieldRenderer = null;
    [SerializeField] private Material clickedMaterial = null;
    [SerializeField] private Material notClickedMaterial = null;

    private Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);

    private void Start()
    {
        shieldRenderer.material = notClickedMaterial;
    }

    private void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = shieldTransform.position.z - mainCamera.transform.position.z;
        mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);

        Vector3 vectorToMouse = mousePosition - shieldCenter;
        // Clamp the shield arc to the top half of the circle.
        // This translates to all y coordinates greater than
        // the center
        vectorToMouse.y = Mathf.Max(shieldCenter.y, vectorToMouse.y);
        // This addition is so that the shield travels the whole
        // path to the other side of the semi-circle instead of
        // jumping there.
        vectorToMouse.y += 1.0F;
        float sqrmag = vectorToMouse.sqrMagnitude;
        float magnitude = (float) Math.Sqrt(sqrmag);
        float normalizedX = vectorToMouse.x / magnitude;
        float normalizedY = vectorToMouse.y / magnitude;
        vectorToMouse.x = normalizedX * shieldRadius;
        vectorToMouse.y = normalizedY * shieldRadius;

        shieldTransform.position = shieldCenter + vectorToMouse;

        if (Input.GetMouseButtonDown(0))
        {
            shieldRenderer.material = clickedMaterial;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            shieldRenderer.material = notClickedMaterial;
        }
    }
}
