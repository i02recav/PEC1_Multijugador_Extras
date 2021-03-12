using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;

namespace Complete
{
    public class GameManager : MonoBehaviour
    {
        public int m_NumRoundsToWin = 1;            // The number of rounds a single player has to win to win the game
        public float m_StartDelay = 3f;             // The delay between the start of RoundStarting and RoundPlaying phases
        public float m_EndDelay = 3f;               // The delay between the end of RoundPlaying and RoundEnding phases
        
        public CameraControl m_CameraControl;       // Reference to the CameraControl script for control during different phases
        public Text m_MessageText;                  // Reference to the overlay Text to display winning text, etc.
        public GameObject m_TankPrefab;             // Reference to the prefab the players will control
        public TankManager[] m_Tanks;               // A collection of managers for enabling and disabling different aspects of the tanks
        public PlayerInput[] m_playerInput;
        public int nTankes, tankPlaying;
        public Camera miniCam, deathcam;
        public GameObject mainCam;
        private int m_RoundNumber;                  // Which round the game is currently on
        private WaitForSeconds m_StartWait;         // Used to have a delay whilst the round starts
        private WaitForSeconds m_EndWait;           // Used to have a delay whilst the round or game ends
        private TankManager m_RoundWinner;          // Reference to the winner of the current round.  Used to make an announcement of who won
        private TankManager m_GameWinner;           // Reference to the winner of the game.  Used to make an announcement of who won
        public bool nextPlayer;
        public GameObject HUD; // Para conectar con el HUD y mostrar las victorias y puntos

        private void Start()
        {
            //            GlobalVariables.Instance.nPlayers = 2;
            nextPlayer = false;
            nTankes = 2;
            tankPlaying = 0;
            mainCam = GameObject.Find("FollowCam");
            miniCam = GameObject.Find("MiniMap").GetComponent<Camera>();
            deathcam = GameObject.Find("DeathCam").GetComponent<Camera>();
            // Create the delays so they only have to be made once
            m_StartWait = new WaitForSeconds (m_StartDelay);
            m_EndWait = new WaitForSeconds (m_EndDelay);

            SpawnAllTanks();
            SetCameraTargets();

            // Once the tanks have been created and the camera is using them as targets, start the game
            StartCoroutine (GameLoop());
        }

        private void Update()
        {

            if (nextPlayer)
            {
                if (nTankes == 2)
                {
                    
                    nTankes++;
                  //  DeleteCamera(0);
                  //  DeleteCamera(1);
                      AddCamera(0, "");
                      AddCamera(1, "");
                     AddCamera(3, "minicam");
                    GameObject cameraMinimap =  GameObject.Find("P4_Cam");
                    cameraMinimap.GetComponent<Camera>().cullingMask |= 1 << (LayerMask.NameToLayer("MiniCam"));
                    miniCam.rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
                    SpawnTanks(nTankes);
                    nextPlayer = false;

                }else
                if (nTankes==3 && nextPlayer)
                {
                    nTankes = 4;
                    SpawnTanks(nTankes);
                }
            }
        }

        private void ReconectKeyboard(GameObject tankInstance, int playerNumber)
        {
            // Get a reference to this player's MultiplayerEventSystem
            MultiplayerEventSystem mpev = GameObject.Find("MultiPlayerEventSystemP" + playerNumber).GetComponent<MultiplayerEventSystem>();

            // Set this MultiplayerEventSystem's Player Root to the Tank instance
            mpev.playerRoot = tankInstance;

            // Get a reference to this Player's Input
            PlayerInput input = tankInstance.GetComponent<PlayerInput>();
            input.uiInputModule = mpev.GetComponent<InputSystemUIInputModule>();
            input.actions= m_Tanks[0].m_Instance.GetComponent<PlayerInput>().actions;
            input.SwitchCurrentActionMap("Player" + playerNumber);
            Debug.Log("Player "+input.currentActionMap.id);
            // Perform Pairing with the Keyboard
            InputUser.PerformPairingWithDevice(Keyboard.current, input.user);
        }

        private void SpawnAllTanks()
		{
//			Camera mainCam = GameObject.Find ("Main Camera").GetComponent<Camera>();

			// For all the tanks...
            for (int i = 0; i < nTankes; i++)
                {
                // ... create them, set their player number and references needed for control
                m_Tanks[i].m_Instance = Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;

            //    m_playerInput[i]= PlayerInput.Instantiate(m_TankPrefab, controlScheme:"Keyboard1", pairWithDevice: Keyboard.current);
				m_Tanks[i].m_PlayerNumber = i + 1;
				m_Tanks[i].Setup();
				AddCamera (i, "");
                changeCamera(i, true, "");
                ReconectKeyboard(m_Tanks[i].m_Instance, m_Tanks[i].m_PlayerNumber);
              //  HUD.GetComponent<HUDController>().ActiveHUD(i);
            }

			mainCam.gameObject.SetActive (false);
		}

