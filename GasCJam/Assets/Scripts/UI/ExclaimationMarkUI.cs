using UnityEngine;

public class ExclaimationMarkUI : MonoBehaviour
{
    public RectTransform m_StartPos;
    public RectTransform m_EndPos;

    public RectTransform m_Mask;

    public void OnEnable()
    {
        m_Mask.position = m_StartPos.position;
    }

    public void UpdateExclaimationMarkUI(float offset)
    {
        //get the precentage its suppose to be highlighted
        float yDist = m_EndPos.position.y - m_StartPos.position.y;

        m_Mask.position = new Vector2(m_Mask.position.x, m_StartPos.position.y + yDist * offset);
    }
}
