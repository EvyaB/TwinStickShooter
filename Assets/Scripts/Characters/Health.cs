using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwinStick
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private int healthValue = 1;
        [SerializeField] private bool dissapearAfterDeath = true;
        [SerializeField] private float dissapearTime = 3f;
        [SerializeField] private int criticalDeathHealth = -1;

        public delegate void CharacterHitEvent(int newHP);
        public delegate void CharacterDeathEvent(bool criticalDeath);
        public event CharacterHitEvent OnHit;
        public event CharacterDeathEvent OnDeath;

        private bool m_hasDied = false;

        private void Start()
        {
            m_hasDied = false;
        }

        public int GetCurrentHealth()
        {
            return healthValue;
        }
        public bool DidCriticalDied()
        {
            return (GetCurrentHealth() <= criticalDeathHealth);
        }

        public void GetHit(int damage)
        {
            healthValue -= damage;

            // call OnHit event so other effects are activated (mostly UI)
            // Note that making the UI active instead of passive with events is probably better design, but I'm just testing the events in unity
            if (OnHit != null) OnHit(healthValue);

            // Check if died due to this hit
            if (!IsAlive())
            {
                KillThis();
            }
        }

        public bool IsAlive()
        {
            return (healthValue > 0);
        }

        private void KillThis()
        {
            ActivateDeathEffects();

            m_hasDied = true;
        }

        private void ActivateDeathEffects()
        {
            // Start rotating due to the last shot hitting ('ragdoll') by allowing collisions to affect the Z axis
            Rigidbody2D rigidBody = GetComponent<Rigidbody2D>();
            if (null != rigidBody)
            {
                rigidBody.freezeRotation = false;
            }

            // Activate other death effects (UI, CharacterSprite effects...)
            if (OnDeath != null) OnDeath(DidCriticalDied());

            // Disappear after some time
            if (dissapearAfterDeath && !m_hasDied)
            {
                StartCoroutine(DissapearAfterDeath());
            }
        }

        IEnumerator DissapearAfterDeath()
        {
            yield return new WaitForSeconds(dissapearTime);
            Destroy(gameObject);
        }
    }
}