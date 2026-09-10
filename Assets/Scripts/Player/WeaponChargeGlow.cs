using UnityEngine;
using PolarityBreach.PolaritySystem;

public class WeaponChargeGlow : MonoBehaviour
{
    [SerializeField] private TestShooter shooter;
    [SerializeField] private Renderer weaponRenderer;

    [Header("Parpadeo")]
    [SerializeField] private string invertProperty = "_InvertAmount";
    [SerializeField] private float blinkSpeed = 4f;
    [SerializeField] private float blinkRange = 0.25f;

    [Header("Vibración")]
    [SerializeField, Range(0f, 1f)] private float vibrationFrequency = 0.5f;
    [SerializeField, Range(0f, 1f)] private float maxAmplitude = 0.6f;

    private Material weaponMaterial;
    private float baseValue;
    private bool wasReady;

    private void Awake()
    {
        if (weaponRenderer == null)
            weaponRenderer = GetComponent<Renderer>();

        if (weaponRenderer != null)
            weaponMaterial = weaponRenderer.material;
    }

    private void Update()
    {
        if (shooter == null || weaponMaterial == null) return;

        bool ready = shooter.ChargeReady;

        if (ready)
        {
            if (!wasReady)
            {
                baseValue = weaponMaterial.GetFloat(invertProperty);
                wasReady = true;
            }

            float wave = (Mathf.Sin(Time.time * blinkSpeed) + 1f) * 0.5f;

            float value = baseValue >= 0.5f
                ? baseValue - (wave * blinkRange)
                : baseValue + (wave * blinkRange);

            weaponMaterial.SetFloat(invertProperty, value);

            OVRInput.SetControllerVibration(vibrationFrequency, wave * maxAmplitude, OVRInput.Controller.RTouch);
        }
        else if (wasReady)
        {
            weaponMaterial.SetFloat(invertProperty, baseValue);
            StopVibration();
            wasReady = false;
        }
    }

    private void StopVibration()
    {
        OVRInput.SetControllerVibration(0f, 0f, OVRInput.Controller.RTouch);
    }

    private void OnDisable()
    {
        StopVibration();
    }
}