using UnityEngine;

public class KiBlast : MonoBehaviour
{
    [SerializeField] private float speed = 0.0F;
    private Camera mainCamera = null;

    private void Start()
    {
        mainCamera = Camera.main;
    }
    private void FixedUpdate()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        bool onScreen = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        if (onScreen == false)
        {
            Destroy(gameObject);
        }

        Vector3 newPosition = transform.position;
        Vector3 moveAmount = transform.up * speed * Time.fixedDeltaTime;
        newPosition.x += moveAmount.x;
        newPosition.y += moveAmount.y;
        //newPosition.z += moveAmount.z;
        transform.position = newPosition;
    }

    private void OnTriggerEnter(Collider other)
    {   
        DestroyKiBlast();
    }

    private void DestroyKiBlast()
    {
        //Do KiBlast destroy vfx and sounds here
        Destroy(this.gameObject);
    }
}
