using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PolarityBreach.Player
{
    public class VRBloodVignette : MonoBehaviour
    {
        [SerializeField] private Volume bloodVolume;
        [SerializeField] private PlayerHealth playerHealth;

        [Header("Apertura del óvalo")]
        [SerializeField] private float minIntensity = 0.25f;
        [SerializeField] private float maxIntensity = 0.55f;

        [Header("Latido")]
        [SerializeField] private float pulseAmount = 0.12f;
        [SerializeField] private float slowPulse = 1.5f;
        [SerializeField] private float fastPulse = 6f;

        [Header("Golpe reciente")]
        [SerializeField] private float hitFlash = 0.25f;
        [SerializeField] private float hitFadeSpeed = 2f;

        private Vignette vignette;
        private float pulseTimer;
        private float hitAmount;

        private void Awake()
        {
            if (bloodVolume == null) bloodVolume = GetComponent<Volume>();

            if (bloodVolume != null && bloodVolume.profile != null)
                bloodVolume.profile.TryGet(out vignette);

            if (bloodVolume != null) bloodVolume.weight = 0f;
        }

        public void OnDamaged()
        {
            hitAmount = hitFlash;
        }

        private void Update()
        {
            if (vignette == null || playerHealth == null) return;

            float healthPercent = playerHealth.CurrentHealth / playerHealth.MaxHealth;
            float lowHealth = 1f - Mathf.Clamp01(healthPercent);

            // Con poca vida late más rápido
            float pulseSpeed = Mathf.Lerp(slowPulse, fastPulse, lowHealth);
            pulseTimer += Time.deltaTime * pulseSpeed;
            float pulse = (Mathf.Sin(pulseTimer) + 1f) * 0.5f * pulseAmount * lowHealth;

            if (hitAmount > 0f)
                hitAmount -= hitFadeSpeed * Time.deltaTime;

            // El óvalo se cierra conforme baja la vida
            float intensity = Mathf.Lerp(minIntensity, maxIntensity, lowHealth) + pulse + Mathf.Max(0f, hitAmount);
            vignette.intensity.value = Mathf.Clamp01(intensity);

            // La nitidez del rojo también sube con el daño
            bloodVolume.weight = Mathf.Clamp01(lowHealth + Mathf.Max(0f, hitAmount));
        }
    }
}