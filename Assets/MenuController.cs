using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // Start is called before the first frame update
    public int nPlayers;
    public float selectorPosition;
    public GameObject selectorItem2,selectorItem3, selectorItem4;
    void Start()
    {
        nPlayers = 2;
        Unselectall();
        selectorItem2.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        selectorPosition = 0;
        if (Input.GetKeyDown(KeyCode.RightArrow)) selectorPosition = 1;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) selectorPosition = -1;
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GlobalVariables.Instance.nPlayers = nPlayers; // Pasa la variable de forma global para la siguiente escena.
            SceneManager.LoadScene("_Complete-Game");
        }

        if (selectorPosition!= 0)
        {
            SwitchPlayers(selectorPosition);
        }
    }

    public void SwitchPlayers(float position)
    {
        if (position < 0)
            nPlayers--;
        else
            nPlayers++;

        switch (nPlayers)
        {

            case 1:
                nPlayers = 4;
                Unselectall();
                selectorItem4.SetActive(true);
                break;
            case 2:
                Unselectall();
                selectorItem2.SetActive(true);
                break;
            case 3:
                Unselectall();
                selectorItem3.SetActive(true);
                break;
            case 4:
                Unselectall();
                selectorItem4.SetActive(true);
                break;
            case 5:
                nPlayers = 2;
                Unselectall();
                selectorItem2.SetActive(true);
                break;             
        }
    }

    public void Unselectall()
    {
        selectorItem2.SetActive(false);
        selectorItem3.SetActive(false);
        selectorItem4.SetActive(false);
    }

}
