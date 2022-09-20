using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private GameObject selectedObject;
    private GameObject selectedSocket;
    public bool movementExist;
    [Header("Level Settings")]
    [SerializeField] private GameObject[] CollisionControlObjects;
    [SerializeField] private GameObject[] Plugs;
    [SerializeField] private int dutySocketCount;
    [SerializeField] private List<bool> CollisionSituations;
    [SerializeField] private int movementRightCount;
    [SerializeField] private HingeJoint[] breakPoints;
    
    private int completedCount;
    private int collisionControlCount;
    private LastPlug lastPlug;
    [Header("Other Objects")]
    [SerializeField] private GameObject[] Lights;
    [SerializeField] private AudioSource plugVoice;
    [Header("UI Objects")] 
    [SerializeField] private GameObject controlPanel;
    [SerializeField] private TextMeshProUGUI controlText;
    [SerializeField] private TextMeshProUGUI movementRightText;
    [SerializeField] private GameObject[] GeneralPanels;
    [SerializeField] private TextMeshProUGUI[] UITexts;
    
    void Start()
    {
        movementRightText.text = "MOVES : " + movementRightCount.ToString();
        for (int i = 0; i < dutySocketCount - 1; i++)
        {
            CollisionSituations.Add(false);
        }

        UITexts[3].text = PlayerPrefs.GetInt("Money").ToString();
        
    }

    /*void SetBreakPoints()
    {
        foreach (var item in breakPoints)
        {
            item.breakForce = 900;
            item.breakTorque = 800;
        }
    }*/
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out RaycastHit hit,100))
            {
                if (hit.collider != null)
                {
                    // ## LAST PLUG
                    if (selectedObject == null && !movementExist)
                    {
                        if (hit.collider.CompareTag("Blue_Plug") || hit.collider.CompareTag("Yellow_Plug") ||
                            hit.collider.CompareTag("Red_Plug"))
                        {
                            plugVoice.Play();
                            lastPlug = hit.collider.GetComponent<LastPlug>();
                            lastPlug.Move("Selection", lastPlug.currentSocket, lastPlug.currentSocket.GetComponent<Socket>().movementPosition);

                            selectedObject = hit.collider.gameObject;
                            selectedSocket = lastPlug.currentSocket;
                            movementExist = true;
                        } 
                    }
                    //## LAST PLUG
                    
                    //## SOCKET
                    if (hit.collider.CompareTag("Socket"))
                    {
                        if (selectedObject != null && !hit.collider.GetComponent<Socket>().fullness && selectedSocket != hit.collider.gameObject)
                        {
                            selectedSocket.GetComponent<Socket>().fullness = false;
                            
                            Socket socket = hit.collider.GetComponent<Socket>();
                            
                            lastPlug.Move("ChangePosition",hit.collider.gameObject,socket.movementPosition);
                            socket.fullness = true;
                            
                            selectedObject = null;
                            selectedSocket = null;
                            movementExist = true;
                            movementRightCount--;
                            movementRightText.text = "MOVES : " + movementRightCount.ToString();
                        }
                        else if (selectedSocket == hit.collider.gameObject)
                        {
                            lastPlug.Move("BackInSocket",hit.collider.gameObject);
                            
                            selectedObject = null;
                            selectedSocket = null;
                            movementExist = true;
                        }
                    }
                    
                    //## SOCKET
                    
                    
                }
            }
        }
    }
    
    public void CheckPlugs()
    {
        foreach (var item in Plugs)
        {
            if (item.GetComponent<LastPlug>().currentSocket.name == item.GetComponent<LastPlug>().socketColor)
            {
                completedCount++;
            }
        }

        if (completedCount == dutySocketCount)
        {
            foreach (var item in CollisionControlObjects)
            {
                item.SetActive(true);
            }

            StartCoroutine(CollisionExists());
        }
        else
        {
            if (movementRightCount <= 0)
            {
                Debug.Log("movement right = 0");
                Lose();
            }
        }

        completedCount = 0;
    }

    public void CheckCollision(int collisionIndex,bool situation)
    {
        CollisionSituations[collisionIndex] = situation;
    }

    IEnumerator CollisionExists()
    {
        Lights[0].SetActive(false);
        Lights[1].SetActive(true);
        
        controlPanel.SetActive(true);
        controlText.text = "BEING CHECKED...";
        
        yield return new WaitForSeconds(4f);

        foreach (var item in CollisionSituations)
        {
            if(item)
                collisionControlCount++;
        }

        if (collisionControlCount == CollisionSituations.Count)
        {
            Lights[1].SetActive(false);
            Lights[2].SetActive(true);
            controlText.text = "CONGRATS...";
            Win();
        }
        else
        {
            Lights[1].SetActive(false);
            Lights[0].SetActive(true);
            controlText.text = "THERE IS A COLLISION";
            Invoke("ClosePanel",2f);
            foreach (var item in CollisionControlObjects)
            {
                item.SetActive(false);
            }

            if (movementRightCount <= 0)
            {
                Debug.Log("movement right = 0");
                Lose(); 
            }
        }
        
        

        collisionControlCount = 0;
    }

    void ClosePanel()
    {
        controlPanel.SetActive(false);
    }

    public void ButtonOperations(string value)
    {
        switch (value)
        {
            case "Stop":
                GeneralPanels[0].SetActive(true);
                Time.timeScale = 0;
                break;
            case "Continue":
                GeneralPanels[0].SetActive(false);
                Time.timeScale = 1;
                break;
            case "Retry":
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                Time.timeScale = 1;
                break;
            case "NextLevel":
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
                Time.timeScale = 0;
                break;
            case "Exit":
                Application.Quit();
                break;
        }
    }

    public void Win()
    {
        Lights[1].SetActive(false);
        Lights[2].SetActive(true);
        PlayerPrefs.SetInt("Level",PlayerPrefs.GetInt("Level") + 1);
        UITexts[0].text = "LEVEL : " + SceneManager.GetActiveScene().name;
        controlText.text = "YOU WIN";

        int randomMoney = Random.Range(5, 20);
        PlayerPrefs.SetInt("Money",PlayerPrefs.GetInt("Money")+randomMoney);
        UITexts[2].text = "MONEY : " + randomMoney;
        GeneralPanels[1].SetActive(true);
        Time.timeScale = 0;
    }

    public void Lose()
    {
        UITexts[1].text = "LEVEL : " + SceneManager.GetActiveScene().name;
        GeneralPanels[2].SetActive(true);
        Time.timeScale = 0;
    }

    public void PlayPlugVoice()
    {
        plugVoice.Play();
    }
}
