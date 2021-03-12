using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

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
        selectorPosition = 0;
    }

    // Update is called once per frame
    void Update() //Cada vez que hay una variación de la variable selectorPosition, la cual se produce en las funciones que se activan mediante el inputmanager, llama a la funcion SwitchPlayer que guarda la posicion de la seleccion y activa el gameobject que lo resalta
    {
        if (selectorPosition!= 0)
        {
            SwitchPlayers(selectorPosition);
        }
    }
     public void OnLeft(InputValue inputValue)
    {
        if (inputValue.isPressed)
        selectorPosition = -1;
    }
    public void OnRight(InputValue inputValue)
    {
        if (inputValue.isPressed)
            selectorPosition = +1;
    }
    public void OnEnter(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            GlobalVariables.Instance.nPlayers = nPlayers; // Pasa la variable de forma global para la siguiente escena.
            SceneManager.LoadScene("_Complete-Game");
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
        selectorPosition = 0;
    }

    public void Unselectall() //Limpia los gameobject de seleccion para marcar desde el switchplayer solo el que proceda.
    {
        selectorItem2.SetActive(false);
        selectorItem3.SetActive(false);
        selectorItem4.SetActive(false);
    }

}
