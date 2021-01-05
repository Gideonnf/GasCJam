using System.Collections.Generic;
using UnityEngine;

public class LevelManager : SingletonBase<LevelManager>
{
    int m_CurrLevel; //if in menu or anything its 0
    int m_LevelUnlocked;
    LevelLoader m_LevelChanger;

    public List<string> m_LevelSceneNames = new List<string>();

    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();

        m_CurrLevel = 1;
        m_LevelUnlocked = 0;

        m_LevelChanger = GetComponent<LevelLoader>();
    }

    public void LevelCleared()
    {
        if (m_CurrLevel > m_LevelUnlocked)
        {
            m_LevelUnlocked = m_CurrLevel;
        }
    }

    //go next level directly
    public void GoToNextLevel()
    {
        m_CurrLevel = m_CurrLevel + 1;
        m_LevelChanger.TransitionScene(m_LevelSceneNames[m_CurrLevel]);
    }

    //going to a specific level
    public void GoToLevel(int levelNumber)
    {
        m_CurrLevel = levelNumber;
        m_LevelChanger.TransitionScene(m_LevelSceneNames[levelNumber]);
    }
}
