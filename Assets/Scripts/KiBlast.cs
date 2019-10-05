using UnityEngine;

public class KiBlast : MonoBehaviour
{
    [SerializeField] private float speed = 0.0F;
    [SerializeField] private float damage = 1.0F;

    private void Update()
    {
        Vector3 newPosition = transform.position;
        Vector3 moveAmount = transform.up * speed * Time.deltaTime;
        newPosition.x += moveAmount.x;
        newPosition.y += moveAmount.y;
        newPosition.z += moveAmount.z;
        transform.position = newPosition;
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
