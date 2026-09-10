using UnityEngine;

namespace PolarityBreach.Player
{
    public class VRHealthHaptics : MonoBehaviour
    {
        [SerializeField] private PlayerHealth playerHealth;

        [Header("Umbral")]
        [SerializeField] private float startBelowHealthPercent = 0.6f;

        [Header("Intensidad")]
        [SerializeField, Range(0f, 1f)] private float minAmplitude = 0.05f;
        [SerializeField, Range(0f, 1f)] private float maxAmplitude = 0.35f;
        [SerializeField, Range(0f, 1f)] private float frequency = 0.3f;

        [Header("Latido")]
        [SerializeField] private float slowPulse = 1.5f;
        [SerializeField] private float fastPulse = 6f;

        private float pulseTimer;

        private void Update()
        {
            if (playerHealth == null) return;

            float healthPercent = playerHealth.CurrentHealth / playerHealth.MaxHealth;

            if (healthPercent >= startBelowHealthPercent || playerHealth.IsDead)
            {
                Stop();
                return;
            }

            float lowHealth = 1f - Mathf.Clamp01(healthPercent / startBelowHealthPercent);

            float pulseSpeed = Mathf.Lerp(slowPulse, fastPulse, lowHealth);
            pulseTimer += Time.deltaTime * pulseSpeed;

            float wave = (Mathf.Sin(pulseTimer) + 1f) * 0.5f;
            float amplitude = Mathf.Lerp(minAmplitude, maxAmplitude, lowHealth) * wave;

            OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.LTouch);
            OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.RTouch);
        }

        private void Stop()
        {
            OVRInput.SetControllerVibration(0f, 0f, OVRInput.Controller.LTouch);
            OVRInput.SetControllerVibration(0f, 0f, OVRInput.Controller.RTouch);
        }

        private void OnDisable()
        {
            Stop();
        }
    }
}