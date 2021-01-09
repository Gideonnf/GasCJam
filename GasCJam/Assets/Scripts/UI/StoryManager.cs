using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public int m_Level = 0;

    public void Start()
    {
        //play song
        //SoundManager.Instance
    }

    public void StartLevel()
    {
        LevelManager.Instance.GoToLevel(m_Level);
    }
}
