using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideBoardSystem : MonoBehaviour {

    // Arrays of pieces object on the sides of the game
    private GameObject[] m_piecesOneArray;
    private GameObject[] m_piecesTwoArray;

    // Holding of the pieces prefab :
    public GameObject p_bluePiece;
    public GameObject p_redPiece;

    // Number of pieces left, used to score
    private static int m_piecesOneLeft;
    private static int m_piecesTwoLeft;


    // Use this for initialization
    void Start () {

        p_bluePiece = (GameObject)Resources.Load("_Prefabs/Player_1_Piece", typeof(GameObject));
        p_redPiece = (GameObject)Resources.Load("_Prefabs/Player_2_Piece", typeof(GameObject));

        m_piecesOneArray = new GameObject[Globals.INITIAL_PIECES_ONE + 1];
        m_piecesTwoArray = new GameObject[Globals.INITIAL_PIECES_TWO + 1];

        initializeSidePieces();

    }


    public void initializeSidePieces()
    {

        m_piecesOneLeft = Globals.INITIAL_PIECES_ONE;
        m_piecesTwoLeft = Globals.INITIAL_PIECES_TWO;

        for (int i = 1; i < Globals.INITIAL_PIECES_ONE + 1; ++i)
        {
            m_piecesOneArray[i] = Instantiate(p_bluePiece, new Vector3(-50.0f, -10, 110 - i * 7.0f), Quaternion.Euler(new Vector3(90, 0, 0)));
            m_piecesOneArray[i].transform.SetParent(GameObject.Find("SideSurfaces").transform);

            // Script destroyed from the piece, kinematic, no gravity : no interaction with environnement
            // TODO : prefab without script, then add the script to the prefabs
            Destroy(m_piecesOneArray[i].GetComponent<PieceSystem>());
            m_piecesOneArray[i].GetComponent<Rigidbody>().isKinematic = true;
            m_piecesOneArray[i].GetComponent<Rigidbody>().useGravity = false;

        }

        for (int i = 1; i < Globals.INITIAL_PIECES_TWO + 1; ++i)
        {
            m_piecesTwoArray[i] = Instantiate(p_redPiece, new Vector3(216.0f, -10, 110 - i * 7.0f), Quaternion.Euler(new Vector3(-90, 0, 0)));
            m_piecesTwoArray[i].transform.SetParent(GameObject.Find("SideSurfaces").transform);
            Destroy(m_piecesTwoArray[i].GetComponent<PieceSystem>());
            m_piecesTwoArray[i].GetComponent<Rigidbody>().isKinematic = true;
            m_piecesTwoArray[i].GetComponent<Rigidbody>().useGravity = false;

        }
    }


    public void deleteSidePieces()
    {
        for (int i = 1; i < Globals.INITIAL_PIECES_ONE + 1; ++i)
        {
            Destroy(m_piecesOneArray[i]);
        }
        for (int i = 1; i < Globals.INITIAL_PIECES_TWO + 1; ++i)
        {
            Destroy(m_piecesTwoArray[i]);
        }
    }

    public void removePiece(int activePlayer)
    {
        switch(activePlayer)
        {
            case Globals.PLAYER_ONE:
                if (m_piecesOneLeft > 0)
                {
                    Destroy(m_piecesOneArray[m_piecesOneLeft]);
                    --m_piecesOneLeft;
                }
                break;
            case Globals.PLAYER_TWO:
                if (m_piecesTwoLeft > 0)
                {
                    Destroy(m_piecesTwoArray[m_piecesTwoLeft]);
                    --m_piecesTwoLeft;
                }
                break;
            default:
                break;

        }
        
    }

    public bool playerHasPieces(int activePlayer)
    {
        switch (activePlayer)
        {
            case Globals.PLAYER_ONE:
                return (m_piecesOneLeft > 0);  
            case Globals.PLAYER_TWO:
                return (m_piecesTwoLeft > 0);
            default:
                return false;
        }
    }

    public bool noPiecesLeft()
    {
        return ((m_piecesOneLeft == 0) && (m_piecesTwoLeft == 0));
    }


}
