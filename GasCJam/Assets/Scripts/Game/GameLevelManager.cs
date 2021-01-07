﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelManager : SingletonBase<GameLevelManager>
{
    [Header("Characters")]
    public PlayerController m_Player;
    public NewKittenMovement m_Kitten;
    public MouseMovement m_Mouse;

    [Header("UI")]
    public GameObject m_LoseScreen;
    public GameObject m_WinScreen;

    [Header("Hole Scenerio")]
    public float m_ScaleMultiplier = 1.0f;

    //lose
    public void RatEnterHole()
    {
        //FOR RAT ENTERING HOLE
        //pause the player
        //kitten starts crying
        //rat shrinks into hole
        //player watch quick cutscene

        m_Player.m_Pause = true;
        //TODO::might need pause the kitten? stop movement
        m_Kitten.m_Animator.SetBool("Crying", true);
        //TODO::make the rat stop too
        StartCoroutine(RatIntoHoleEffects());
    }

    IEnumerator RatIntoHoleEffects()
    {
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

        //show the lose UI and restart button
    }

    //lose
    public void KittenSeesCat()
    {
        //FOR KITTEN SEEING PLAYER
        //pause the player
        //immediately show kitten shock
    }

    //kitten manage to eat the rat
    //go to next level
    public void Win()
    {
        //wait for a second or so before showing the win screen?
        //SHOW THE WIN UI
    }
}
