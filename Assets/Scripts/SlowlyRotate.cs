using UnityEngine;

public class SlowlyRotate : MonoBehaviour
{
    public GameObject parentGameObject;
    public float rotateSpeed = 1;

    // Update is called once per frame
    void FixedUpdate()
    {
        float rotateAmount = rotateSpeed * Time.fixedDeltaTime;
        Vector3 oldEulerAngles = parentGameObject.transform.eulerAngles;
        parentGameObject.transform.Rotate(0f, rotateAmount, 0f);
    }
}
