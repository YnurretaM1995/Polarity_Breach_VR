using PolarityBreach.Audio;
using PolarityBreach.Player;
using PolarityBreach.UI;
using UnityEngine;

namespace PolarityBreach.PolaritySystem
{
    [RequireComponent(typeof(PolarityComponent))]
    [RequireComponent(typeof(PlayerStatsData))]
    public class TestShooter : MonoBehaviour
    {
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private GameObject _chargedProjectilePrefab;
        [SerializeField] private ProjectilePool _normalProjectilePool;
        [SerializeField] private ProjectilePool _chargedProjectilePool;

        [SerializeField] private AudioClip[] shootSounds;
        [SerializeField] private AudioClip shootSound;
        [SerializeField, Range(0f, 1f)] private float shootSoundVolume = 1f;
        [SerializeField] private bool playShootSoundAs2D;

        [Header("Charge Shot SFX")]
        [SerializeField] private AudioSource chargeLoopSource;
        [SerializeField] private AudioClip chargeSound;
        [SerializeField, Range(0f, 1f)] private float chargeSoundVolume = 1f;
        [SerializeField] private AudioSource heldChargeLoopSource;
        [SerializeField] private AudioClip heldChargeSound;
        [SerializeField, Range(0f, 1f)] private float heldChargeSoundVolume = 1f;
        [SerializeField] private AudioClip chargeCancelSound;
        [SerializeField, Range(0f, 1f)] private float chargeCancelSoundVolume = 1f;
        [SerializeField] private AudioClip chargedShotSound;
        [SerializeField, Range(0f, 1f)] private float chargedShotSoundVolume = 1f;
        [SerializeField] private bool playChargeShotSoundsAs2D = true;

        [Header("Charge")]
        [Tooltip("Delay de referencia. Con este valor la carga tarda exactamente chargeTime.")]
        [SerializeField] private float referenceAttackDelay = 0.5f;
        [SerializeField] private float minChargePitch = 0.7f;
        [SerializeField] private float maxChargePitch = 2f;

        [Header("Vibración")]
        [SerializeField, Range(0f, 1f)] private float vibrationFrequency = 0.5f;
        [SerializeField, Range(0f, 1f)] private float vibrationAmplitude = 0.8f;
        [SerializeField] private float vibrationDuration = 0.15f;

        private bool _isCharging;
        private bool _chargeReady;
        private float _chargeStartTime;
        private bool _wasPressed;

        [SerializeField] private Transform _muzzle;
        private PolarityComponent _polarity;
        private float _lastShotTime = float.NegativeInfinity;
        private PlayerStatsData _playerStats;

        public bool IsCharging => _isCharging;
        public bool ChargeReady => _chargeReady;

        public float ChargeProgress
        {
            get
            {
                if (!_isCharging) return 0f;
                float required = GetChargeTime();
                if (required <= 0f) return 1f;
                return Mathf.Clamp01((Time.time - _chargeStartTime) / required);
            }
        }

        private void Awake()
        {
            _polarity = GetComponent<PolarityComponent>();
            _playerStats = GetComponent<PlayerStatsData>();

            if (_muzzle == null)
                _muzzle = transform;

            if (chargeLoopSource == null)
                chargeLoopSource = CreateLoopSource("Charge Shot Loop Audio");
            if (heldChargeLoopSource == null)
                heldChargeLoopSource = CreateLoopSource("Held Charge Shot Loop Audio");
        }

        private void OnEnable()
        {
            PauseMenu.OnPauseChanged += HandlePause;
            UIQueue.OnBlockingChanged += HandlePause;
        }

        private void OnDisable()
        {
            PauseMenu.OnPauseChanged -= HandlePause;
            UIQueue.OnBlockingChanged -= HandlePause;
            StopChargeLoop();
            StopHeldChargeLoop();
            StopVibration();
        }

        private void Update()
        {
            if (UIQueue.IsBlocking || PauseMenu.IsPaused)
            {
                _wasPressed = false;
                return;
            }

            float trigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
            bool pressed = trigger >= 0.5f;

            // Gatillo suelto
            if (!pressed)
            {
                if (_isCharging) ReleaseCharge();
                _wasPressed = false;
                return;
            }

            // Primera presión: una bala si el cooldown lo permite
            if (!_wasPressed)
            {
                _wasPressed = true;

                if (Time.time >= _lastShotTime + _playerStats.attackSpeedDelay)
                {
                    Shoot();

                    if (_playerStats.chargeShotUnlocked)
                        StartCharging();
                }

                return;
            }

            // Sostenido: carga el especial
            if (_isCharging) UpdateCharging();
        }

        private void HandlePause(bool paused)
        {
            if (paused)
            {
                CancelCharge(false);
                _wasPressed = false;
            }
        }

        private float GetChargeTime()
        {
            if (referenceAttackDelay <= 0f) return _playerStats.chargeTime;
            return _playerStats.chargeTime * (_playerStats.attackSpeedDelay / referenceAttackDelay);
        }

        private float GetChargePitch()
        {
            if (chargeSound == null) return 1f;

            float required = GetChargeTime();
            if (required <= 0f) return maxChargePitch;

            float pitch = chargeSound.length / required;
            return Mathf.Clamp(pitch, minChargePitch, maxChargePitch);
        }

        private void StartCharging()
        {
            _isCharging = true;
            _chargeReady = false;
            _chargeStartTime = Time.time;
            StopHeldChargeLoop();
            StartChargeLoop();
        }

        private void UpdateCharging()
        {
            float holdTime = Time.time - _chargeStartTime;

            if (holdTime >= GetChargeTime() && !_chargeReady)
            {
                _chargeReady = true;
                StartHeldChargeLoop();
                StopChargeLoop();
                StartCoroutine(VibrateReady());
            }
        }

