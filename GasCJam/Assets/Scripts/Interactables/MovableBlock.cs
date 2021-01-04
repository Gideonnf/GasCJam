using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableBlock : MonoBehaviour
{
    private Vector2Int m_CurrGridPos;
    private bool m_BeingPushed;

    // Start is called before the first frame update
    void Start()
    {
        //get current grid pos and store
        m_CurrGridPos = MapManager.Instance.GetWorldToTilePos(transform.position);
        MapManager.Instance.AddToMap(m_CurrGridPos);

        m_BeingPushed = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //on collision with the player, need check to see whether the block left its original grid pos
        if (collision.collider.tag == "Player")
        {
            m_BeingPushed = true;
            StartCoroutine("Pushed");
            //Debug.Log("being pushed");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //dont need to check
        if (collision.collider.tag == "Player")
        {
            m_BeingPushed = false;
            StopCoroutine("Pushed");
            //Debug.Log("stop pushed");
        }
    }

    IEnumerator Pushed()
    {
        //update it in the map
        while (m_BeingPushed)
        {
            Vector2Int newGridPos = MapManager.Instance.GetWorldToTilePos(transform.position);
            if (newGridPos != m_CurrGridPos)
            {
                MapManager.Instance.RemoveFromMap(m_CurrGridPos);
                MapManager.Instance.AddToMap(newGridPos);

                m_CurrGridPos = newGridPos;
            }

            yield return null;
        }
    }
}
