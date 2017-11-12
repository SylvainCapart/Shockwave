using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public struct BoardSlot
{
    GameObject square; // supports the game, allows to place a piece
    GameObject piece; // pieces used to play. Move on the board following collisions with pieces of the opposite type.
    byte occupiedbyPlayer; // indicates what type of piece 

    public GameObject Square
    {
        get
        {
            return square;
        }

        set
        {
            square = value;
        }
    }

    public byte OccupiedbyPlayer
    {
        get
        {
            return occupiedbyPlayer;
        }

        set
        {
            occupiedbyPlayer = value;
        }
    }

    public GameObject Piece
    {
        get
        {
            return piece;
        }

        set
        {
            piece = value;
        }
    }
}
public class BoardSystem : MonoBehaviour
{

    private float m_boardWidth; // width of a board square unit
    private float m_boardHeight; // height of a board square unit

    private byte m_xGridSlotNb; // number of square units on the board following the x axis
    private byte m_yGridSlotNb; // number of square units on the board following the z axis
    // TODO : examine the benefits of byte towards int here

    private BoardSlot[,] m_grid; // grid holding the square units, the pieces and their owner

    private byte m_activePlayer; // player active this turn

    // objects holding prefabs
    private GameObject m_blackBoard;
    private GameObject m_whiteBoard;
    private GameObject m_bluePiece;
    private GameObject m_redPiece;
    private GameObject m_explosionRed;
    private GameObject m_explosionBlue;

    // objects holding 3D texts
    private GameObject m_playerOnePopup;
    private GameObject m_playerTwoPopup;
    private GameObject m_restartText;
    private GameObject m_winText;

    //public Text m_guiText;
    private static bool m_scriptActive;
    private static bool m_endGameLaunched;
    private static bool m_playerChangeAllowed;

    // table of int holding the scores
    private int[] m_scoreArray;

    // table of objects holding the texts for the score
    private GameObject[] m_scoreTextArray;


    void Start()
    {
        m_scriptActive = true; // used to deactivate functions
        m_endGameLaunched = false; // detects if the game is finished
        m_playerChangeAllowed = true; // is a player change allowed or not

        m_boardWidth = Globals.BOARD_SLOT_WIDTH;
        m_boardHeight = Globals.BOARD_SLOT_HEIGHT;
        m_xGridSlotNb = Globals.BOARD_X_SLOT_NB;
        m_yGridSlotNb = Globals.BOARD_Y_SLOT_NB;

        m_grid = new BoardSlot[m_xGridSlotNb, m_yGridSlotNb];
        m_scoreArray = new int[Globals.PLAYER_NUMBER];
        m_scoreTextArray = new GameObject[Globals.PLAYER_NUMBER];

        m_blackBoard = (GameObject)Resources.Load("_Prefabs/Black_Board", typeof(GameObject));
        m_whiteBoard = (GameObject)Resources.Load("_Prefabs/White_Board", typeof(GameObject));
        m_bluePiece = (GameObject)Resources.Load("_Prefabs/Player_1_Piece", typeof(GameObject));
        m_redPiece = (GameObject)Resources.Load("_Prefabs/Player_2_Piece", typeof(GameObject));
        m_explosionRed = (GameObject)Resources.Load("_Prefabs/explosion_red", typeof(GameObject));
        m_explosionBlue = (GameObject)Resources.Load("_Prefabs/explosion_blue", typeof(GameObject));

        createBoard();
        playerStart();
        initializeScore();
    }

