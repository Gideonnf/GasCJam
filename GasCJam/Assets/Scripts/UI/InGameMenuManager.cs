using UnityEngine;

public class InGameMenuManager : SingletonBase<InGameMenuManager>
{
    //keeps the gameobject for the blur
    public GameObject m_BlurEffect;

    //keeps the different type of menus
    public GameObject[] m_MenuType;

    public void OpenMenu(int type)
    {
        if (m_BlurEffect != null)
            m_BlurEffect.SetActive(true);

        if (m_MenuType.Length >= type)
            m_MenuType[type].SetActive(true);
    }

    public void CloseMenu(int type)
    {
        //close the blur first
        if (m_BlurEffect != null)
            m_BlurEffect.SetActive(false);

        if (m_MenuType.Length >= type)
            m_MenuType[type].SetActive(false);
    }

    public void NextLevel()
    {
        LevelManager.Instance.GoToNextLevel();
    }

    public void RestartLevel()
    {
        LevelManager.Instance.ReplayCurrentLevel();
    }

    public void GoBackToMainMenu()
    {
        LevelManager.Instance.GoBackToMenu();
    }
}

public enum MenuType
{
    WIN_SCREEN,
    LOSE_SCREEN,
    PAUSE_SCREEN
}