using UnityEngine;

public class InGameMenuManager : SingletonBase<InGameMenuManager>
{
    //keeps the gameobject for the blur
    public GameObject m_BlurEffect;

    //keeps the different type of menus
    public GameObject[] m_MenuType;
    public GameObject m_InGameMenu;

    public void OpenMenu(int type)
    {
        if (m_BlurEffect != null)
            m_BlurEffect.SetActive(true);

        if (m_MenuType.Length >= type)
            m_MenuType[type].SetActive(true);

        if (type == (int)MenuType.WIN_SCREEN || type == (int)MenuType.LOSE_SCREEN)
        {
            if (m_InGameMenu != null)
                m_InGameMenu.SetActive(false);
        }
    }

    public void CloseMenu(int type)
    {
        //close the blur first
        if (m_BlurEffect != null)
            m_BlurEffect.SetActive(false);

        if (m_MenuType.Length >= type)
            m_MenuType[type].SetActive(false);
    }

    public void TogglePauseMenu()
    {
        if (m_BlurEffect != null)
            m_BlurEffect.SetActive(!m_BlurEffect.activeSelf);

        if (m_MenuType[(int)MenuType.PAUSE_SCREEN] != null)
            m_MenuType[(int)MenuType.PAUSE_SCREEN].SetActive(!m_MenuType[(int)MenuType.PAUSE_SCREEN].activeSelf);
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

    public void PlayButtonClick()
    {
        SoundManager.Instance.Play("ButtonClick");
    }
}

public enum MenuType
{
    WIN_SCREEN,
    LOSE_SCREEN,
    PAUSE_SCREEN
}