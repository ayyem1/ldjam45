using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//No, this doesn't actually fade.  Yes I am sorry.
//It is 6:36 and I am dying
public class FadeOutAfterTime : MonoBehaviour
{
    public float timeBeforeFade;
    public GameObject fadingImage;

    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(this.FadeOutImage());
    }

    private IEnumerator FadeOutImage()
    {
        yield return new WaitForSeconds(timeBeforeFade);

        this.gameObject.SetActive(false);
    }
}
