using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    public class HUDController : MonoBehaviour
    {
        public GameObject[] hud;
        
        public GameObject gameManager;
        public GameObject[] points;
        public GameObject[] victories;
        // Start is called before the first frame update
        void Start()
        {
            hud[0].SetActive(true); //inicializamos los 2 primeros jugadores que participan siempre.
            hud[1].SetActive(true);
          
        }

        // Update is called once per frame
        void Update()
        {
            GetInformation();
        }

        public void ActiveHUD(int nPlayer)
        {
            hud[nPlayer].SetActive(true);
            points[nPlayer].GetComponent<Text>().text = gameManager.GetComponent<GameManager>().GetPoints(nPlayer).ToString();
            victories[nPlayer].GetComponent<Text>().text = gameManager.GetComponent<GameManager>().m_Tanks[nPlayer].m_Wins.ToString();

        }
        public void GetInformation()
        {
            for (int n=0; n < 4; n++)
            {
                points[n].GetComponent<Text>().text = gameManager.GetComponent<GameManager>().GetPoints(n).ToString();
                victories[n].GetComponent<Text>().text = gameManager.GetComponent<GameManager>().m_Tanks[n].m_Wins.ToString();
            }
        }
        public void ShowInformation()
        {
            for ( int n=0; n < 4; n++)
            {
          //      hud[i].GetComponentinChildren<
            }
        }
    }
}
