using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
    }

    public void TransitionPage()
    {
        LevelManager.Instance.Transition();
    }

    public void GoToLevel(int levelNumber)
    {
        LevelManager.Instance.GoToLevel(levelNumber);
    }
}
