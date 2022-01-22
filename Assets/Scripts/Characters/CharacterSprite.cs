using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwinStick
{
    public class CharacterSprite : MonoBehaviour
    {
        private Health m_characterHealth;

        public void Start()
        {
            m_characterHealth = GetComponent<Health>();
            if (m_characterHealth != null)
            {
                m_characterHealth.OnDeath += ActivateCharacterDeathEffects;
            }
        }

        public void ActivateCharacterDeathEffects(bool criticalDeath)
        {
            GreyOutSprites();

            // Explode on critical damage
            if (criticalDeath)
            {
                Explodable explodable = GetComponent<Explodable>();
                if (explodable != null) { explodable.explode(); }
            }
        }

        private void GreyOutSprites()
        {
            foreach (SpriteRenderer spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
            {
                spriteRenderer.color = spriteRenderer.color * Color.gray;
            }

            SpriteRenderer selfSpriteRenderer = GetComponent<SpriteRenderer>();
            if (selfSpriteRenderer) { selfSpriteRenderer.color = selfSpriteRenderer.color * Color.gray; }
        }
    }
}