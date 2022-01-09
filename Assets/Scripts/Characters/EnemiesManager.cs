using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwinStick
{
    public class EnemiesManager : MonoBehaviour
    {
        [SerializeField] private LevelCompletePanel levelCompletePanel;
        private List<IEnemyAI> enemies;

        private int deadEnemiesCounter;

        private void Start()
        {
            enemies = new List<IEnemyAI>();
            deadEnemiesCounter = 0;

            // Add all the children enemies into the list
            foreach (IEnemyAI enemy in GetComponentsInChildren<IEnemyAI>())
            {
                enemies.Add(enemy);
                enemy.GetComponent<Health>().OnDeath += ReportEnemyDeath;
            }
        }

        private void ReportEnemyDeath()
        {
            deadEnemiesCounter++;

            // Check if all enemies are dead
            if (deadEnemiesCounter >= enemies.Count) OnAllEnemiesDeath();
        }

        private void OnAllEnemiesDeath()
        {
            levelCompletePanel.ShowPanel();
        }
    }
}