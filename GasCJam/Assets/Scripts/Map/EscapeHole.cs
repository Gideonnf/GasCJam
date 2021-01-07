using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeHole : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Prey")
        {
            GameLevelManager.Instance.RatEnterHole();
        }
    }
}
