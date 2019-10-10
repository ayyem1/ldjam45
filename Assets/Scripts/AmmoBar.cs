using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoBar : MonoBehaviour
{
    [SerializeField] private Slider ammoUi = null;
    [SerializeField] private float drainRateInSeconds = 0.5f;

    private void Awake()
    {
        ammoUi.maxValue = GameManager.Instance.maximumAmmoCount;
        StartCoroutine(this.DrainAmmo());
    }

    // Update is called once per frame
    void Update()
    {
        ammoUi.value = GameManager.Instance.ammoAmount;
    }

    private IEnumerator DrainAmmo()
    {
        while (true)
        {
            yield return new WaitForSeconds(this.drainRateInSeconds);

            GameManager.Instance.RemoveAmmo(1);
        }
    }
}
