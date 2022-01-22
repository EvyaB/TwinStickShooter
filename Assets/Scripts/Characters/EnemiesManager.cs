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
            enemies = new List<IEnemyAI>();

            // Add all the children enemies into the list
            foreach (IEnemyAI enemy in GetComponentsInChildren<IEnemyAI>())
            {
                enemies.Add(enemy);
                enemy.GetComponent<Health>().OnDeath += ReportEnemyDeath;
            }
        }

        private void ReportEnemyDeath(bool criticalDeath)
        {
            // Check if all enemies are dead
            if (enemies.Any(enemy => enemy.GetComponent<Health>().IsAlive()) == false) OnAllEnemiesDeath();
        }

        private void OnAllEnemiesDeath()
        {
            levelCompletePanel.ShowPanel();
        }
    }
}