        private void SpawnTanks(int tank)
        {
            
            tank--;
           // Camera mainCam = GameObject.Find("Main Camera").GetComponent<Camera>();
            m_Tanks[tank].m_Instance = Instantiate(m_TankPrefab, m_Tanks[tank].m_SpawnPoint.position, m_Tanks[tank].m_SpawnPoint.rotation) as GameObject;
            m_Tanks[tank].m_PlayerNumber = tank + 1;
            m_Tanks[tank].Setup();
            AddCamera(tank, "");
            changeCamera(tank, true, "");
           ReconectKeyboard(m_Tanks[tank].m_Instance, m_Tanks[tank].m_PlayerNumber);
            HUD.GetComponent<HUDController>().ActiveHUD(tank);
            //  SetCameraTargets();
            //  mainCam.gameObject.SetActive(false);
        }

        private void DeleteCamera (int i)  //Elimina camaras
        {
            Destroy(GameObject.Find("Camera" + (i + 1)));
           // GameObject.Find("Camera" + (i + 1)).SetActive(false);
        }

        private void AddCamera (int i, string minicam)
        {
           if (minicam=="minicam")
            {
                GameObject newCam = GameObject.Instantiate(GameObject.Find("MiniMap"));
                newCam.name = ("Minicam3P");
               /* newCam.AddComponent<Cinemachine.CinemachineVirtualCamera>();
                newCam.AddComponent<Camera>();
                // newCam.AddComponent<Cinemachine.CinemachineVirtualCamera>();
                //    newCam = mainCam;
                //  newCam.name = "FollowCam" + (i + 1);
                newCam.layer = 16; //Asigna al layer correcto segun la camara
                newCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = miniCam.transform;
                newCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = miniCam.transform;
                newCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().AddCinemachineComponent<Cinemachine.CinemachineTransposer>();
                newCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().AddCinemachineComponent<Cinemachine.CinemachineHardLookAt>();
                newCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine.CinemachineHardLookAt>();

                newCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine.CinemachineTransposer>().m_BindingMode = 0;
                newCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine.CinemachineTransposer>().m_FollowOffset = new Vector3(15f, 15f, 15f);
                //  newCam.transform.parent = m_Tanks[i].m_Instance.transform;*/
                newCam.GetComponent<Camera>().cullingMask |= 1 << (LayerMask.NameToLayer("MiniCam"));
                newCam.GetComponent<Camera>().rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
            }
            else
            {


                //	GameObject childCam = new GameObject ("FollowCam" + (i + 1));
                //  Debug.Log("entro por " + (i + 1));
                if (GameObject.Find("FollowCam" + (i + 1)) == null)
                {
                    //    Debug.Log("Creo cam " + i + 1);
                    GameObject newCam = new GameObject("FollowCam" + (i + 1));
                    newCam.AddComponent<Cinemachine.CinemachineVirtualCamera>();
                    // newCam.AddComponent<Cinemachine.CinemachineVirtualCamera>();
                    //    newCam = mainCam;
                    //  newCam.name = "FollowCam" + (i + 1);
                    newCam.layer = 10 + i + 1; //Asigna al layer correcto segun la camara
                    newCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = m_Tanks[i].m_Instance.transform;
                    newCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = m_Tanks[i].m_Instance.transform;
                    newCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().AddCinemachineComponent<Cinemachine.CinemachineTransposer>();
                    newCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().AddCinemachineComponent<Cinemachine.CinemachineHardLookAt>();
                    newCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine.CinemachineHardLookAt>();

                    newCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine.CinemachineTransposer>().m_BindingMode = 0;
                    newCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine.CinemachineTransposer>().m_FollowOffset = new Vector3(15f, 15f, 15f);
                    newCam.transform.parent = m_Tanks[i].m_Instance.transform;


                }

                Camera camObject = GameObject.Find("P" + (i + 1) + "_Cam").GetComponent<Camera>();
                if (nTankes == 2)
                {
                    if (i == 0)
                    {
                        camObject.rect = new Rect(0.0f, 0.5f, 1f, 0.5f);

                    }
                    else
                    {
                        camObject.rect = new Rect(0f, 0.0f, 1f, 0.5f);

                    }
                }
                else
                {
                    switch (i)
                    {
                        case 0:
                            //    Camera camObject1 = GameObject.Find("P1_Cam").GetComponent<Camera>();
                            //    camObject1.rect = new Rect(0.0f, 0.5f, 0.5f, 0.5f);
                            camObject.rect = new Rect(0.0f, 0.5f, 0.5f, 0.5f);
                            Debug.Log("Reconfiguro cam 1");
                            break;

                        case 1:
                            //   Camera camObject2 = GameObject.Find("P2_Cam").GetComponent<Camera>();
                            //  camObject2.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                            camObject.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                            break;
                        case 2:
                            Camera camObjectMiniCam = GameObject.Find("MiniMap").GetComponent<Camera>();
                            camObject.rect = new Rect(0f, 0f, 0.5f, 0.5f);
                            camObjectMiniCam.rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
                            break;
                        case 3:
                            camObject.rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
                            break;

                    }
                }
            }
		}
// set depth eliminable
        public void SetDepthCam(Camera camara, float depth)
        {
            camara.depth = depth;
        }


