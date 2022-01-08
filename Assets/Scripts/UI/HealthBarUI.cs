using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwinStick
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private List<HealthImageUI> HP;
        private Health m_playerHealth;
        private int m_currentHP;

        private void Awake()
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                m_playerHealth = playerObject.GetComponent<Health>();
                if (m_playerHealth != null)
                {
                    m_playerHealth.OnHit += UpdateHP;
                    m_currentHP = m_playerHealth.GetCurrentHealth();
                }
                else
                {
                    Debug.LogError("Couldn't find Health data on the Player object '" + playerObject.name + "' for the Health UI.");
                }
            }
            else
            {
                Debug.LogError("Couldn't find the player object for the Health UI");
            }
        }

        private void UpdateHP(int newHp)
        {
            // Check if HP has increased
            if (m_currentHP < newHp)
            {
                for (int i = m_currentHP; i < newHp && i < HP.Count; i++)
                {
                    StartCoroutine(HP[i].RegainHealth());
                }
            }
            // Check if HP has decreased
            else if (m_currentHP > newHp)
            {
                for (int i = newHp; i < m_currentHP && i < HP.Count; i++)
                {
                    StartCoroutine(HP[i].LoseHealth());
                }
            }

            m_currentHP = newHp;
        }
    }
}