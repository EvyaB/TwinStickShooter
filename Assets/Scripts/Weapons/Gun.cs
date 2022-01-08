using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwinStick
{
    public class Gun : MonoBehaviour
    {
        [SerializeField] private Transform bulletSpawnOffsetPosition;
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private float fireRatePerSecond = 0.5f;
        private float timeSinceLastShot = float.PositiveInfinity;

        public void Start()
        {
            if (null == bulletPrefab)
            {
                Debug.LogError("No bullet prefab assigned for the gun " + this.name + ".");
            }
        }

        // Return bullet objects
        public List<Bullet> FireGun()
        {
            List<Bullet> bullets = new List<Bullet>();
            if (timeSinceLastShot > fireRatePerSecond)
            {
                timeSinceLastShot = 0f;
                bullets.Add(Instantiate(bulletPrefab, bulletSpawnOffsetPosition.position, bulletSpawnOffsetPosition.rotation));
            }

            return bullets;
        }

        public void Update()
        {
            timeSinceLastShot += Time.deltaTime;
        }

    }
}