        public void SetPriorityCam(Camera camara, int priority)
        {
            camara.GetComponentInParent<Cinemachine.CinemachineVirtualCamera>().Priority = priority;
        }


        public void changeCamera(int i, bool aparece, string tipoCam)
        {

            GameObject newCam = GameObject.Find("P" + (i + 1) + "_Cam");
            if (aparece)
            {
                newCam.GetComponent<Camera>().cullingMask |= 1 <<( LayerMask.NameToLayer("Cam" + (i + 1)));
            }
            else
            {
                newCam.GetComponent<Camera>().cullingMask &= ~( 1<< LayerMask.NameToLayer("Cam" + (i + 1)));
                if (tipoCam == "mini")
                {
                    newCam.GetComponent<Camera>().cullingMask &= ~(1 << (LayerMask.NameToLayer("DeathCam")));
                    newCam.GetComponent<Camera>().cullingMask |= 1 << (LayerMask.NameToLayer("MiniCam"));
                }
                if (tipoCam == "death") 
                {
                    newCam.GetComponent<Camera>().cullingMask &= ~(1 << (LayerMask.NameToLayer("MiniCam"))); //forzamos a que aparezca la layer de la deathcam si antes tenía un minimapa eliminando el layer ene lcullingmask
                    newCam.GetComponent<Camera>().cullingMask |= 1 << (LayerMask.NameToLayer("DeathCam"));
                }
            }
           
            // newCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = m_Tanks[i].m_Instance.transform;
            // newCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = GameObject.Find(Camera).transform;

        }

        public void ReconfigureCamera(int i)
        {
            tankPlaying=0;
         //   SetDepthCam(miniCam, 2);
         //   SetDepthCam(deathcam, 1);

            for (int x = 0; x < nTankes; x++)
            {
                // comprobamos cuantos hay eliminados
                if (m_Tanks[x].m_Instance.GetComponent<TankHealth>().m_Dead)
                {
                    tankPlaying++;
                }
            }

            if (tankPlaying == 1 && nTankes!=3) //un eliminado y estan jugando 3 o 4 pondremos camara de muerte, si existen 3 player significa que ya hay una minicam, no hay que volver a mostrarla
            {
                switch (i - 1)
                {
                    case 0:
                        if (nTankes != 2)
                        {
                            changeCamera(0, false, "mini");
                        }
                        if (nTankes ==2)
                        {
                            changeCamera(0, false, "death");
                        }
                        break;

                    case 1:
                        
                        if (nTankes != 2) // Evita que al haber solo 2 player de inicio se cambie la camara al ser destruido uno de ellos
                        {
                            changeCamera(1, false, "mini");
                        }
                        if (nTankes==2)
                        {
                            changeCamera(1, false, "death");
                        }
                        break;
                    case 2:
                        changeCamera(2, false, "mini"); 
                        break;
                    case 3:
                        changeCamera(3, false, "mini");

                        break;
                }
            }
            else
            {
              if (tankPlaying>=2  || (tankPlaying==1 && nTankes == 3))
                {
                    changeCamera(i - 1, false, "death"); 
                }              
            }          
        }

        private void SetCameraTargets()
        {
            // Create a collection of transforms the same size as the number of tanks
            Transform[] targets = new Transform[nTankes];

            // For each of these transforms...
            for (int i = 0; i < targets.Length; i++)
            {
                // ... set it to the appropriate tank transform
                targets[i] = m_Tanks[i].m_Instance.transform;
            }
            if (nTankes == 3) //mostramos el minimapa en caso de iniciar la partida 3 jugadores
            {
                miniCam.rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
               
            }

            // These are the targets the camera should follow
            m_CameraControl.m_Targets = targets;
        }


