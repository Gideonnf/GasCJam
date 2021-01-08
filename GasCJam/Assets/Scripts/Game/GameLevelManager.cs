using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelManager : SingletonBase<GameLevelManager>
{
    [Header("Characters")]
    public PlayerController m_Player;
    public NewKittenMovement m_Kitten;
    public MouseMovement m_Mouse;

    [Header("Hole Scenerio")]
    public float m_ScaleMultiplier = 1.0f;

    public void Start()
    {
        SoundManager.Instance.Play("BackgroundMusic");
    }

    //FOR RAT ENTERING HOLE
    public void RatEnterHole()
    {
        SoundManager.Instance.Play("RatInHole");

        //pause the player
        m_Player.m_Stop = true;

        //might need pause the kitten? stop movement
        m_Kitten.m_Stop = true;

        //kitten starts crying
        m_Kitten.m_Animator.SetBool("Crying", true);

        //make the rat stop too
        m_Mouse.m_Stop = true;

        StartCoroutine(RatIntoHoleEffects());
    }

    IEnumerator RatIntoHoleEffects()
    {
        //rat shrinks into hole
        Vector3 scale = m_Mouse.gameObject.transform.localScale;
        while (scale.x > 0.0f)
        {
            scale.x -= Time.deltaTime * m_ScaleMultiplier;
            scale.y -= Time.deltaTime * m_ScaleMultiplier;

            if (scale.x < 0.0f || scale.y < 0.0f)
            {
                scale = Vector3.zero;
            }

            m_Mouse.gameObject.transform.localScale = scale;
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        //show the lose UI and restart button
        InGameMenuManager.Instance.OpenMenu((int)MenuType.LOSE_SCREEN);
        SoundManager.Instance.Play("LoseScreen");
    }

    //FOR KITTEN SEEING PLAYER
    public void KittenSeesCat()
    {
        //pause the player
        m_Player.m_Stop = true;

        StartCoroutine(KittenSeesCatEffects());
    }

    IEnumerator KittenSeesCatEffects()
    {
        yield return new WaitForSeconds(0.7f);

        InGameMenuManager.Instance.OpenMenu((int)MenuType.LOSE_SCREEN);
        SoundManager.Instance.Play("LoseScreen");
    }

    //kitten manage to eat the rat
    //go to next level
    public void Win()
    {
        //wait for a second or so before showing the win screen?
        //SHOW THE WIN UI

        //pause the rat and mouse and player
        m_Player.m_Stop = true;
        m_Kitten.m_Stop = true;
        m_Mouse.m_Stop = true;

        StartCoroutine(WinEffect());
    }

    IEnumerator WinEffect()
    {
        yield return new WaitForSeconds(1.0f);

        LevelManager.Instance.LevelCleared();
        InGameMenuManager.Instance.OpenMenu((int)MenuType.WIN_SCREEN);
        SoundManager.Instance.Play("WinScreen");
    }
}
