using UnityEngine;
using System.Collections;

// Copied from commit by 'carohauta' on the original version of Unity-2D-Destruction
[RequireComponent(typeof(Renderer))]
public class DestroyObjectDelay : MonoBehaviour
{
    public float delay = 1;

    private float initialAlpha;
    private float timeSinceStart;

    private void Start()
    {
        initialAlpha = GetComponent<Renderer>().material.color.a;
        timeSinceStart = 0f;

        Invoke("DestroyObject", delay);
    }

    private void Update()
    {
        var material = GetComponent<Renderer>().material;
        var color = material.color;

        timeSinceStart += Time.deltaTime;
        material.color = new Color(color.r, color.g, color.b, Mathf.Lerp(initialAlpha, 0f, (timeSinceStart / delay)));
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}