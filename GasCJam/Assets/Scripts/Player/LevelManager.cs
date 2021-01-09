using System.Collections.Generic;

public class LevelManager : SingletonBase<LevelManager>
{
    int m_CurrLevel; //if in menu or anything its 0
    public int m_LevelUnlocked;
    LevelLoader m_LevelChanger;

    public List<string> m_LevelSceneNames = new List<string>();
    public string m_MainMenuName;

    public int m_MidLevelStory = 5;

    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();

        m_CurrLevel = 0;
        m_LevelUnlocked = 0;

        m_LevelChanger = GetComponent<LevelLoader>();
    }

    public void LevelCleared()
    {
        if (m_CurrLevel == m_LevelUnlocked)
        {
            m_LevelUnlocked = m_CurrLevel + 1;
        }
    }

    public void ReplayCurrentLevel()
    {
        m_LevelChanger.TransitionScene(m_LevelSceneNames[m_CurrLevel]);
    }

    //go next level directly
    public void GoToNextLevel()
    {
        LevelCleared();

        m_CurrLevel = m_CurrLevel + 1;

        if (m_CurrLevel == m_MidLevelStory)
        {
            OpenScene("StoryMode2");
        }
        else
        {
            m_LevelChanger.TransitionScene(m_LevelSceneNames[m_CurrLevel]);
        }
    }

    //going to a specific level
    public void GoToLevel(int levelNumber)
    {
        m_CurrLevel = levelNumber;
        m_LevelChanger.TransitionScene(m_LevelSceneNames[levelNumber]);
    }

    public void OpenScene(string sceneName)
    {
        m_LevelChanger.TransitionScene(sceneName);
    }

    public void GoBackToMenu()
    {
        m_LevelChanger.TransitionScene(m_MainMenuName);
    }

    public void Transition()
    {
        m_LevelChanger.Transition();
    }
}
