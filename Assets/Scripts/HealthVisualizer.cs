using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthVisualizer : MonoBehaviour
{
    private SpriteRenderer[] healthCircles;
    private int healthCircleIndex = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        GameManager.PlayerDamaged -= UpdateHealthDisplay;
        GameManager.PlayerDamaged += UpdateHealthDisplay;

        GameManager.OnGameStarted -= SetupHealthDisplay;
        GameManager.OnGameStarted += SetupHealthDisplay;

        this.healthCircles = GetComponentsInChildren<SpriteRenderer>();
    }

    private void UpdateHealthDisplay()
    {
        if (this.healthCircleIndex < this.healthCircles.Length)
        {
            this.healthCircles[this.healthCircleIndex].gameObject.SetActive(false);
        }

        this.healthCircleIndex++;
    }

    private void SetupHealthDisplay()
    {
        for (int i = 0; i < this.healthCircles.Length; i++)
        {
            this.healthCircles[i].gameObject.SetActive(true);
        }

        this.healthCircleIndex = 0;
    }
}
