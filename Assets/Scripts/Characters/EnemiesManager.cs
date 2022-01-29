using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TwinStick
{
    public class EnemiesManager : MonoBehaviour
    {
        [SerializeField] private LevelCompletePanel levelCompletePanel;
        private List<IEnemyAI> enemies;

        private void Start()
        {
            if (levelCompletePanel == null) levelCompletePanel = GameObject.FindObjectOfType<LevelCompletePanel>();

            enemies = new List<IEnemyAI>();

            // Add all the children enemies into the list
            foreach (IEnemyAI enemy in GetComponentsInChildren<IEnemyAI>())
            {
                enemies.Add(enemy);
                enemy.GetComponent<Health>().OnDeath += ReportEnemyDeath;
            }
        }

        public bool AnyEnemiesAlive()
        {
            return (enemies.Any(enemy => enemy.GetComponent<Health>().IsAlive()));
        }

        private void ReportEnemyDeath(bool criticalDeath)
        {
            // Check if all enemies are dead
            if (AnyEnemiesAlive() == false) OnAllEnemiesDeath();
        }

        private void OnAllEnemiesDeath()
        {
            levelCompletePanel.ShowPanel();
        }
    }
}