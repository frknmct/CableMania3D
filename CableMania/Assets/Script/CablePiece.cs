using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CablePiece : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private ParticleSystem[] breakEffects;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Socket"))
        {
            gameManager.Lose();
            breakEffects[0].gameObject.SetActive(true);
            breakEffects[1].gameObject.SetActive(true);
            breakEffects[0].Play();
            breakEffects[1].Play();
        }
    }

    
}
