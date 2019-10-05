using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] private Transform shieldTransform = null;
    [SerializeField] private Camera mainCamera = null;
    private Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);

    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        if (mousePosition.x < 0 || mousePosition.x > screenRect.width || mousePosition.y < 0 || mousePosition.y > screenRect.height)
        {
            return;
        }

        mousePosition.z = shieldTransform.position.z - mainCamera.transform.position.z;
        shieldTransform.position = mainCamera.ScreenToWorldPoint(mousePosition);
    }
}
