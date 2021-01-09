using UnityEngine;

public class EndScreenManager : MonoBehaviour
{
    public void Start()
    {
        //play song
        SoundManager.Instance.Play("Background");
    }

    // Start is called before the first frame update
    public void GoBackToMainMenu()
    {
        LevelManager.Instance.GoBackToMenu();
    }

    public void ButtonClickNoise()
    {
        SoundManager.Instance.Play("ButtonClick");
    }
}
