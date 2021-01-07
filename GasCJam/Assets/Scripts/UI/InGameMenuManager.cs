using UnityEngine;

public class InGameMenuManager : SingletonBase<InGameMenuManager>
{
    //keeps the gameobject for the blur
    public GameObject m_BlurEffect;

    //keeps the different type of menus
    public GameObject[] m_MenuType;

    public void OpenMenu(MenuType type)
    {
        if (m_BlurEffect != null)
            m_BlurEffect.SetActive(true);

        if (m_MenuType.Length >= (int)type)
            m_MenuType[(int)type].SetActive(true);
    }

    public void CloseMenu(MenuType type)
    {
        //close the blur first
        if (m_BlurEffect != null)
            m_BlurEffect.SetActive(false);

        if (m_MenuType.Length >= (int)type)
            m_MenuType[(int)type].SetActive(false);
    }
}

public enum MenuType
{
    WIN_SCREEN,
    LOSE_SCREEN,
    PAUSE_SCREEN
}