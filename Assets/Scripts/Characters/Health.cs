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

        public delegate void CharacterHitEvent(int newHP);
        public delegate void CharacterDeathEvent();
        public event CharacterHitEvent OnHit;
        public event CharacterDeathEvent OnDeath;

        public int GetCurrentHealth()
        {
            return healthValue;
        }

        public void GetHit(int damage)
        {
            if (!IsAlive()) return; // Ignore if already dead

            healthValue -= damage;

            // call OnHit event so other effects are activated (mostly UI)
            // Note that making the UI active instead of passive with events is probably better design, but I'm just testing the events in unity
            if (OnHit != null) OnHit(healthValue);

            // Check if died due to this hit
            if (!IsAlive())
            {
                GetKilled();
            }
        }

        public bool IsAlive()
        {
            return (healthValue > 0);
        }

        private void GetKilled()
        {
            ActivateDeathEffects();

            if (dissapearAfterDeath)
            {
                StartCoroutine(DissapearAfterDeath());
            }
        }

        private void ActivateDeathEffects()
        {
            // Start rotating due to the last shot hitting ('ragdoll') by allowing collisions to affect the Z axis
            Rigidbody2D rigidBody = GetComponent<Rigidbody2D>();
            if (null != rigidBody)
            {
                rigidBody.freezeRotation = false;
            }

            CharacterSprite characterSprite = GetComponentInChildren<CharacterSprite>();
            if (null != characterSprite) 
            {
                characterSprite.ActivateCharacterDeathEffects();
            }

            // Activate other death effects (mostly UI)
            if (OnDeath != null) OnDeath();
        }

        IEnumerator DissapearAfterDeath()
        {
            yield return new WaitForSeconds(dissapearTime);
            Destroy(gameObject);
        }
    }
}