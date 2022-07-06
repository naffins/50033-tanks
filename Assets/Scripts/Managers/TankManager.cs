using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TankManager
{

    // Debuffs to be applied for each round won
    public float m_HorizontalShellDispersionAngleRangeIncrement = 4f;
    public float m_VerticalShellDispersionAngleRangeIncrement = 1.5f;
    public float m_FireRateIncrementRate = 0.25f;
    public float m_SpeedDecrementRate = 1f;
    
    public Color m_PlayerColor;
    public Transform m_SpawnPoint;
    [HideInInspector] public int m_PlayerNumber;
    [HideInInspector] public string m_ColoredPlayerText;
    [HideInInspector] public GameObject m_Instance;
    [HideInInspector] public int m_Wins;


    private TankMovement m_Movement;
    private TankShooting m_Shooting;
    private GameObject m_CanvasGameObject;
    private StateController m_StateController;

    public void SetupAI(List<Transform> wayPointList)
    {
        m_StateController = m_Instance.GetComponent<StateController>();
        m_StateController.SetupAI(true, wayPointList);

        m_Shooting = m_Instance.GetComponent<TankShooting>();
        m_Shooting.m_PlayerNumber = m_PlayerNumber;

        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;
        m_ColoredPlayerText = $"<color=#{ColorUtility.ToHtmlStringRGB(m_PlayerColor)}>PLAYER {m_PlayerNumber}</color>";

        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++) renderers[i].material.color = m_PlayerColor;
    }


    public void SetupPlayerTank()
    {
        m_Movement = m_Instance.GetComponent<TankMovement>();
        m_Shooting = m_Instance.GetComponent<TankShooting>();
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;

        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Shooting.m_PlayerNumber = m_PlayerNumber;

        m_ColoredPlayerText = $"<color=#{ColorUtility.ToHtmlStringRGB(m_PlayerColor)}>PLAYER {m_PlayerNumber}</color>";

        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++) renderers[i].material.color = m_PlayerColor;
    }

    public void DisableControl()
    {
        if (m_Movement != null) m_Movement.enabled = false;

        if (m_StateController != null) m_StateController.enabled = false;

        m_Shooting.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }

    public void EnableControl()
    {
        if (m_StateController != null) m_StateController.enabled = true;

        m_Shooting.enabled = true;
        m_CanvasGameObject.SetActive(true);

        // In case of player
        if (m_Movement != null) {
            m_Movement.enabled = true;
            ApplyPlayerDebuffs();
        }
    }

    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }

    private void ApplyPlayerDebuffs() {
        // Speed reduction
        m_Movement.m_SpeedDecrement = m_SpeedDecrementRate * m_Wins;
        // Firing vertical dispersion increase
        m_Shooting.m_VerticalShellDispersionAngleRange = m_VerticalShellDispersionAngleRangeIncrement * m_Wins;
        // Firing horizontal dispersion increase
        m_Shooting.m_HorizontalShellDispersionAngleRange = m_HorizontalShellDispersionAngleRangeIncrement * m_Wins;
        // Reload time increase
        m_Shooting.m_FireRateIncrement = m_FireRateIncrementRate * m_Wins;
    }

}