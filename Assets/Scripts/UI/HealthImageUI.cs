using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TwinStick
{
    [RequireComponent(typeof(Image))]
    public class HealthImageUI : MonoBehaviour
    {
        [SerializeField] private float losingHpAnimDuration = 3f;
        [SerializeField] private float gainingHpAnimDuration = 3f;
        [SerializeField] private float fadeMaxValue = 1f;
        [SerializeField] private float hologramBlendMaxValue = 0.65f;
        Material m_imageMaterial;

        private void Awake()
        {
            Image image = GetComponent<Image>();
            image.material = new Material(image.material); // Duplicate the material so all the effects on the AllIn1Shader will affect only a single gameobject
            m_imageMaterial = image.material;
        }

        private void OnDestroy()
        {
            // Editing the Image materials for AllIn1Shader persists between playthroughs because why not, so we reset their values
            ResetEffects();
        }

        private void ResetEffects()
        {
            m_imageMaterial.SetFloat("_FadeAmount", 0f);
            m_imageMaterial.SetFloat("_HologramBlend", 0f);
        }

        public IEnumerator LoseHealth()
        {
            float timeSinceAnimationStarted = 0f;

            while (timeSinceAnimationStarted < losingHpAnimDuration * 1000f)
            {
                timeSinceAnimationStarted += Time.fixedDeltaTime;
                float fadeAmount = Mathf.Lerp(0f, fadeMaxValue, timeSinceAnimationStarted / losingHpAnimDuration);
                float hologramBlendAmount = Mathf.Lerp(0f, hologramBlendMaxValue, timeSinceAnimationStarted / losingHpAnimDuration);
                m_imageMaterial.SetFloat("_FadeAmount", fadeAmount);
                m_imageMaterial.SetFloat("_HologramBlend", hologramBlendAmount);
                yield return new WaitForFixedUpdate();
            }
        }

        public IEnumerator RegainHealth()
        {
            float timeSinceAnimationStarted = 0f;

            while (timeSinceAnimationStarted < gainingHpAnimDuration * 1000f)
            {
                timeSinceAnimationStarted += Time.fixedDeltaTime;
                float fadeAmount = Mathf.Lerp(fadeMaxValue, 0f, timeSinceAnimationStarted / losingHpAnimDuration);
                float hologramBlendAmount = Mathf.Lerp(hologramBlendMaxValue, 0f, timeSinceAnimationStarted / losingHpAnimDuration);
                m_imageMaterial.SetFloat("_FadeAmount", fadeAmount);
                m_imageMaterial.SetFloat("_HologramBlend", hologramBlendAmount);
                yield return new WaitForFixedUpdate();
            }
        }
    }
}