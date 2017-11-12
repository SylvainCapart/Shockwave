using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple fade animation, then destroys the object
/// </summary>
public class TextFader : MonoBehaviour {

    private TextMesh m_textMesh;
    private IEnumerator m_co;
    private float m_fadeTime;

    public float FadeTime
    {
        get
        {
            return m_fadeTime;
        }

        set
        {
            m_fadeTime = value;
        }
    }

    void Start()
    {
        m_fadeTime = Globals.GENERIC_TEXT_FADE;
    }

    public void fade(float fadeTime)
    {
        m_fadeTime = fadeTime;
        m_co = fadeOut();
        StartCoroutine(m_co);
        Destroy(this.gameObject, m_fadeTime);
    }

    IEnumerator fadeOut()
    {
        
        m_textMesh = GetComponentInParent<TextMesh>();

        if (m_textMesh)
        {
            Color originalColor = m_textMesh.color;
            originalColor.a = 1.0f;
            for (float t = 0.0f; t < m_fadeTime; t += Time.deltaTime)
            {
                m_textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, originalColor.a * (1 - t / m_fadeTime));
                yield return null;
            }
        }

        yield return null;
    }

}