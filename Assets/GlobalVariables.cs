using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class GlobalVariables : MonoBehaviour // Utilizaremos esta instancia para pasar información de los coches y los circuitos entre la escena de Inicio y los circuitos
{
    public static GlobalVariables Instance;

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    public int nPlayers;


}