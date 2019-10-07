using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutTitle : MonoBehaviour
{
    float fadeSpeed = 0.01f;
    public Image titlePanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (CutsceneManager.isFirstSpacebarHit == false && CutsceneManager.isCutsceneStarted == true)
        {
            float newAlpha = titlePanel.color.a - fadeSpeed;

            titlePanel.color = new Color(titlePanel.color.r, titlePanel.color.g, titlePanel.color.b, newAlpha);
        }
    }
}
