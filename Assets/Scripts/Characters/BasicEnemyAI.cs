using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwinStick
{
    public class BasicEnemyAI : IEnemyAI
    {
        [SerializeField] float minTimeToFire = 2f;
        [SerializeField] float maxTimeToFire = 5f;
        [SerializeField] Gun m_gunObject;
        [SerializeField] float speed = 1f;

        [SerializeField] float minTimeToChangeDirection = 4f;
        [SerializeField] float maxTimeToChangeDirection = 7f;

        Vector3 m_currentDirection;


        // TODO: 'Remember' dir if going behind a wall. Use move rotation instead of 'jumping' do specific direction

        override protected void Start()
        {
            base.Start();

            StartCoroutine(FireGun());
            StartCoroutine(ChangeDirection());
        }

        void FixedUpdate()
        {
            HandleMovement();

            if (m_health.IsAlive())
            {
                AimAtPlayer();
            }
        }

        void HandleMovement()
        {
            m_rigidBody.MovePosition(transform.position + m_currentDirection * speed * Time.deltaTime + (Vector3)m_pushDir * m_pushForce * Time.deltaTime);

            // Lower push force overtime
            m_pushForce = m_pushForce > 0 ? m_pushForce - 0.03f : 0;
        }

        void AimAtPlayer()
        {
            Vector3 aimDirection;

            // Validity check
            if (null != m_player && HasLineOfSightToPlayer())
            {
                // Aim at the player
                aimDirection = (m_player.position - transform.position).normalized;
            }
            else
            {
                // No player - aim along the movement direction
                aimDirection = m_currentDirection.normalized;
            }

            // Calculate euler angles to aim at according to the direction vector
            float targetAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            m_rigidBody.MoveRotation(Mathf.LerpAngle(m_rigidBody.rotation, targetAngle, 0.08f)); // 'slowly' rotate towards the target angle.
        }

        Vector3 ChooseRandomMovementDirection()
        {
            const float randomRange = 0.5f;
            Vector3 movementDirection;

            if (null != m_player && HasLineOfSightToPlayer())
            {
                // Choose a random direction that is somewhat in the general direction of the player
                Vector3 aimDirection = (m_player.position - transform.position).normalized;
                float randomX = Random.Range(Mathf.Clamp(aimDirection.x - randomRange, -1f, 1f), Mathf.Clamp(aimDirection.x + randomRange, -1f, 1f));
                float randomY = Random.Range(Mathf.Clamp(aimDirection.y - randomRange, -1f, 1f), Mathf.Clamp(aimDirection.y + randomRange, -1f, 1f));
                movementDirection = new Vector3(randomX, randomY, 0f);
            }
            else
            {
                // No target - choose a random direction
                movementDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
            }

            return movementDirection;
        }


        IEnumerator ChangeDirection()
        {
            // Don't change direction after death
            while (m_health.IsAlive())
            {
                m_currentDirection = ChooseRandomMovementDirection();
                yield return new WaitForSeconds(Random.Range(minTimeToChangeDirection, maxTimeToChangeDirection));
            }
        }

        // Randomly fire the gun every so often depending on the min and max time
        IEnumerator FireGun()
        {
            // Don't immediately shoot when entering this method
            yield return new WaitForSeconds(Random.Range(minTimeToFire, maxTimeToFire));

            // Continue firing as long as this enemy is still alive and active
            while (m_health.IsAlive())
            {
                // Fire if the player is alive
                if (null != m_player)
                {
                    m_gunObject.FireGun();
                }

                yield return new WaitForSeconds(Random.Range(minTimeToFire, maxTimeToFire));
            }
        }
    }
}