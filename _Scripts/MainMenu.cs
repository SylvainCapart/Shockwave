using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {

    //TODO : use an array of gameobjects corresponding to the texts
    private GameObject m_titleObj; //3D text for the name of the game
    private GameObject m_versionObj; //3D text for the version of the game
    private GameObject m_startObj; //3D text for the start

    //TODO : examine if Unity needs explicit destructions

    public GameObject TitleObj
    {
        get
        {
            return m_titleObj;
        }

        set
        {
            m_titleObj = value;
        }
    }

    public GameObject VersionObj
    {
        get
        {
            return m_versionObj;
        }

        set
        {
            m_versionObj = value;
        }
    }

    public GameObject StartObj
    {
        get
        {
            return m_startObj;
        }

        set
        {
            m_startObj = value;
        }
    }

    void Start () {

        // TODO : remove hard-coded values
        m_titleObj = TextSystem.initializeText("ShockWave", "Title", new Vector3(53f, 64f, 93f), Quaternion.Euler(new Vector3(Globals.PLAYER_TEXT_RX, Globals.PLAYER_TEXT_RY, Globals.PLAYER_TEXT_RZ)),
            Globals.p_fontGames, 78, ColorMgt.p_cyan);
        m_versionObj = TextSystem.initializeText("v0.1", "Version", new Vector3(70f, 64f, 83f), Quaternion.Euler(new Vector3(Globals.PLAYER_TEXT_RX, Globals.PLAYER_TEXT_RY, Globals.PLAYER_TEXT_RZ)),
            Globals.p_fontGames, 48, ColorMgt.p_cyan);
        m_startObj = TextSystem.initializeText("Click to start", "Start", new Vector3(61f, 64f, 70f), Quaternion.Euler(new Vector3(Globals.PLAYER_TEXT_RX, Globals.PLAYER_TEXT_RY, Globals.PLAYER_TEXT_RZ)),
            Globals.p_fontThirsty, 48, ColorMgt.p_cyan);

        // highlight the start text
        m_startObj.AddComponent<TextBlinker>().blink(Globals.BLINK_TIME, Globals.BLINK_TIME, true, true, false);

    }

    /// <summary>
    /// Game is launched, stop the blinking and start fading the texts :
    /// </summary>
    public void initialize()
    {
        Destroy(m_startObj.GetComponent<TextBlinker>());
        m_versionObj.AddComponent<TextFader>().fade(Globals.GENERIC_TEXT_FADE);
        m_startObj.AddComponent<TextFader>().fade(Globals.GENERIC_TEXT_FADE);
        m_titleObj.AddComponent<TextFader>().fade(Globals.GENERIC_TEXT_FADE);

    }

}
 