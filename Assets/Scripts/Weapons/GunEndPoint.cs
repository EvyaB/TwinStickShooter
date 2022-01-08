using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TwinStick
{
    public class GunEndPoint : MonoBehaviour
    {
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.05f);
        }
    }
}