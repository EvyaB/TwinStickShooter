using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.IO;

namespace TwinStick
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float speed = 4.5f;

        [SerializeField] private float minRelativeVelocityMagnitudeForCrashing = 3f;
        [SerializeField] private float minRelativeVelocityMagnitudeForCriticalCrashDamage = 5f;
        [SerializeField] private int crashDamage = 1;
        [SerializeField] private int criticalCrashDamageVal = 3;

        private Rigidbody2D m_rigidBody;
        private Health m_playerHealth;

        private void Awake()
        {
            m_rigidBody = GetComponent<Rigidbody2D>();

            m_playerHealth = GetComponent<Health>();
            if (null == m_playerHealth)
            {
                Debug.LogError("No Health script for the player character.");
            }
        }

        void FixedUpdate()
        {
            if (!m_playerHealth.IsAlive()) return;

            //m_rigidBody.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, Input.GetAxis("Vertical") * speed);
            m_rigidBody.AddForce(new Vector2(Input.GetAxis("Horizontal") * speed, Input.GetAxis("Vertical") * speed), ForceMode2D.Force);
        }

        // SMASH enemies when colliding into them (Wrecking Ball!)
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                var crashForce = collision.relativeVelocity.magnitude;

                if (crashForce > minRelativeVelocityMagnitudeForCrashing)
                {
                    Health enemyHealth = collision.gameObject.GetComponent<Health>();
                    IEnemyAI enemyAI = collision.gameObject.GetComponent<IEnemyAI>();
                 
                    // Calculate Angle Between the collision point and the player
                    Vector2 pushDir = (collision.GetContact(0).point - (Vector2)transform.position).normalized;

                    // Check if the crash is powerful enough to be a Critical
                    if (crashForce < minRelativeVelocityMagnitudeForCriticalCrashDamage)
                    {
                        // Normal Crash
                        enemyHealth.GetHit(crashDamage);
                        enemyAI.PushThisEnemy(pushDir, crashForce / 2f);
                    }
                    else
                    {
                        // Critical Crash
                        Debug.Log("Critical Crash!");
                        enemyHealth.GetHit(criticalCrashDamageVal);
                        enemyAI.PushThisEnemy(pushDir, crashForce);
                    }
                }
            }
        }
    }
}