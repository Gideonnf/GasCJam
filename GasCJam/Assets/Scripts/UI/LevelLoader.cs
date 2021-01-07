using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField]
    Animator m_Transition;
    [SerializeField]
    float m_TransitionTime;

    public void TransitionScene(string sceneName)
    {
        StartCoroutine(LoadLevel(sceneName));
    }

    public void Transition()
    {
        StartCoroutine(TransitionAnimation());
    }

    IEnumerator TransitionAnimation()
    {
        m_Transition.SetTrigger("Start");

        yield return new WaitForSeconds(m_TransitionTime);

        m_Transition.SetTrigger("End");
    }

    IEnumerator LoadLevel(string sceneName)
    {
        m_Transition.SetTrigger("Start");

        yield return new WaitForSeconds(m_TransitionTime);

        SceneManager.LoadScene(sceneName);

        m_Transition.SetTrigger("End");
    }
}
