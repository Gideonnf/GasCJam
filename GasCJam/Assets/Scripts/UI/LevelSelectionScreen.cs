using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionScreen : MonoBehaviour
{
    //store all the available levels UI button
    public GameObject m_LevelButtons;

    Button[] m_Buttons;

    // Start is called before the first frame update
    void Start()
    {
        m_Buttons = m_LevelButtons.GetComponentsInChildren<Button>();

        //get from the level manager which level is currently unlocked
        int levelUnlocked = LevelManager.Instance.m_LevelUnlocked;
        for (int i = 0; i < m_Buttons.Length; ++i)
        {
            if (i > levelUnlocked)
                m_Buttons[i].interactable = false;
            else
                m_Buttons[i].interactable = true;
        }
    }
}
