using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void Start()
    {
        SoundManager.Instance.Play("BackgroundMusic");
    }

    public void QuitGame()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
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

    public void ButtonClick()
    {
        SoundManager.Instance.Play("ButtonClick");
    }

    public void GoToScene(string sceneName)
    {
        LevelManager.Instance.OpenScene(sceneName);
    }
}
