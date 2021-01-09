using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionScreenUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button m_RightButton;
    public Button m_LeftButton;

    [Header("Panels")]
    public GameObject m_ParentPanel;

    List<GameObject> m_InstructionPanels = new List<GameObject>();

    int m_CurrPanel;

    // Start is called before the first frame update
    void Awake()
    {
        foreach (Transform child in m_ParentPanel.transform)
        {
            //Do your stuff
            m_InstructionPanels.Add(child.gameObject);
        }

        m_CurrPanel = 0;
    }

    private void OnEnable()
    {
        m_CurrPanel = 0;

        foreach (GameObject panel in m_InstructionPanels)
        {
            panel.SetActive(false);
        }

        m_InstructionPanels[m_CurrPanel].SetActive(true);
        if (m_LeftButton != null)
            m_LeftButton.interactable = false;
        if (m_RightButton != null)
            m_RightButton.interactable = true;
    }

    public void NextPanel()
    {
        ++m_CurrPanel;
        if (m_CurrPanel >= m_InstructionPanels.Count - 1)
        {
            m_CurrPanel = m_InstructionPanels.Count - 1;
            m_RightButton.interactable = false;
        }

        //enable left button
        if (m_LeftButton != null)
        {
            if (!m_LeftButton.interactable)
                m_LeftButton.interactable = true;
        }

        if (m_CurrPanel - 1 >= 0)
            m_InstructionPanels[m_CurrPanel - 1].SetActive(false);

        m_InstructionPanels[m_CurrPanel].SetActive(true);
    }

    public void LeftPanel()
    {
        --m_CurrPanel;
        if (m_CurrPanel <= 0)
        {
            m_CurrPanel = 0;
            m_LeftButton.interactable = false;
        }

        //enable left button
        if (!m_RightButton.interactable)
            m_RightButton.interactable = true;

        if (m_CurrPanel + 1 < m_InstructionPanels.Count)
            m_InstructionPanels[m_CurrPanel + 1].SetActive(false);

        m_InstructionPanels[m_CurrPanel].SetActive(true);
    }

    public void NextPanelTransitionScene()
    {
        ++m_CurrPanel;
        if (m_CurrPanel >= m_InstructionPanels.Count)
        {
            //go to next scene
            LevelManager.Instance.GoToLevel(0);
            return;
        }

        if (m_CurrPanel - 1 >= 0)
            m_InstructionPanels[m_CurrPanel - 1].SetActive(false);

        m_InstructionPanels[m_CurrPanel].SetActive(true);
    }
}
