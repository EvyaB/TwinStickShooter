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
        public event CharacterHitEvent OnHit;

        public int GetCurrentHealth()
        {
            return healthValue;
        }

        public void GetHit(int damage)
        {
            healthValue -= damage;

            // call OnHit event so other effects are activated (mostly UI)
            // Note that making the UI active instead of passive with events is probably better design, but I'm just testing the events in unity
            if (OnHit != null)
            {
                OnHit(healthValue);
            }

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
        }

        IEnumerator DissapearAfterDeath()
        {
            yield return new WaitForSeconds(dissapearTime);
            Destroy(gameObject);
        }
    }
}