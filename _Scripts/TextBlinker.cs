using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBlinker : MonoBehaviour
{

    private TextMesh m_textMesh; //TextMesh of the gameObject that should blink
    private IEnumerator m_co; //coroutine to launch
    private bool m_loopActive; // will the text blink forever or not
    private float m_appearTime; // time taken by the text to appear
    private float m_fadeTime; // time taken by the text to fade
    private bool m_startFading; // will the text start fading or disappearing
    private bool m_autoDestroy; // will the object destroy itself if m_autoDestroy is true AND m_loopActive is false

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

    public float AppearTime
    {
        get
        {
            return m_appearTime;
        }

        set
        {
            m_appearTime = value;
        }
    }

    public bool LoopActive
    {
        get
        {
            return m_loopActive;
        }

        set
        {
            m_loopActive = value;
        }
    }


    public bool StartFading
    {
        get
        {
            return m_startFading;
        }

        set
        {
            m_startFading = value;
        }
    }

    public bool AutoDestroy
    {
        get
        {
            return m_autoDestroy;
        }

        set
        {
            m_autoDestroy = value;
        }
    }

    void Start()
    {

    }


    /// <summary>
    /// Initializes the parameters of the blinking, and launches the coroutine for the blinking
    /// </summary>
    /// <param name="appearTime">time taken by the text to appear</param>
    /// <param name="fadeTime">time taken by the text to fade</param>
    /// <param name="loopActive"> will the text blink forever or not</param>
    /// <param name="startFading"> will the text start fading or disappearing</param>
    /// <param name="autoDestroy">will the object destroy itself if m_autoDestroy is true AND m_loopActive is false</param>
    public void blink(float appearTime, float fadeTime, bool loopActive, bool startFading, bool autoDestroy)
    {
        m_appearTime = appearTime;
        m_fadeTime = fadeTime;
        m_loopActive = loopActive;
        m_startFading = startFading;
        m_autoDestroy = autoDestroy;

        m_co = blinkRoutine();
        StartCoroutine(m_co);

        if ((!m_loopActive) && m_autoDestroy)
            Destroy(this.gameObject, (m_fadeTime + m_appearTime));
    }

    /// <summary>
    /// Changes the alpha component of the text for the blinking
    /// </summary>
    /// <returns></returns>
    IEnumerator blinkRoutine()
    {
        m_textMesh = GetComponentInParent<TextMesh>();
        Color originalColor = m_textMesh.color;

        //In case the text color has its alpha component not set to 255, reinitialize it :
        originalColor.a = 1.0f;

        // blinking forever is activated by default
        bool localLoop = true;

        if (m_textMesh)
        {
            while (localLoop)
            {
                if (m_startFading)
                {
                    for (float t = 0.0f; t < m_fadeTime; t += Time.deltaTime)
                    {
                        m_textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, originalColor.a * (1 - t / m_fadeTime));
                        yield return null;
                    }

                    for (float t = 0.0f; t < m_appearTime; t += Time.deltaTime)
                    {
                        m_textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, originalColor.a * (t / m_appearTime));
                        yield return null;
                    }
                }
                else
                {
                    for (float t = 0.0f; t < m_appearTime; t += Time.deltaTime)
                    {
                        m_textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, originalColor.a * (t / m_appearTime));
                        yield return null;
                    }

                    for (float t = 0.0f; t < m_fadeTime; t += Time.deltaTime)
                    {
                        m_textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, originalColor.a * (1 - t / m_fadeTime));
                        yield return null;
                    }
                }
                localLoop = m_loopActive;
            }

        }

        yield return null;
    }

}