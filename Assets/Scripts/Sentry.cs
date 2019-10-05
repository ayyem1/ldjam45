using System;
using System.Collections;
using UnityEngine;

public class Sentry : MonoBehaviour
{
    [SerializeField] private Transform sentryTransform = null;
    [SerializeField] private Camera mainCamera = null;
    [SerializeField] private float sentryRadius = 0.0F;
    [SerializeField] private Vector3 sentryCenter = Vector3.zero;
    [SerializeField] private float sentryCooldownInSeconds = 0.1F;
    [SerializeField] private uint ammoAmount = 10;
    [SerializeField] private GameObject projectileToSpawn;

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
        mousePosition.z = sentryTransform.position.z - mainCamera.transform.position.z;
        mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);

        Vector3 vectorToMouse = mousePosition - sentryCenter;
        Debug.Log("Vector to mouse: " + vectorToMouse);
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

        Quaternion rotation = transform.rotation;
        rotation.SetLookRotation(vectorToMouse);
        vectorToMouse.x += sentryCenter.x;
        vectorToMouse.y += sentryCenter.y;
        vectorToMouse.z += sentryCenter.z;
        sentryTransform.SetPositionAndRotation(vectorToMouse, rotation);
    }

    private IEnumerator Shoot()
    {
        ammoAmount -= 1;
        // TODO: Spawn projectile.
        yield return new WaitForSeconds(sentryCooldownInSeconds);
        isSentryCoolingDown = false;
    }

    public uint GetAmmoAmout()
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
