using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] public GameObject victoryMenu;
    [SerializeField] public GameObject screenOverlay;

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.tag == "Player")
        {
            GameManager.WinGame(victoryMenu, screenOverlay);
        }
    }
}
