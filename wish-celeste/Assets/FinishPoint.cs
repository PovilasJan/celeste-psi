using System;
using UnityEngine;

public class FinishPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // cia pakeisti kai playeris tures taga
        {
            SceneController.instance.NextLevel();
        }
    }
}
