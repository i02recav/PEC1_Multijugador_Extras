using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;
namespace Complete
{
    public class TankMovement : MonoBehaviour
    {
        public int m_PlayerNumber = 1;              // Used to identify which tank belongs to which player. This is set by this tank's manager
        public float m_Speed = 12f;                 // How fast the tank moves forward and back
        public float m_TurnSpeed = 180f;            // How fast the tank turns in degrees per second
        public AudioSource m_MovementAudio;         // Reference to the audio source used to play engine sounds. NB: different to the shooting audio source
        public AudioClip m_EngineIdling;            // Audio to play when the tank isn't moving
        public AudioClip m_EngineDriving;           // Audio to play when the tank is moving
		public float m_PitchRange = 0.2f;           // The amount by which the pitch of the engine noises can vary


        private string m_MovementAxisName;          // The name of the input axis for moving forward and back
        private string m_TurnAxisName;              // The name of the input axis for turning
        private Rigidbody m_Rigidbody;              // Reference used to move the tank
        private float m_MovementInputValue;         // The current value of the movement input
        private float m_TurnInputValue;             // The current value of the turn input
        private float m_OriginalPitch;              // The pitch of the audio source at the start of the scene


        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
        }


        private void OnEnable()
        {
            // When the tank is turned on, make sure it's not kinematic
            m_Rigidbody.isKinematic = false;

            // Also reset the input values
            m_MovementInputValue = 0f;
            m_TurnInputValue = 0f;
        }

        private void OnMovement(InputValue value)
        {
            m_MovementInputValue= value.Get<Vector2>().y;
            m_TurnInputValue = value.Get<Vector2>().x;
        }

        private void OnDisable()
        {
            // When the tank is turned off, set it to kinematic so it stops moving
            m_Rigidbody.isKinematic = true;
        }


