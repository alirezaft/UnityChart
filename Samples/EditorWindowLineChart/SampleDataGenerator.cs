using System;
using UnityChart.Runtime;
using UnityEngine;
using Random = UnityEngine.Random;

public class SampleDataGenerator : MonoBehaviour
{
    [Header("Current Stats")]
    [Range(0, 100)] public float health;
    [Range(0, 100)] public float stamina;
    [Range(0, 100)] public float mana;
    [Range(0, 100)] public float experience;
    private float m_Seed;

    private DataProvider m_HealthDataProvider; 
    private DataProvider m_StaminaDataProvider; 
    private DataProvider m_ManaDataProvider; 
    private DataProvider m_ExperienceDataProvider;

    private void Start()
    {
        m_HealthDataProvider = new DataProvider(Color.red, "Health", "hp",  this);
        m_StaminaDataProvider = new DataProvider(Color.green, "Stamina", "stamina", this);
        m_ManaDataProvider = new DataProvider(Color.magenta, "Mana", "mana", this);
        m_ExperienceDataProvider = new DataProvider(Color.blue, "Experience", "xp", this);

        m_Seed = Random.value * 10 - 5;
    }

    private void Update()
    {
        float t = Time.time;

        health = 70f + Mathf.Sin(t * 0.5f * m_Seed) * 20f;
        m_HealthDataProvider.AddDataPoint(health);
        
        stamina = 50f + Mathf.Sin(t *  m_Seed * 0.8f + 1.0f) * 30f;
        m_StaminaDataProvider.AddDataPoint(stamina);
        
        mana = 60f + Mathf.Sin(t * m_Seed * 0.3f + 2.0f) * 25f;
        m_ManaDataProvider.AddDataPoint(mana);
        
        experience = 40f + Mathf.Sin(t * 0.2f + 3.0f + m_Seed) * 35f;
        m_ExperienceDataProvider.AddDataPoint(experience);
    }
}