using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwinStick
{
    [RequireComponent(typeof(ParticleSystem))]
    public class CharacterHitEffectScript : MonoBehaviour
    {
        private ParticleSystem m_particleSystem;

        // Start is called before the first frame update
        void Awake()
        {
            m_particleSystem = GetComponent<ParticleSystem>();
        }

        void Start()
        {
            Health healthComponent = GetComponentInParent<Health>();
            if (healthComponent != null)
            {
                healthComponent.OnHit += PlayHitEffect;
            }
        }

        void PlayHitEffect(int newHp)
        {
            m_particleSystem.Play();
        }
    }
}