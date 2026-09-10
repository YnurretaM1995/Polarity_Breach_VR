using System.Collections;
using UnityEngine;

public class VRScreenFade : MonoBehaviour
{
    [SerializeField] private Renderer fadeRenderer;
    [SerializeField] private float fadeDuration = 1f;

    private Material fadeMaterial;

    private void Awake()
    {
        if (fadeRenderer == null) fadeRenderer = GetComponent<Renderer>();

        fadeMaterial = fadeRenderer.material;
        SetAlpha(0f);
        fadeRenderer.enabled = false;
    }

    public IEnumerator FadeOut()
    {
        fadeRenderer.enabled = true;
        yield return Fade(0f, 1f);
    }

    public IEnumerator FadeIn()
    {
        yield return Fade(1f, 0f);
        fadeRenderer.enabled = false;
    }

    private IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        SetAlpha(from);

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            SetAlpha(Mathf.Lerp(from, to, Mathf.Clamp01(t / fadeDuration)));
            yield return null;
        }

        SetAlpha(to);
    }

    private void SetAlpha(float a)
    {
        if (fadeMaterial == null) return;

        Color c = fadeMaterial.GetColor("_BaseColor");
        c.a = a;
        fadeMaterial.SetColor("_BaseColor", c);
    }
}