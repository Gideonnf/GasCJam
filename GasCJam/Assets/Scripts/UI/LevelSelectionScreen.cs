using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectionScreen : MonoBehaviour
{
    //store all the available levels UI button

    // Start is called before the first frame update
    void Start()
    {
        //get from the level manager which level is currently unlocked
        int levelUnlocked = LevelManager.Instance.m_LevelUnlocked;

        //greyed out the levels that has yet to be unlocked,
    }
}
