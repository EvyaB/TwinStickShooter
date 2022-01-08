using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwinStick
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class CharacterSprite : MonoBehaviour
    {
        public void ActivateCharacterDeathEffects()
        {
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            renderer.color = renderer.color * Color.gray;
        }
    }
}