using System;
using System.Collections;
using UnityEngine;

public class Sentry : MonoBehaviour
{
    // Movement
    [SerializeField] private Camera mainCamera = null;
    [SerializeField] private float sentryRadius = 0.0F;
    [SerializeField] private Vector3 sentryCenter = Vector3.zero;

    // Shoot
    [SerializeField] private float sentryCooldownInSeconds = 0.1F;
    [SerializeField] private uint ammoAmount = 10;
    [SerializeField] private Transform launchPoint = null;
    [SerializeField] private GameObject kiBlastPrefabToSpawn = null;

    private bool isSentryCoolingDown = false;

    private void Update()
    {
        if (Input.GetAxis("Fire1") > 0)
        {
            if (isSentryCoolingDown == false && ammoAmount > 0)
            {
                StartCoroutine(Shoot());
                isSentryCoolingDown = true;
            }
        }

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = transform.position.z - mainCamera.transform.position.z;
        mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);

        Vector3 vectorToMouse = mousePosition - sentryCenter;
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

    private IEnumerator Shoot()
    {
        ammoAmount -= 1;
        GameObject projectile = Instantiate(kiBlastPrefabToSpawn);
        projectile.transform.SetPositionAndRotation(launchPoint.position, launchPoint.rotation);
        yield return new WaitForSeconds(sentryCooldownInSeconds);
        isSentryCoolingDown = false;
    }

    public uint GetAmmoAmount()
    {
        return ammoAmount;
    }

    public void AddAmmo(uint amountToAdd)
    {
        ammoAmount += amountToAdd;
    }

    public void RemoveAmmo(uint amountToRemove)
    {
        if (amountToRemove > ammoAmount)
        {
            ammoAmount = 0;
        }
        else
        {
            ammoAmount -= amountToRemove;
        }
    }
}
