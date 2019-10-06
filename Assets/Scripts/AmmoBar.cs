using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoBar : MonoBehaviour
{
    [SerializeField] private Slider ammoUi;

    private void Awake()
    {
        ammoUi.maxValue = GameManager.Instance.maximumAmmoCount;
    }

    // Update is called once per frame
    void Update()
    {
        ammoUi.value = GameManager.Instance.sentry.GetAmmoAmount(); 
    }
}