        private System.Collections.IEnumerator VibrateReady()
        {
            OVRInput.SetControllerVibration(vibrationFrequency, vibrationAmplitude, OVRInput.Controller.RTouch);
            yield return new WaitForSecondsRealtime(vibrationDuration);
            StopVibration();
        }

        private void StopVibration()
        {
            OVRInput.SetControllerVibration(0f, 0f, OVRInput.Controller.RTouch);
        }

        private void ReleaseCharge()
        {
            if (_isCharging && _chargeReady)
            {
                StopChargeLoop();
                StopHeldChargeLoop();
                ChargeShot();
            }
            else
            {
                CancelCharge(true);
            }

            _isCharging = false;
            _chargeReady = false;
        }

        private void Shoot()
        {
            ShootFromPool(_normalProjectilePool,
                _playerStats.attackSpeed,
                _playerStats.attackDamage,
                _playerStats.knockBackPower);
            PlayShootSfx();
        }

        private void ChargeShot()
        {
            ShootFromPool(_chargedProjectilePool,
                _playerStats.chargeShotSpeed,
                _playerStats.chargeShotDamage,
                _playerStats.chargeShotKnockBackPower);
            PlayChargeShotSfx();
        }

        private void ShootFromPool(ProjectilePool pool, float speed, float damage, float knockbackForce)
        {
            if (pool == null) return;

            ShootProjectile projectile = pool.GetProjectile(_muzzle.position, _muzzle.rotation);

            if (projectile == null) return;
            projectile.SetStats(speed, damage, knockbackForce);

            var bulletPolarity = projectile.GetComponent<PolarityComponent>();
            if (bulletPolarity != null) bulletPolarity.SetPolarity(_polarity.CurrentPolarity);

            _lastShotTime = Time.time;
        }

        private AudioClip GetRandomShootSound()
        {
            if (shootSounds != null && shootSounds.Length > 0)
            {
                return shootSounds[Random.Range(0, shootSounds.Length)];
            }

            return shootSound;
        }

        private void PlayShootSfx()
        {
            AudioClip clip = GetRandomShootSound();

            if (playShootSoundAs2D)
            {
                AudioHandler.Play2DSound(clip, shootSoundVolume);
                return;
            }

            AudioHandler.Play3DSound(clip, transform.position, shootSoundVolume);
        }

        private void StartChargeLoop()
        {
            if (chargeSound == null) return;

            if (chargeLoopSource == null)
                chargeLoopSource = CreateLoopSource("Charge Shot Loop Audio");

            chargeLoopSource.clip = chargeSound;
            chargeLoopSource.volume = chargeSoundVolume;
            chargeLoopSource.loop = true;
            chargeLoopSource.spatialBlend = playChargeShotSoundsAs2D ? 0f : 1f;
            chargeLoopSource.outputAudioMixerGroup = AudioHandler.DefaultSfxMixerGroup;
            chargeLoopSource.pitch = GetChargePitch();
            chargeLoopSource.Play();
        }

        private void StopChargeLoop()
        {
            if (chargeLoopSource == null) return;

            if (chargeLoopSource.isPlaying && chargeLoopSource.clip == chargeSound)
                chargeLoopSource.Stop();

            chargeLoopSource.loop = false;
            chargeLoopSource.pitch = 1f;
        }

        private void StartHeldChargeLoop()
        {
            if (heldChargeSound == null) return;

            if (heldChargeLoopSource == null)
                heldChargeLoopSource = CreateLoopSource("Held Charge Shot Loop Audio");

            heldChargeLoopSource.clip = heldChargeSound;
            heldChargeLoopSource.volume = heldChargeSoundVolume;
            heldChargeLoopSource.loop = true;
            heldChargeLoopSource.spatialBlend = playChargeShotSoundsAs2D ? 0f : 1f;
            heldChargeLoopSource.outputAudioMixerGroup = AudioHandler.DefaultSfxMixerGroup;
            heldChargeLoopSource.Play();
        }

        private void StopHeldChargeLoop()
        {
            if (heldChargeLoopSource == null) return;

            if (heldChargeLoopSource.isPlaying && heldChargeLoopSource.clip == heldChargeSound)
                heldChargeLoopSource.Stop();

            heldChargeLoopSource.loop = false;
        }

        private void CancelCharge(bool playCancelSound)
        {
            if (!_isCharging) return;

            StopChargeLoop();
            StopHeldChargeLoop();
            StopVibration();
            _lastShotTime = Time.time;

            if (playCancelSound)
            {
                PlayChargeCancelSfx();
            }

            _isCharging = false;
            _chargeReady = false;
        }

        private void PlayChargeCancelSfx()
        {
            if (playChargeShotSoundsAs2D)
            {
                AudioHandler.Play2DSound(chargeCancelSound, chargeCancelSoundVolume);
                return;
            }

            AudioHandler.Play3DSound(chargeCancelSound, transform.position, chargeCancelSoundVolume);
        }

        private void PlayChargeShotSfx()
        {
            if (playChargeShotSoundsAs2D)
            {
                AudioHandler.Play2DSound(chargedShotSound, chargedShotSoundVolume);
                return;
            }

            AudioHandler.Play3DSound(chargedShotSound, transform.position, chargedShotSoundVolume);
        }

        private AudioSource CreateLoopSource(string sourceName)
        {
            GameObject sourceObject = new GameObject(sourceName);
            sourceObject.transform.SetParent(transform);
            sourceObject.transform.localPosition = Vector3.zero;

            AudioSource source = sourceObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            return source;
        }
    }
}