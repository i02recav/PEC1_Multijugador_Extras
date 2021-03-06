using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.UI;


namespace Complete
{
    public class TankShooting : MonoBehaviour
    {
        public int m_PlayerNumber = 1;              // Used to identify the different players
        public Rigidbody m_Shell;                   // Prefab of the shell
		public Rigidbody m_ShellAlt;                // Prefab of the Alternate shell
        public Transform m_FireTransform;           // A child of the tank where the shells are spawned
        public Slider m_AimSlider;                  // A child of the tank that displays the current launch force
        public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source
        public AudioClip m_ChargingClip;            // Audio that plays when each shot is charging up
        public AudioClip m_FireClip;                // Audio that plays when each shot is fired
        public float m_MinLaunchForce = 15f;        // The force given to the shell if the fire button is not held
        public float m_MaxLaunchForce = 30f;        // The force given to the shell if the fire button is held for the max charge time
        public float m_MaxChargeTime = 0.75f;       // How long the shell can charge for before it is fired at max force
        public bool isHold,isDown,isUp;             // Estado de los botones

        private string m_FireButton;                // The input axis that is used for launching shells
		private string m_AltFireButton;  
        public float m_CurrentLaunchForce;         // The force that will be given to the shell when the fire button is released
        private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time
        public bool m_Fired;                       // Whether or not the shell has been launched with this button press
		private bool m_AltFire;                     // Whether or not the alternate shell has been launched with this button press


        private void OnEnable()
        {
            // When the tank is turned on, reset the launch force and the UI
            m_CurrentLaunchForce = m_MinLaunchForce;
            m_AimSlider.value = m_MinLaunchForce;
        }


        private void Start() { 
        
            isHold=false;
            isUp = false;
            isDown = false;
            m_Fired = true;
			m_AltFireButton = "AltFire" + m_PlayerNumber;
            // The rate that the launch force charges up is the range of possible forces by the max charge time
            m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;

        }

/* FUNCIONES DE LAS ACCIONES DEFINIDAS EN EL INPUT SYSTEM, Contienen la lógica tras las pulsación de las teclas */
        public void OnPlayer3() // Botón para que inicie la partida el jugador 3 
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().nextPlayer = true;
        }
        public void OnPlayer4() // Botón para que inicie la partida el jugador 4
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().nextPlayer = true;
        }

        public void OnAltFirePress(InputValue inputValue) // presionar boton de fuego alternativo
        {
            m_AltFire = true;
            OnFirePress(inputValue);
        }

        public void OnAltFireRelease(InputValue inputValue) // soltar boton de fuego alternativo
        {
            m_AltFire = true;
            OnFireRelease(inputValue);
        }

        public void OnFirePress(InputValue inputValue) // presionar boton de fuegoi
        {
            if (inputValue.isPressed && m_Fired)
            {
                m_Fired = false;
                m_CurrentLaunchForce = m_MinLaunchForce;
                // Change the clip to the charging clip and start it playing
                m_ShootingAudio.clip = m_ChargingClip;
                m_ShootingAudio.Play();
            }       
        }

        private void OnFireRelease(InputValue inputValue) // soltar el boton de fuego
        {      
            if (!inputValue.isPressed)
            {
                if (!m_Fired)
                {
                    Fire();
                }
            }
 
        }

        private void Update()
        {
            m_AimSlider.value = m_MinLaunchForce;
            if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
            {
                // ... use the max force and launch the shell
                m_CurrentLaunchForce = m_MaxLaunchForce;
                Fire();
            }else
            if (!m_Fired)
            {
                // Increment the launch force and update the slider
                m_CurrentLaunchForce += m_ChargeSpeed* Time.deltaTime;
                m_AimSlider.value = m_CurrentLaunchForce;
            }
        }

        private void Fire()  {
            // Set the fired flag so only Fire is only called once
            m_Fired = true;

            // Create an instance of the shell and store a reference to it's rigidbody
			Rigidbody shellInstance;
			if (m_AltFire) shellInstance =
				Instantiate (m_ShellAlt, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
			else
			    shellInstance =
					Instantiate (m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

            // Set the shell's velocity to the launch force in the fire position's forward direction
            shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;
            shellInstance.GetComponent<ShellExplosion>().playerOwner = m_PlayerNumber; //Le proporcionamos el nuevo objeto el valor del tanke que lo ha disparado
			if (m_AltFire)
				shellInstance.velocity *= 1.50f;

            // Change the clip to the firing clip and play it
            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play();
            m_AltFire = false;
            // Reset the launch force.  This is a precaution in case of missing button events
            m_CurrentLaunchForce = m_MinLaunchForce;
        }
    }
}