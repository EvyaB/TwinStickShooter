using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TwinStick
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float speed = 3f;
        [SerializeField] private int bounceCounter = 0;
        [SerializeField] private int damage = 1;
        [SerializeField] private bool playerBullet = true;

        private Rigidbody2D m_rigidBody;

        private void Awake()
        {
            m_rigidBody = GetComponent<Rigidbody2D>();
            m_rigidBody.useFullKinematicContacts = true;
        }

        private void FixedUpdate()
        {
            m_rigidBody.MovePosition(transform.position + -transform.up * speed * Time.deltaTime);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Check if hit live entity and react accordingly
            if ((collision.gameObject.tag == "Player" && !playerBullet) || (collision.gameObject.tag == "Enemy" && playerBullet))
            {
                Health targetHealth = collision.gameObject.GetComponent<Health>();
                if (null != targetHealth)
                {
                    targetHealth.GetHit(this.damage);
                }

                IEnemyAI enemyAi = collision.gameObject.GetComponent<IEnemyAI>();
                if (null != enemyAi)
                {
                    // Push enemies a bit when they are hit
                    enemyAi.PushThisEnemy(-transform.up, 1f);
                }

                Destroy(this.gameObject);
            }
            else
            {
                // Hit a wall, bounce if there are bounces left, otherwise disappear
                if (bounceCounter <= 0)
                {
                    Destroy(this.gameObject);
                }
                else
                {
                    // bounce the bullet
                    bounceCounter--;
                    var direction = Vector2.Reflect(m_rigidBody.velocity.normalized, collision.contacts[0].normal);
                    m_rigidBody.MoveRotation(Quaternion.LookRotation(-direction, Vector3.forward)); // Not sure why turning 'directino' to negative, but since there is not enough documentation on Vector2.Reflect idk...
                }
            }
        }
    }
}