    /// <summary>
    /// Chooses randomly the first player
    /// Creates the first popup following the player chosen
    /// </summary>
    void playerStart()
    {
        m_activePlayer = (byte)UnityEngine.Random.Range(Globals.PLAYER_ONE, Globals.PLAYER_TWO + 1);
        GameObject playerStartPopup;
        switch (m_activePlayer)
        {
            case Globals.PLAYER_ONE:
                playerStartPopup = TextSystem.initializeText("Player 1 starts", "Player One Popup", new Vector3(51f, 65f, 86f),
                    Quaternion.Euler(new Vector3(Globals.PLAYER_TEXT_RX, Globals.PLAYER_TEXT_RY, Globals.PLAYER_TEXT_RZ)), Globals.p_fontThirsty, 78, ColorMgt.p_paleBlue);
                playerStartPopup.AddComponent<TextBlinker>().blink(Globals.PLAYER_TEXT_APPEAR, Globals.PLAYER_TEXT_FADE, false, false, true);
                break;
            case Globals.PLAYER_TWO:
                playerStartPopup = TextSystem.initializeText("Player 2 starts", "Player Two Popup", new Vector3(51f, 65f, 86f),
                    Quaternion.Euler(new Vector3(Globals.PLAYER_TEXT_RX, Globals.PLAYER_TEXT_RY, Globals.PLAYER_TEXT_RZ)), Globals.p_fontThirsty, 78, ColorMgt.p_paleRed);
                playerStartPopup.AddComponent<TextBlinker>().blink(Globals.PLAYER_TEXT_APPEAR, Globals.PLAYER_TEXT_FADE, false, false, true);
                break;
            default:
                break;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(Globals.FIRE1) && m_endGameLaunched)
        {
            resetGame();
        }
        else
            placePiece();
    }

    /// <summary>
    /// Deletes each square of the grid
    /// </summary>
    /// <returns></returns>
    IEnumerator deleteBoard()
    {

        for (int i = 0; i < m_xGridSlotNb; ++i)
        {
            for (int j = 0; j < m_yGridSlotNb; ++j)
            {
                Destroy(m_grid[i, j].Square);
            }
        }
        yield return null;

    }

    /// <summary>
    /// Deletes each pieces
    /// Creates an explosion at the former position
    /// Set the board slot to empty
    /// </summary>
    /// <returns></returns>
    IEnumerator deletePieces()
    {

        for (int i = 0; i < m_xGridSlotNb; ++i)
        {
            for (int j = 0; j < m_yGridSlotNb; ++j)
            {
                if (m_grid[i, j].Piece != null)
                {
                    Destroy(m_grid[i, j].Piece.GetComponent<Rigidbody>());
                    Destroy(m_grid[i, j].Piece);

                    explosionInstantiate(m_grid[i, j].OccupiedbyPlayer, i, j);

                    m_grid[i, j].OccupiedbyPlayer = Globals.EMPTY;
                }
            }
        }
        yield return null;
    }

    /// <summary>
    /// Creates a board with altenatively a black and a white square
    /// </summary>
    void createBoard()
    {
        bool isBlack = true;
        for (int i = 0; i < m_xGridSlotNb; ++i)
        {
            for (int j = 0; j < m_yGridSlotNb; ++j)
            {

                m_grid[i, j].OccupiedbyPlayer = Globals.EMPTY;
                if (isBlack)
                    m_grid[i, j].Square = Instantiate(m_blackBoard, new Vector3(i * m_boardWidth, 0, j * m_boardHeight), Quaternion.identity);
                else
                    m_grid[i, j].Square = Instantiate(m_whiteBoard, new Vector3(i * m_boardWidth, 0, j * m_boardHeight), Quaternion.identity);
                m_grid[i, j].Square.transform.SetParent(GameObject.Find("Surfaces").transform);

                isBlack = !isBlack;

                m_grid[i, j].Piece = null;

            }
            if (m_yGridSlotNb % 2 == 0)
                isBlack = !isBlack;
        }
    }

