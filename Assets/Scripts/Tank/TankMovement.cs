using UnityEngine;
using System;

public class TankMovement : MonoBehaviour
{

    public int m_PlayerNumber = 1;         
    public float m_Speed = 12f;            

    // Acceleration constant of tank
    public float m_ForwardAcceleration = 24f;

    // Tolerance of difference between desired and actual forward speed
    public float m_SpeedTolerance = 0.1f;

    // Sideward drag constant to reduce drifting
    public float m_SidewardDragConstant = 0.1f;
    public float m_TurnSpeed = 180f;       
    public AudioSource m_MovementAudio;    
    public AudioClip m_EngineIdling;       
    public AudioClip m_EngineDriving;      
    public float m_PitchRange = 0.2f;

    [HideInInspector]
    public float m_SpeedDecrement = 0f;


    private string m_MovementAxisName;     
    private string m_TurnAxisName;         
    private Rigidbody m_Rigidbody;         
    private float m_MovementInputValue;    
    private float m_TurnInputValue;        
    private float m_OriginalPitch;     


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    private void OnEnable ()
    {
        m_Rigidbody.isKinematic = false;
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }


    private void OnDisable ()
    {
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        m_MovementAxisName = "Vertical" + m_PlayerNumber;
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;

        m_OriginalPitch = m_MovementAudio.pitch;
    }

    private void Update()
    {
        // Store the player's input and make sure the audio for the engine is playing.
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);

        EngineAudio ();
    }


    private void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.
        bool isIdle = Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f;
        AudioClip currentClip = isIdle ? m_EngineIdling : m_EngineDriving;

        if (m_MovementAudio.clip != currentClip)
        {
            m_MovementAudio.clip = currentClip;
            m_MovementAudio.pitch = UnityEngine.Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
            m_MovementAudio.Play();
        }
    }


    private void FixedUpdate()
    {
        // Move and turn the tank.
        Move();
        Turn();
    }


    private void Move()
    {
        
        // Propel tank in the forward or backward direction
        MoveForward();

        // Reduce tank's sideways movement
        SuppressSidewaysMovement();
        
        
    }

    private void MoveForward() {
        // Compute current net speed in the direction of movement
        float currentMovementSpeed = Vector3.Dot(m_Rigidbody.velocity, transform.forward);

        // Compute target net speed in the direction of movement 
        float targetMovementSpeed = m_MovementInputValue * (m_Speed - m_SpeedDecrement);

        // If forward speed is within tolerance limits, don't bother applying acceleration to the tank
        if (Math.Abs(currentMovementSpeed - targetMovementSpeed)<=m_SpeedTolerance) return;
        
        // Compute target acceleration based on whether the tank is to move forwards or backwards
        Vector3 targetAcceleration = m_ForwardAcceleration * (currentMovementSpeed<targetMovementSpeed? 1f : -1f) * transform.forward;

        // Apply acceleration on tank
        m_Rigidbody.AddForce(targetAcceleration, ForceMode.Acceleration);
    }

    private void SuppressSidewaysMovement() {
        // Compute current net speed in the direction of movement
        float currentMovementSpeed = Vector3.Dot(m_Rigidbody.velocity, transform.forward);

        // Compute sidewards velocity of tank
        Vector3 sidewardsVelocity = m_Rigidbody.velocity - (transform.forward * currentMovementSpeed);

        // Compute sidewards drag (acceleration) to reduce drifting effects
        Vector3 sidewardsDrag = -1F * sidewardsVelocity * m_SidewardDragConstant;

        // Apply sideward drag (acceleration) on tank
        m_Rigidbody.AddForce(sidewardsDrag, ForceMode.VelocityChange);
    }


    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }
}