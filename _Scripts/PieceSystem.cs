using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached to a piece object
/// TODO : examine what functionalities from boardSystem should be held here
/// </summary>
public class PieceSystem : MonoBehaviour
{
    private bool m_isMoving;
    private GameObject m_pieceObject;
    private Rigidbody m_pieceRigididbody;

    public bool IsMoving
    {
        get
        {
            return m_isMoving;
        }

        set
        {
            m_isMoving = value;
        }
    }

    public Rigidbody PieceRigididbody
    {
        get
        {
            return m_pieceRigididbody;
        }

        set
        {
            m_pieceRigididbody = value;
        }
    }

    public GameObject PieceObject
    {
        get
        {
            return m_pieceObject;
        }

        set
        {
            m_pieceObject = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        m_pieceObject = null;
        PieceRigididbody = GetComponent<Rigidbody>();
        m_isMoving = false;

    }

    /// <summary>
    /// Moves the piece from point A to point B in time seconds
    /// </summary>
    /// <param name="pointA"></param>
    /// <param name="pointB"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public IEnumerator MoveFromTo(Vector3 pointA, Vector3 pointB, float time)
    {
        if (!m_isMoving)
        { // do nothing if already moving
            m_isMoving = true; 
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / time; 
                transform.position = Vector3.Lerp(pointA, pointB, t); // set position proportional to t
                yield return null; // returns here during the next frame
            }
            m_isMoving = false;
        }
    }
}

