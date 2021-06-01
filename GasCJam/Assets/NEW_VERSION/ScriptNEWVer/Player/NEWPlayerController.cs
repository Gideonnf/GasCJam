using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NEWPlayerController : MonoBehaviour
{
    [Header("Movement data")]
    public float m_MoveSpeed = 1.0f;

    Vector2Int m_NextTilePos;
    Vector2Int m_Dir;
    bool m_NextTileReached = true;

    [Header("Visual")]
    public Animator m_Animator;

    void Start()
    {
        Vector2Int currTilePos = MapManager.Instance.GetWorldToTilePos(transform.position);
        m_NextTilePos = currTilePos;

        transform.position = MapManager.Instance.GetTileToWorldPos(currTilePos); //make sure player is exactly in middle of tile
        m_Dir = Vector2Int.zero;

        m_NextTileReached = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_NextTileReached)
            return;

        // get input for next direction
        if (Input.GetButton("Vertical"))
        {
            if (Input.GetAxis("Vertical") < 0.0f)
                SetNextTile(0, -1);
            else
                SetNextTile(0, 1);
        }
        else if (Input.GetButton("Horizontal"))
        {
            if (Input.GetAxis("Horizontal") < 0.0f)
                SetNextTile(-1, 0);
            else
                SetNextTile(1, 0);
        }

        UpdateSound();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        //move player to the proper grid pos
        Vector2 nextTileWorldPos = MapManager.Instance.GetTileToWorldPos(m_NextTilePos);
        Vector2 dir = nextTileWorldPos - (Vector2)transform.position;
        if (Vector2.Dot(dir, m_Dir) <= 0) //overshot the position, set exactly to the nextTileWorldPos
        {
            transform.position = nextTileWorldPos;
            m_Dir = Vector2Int.zero;
            m_NextTileReached = true;
        }
        else
        {
            transform.position = (Vector2)transform.position + (Vector2)m_Dir * m_MoveSpeed * Time.fixedDeltaTime;
        }
    }

    public void SetNextTile(int nextTileDirX, int nextTileDirY)
    {
        Vector2Int dir = new Vector2Int(nextTileDirX, nextTileDirY);
        Vector2Int nextTile = m_NextTilePos + dir;

        if (!MapManager.Instance.IsThereTileOnMap(nextTile)) //if next tile is empty, allow to walk
        {
            m_Dir = dir;
            m_NextTilePos = nextTile;
            m_NextTileReached = false;
        }
    }

    public void UpdateAnimation()
    {
        if (m_Animator != null)
        {
            m_Animator.SetFloat("Horizontal", m_Dir.x);
            m_Animator.SetFloat("Vertical", m_Dir.y);
        }
    }

    public void UpdateSound()
    {
        if (m_Dir == Vector2.zero)
            SoundManager.Instance.Stop("CatWalking");
        else
            SoundManager.Instance.Play("CatWalking");
    }
}
