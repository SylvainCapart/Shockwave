using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{

    private GameObject m_mainMenu; // object hlding the Menu texts
    private bool m_launch; // prevents the launch from happening twice

    // Use this for initialization
    void Start()
    {
        // Initialization of the main menu
        m_launch = false;
        m_mainMenu = new GameObject();
        m_mainMenu.transform.SetParent(GameObject.Find("Canvas").transform);
        m_mainMenu.name = "MainMenuScript";
        m_mainMenu.AddComponent<MainMenu>();
    }

    void Update()
    {
        // Launch corountine executed only once
        StartCoroutine(Launch());
    }

    IEnumerator Launch()
    {
        // Game is launched when the user clicks
        if (Input.GetMouseButtonDown(Globals.FIRE1))
        {
            if (!m_launch)
            {
                m_launch = true;
                m_mainMenu.GetComponent<MainMenu>().initialize();

                // wait until the menu texts fade away
                yield return new WaitForSeconds(Globals.GENERIC_TEXT_FADE);

                // Surfaces is the gameobject holding the board, the pieces
                GameObject surfaces = new GameObject("Surfaces");

                // BoardSystem is the main script holding the game functionnalities
                surfaces.AddComponent<BoardSystem>();

                // SideSurfaces is the gameobject holding the side pieces, and managing the pieces left
                GameObject sideSurfaces = new GameObject("SideSurfaces");
                sideSurfaces.AddComponent<SideBoardSystem>();

                Destroy(m_mainMenu);
                Destroy(this.gameObject);
            }
        }
        yield return null;
    }

}
