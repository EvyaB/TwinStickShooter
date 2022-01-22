using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TwinStick
{
    public class GameManagement : MonoBehaviour
    {
        [SerializeField] private int nextSceneIndex;

        private EnemiesManager m_enemiesManager;
        private Health m_player;

        private void Reset()
        {
            nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        }

        private void Start()
        {
            m_enemiesManager = GameObject.FindObjectOfType<EnemiesManager>();
            m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        }

        private void FixedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Space) && DidWin())
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            if (Input.GetKeyDown(KeyCode.R) && DidLose())
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
        }

        public bool DidWin()
        {
            return (m_enemiesManager.AnyEnemiesAlive() == false);
        }
        public bool DidLose()
        {
            return (m_player.IsAlive() == false);
        }
    }
}
