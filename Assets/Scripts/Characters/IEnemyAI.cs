using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwinStick
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Health))]
    public class IEnemyAI : MonoBehaviour
    {
        protected Transform m_player;
        protected Rigidbody2D m_rigidBody;
        protected Health m_health;

        protected Vector2 m_pushDir = Vector2.zero;
        protected float m_pushForce = 0f;

        private LayerMask wallsLayerMask;

        virtual protected void Start()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (null != player)
            {
                m_player = player.transform;
            }
            else
            {
                Debug.LogError("Cannot find player object!");
            }

            m_rigidBody = GetComponent<Rigidbody2D>();
            m_health = GetComponent<Health>();
        }

        public void PushThisEnemy(Vector2 pushDir, float force)
        {
            m_pushDir = pushDir;
            m_pushForce = force;
        }

        protected bool HasLineOfSightToPlayer()
        {
            var raycastHit = Physics2D.Linecast(transform.position, m_player.position, LayerMask.GetMask("World"));
            return !raycastHit;
        }
    }
}