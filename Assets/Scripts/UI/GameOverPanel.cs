using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwinStick
{
    public class GameOverPanel : MonoBehaviour
    {
        private void Start()
        {
            HidePanel();

            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                Health playerHealth = playerObject.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.OnDeath += ShowPanel;
                }
                else
                {
                    Debug.LogError("Couldn't find Health data on the Player object '" + playerObject.name + "' for the Level Completion UI.");
                }
            }
            else
            {
                Debug.LogError("Couldn't find the player object for the Health UI");
            }
        }

        private void HidePanel()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        private void ShowPanel()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }
    }
}