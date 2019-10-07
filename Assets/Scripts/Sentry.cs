using System;
using System.Collections;
using UnityEngine;

public class Sentry : MonoBehaviour
{
    [SerializeField] private float sentryRadius = 0.0F;
    [SerializeField] private Vector3 sentryCenter = Vector3.zero;
    [SerializeField] private float sentryCooldownInSeconds = 0.1F;
    [SerializeField] private Transform launchPoint = null;
    [SerializeField] private GameObject kiBlastPrefabToSpawn = null;

    private Camera mainCamera = null;
    private bool isSentryCoolingDown = false;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Vector3 toObjectVector = transform.position - mainCamera.transform.position;
        Vector3 linearDistanceVector = Vector3.Project(toObjectVector, Camera.main.transform.forward);

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = linearDistanceVector.magnitude;
        mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);

        Vector3 vectorToMouse = mousePosition - sentryCenter;
        // Ignore z for the arc math.
        vectorToMouse.z = 0;
        // Clamp the sentry arc to the top half of the circle.
        // This translates to all y coordinates greater than
        // the center
        vectorToMouse.y = Mathf.Max(0.0F, vectorToMouse.y);
        // This addition is so that the sentry travels the whole
        // path to the other side of the semi-circle instead of
        // jumping there.
        vectorToMouse.y += .25F;

        float sqrmag = vectorToMouse.sqrMagnitude;
        float magnitude = (float) Math.Sqrt(sqrmag);
        float normalizedX = vectorToMouse.x / magnitude;
        float normalizedY = vectorToMouse.y / magnitude;
        vectorToMouse.x = normalizedX * sentryRadius;
        vectorToMouse.y = normalizedY * sentryRadius;

        Quaternion rotation = Quaternion.LookRotation(transform.forward, vectorToMouse);
        vectorToMouse.x += sentryCenter.x;
        vectorToMouse.y += sentryCenter.y;
        vectorToMouse.z += sentryCenter.z;

        transform.SetPositionAndRotation(vectorToMouse, rotation);
    }

    public void FireSentry()
    {
        if (isSentryCoolingDown == false && GameManager.Instance.ammoAmount > 0)
        {
            StartCoroutine(Shoot());
            isSentryCoolingDown = true;
        }
    }

    private IEnumerator Shoot()
    {
        GameManager.Instance.ammoAmount -= 1;
        GameObject projectile = Instantiate(kiBlastPrefabToSpawn);
        projectile.transform.SetPositionAndRotation(launchPoint.position, launchPoint.rotation);
        yield return new WaitForSeconds(sentryCooldownInSeconds);
        isSentryCoolingDown = false;
    }

    private void TestFunction()
    {
        Vector3 toObjectVector = transform.position - mainCamera.transform.position;
        Vector3 linearDistanceVector = Vector3.Project(toObjectVector, Camera.main.transform.forward);
        float actualDistance = linearDistanceVector.magnitude;
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = actualDistance;
        transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
    }
}
