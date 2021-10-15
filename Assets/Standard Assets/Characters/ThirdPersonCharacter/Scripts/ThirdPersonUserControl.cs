using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
        [HideInInspector]
        public bool isLock = false;
        [Range(1.0f,3.0f)]
        public float runSpeedMod = 2.0f;
        public bool enableUnstuck;
        public int stuckDelay;
        public Vector3 stuckBounceVelocity;
        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
        private float stuckCounter;

        private void Start()
        {
            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();

            stuckCounter = 0;
        }


        private void Update()
        {
            if(!isLock)
            {
                if (!m_Jump)
                {
                    m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
                }
            }

            //Player is stuck fix
            //if the player is stuck in mid-air and not moving
            if (enableUnstuck && !GetComponent<ThirdPersonCharacter>().getGroundStatus() && GetComponent<Rigidbody>().velocity.magnitude < 0.5)
            {
                stuckCounter++;
                //Player has been stuck, Pop them off the object
                if (stuckCounter > stuckDelay)
                {
                    GetComponent<Rigidbody>().AddRelativeForce(stuckBounceVelocity, ForceMode.VelocityChange);
                    stuckCounter = 0;
                }
            }
            else
                stuckCounter = 0;

        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            float h;
            float v;

            bool crouch = false;

            if (!isLock)
            {
                // read inputs
                h = CrossPlatformInputManager.GetAxis("Horizontal") * 0.5f;
                v = CrossPlatformInputManager.GetAxis("Vertical") * 0.5f;

                crouch = Input.GetKey(KeyCode.C);
            }
            else
            {
                h = 0f;
                v = 0f;
            }

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = v * m_CamForward + h * m_Cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = v * Vector3.forward + h * Vector3.right;
            }
#if !MOBILE_INPUT
            // walk speed multiplier
            if (Input.GetKey(KeyCode.LeftShift)) m_Move *= runSpeedMod;
#endif

            // pass all parameters to the character control script
            m_Character.Move(m_Move, crouch, m_Jump);
            m_Jump = false;
            
        }
    }
}
