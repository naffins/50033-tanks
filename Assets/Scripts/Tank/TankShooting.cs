using UnityEngine;
using UnityEngine.UI;
using System;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;
    public Rigidbody m_Shell;
    public Transform m_FireTransform;
    public Slider m_AimSlider;
    public AudioSource m_ShootingAudio;
    public AudioClip m_ChargingClip;
    public AudioClip m_FireClip;
    public float m_MinLaunchForce = 15f;
    public float m_MaxLaunchForce = 30f;
    public float m_MaxChargeTime = 0.75f;
    public float m_FireRate = 1f;
    
    [HideInInspector]
    public float m_HorizontalShellDispersionAngleRange = 0F;
    [HideInInspector]
    public float m_VerticalShellDispersionAngleRange = 0F;
    [HideInInspector]
    public float m_FireRateIncrement = 0f;

    private string m_FireButton;
    private float m_CurrentLaunchForce;
    private float m_ChargeSpeed;
    private bool m_Fired;
    private float nextFireTime;

    private System.Random m_Rng;

    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
        m_Rng = new System.Random();
    }


    private void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber;
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }


    private void Update()
    {
        m_AimSlider.value = m_MinLaunchForce;

        if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire(m_CurrentLaunchForce, m_FireRate + m_FireRateIncrement);
        }
        else if (Input.GetButtonDown(m_FireButton))
        {
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;

            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play();
        }
        else if (Input.GetButton(m_FireButton) && !m_Fired)
        {
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
            m_AimSlider.value = m_CurrentLaunchForce;
        }
        else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
        {
            Fire(m_CurrentLaunchForce, m_FireRate + m_FireRateIncrement);
        }
    }


    public void Fire(float launchForce, float fireRate)
    {
        if (Time.time <= nextFireTime) return;

        nextFireTime = Time.time + fireRate;
        m_Fired = true;

        Quaternion horizontalFiringRotation = GetHorizontalFiringRotation(), verticalFiringRotation = GetVerticalFiringRotation();

        Rigidbody shellInstance =
            Instantiate(m_Shell, m_FireTransform.position, GetShellRotation(horizontalFiringRotation,verticalFiringRotation)) as Rigidbody;
        shellInstance.velocity = m_CurrentLaunchForce * GetShellDirection(horizontalFiringRotation,verticalFiringRotation);

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        m_CurrentLaunchForce = m_MinLaunchForce;
    }

    private Quaternion GetHorizontalFiringRotation() {
        return Quaternion.Euler(0F,(float)(m_Rng.NextDouble() - 0.5F) * m_HorizontalShellDispersionAngleRange,0F);
    }

    private Quaternion GetVerticalFiringRotation() {
        return Quaternion.Euler((float)(m_Rng.NextDouble() - 0.5F) * m_VerticalShellDispersionAngleRange,0F,0F);
    }

    private Quaternion GetShellRotation(Quaternion horizontalFiringRotation, Quaternion verticalFiringRotation) {
        return m_FireTransform.rotation * horizontalFiringRotation * verticalFiringRotation;
    }

    private Vector3 GetShellDirection(Quaternion horizontalFiringRotation, Quaternion verticalFiringRotation) {
        return verticalFiringRotation * (horizontalFiringRotation * m_FireTransform.forward);
    }
    
}