    /// <summary>
    /// Places a piece on the board
    /// Moves the pieces following conditions (see below)
    /// </summary>
    void placePiece()
    {
        if (m_scriptActive)
        {
            // if the user performs a left click when the game is not finished
            if (Input.GetMouseButtonDown(Globals.FIRE1) && !m_endGameLaunched)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                float x;
                float z;
                byte iSlotIndex = 0;
                byte jSlotIndex = 0;

                // if the mouse clicks on an object within a distance of MAX_DISTANCE, stores its position in hit 
                if (Physics.Raycast(ray, out hit, Globals.MAX_DISTANCE))
                {
                    x = hit.transform.gameObject.transform.position.x;
                    z = hit.transform.gameObject.transform.position.z;

                    // Indexes of the board slot being clicked
                    if (m_boardWidth != 0.0F)
                        iSlotIndex = (byte)(Math.Round(x, 0) / m_boardWidth);

                    if (m_boardHeight != 0.0F)
                        jSlotIndex = (byte)(Math.Round(z, 0) / m_boardWidth);

                    // if the boardslot clicked is empty (no piece on this square)
                    if (m_grid[iSlotIndex, jSlotIndex].OccupiedbyPlayer == Globals.EMPTY)
                    {
                        // if the active player has still pieces left to place
                        if (GameObject.Find("SideSurfaces").GetComponent<SideBoardSystem>().playerHasPieces(m_activePlayer))
                        {
                            // the selected slot is occupied by the active player
                            m_grid[iSlotIndex, jSlotIndex].OccupiedbyPlayer = m_activePlayer;

                            // a piece is physically instantiated on the board
                            m_grid[iSlotIndex, jSlotIndex].Piece = pieceInstantiate(m_activePlayer, x, z);

                            // the active player has its score incremented
                            incrementScore(m_activePlayer, 1);

                            // put the piece object in the category "Pieces"
                            m_grid[iSlotIndex, jSlotIndex].Piece.transform.SetParent(GameObject.Find("Pieces").transform);

                            // removes a piece from the active player on the side
                            GameObject.Find("SideSurfaces").GetComponent<SideBoardSystem>().removePiece(m_activePlayer);

                            // if the piece was successfully instantiated
                            if (m_grid[iSlotIndex, jSlotIndex].Piece != null)
                            {
                                // examine the 8 slots around the pieces and performs movements following conditions
                                inspectAroundPieceIndex(m_grid[iSlotIndex, jSlotIndex].OccupiedbyPlayer, iSlotIndex, jSlotIndex);
                            }

                            // checks if the games shall end following the number of pieces left
                            StartCoroutine(checkEndGame());

                            // if the game is not be finished and a player change is allowed
                            if (!m_endGameLaunched && m_playerChangeAllowed)
                            {
                                playerChange();
                                StartCoroutine(playerChangePopup());
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Instantiates a piece following which player is active, at the position (x_pos, z_pos). The y-component is the altitude of the board.
    /// </summary>
    /// <param name="activePlayer"></param>
    /// <param name="x_pos"></param>
    /// <param name="z_pos"></param>
    /// <returns>the piece object instantiated from the prefab</returns>
    GameObject pieceInstantiate(byte activePlayer, float x_pos, float z_pos)
    {
        GameObject resultObject = null;
        switch (activePlayer)
        {
            case Globals.PLAYER_ONE:
                resultObject = Instantiate(m_bluePiece, new Vector3(x_pos, Globals.PIECES_Y_ORIGIN, z_pos), Quaternion.identity);
                break;
            case Globals.PLAYER_TWO:
                resultObject = Instantiate(m_redPiece, new Vector3(x_pos, Globals.PIECES_Y_ORIGIN, z_pos), Quaternion.identity);
                break;
            default:
                break;
        }
        return resultObject;
    }

    /// <summary>
    /// Instantiates an explosion following which player is active, at the position (x_pos, y_pos, z_pos).
    /// </summary>
    /// <param name="activePlayer"></param>
    /// <param name="x_pos"></param>
    /// <param name="y_pos"></param>
    /// <param name="z_pos"></param>
    void explosionInstantiate(byte activePlayer, float x_pos, float y_pos, float z_pos)
    {
        switch (activePlayer)
        {
            case Globals.PLAYER_ONE:
                if (m_explosionBlue)
                    Instantiate(m_explosionBlue, new Vector3(x_pos, y_pos, z_pos), Quaternion.identity);
                else
                    Debug.Log("Explosion effect not found");
                break;
            case Globals.PLAYER_TWO:
                if (m_explosionRed)
                    Instantiate(m_explosionRed, new Vector3(x_pos, y_pos, z_pos), Quaternion.identity);
                else
                    Debug.Log("Explosion effect not found");
                break;
            default:
                break;
        }

    }

    /// <summary>
    /// Instantiates an explosion following which player is active, at the position (i_pos, j_pos) on the board.
    /// </summary>
    /// <param name="activePlayer"></param>
    /// <param name="i_pos"></param>
    /// <param name="j_pos"></param>
    void explosionInstantiate(byte activePlayer, int i_pos, int j_pos)
    {
        switch (activePlayer)
        {
            case Globals.PLAYER_ONE:
                if (m_explosionBlue)
                    Instantiate(m_explosionBlue, new Vector3(i_pos * Globals.BOARD_SLOT_WIDTH, Globals.PIECES_Y_ORIGIN, j_pos * Globals.BOARD_SLOT_HEIGHT), Quaternion.identity);
                else
                    Debug.Log("Explosion effect not found");
                break;
            case Globals.PLAYER_TWO:
                if (m_explosionRed)
                    Instantiate(m_explosionRed, new Vector3(i_pos * Globals.BOARD_SLOT_WIDTH, Globals.PIECES_Y_ORIGIN, j_pos * Globals.BOARD_SLOT_HEIGHT), Quaternion.identity);
                else
                    Debug.Log("Explosion effect not found");
                break;
            default:
                break;
        }

    }

    /// <summary>
    /// Goal : analyze the 8 slots (if existing) around the slot of index (iIndex, jIndex)
    /// and proceed to conditional treatement.
    /// </summary>
    /// <param name="iIndex"></param>
    /// <param name="jIndex"></param>
    ///  <returns>true if a piece is movable around (iIndex, jIndex)</returns>
    bool inspectAroundPieceIndex(int slotOwner, byte iIndex, byte jIndex)
    {
        bool movablePieceAround = false;
        for (int i = iIndex - 1; i <= iIndex + 1; ++i) // (i, j) = piece to be moved if possible
        {
            for (int j = jIndex - 1; j <= jIndex + 1; ++j)
            {
                // if the index examined is inside the board
                if (i >= 0 && i < m_xGridSlotNb && j >= 0 && j < m_yGridSlotNb)
                {
                    // if the slot examined is not empty and is occupied by a player different from slotOwner (aka the piece that is going to move other pieces)
                    if (m_grid[i, j].OccupiedbyPlayer != Globals.EMPTY && (m_grid[i, j].OccupiedbyPlayer != slotOwner))
                    {
                        // if the slot that the movable piece should occupy is free
                        if (isSlotEmpty(2 * i - iIndex, 2 * j - jIndex))
                        {
                            movablePieceAround = true;

                            // translates the piece
                            StartCoroutine(pieceTranslation(i, j, 2 * i - iIndex, 2 * j - jIndex));
                        }
                    }
                }
            }
        }
        return movablePieceAround;
    }

    /// <summary>
    /// Returns true if the slot of index (i_destIndex, j_destIndex) is empty, aka there is no piece, 
    /// or it is out of the board.
    /// Returns false otherwise.
    /// </summary>
    /// <param name="i_destIndex"></param>
    /// <param name="j_destIndex"></param>
    /// <returns></returns>
    bool isSlotEmpty(int i_destIndex, int j_destIndex)
    {
        if (i_destIndex >= 0 && i_destIndex < m_xGridSlotNb && j_destIndex >= 0 && j_destIndex < m_yGridSlotNb)
        {
            if (m_grid[i_destIndex, j_destIndex].OccupiedbyPlayer != Globals.EMPTY)
                return false;
            else
                return true;
        }
        else
        {
            return true;
        }
    }


    /// <summary>
    /// The piece placed at the slot (i_origIndex, j_origIndex) gets its movement target set to the coordinates of 
    /// the slot (i_destIndex, j_destIndex). This script is deactivated by any moving Piece until it reaches its goal.
    /// The grid of BoardSlots is updated for the fields : 
    ///  - OccupiedbyPlayer : origin slot becomes empty, destination becomes occupied by active player
    ///  - Piece : origin becomes null after destination pointer is set to the instance of the piece
    /// </summary>
    /// <param name="i_origIndex"></param>
    /// <param name="j_origIndex"></param>
    /// <param name="i_destIndex"></param>
    /// <param name="j_destIndex"></param>
    IEnumerator pieceTranslation(int i_orig, int j_orig, int i_dest, int j_dest)
    {
        if (m_grid[i_orig, j_orig].Piece != null)
        {
            Rigidbody rb = m_grid[i_orig, j_orig].Piece.GetComponent<Rigidbody>();
            Vector3 origin = new Vector3(rb.transform.position.x, 0.75f, rb.transform.position.z);
            Vector3 target = new Vector3(i_dest * Globals.BOARD_SLOT_WIDTH, 0.75f, j_dest * Globals.BOARD_SLOT_HEIGHT);
            Vector3 direction = (target - origin).normalized;

            // moves the piece from origin to target in PIECE_MOVE_TIME
            StartCoroutine(m_grid[i_orig, j_orig].Piece.GetComponent<PieceSystem>().MoveFromTo(origin, target, Globals.PIECE_MOVE_TIME));

            // wait until the piece finishes its movement
            yield return new WaitForSeconds(Globals.PIECE_MOVE_TIME);

            // Destroy the original piece once it is arrived, in order to create a new one at the proper index in the grid
            Destroy(m_grid[i_orig, j_orig].Piece.GetComponent<Rigidbody>());
            Destroy(m_grid[i_orig, j_orig].Piece);

            // if the piece that moved is still on the board
            if (i_dest >= 0 && i_dest < m_xGridSlotNb && j_dest >= 0 && j_dest < m_yGridSlotNb)
            {

                m_grid[i_dest, j_dest].OccupiedbyPlayer = m_grid[i_orig, j_orig].OccupiedbyPlayer;
                m_grid[i_dest, j_dest].Piece = pieceInstantiate(m_grid[i_orig, j_orig].OccupiedbyPlayer, i_dest * Globals.BOARD_SLOT_WIDTH, j_dest * Globals.BOARD_SLOT_HEIGHT);
                if (!m_grid[i_dest, j_dest].Piece)
                    Debug.Log("Error instantiating new Piece :" + m_grid[i_orig, j_orig].OccupiedbyPlayer);
                m_grid[i_dest, j_dest].Piece.transform.SetParent(GameObject.Find("Pieces").transform);

                //recursive call to inspectAroundPieceIndex : the moved piece will also move the other pieces once it is arrived
                inspectAroundPieceIndex(m_grid[i_dest, j_dest].OccupiedbyPlayer, (byte)i_dest, (byte)j_dest);
                // TODO : examine the benefits of byte towards int here

                // In case two pieces of the same player collide on the same slot, one is bumped and is destroyed following its altitude
                StartCoroutine(checkAltitude(m_grid[i_dest, j_dest]));
            }
            else // if the piece that moved is out of the board, instantiate an explosion and decrease the score
            {
                explosionInstantiate(m_grid[i_orig, j_orig].OccupiedbyPlayer, i_dest, j_dest);
                incrementScore(m_grid[i_orig, j_orig].OccupiedbyPlayer, -1);
            }

            m_grid[i_orig, j_orig].OccupiedbyPlayer = Globals.EMPTY;
        }
    }

    /// <summary>
    /// If the piece altitude is above PIECES_Y_OUT after CHECK_ALTITUDE_TIME, it is destroyed, an explosion is instantiated and the score is decreased
    /// </summary>
    /// <param name="bs"></param>
    /// <returns></returns>
    IEnumerator checkAltitude(BoardSlot bs)
    {
        Rigidbody rb = bs.Piece.GetComponent<Rigidbody>();
        Vector3 piecePosition = bs.Piece.transform.position;

        yield return new WaitForSeconds(Globals.CHECK_ALTITUDE_TIME);

        if (rb.transform.position.y > Globals.PIECES_Y_OUT)
        {
            explosionInstantiate(bs.OccupiedbyPlayer, piecePosition.x, piecePosition.y, piecePosition.z);
            incrementScore(bs.OccupiedbyPlayer, -1);
            bs.OccupiedbyPlayer = Globals.EMPTY;
            Destroy(bs.Piece);
        }
        yield return null;
    }


    void playerChange()
    {

        if (m_activePlayer == Globals.PLAYER_ONE)
        {
            m_activePlayer = Globals.PLAYER_TWO;
        }
        else
        {
            m_activePlayer = Globals.PLAYER_ONE;
        }
        highlightPlayerScore();
    }

    /// <summary>
    /// A popup is created, blinking once, following the active player.
    /// </summary>
    /// <returns></returns>
    IEnumerator playerChangePopup()
    {
        //script deactivated during movements and popups
        m_scriptActive = false;

        // wait the end of the pieces movements before displaying the popup
        yield return new WaitForSeconds(Globals.PIECE_MOVE_TIME);

        switch (m_activePlayer)
        {
            case Globals.PLAYER_ONE:
                m_playerOnePopup = TextSystem.initializeText("Player 1", "Player One Popup", new Vector3(Globals.PLAYER_TEXT_X, Globals.PLAYER_TEXT_Y, Globals.PLAYER_TEXT_Z),
                        Quaternion.Euler(new Vector3(Globals.PLAYER_TEXT_RX, Globals.PLAYER_TEXT_RY, Globals.PLAYER_TEXT_RZ)), Globals.p_fontThirsty, 78, ColorMgt.p_paleBlue);
                m_playerOnePopup.AddComponent<TextBlinker>().blink(Globals.PLAYER_TEXT_APPEAR, Globals.PLAYER_TEXT_FADE, false, false, true);
                break;
            case Globals.PLAYER_TWO:
                m_playerTwoPopup = TextSystem.initializeText("Player 2", "Player Two Popup", new Vector3(Globals.PLAYER_TEXT_X, Globals.PLAYER_TEXT_Y, Globals.PLAYER_TEXT_Z),
                        Quaternion.Euler(new Vector3(Globals.PLAYER_TEXT_RX, Globals.PLAYER_TEXT_RY, Globals.PLAYER_TEXT_RZ)), Globals.p_fontThirsty, 78, ColorMgt.p_paleRed);
                m_playerTwoPopup.AddComponent<TextBlinker>().blink(Globals.PLAYER_TEXT_APPEAR, Globals.PLAYER_TEXT_FADE, false, false, true);
                break;
            default:
                break;

        }

        // wait the end of the popup
        yield return new WaitForSeconds(Globals.PLAYER_TEXT_TIME);

        m_scriptActive = true;
    }

    /// <summary>
    /// Score initialization : text and values
    /// </summary>
    void initializeScore()
    {
        for (int i = 0; i < Globals.PLAYER_NUMBER; ++i)
        {
            m_scoreArray[i] = 0;

            if (m_scoreTextArray[i] != null)
                Destroy(m_scoreTextArray[i]);
        }

        //TODO : remove hard-coded values
        m_scoreTextArray[Globals.PLAYER_ONE - 1] = TextSystem.initializeText(m_scoreArray[Globals.PLAYER_ONE - 1].ToString(), "Score1", new Vector3(48.5f, 69f, 96f),
          Quaternion.Euler(new Vector3(Globals.PLAYER_TEXT_RX, Globals.PLAYER_TEXT_RY, Globals.PLAYER_TEXT_RZ)), Globals.p_fontGames, 50, ColorMgt.p_paleBlue);

        m_scoreTextArray[Globals.PLAYER_TWO - 1] = TextSystem.initializeText(m_scoreArray[Globals.PLAYER_TWO - 1].ToString(), "Score2", new Vector3(100f, 69f, 96f),
           Quaternion.Euler(new Vector3(Globals.PLAYER_TEXT_RX, Globals.PLAYER_TEXT_RY, Globals.PLAYER_TEXT_RZ)), Globals.p_fontGames, 50, ColorMgt.p_paleRed);

        for (int i = 0; i < Globals.PLAYER_NUMBER; ++i)
        {
            m_scoreTextArray[i].AddComponent<TextBlinker>();
        }
        highlightPlayerScore();

    }

    void incrementScore(int playerToIncrement, int pointsToAdd)
    {
        m_scoreArray[playerToIncrement - 1] = m_scoreArray[playerToIncrement - 1] + pointsToAdd;
        updateScore(playerToIncrement);
    }

    /// <summary>
    /// Active playe score blinks.
    /// </summary>
    void highlightPlayerScore()
    {
        for (int i = 0; i < Globals.PLAYER_NUMBER; ++i)
        {
            m_scoreTextArray[i].GetComponent<TextBlinker>().LoopActive = false;
        }
        m_scoreTextArray[m_activePlayer - 1].GetComponent<TextBlinker>().blink(Globals.BLINK_TIME, Globals.BLINK_TIME, true, true, false);

    }

    void updateScore(int playerToUpdate)
    {
        m_scoreTextArray[playerToUpdate - 1].GetComponent<TextMesh>().text = m_scoreArray[playerToUpdate - 1].ToString();
    }

    /// <summary>
    /// If there is no pieces left, launches the end of the game.
    /// </summary>
    /// <returns></returns>
    IEnumerator checkEndGame()
    {
        if (GameObject.Find("SideSurfaces").GetComponent<SideBoardSystem>().noPiecesLeft())
        {
            m_playerChangeAllowed = false;
            if (!m_endGameLaunched)
            {
                m_scriptActive = false;
                m_scoreTextArray[m_activePlayer = 1].GetComponent<TextBlinker>().LoopActive = false;

                yield return (new WaitForSeconds(Globals.ENDGAME_TIMER));
                StartCoroutine(endGame());
            }
        }
        yield return null;
    }

    /// <summary>
    /// Delete pieces.
    /// Delete the board.
    /// Creates the texts of the end screen.
    /// </summary>
    /// <returns></returns>
    IEnumerator endGame()
    {
        StartCoroutine(deletePieces());
        StartCoroutine(deleteBoard());

        if (m_scoreArray[Globals.PLAYER_ONE - 1] > m_scoreArray[Globals.PLAYER_TWO - 1])
        {
            m_winText = TextSystem.initializeText("Player 1 wins !\n\n\t Badass !", "m_winText", new Vector3(45f, 54f, 90f),
               Quaternion.Euler(new Vector3(Globals.PLAYER_TEXT_RX, Globals.PLAYER_TEXT_RY, Globals.PLAYER_TEXT_RZ)), Globals.p_fontGames, 78, ColorMgt.p_cyan);
        }
        else if (m_scoreArray[Globals.PLAYER_ONE - 1] < m_scoreArray[Globals.PLAYER_TWO - 1])
        {
            m_winText = TextSystem.initializeText("Player 2 wins !\n\n\t Badass !", "m_winText", new Vector3(45f, 54f, 90f),
               Quaternion.Euler(new Vector3(Globals.PLAYER_TEXT_RX, Globals.PLAYER_TEXT_RY, Globals.PLAYER_TEXT_RZ)), Globals.p_fontGames, 78, ColorMgt.p_cyan);
        }
        else
        {
            m_winText = TextSystem.initializeText("Draw " + m_scoreArray[Globals.PLAYER_ONE - 1] + " : " + m_scoreArray[Globals.PLAYER_TWO - 1] + " !\n\n   Again !", "m_winText", new Vector3(55f, 54f, 90f),
               Quaternion.Euler(new Vector3(Globals.PLAYER_TEXT_RX, Globals.PLAYER_TEXT_RY, Globals.PLAYER_TEXT_RZ)), Globals.p_fontGames, 78, ColorMgt.p_cyan);
        }

        m_restartText = TextSystem.initializeText("Click to restart", "Restart Text", new Vector3(58f, 54f, 58f),
    Quaternion.Euler(new Vector3(Globals.PLAYER_TEXT_RX, Globals.PLAYER_TEXT_RY, Globals.PLAYER_TEXT_RZ)), Globals.p_fontThirsty, 48, ColorMgt.p_cyan);

        m_restartText.AddComponent<TextBlinker>().blink(Globals.BLINK_TIME, Globals.BLINK_TIME, true, false, false);

        GameObject.Find("SideSurfaces").GetComponent<SideBoardSystem>().deleteSidePieces();

        yield return new WaitForSeconds(Globals.ENDSCREEN_TIMER);

        m_endGameLaunched = true;

        yield return true;
    }


    void resetGame()
    {
        m_endGameLaunched = false;

        if (m_restartText)
        {
            Destroy(m_restartText.GetComponent<TextBlinker>());
            m_restartText.AddComponent<TextFader>().fade(Globals.GENERIC_TEXT_FADE);
        }

        if (m_winText)
            m_winText.AddComponent<TextFader>().fade(Globals.GENERIC_TEXT_FADE);

        createBoard();
        GameObject.Find("SideSurfaces").GetComponent<SideBoardSystem>().deleteSidePieces();
        GameObject.Find("SideSurfaces").GetComponent<SideBoardSystem>().initializeSidePieces();
        initializeScore();
        m_scriptActive = true;
        m_playerChangeAllowed = true;
        playerStart();
        highlightPlayerScore();
    }

}
