using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.IO;

namespace TwinStick
{
    public class PlayerShootingController : MonoBehaviour
    {
        [SerializeField] private Gun m_gunObject;
        private Health m_playerHealth;

        void Start()
        {
            if (null == m_gunObject)
            {
                Debug.LogError("No gun object assigned to the player character.");
            }
            m_playerHealth = GetComponent<Health>();
            if (null == m_playerHealth)
            {
                Debug.LogError("No Health script for the player character.");
            }
        }

        void Update()
        {
            // Check if player is still alive
            if (!m_playerHealth.IsAlive())
            {
                return;
            }

            if (Input.GetMouseButton(0))
            {
                m_gunObject.FireGun();
            }
        }

        void FixedUpdate()
        {
            // Check if player is still alive - if not ignore his inputs
            if (!m_playerHealth.IsAlive())
            {
                return;
            }

            AimAtMouse();
        }

        private void AimAtMouse()
        {
            // Get mouse position
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;

            Vector3 aimDirection = (mousePosition - transform.position).normalized;
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0f, 0f, angle);
        }
    }
}