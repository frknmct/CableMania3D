using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionControl : MonoBehaviour
{
    public GameManager gameManager;
    public int collisionIndex;
    
    void Update()
    {
        Collider[] HitColl = Physics.OverlapBox(transform.position, transform.localScale / 2, Quaternion.identity);

        for (int i = 0; i < HitColl.Length; i++)
        {
            if (HitColl[i].CompareTag("CablePiece"))
            {
                gameManager.CheckCollision(collisionIndex,false);
            }
            else
            {
                gameManager.CheckCollision(collisionIndex,true);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position,transform.localScale/2);
    }
}
