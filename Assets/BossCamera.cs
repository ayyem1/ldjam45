using System.Collections;
using UnityEngine;

public class BossCamera : MonoBehaviour
{
    [SerializeField] private Vector3 desiredAmountToMoveBack = Vector3.zero;
    private float nonBossZ = 0.0F;
    private Camera mainCamera = null;

    private void Awake()
    {
        GameManager.OnBossSpawned += OnBossSpawned;
        Temptation.OnBossDied += ResetZValue;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        nonBossZ = mainCamera.transform.position.z;
    }

    private void OnBossSpawned()
    {
        StartCoroutine(MoveCameraBack());
    }

    private IEnumerator MoveCameraBack()
    {
        float amountToMovePerFrame = 5.0f;
        Vector3 position;
        while (true)
        {
            position = transform.position;
            position.x -= Mathf.Min(amountToMovePerFrame, desiredAmountToMoveBack.x);
            position.y -= Mathf.Min(amountToMovePerFrame, desiredAmountToMoveBack.y);
            position.z -= Mathf.Min(amountToMovePerFrame, desiredAmountToMoveBack.z);
            yield return 0;
        }

    }

    private void ResetZValue()
    {
        Vector3 position = mainCamera.transform.position;
        if (position.z != nonBossZ)
        {
            position.z = nonBossZ;
            mainCamera.transform.position = position;
        }
    }
}