        public void Start()
        {
            // The axes names are based on player number
      //      m_MovementAxisName = "Vertical" + m_PlayerNumber;
       //     m_TurnAxisName = "Horizontal" + m_PlayerNumber;

            // Store the original pitch of the audio source
            m_OriginalPitch = m_MovementAudio.pitch;
            //    var playerInput = PlayerInput.Instantiate( controlScheme: "Keyboard" + m_PlayerNumber);

            //  this.GetComponent<PlayerInput>().SwitchCurrentControlScheme("Keyboard" + m_PlayerNumber, Keyboard.current);
            //    this.GetComponent<PlayerInput>().user
            // this.GetComponent<PlayerInput>().SwitchCurrentControlScheme("Keyboard" + m_PlayerNumber);


            /*         PlayerInput multieven = GameObject.Find("MultiPlayerEventSystemP" + m_PlayerNumber).GetComponent<PlayerInput>();
                     multieven.defaultControlScheme="Keyboard" + m_PlayerNumber;
                     multieven.SwitchCurrentControlScheme("Keyboard1", Keyboard.current);

                     multieven.defaultActionMap = "Player" + m_PlayerNumber;
                     MultiplayerEventSystem multiplayer = GameObject.Find("MultiPlayerEventSystemP" + m_PlayerNumber).GetComponent<MultiplayerEventSystem>();
                     multiplayer.playerRoot = GameObject.Find("GameManager").GetComponent<GameManager>().m_Tanks[m_PlayerNumber-1].m_Instance;

                     // Get a reference to this Player's Input
                     PlayerInput input = multiplayer.GetComponent<PlayerInput>();

                     // Perform Pairing with the Keyboard
                     InputUser.PerformPairingWithDevice(Keyboard.current, input.user);*/

            PlayerInput multieven = GameObject.Find("GameManager").GetComponent<GameManager>().m_Tanks[m_PlayerNumber - 1].m_Instance.GetComponent<PlayerInput>();
            multieven.defaultControlScheme = "Keyboard1"; // + m_PlayerNumber;
            multieven.SwitchCurrentControlScheme("Keyboard1", Keyboard.current);

            multieven.defaultActionMap = "Player" + m_PlayerNumber;
            MultiplayerEventSystem multiplayer = GameObject.Find("MultiPlayerEventSystemP" + m_PlayerNumber).GetComponent<MultiplayerEventSystem>();
            multiplayer.playerRoot = GameObject.Find("GameManager").GetComponent<GameManager>().m_Tanks[m_PlayerNumber - 1].m_Instance;

            // Get a reference to this Player's Input
            PlayerInput input = GameObject.Find("GameManager").GetComponent<GameManager>().m_Tanks[m_PlayerNumber - 1].m_Instance.GetComponent<PlayerInput>();
            input.uiInputModule = multiplayer.GetComponent<InputSystemUIInputModule>();
            input.actions = GameObject.Find("GameManager").GetComponent<GameManager>().m_Tanks[m_PlayerNumber - 1].m_Instance.GetComponent<PlayerInput>().actions;
            input.SwitchCurrentActionMap("Player" + (m_PlayerNumber));
            Debug.Log("Player " + input.currentActionMap.id);
            // Perform Pairing with the Keyboard
            InputUser.PerformPairingWithDevice(Keyboard.current, input.user);
            // Perform Pairing with the Keyboard
       /*     InputUser.PerformPairingWithDevice(Keyboard.current, input.user);


            MultiplayerEventSystem mpev = GameObject.Find("MultiPlayerEventSystemP" + playerNumber).GetComponent<MultiplayerEventSystem>();

            // Set this MultiplayerEventSystem's Player Root to the Tank instance
            mpev.playerRoot = tankInstance;

            // Get a reference to this Player's Input
            PlayerInput input = tankInstance.GetComponent<PlayerInput>();
            input.uiInputModule = mpev.GetComponent<InputSystemUIInputModule>();
            input.actions = m_Tanks[0].m_Instance.GetComponent<PlayerInput>().actions;
            input.SwitchCurrentActionMap("Player" + playerNumber);
            Debug.Log("Player " + input.currentActionMap.id);
            // Perform Pairing with the Keyboard
            InputUser.PerformPairingWithDevice(Keyboard.current, input.user);*/


        }


        private void Update ()
        {
            // Store the value of both input axes
        //    m_MovementInputValue = Input.GetAxis (m_MovementAxisName);
        //    m_TurnInputValue = Input.GetAxis (m_TurnAxisName);

            EngineAudio();
        }


        private void EngineAudio()
        {
            // If there is no input (the tank is stationary)...
            if (Mathf.Abs (m_MovementInputValue) < 0.1f && Mathf.Abs (m_TurnInputValue) < 0.1f)
            {
                // ... and if the audio source is currently playing the driving clip...
                if (m_MovementAudio.clip == m_EngineDriving)
                {
                    // ... change the clip to idling and play it
                    m_MovementAudio.clip = m_EngineIdling;
                    m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                    m_MovementAudio.Play ();
                }
            }
            else
            {
                // Otherwise if the tank is moving and if the idling clip is currently playing...
                if (m_MovementAudio.clip == m_EngineIdling)
                {
                    // ... change the clip to driving and play
                    m_MovementAudio.clip = m_EngineDriving;
                    m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                    m_MovementAudio.Play();
                }
            }
        }


        private void FixedUpdate()
        {
            // Adjust the rigidbodies position and orientation in FixedUpdate.
            Move();
            Turn();
        }


        private void Move()
        {
            // Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames
            Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;

            // Apply this movement to the rigidbody's position
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
        }


        private void Turn()
        {
            // Determine the number of degrees to be turned based on the input, speed and time between frames
            float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

            // Make this into a rotation in the y axis
            Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);

            // Apply this rotation to the rigidbody's rotation
            m_Rigidbody.MoveRotation (m_Rigidbody.rotation * turnRotation);
        }
    }
}