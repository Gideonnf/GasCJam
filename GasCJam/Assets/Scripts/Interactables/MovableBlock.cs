using System.Collections;
using UnityEngine;

public class MovableBlock : MonoBehaviour
{
    private Vector2Int m_CurrGridPos;

    private bool m_BeingPushed;
    private PlayerController m_PlayerController;
    private Vector2 m_PushedDir;
    private bool m_StopParticleEffect;

    [Header("Particle Effect")]
    public ParticleSystem m_RightParticle;
    public ParticleSystem m_LeftParticle;
    public ParticleSystem[] m_DownParticle;
    public ParticleSystem[] m_UpParticle;


    // Start is called before the first frame update
    void Start()
    {
        //get current grid pos and store
        m_CurrGridPos = MapManager.Instance.GetWorldToTilePos(transform.position);
        MapManager.Instance.AddToMap(m_CurrGridPos);

        m_PlayerController = FindObjectOfType<PlayerController>();

        m_BeingPushed = true;
        m_StopParticleEffect = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //on collision with the player, need check to see whether the block left its original grid pos
        if (collision.collider.tag == "Player")
        {
            m_BeingPushed = true;
            StartCoroutine("Pushed");

            if (m_PlayerController != null)
                m_PlayerController.Pushing(m_BeingPushed);

            TurnOnParticleEffect();
        }
    }

    public void TurnOnParticleEffect()
    {
        if (m_PlayerController == null)
            return;

        //check the direction of the player
        m_PushedDir = m_PlayerController.m_Dir;
        m_StopParticleEffect = false;

        if (m_PushedDir.x < 0.0f) //going left
        {
            m_LeftParticle.Play();
        }
        else if (m_PushedDir.x > 0.0f)
        {
            m_RightParticle.Play();
        }
        else
        {
            if (m_PushedDir.y < 0.0f)
            {
                m_DownParticle[0].Play();
                m_DownParticle[1].Play();
            }
            else if (m_PushedDir.y > 0.0f)
            {
                m_UpParticle[0].Play();
                m_UpParticle[1].Play();
            }
        }
    }

    public void TurnOffParticleEffect()
    {
        if (m_PlayerController == null)
            return;

        m_StopParticleEffect = true;

        //check the direction of the player
        if (m_PushedDir.x < 0.0f) //going left
        {
            m_LeftParticle.Stop();
        }
        else if (m_PushedDir.x > 0.0f)
        {
            m_RightParticle.Stop();
        }
        else
        {
            if (m_PushedDir.y < 0.0f)
            {
                m_DownParticle[0].Stop();
                m_DownParticle[1].Stop();
            }
            else if (m_PushedDir.y > 0.0f)
            {
                m_UpParticle[0].Stop();
                m_UpParticle[1].Stop();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //dont need to check
        if (collision.collider.tag == "Player")
        {
            m_BeingPushed = false;
            StopCoroutine("Pushed");

            if (m_PlayerController != null)
                m_PlayerController.Pushing(m_BeingPushed);

            TurnOffParticleEffect();
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

            if (m_PlayerController != null)
            {
                if (m_PlayerController.m_Dir == Vector3.zero)
                {
                    TurnOffParticleEffect();
                }
                else
                {
                    if (m_StopParticleEffect)
                        TurnOnParticleEffect();
                }
            }

            yield return null;
        }
    }
}
