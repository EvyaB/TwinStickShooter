using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwinStick
{
    public class LevelCompletePanel : MonoBehaviour
    {
        private void Awake()
        {
            HidePanel();
        }

        public void HidePanel()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        public void ShowPanel()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }
    }
}