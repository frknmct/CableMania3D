using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastPlug : MonoBehaviour
{
    public GameObject currentSocket;
    public string socketColor;
    [SerializeField] private GameManager gameManager;

    private bool selected;
    private bool changePos;
    private bool plugIn;

    private GameObject movementPosition;
    private GameObject socketPosition;

    public void Move(string operation,GameObject socket,GameObject objectToGo=null)
    {
        switch (operation)
        {
            case "Selection":
                movementPosition = objectToGo;
                selected = true;
                break;
            case "ChangePosition":
                socketPosition = socket;
                movementPosition = objectToGo;
                changePos = true;
                break;
            case "BackInSocket":
                socketPosition = socket;
                plugIn = true;
                break;
        }
    }
    
   

   
    void Update()
    {
        if (selected)
        {
            transform.position = Vector3.Lerp(transform.position,movementPosition.transform.position,.040f);
            if (Vector3.Distance(transform.position,movementPosition.transform.position) <.010)
            {
                selected = false;
            }
        }
        
        if (changePos)
        {
            transform.position = Vector3.Lerp(transform.position,movementPosition.transform.position,.040f);
            if (Vector3.Distance(transform.position,movementPosition.transform.position) <.010)
            {
                changePos = false;
                plugIn = true;
            }
        }
        
        if (plugIn)
        {
            transform.position = Vector3.Lerp(transform.position,socketPosition.transform.position,.040f);
            if (Vector3.Distance(transform.position,socketPosition.transform.position) <.010)
            {
                gameManager.PlayPlugVoice();
                plugIn = false;
                gameManager.movementExist = false;
                currentSocket = socketPosition;
                gameManager.CheckPlugs();
            }
        }
    }
}
