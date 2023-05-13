using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Fader : MonoBehaviour
{
    private Image toFade;
    private Color faded;

    [SerializeField]
    private float fadeSpeed = 1;

    private void Awake()
    {
        toFade = GetComponent<Image>();
        faded = toFade.color;
    }
    public void Hide(bool hidden)
    {
        toFade.enabled = !hidden;
    }
    public IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = faded.a;
        float t = 0;

        do
        {
            t += Time.deltaTime * fadeSpeed;
            if(t > 1)
                t = 1;

            faded.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            toFade.color = faded;
            yield return null;
        }
        while(t != 1);
    }
}