        // This is called from start and will run each phase of the game one after another
        private IEnumerator GameLoop()
        {
            // Start off by running the 'RoundStarting' coroutine but don't return until it's finished
            yield return StartCoroutine (RoundStarting());

            // Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished
            yield return StartCoroutine (RoundPlaying());

            // Once execution has returned here, run the 'RoundEnding' coroutine, again don't return until it's finished
            yield return StartCoroutine (RoundEnding());

            // This code is not run until 'RoundEnding' has finished.  At which point, check if a game winner has been found
            if (m_GameWinner != null)
            {
                // If there is a game winner, restart the level
                SceneManager.LoadScene ("Menu");
            }
            else
            {
                // If there isn't a winner yet, restart this coroutine so the loop continues
                // Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end
                StartCoroutine (GameLoop());
            }
        }


        private IEnumerator RoundStarting()
        {
            // As soon as the round starts reset the tanks and make sure they can't move
            ResetAllTanks();
            DisableTankControl();

            // Snap the camera's zoom and position to something appropriate for the reset tanks
            m_CameraControl.SetStartPositionAndSize();

            // Increment the round number and display text showing the players what round it is
            m_RoundNumber++;
            m_MessageText.text = "ROUND " + m_RoundNumber;

            // Wait for the specified length of time until yielding control back to the game loop
            yield return m_StartWait;
        }


        private IEnumerator RoundPlaying()
        {
            // As soon as the round begins playing let the players control the tanks
            EnableTankControl();

            // Clear the text from the screen
            m_MessageText.text = string.Empty;

            // While there is not one tank left...
            while (!OneTankLeft())
            {
                // ... return on the next frame
                yield return null;
            }
        }


        private IEnumerator RoundEnding()
        {
            // Stop tanks from moving
            DisableTankControl();

            // Clear the winner from the previous round
            m_RoundWinner = null;

            // See if there is a winner now the round is over
            m_RoundWinner = GetRoundWinner();

            // If there is a winner, increment their score
            if (m_RoundWinner != null)
                m_RoundWinner.m_Wins++;

            // Now the winner's score has been incremented, see if someone has one the game
            m_GameWinner = GetGameWinner();

            // Get a message based on the scores and whether or not there is a game winner and display it
            string message = EndMessage();
            m_MessageText.text = message;

            // Wait for the specified length of time until yielding control back to the game loop
            yield return m_EndWait;
        }


        // This is used to check if there is one or fewer tanks remaining and thus the round should end
        private bool OneTankLeft()
        {
            // Start the count of tanks left at zero
            int numTanksLeft = 0;

            // Go through all the tanks...
            for (int i = 0; i < nTankes; i++)
            {
                // ... and if they are active, increment the counter
                if (m_Tanks[i].m_Instance.activeSelf)
                    numTanksLeft++;
            }

            // If there are one or fewer tanks remaining return true, otherwise return false
            return numTanksLeft <= 1;
        }
        
        
        // This function is to find out if there is a winner of the round
        // This function is called with the assumption that 1 or fewer tanks are currently active
        private TankManager GetRoundWinner()
        {
            // Go through all the tanks...
            for (int i = 0; i < nTankes; i++)
            {
                // ... and if one of them is active, it is the winner so return it
                if (m_Tanks[i].m_Instance.activeSelf)
                {
                    return m_Tanks[i];
                }
            }

            // If none of the tanks are active it is a draw so return null
            return null;
        }


        // This function is to find out if there is a winner of the game
        private TankManager GetGameWinner()
        {
            // Go through all the tanks...
            for (int i = 0; i < nTankes; i++)
            {
                // ... and if one of them has enough rounds to win the game, return it
                if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                {
                    return m_Tanks[i];
                }
            }

            // If no tanks have enough rounds to win, return null
            return null;
        }

        public int GetPoints(int nPlayer)  //Devuelve el numero de puntos
        {
           return m_Tanks[nPlayer].points;
        }


        // Returns a string message to display at the end of each round.
        private string EndMessage()
        {
            // By default when a round ends there are no winners so the default end message is a draw
            string message = "DRAW!";

            // If there is a winner then change the message to reflect that
            if (m_RoundWinner != null)
            {
                message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";
            }

            // Add some line breaks after the initial message
            message += "\n\n\n\n";

            // Go through all the tanks and add each of their scores to the message
            for (int i = 0; i < nTankes; i++)
            {
                message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
            }

            // If there is a game winner, change the entire message to reflect that
            if (m_GameWinner != null)
            {
                message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";
            }

            return message;
        }


        // This function is used to turn all the tanks back on and reset their positions and properties
        private void ResetAllTanks()
        {
            for (int i = 0; i < nTankes; i++)
            {
                changeCamera(i, true, ""); //devolvemos la vista de la camara a los players
                m_Tanks[i].Reset();
            }
        }


        private void EnableTankControl()
        {
            for (int i = 0; i < nTankes; i++)
            {
                m_Tanks[i].EnableControl();
            }
        }


        private void DisableTankControl()
        {
            for (int i = 0; i < nTankes; i++)
            {
                m_Tanks[i].DisableControl();
            }
        }
    }
}