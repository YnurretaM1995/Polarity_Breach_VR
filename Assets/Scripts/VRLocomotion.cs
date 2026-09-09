using System.Collections;
using UnityEngine;
using PolarityBreach.Player;

public class VRLocomotion : MonoBehaviour
{
    [Header("Referencias")]
    public Transform centerEye;
    public CharacterController controller;
    public PlayerStatsData playerStats;

    [Header("Movimiento")]
    public float speed = 4f;
    public float gravity = -9.81f;

    [Header("Dash")]
    public AbilityUIDisplay dashUI;
    public AudioSource dashSfxSource;
    public AudioClip dashSound;

    private float verticalVelocity;
    private bool isDashing;
    private bool canDash = true;
    private Vector3 dashDirection;
    private Vector2 lastInput;

    void Update()
    {
        Vector2 input = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        lastInput = input;

        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick))
            TryDash();

        Vector3 forward = centerEye.forward;
        Vector3 right = centerEye.right;
        forward.y = 0f;
        right.y = 0f;

        if (forward.sqrMagnitude < 0.001f) return;

        forward.Normalize();
        right.Normalize();

        Vector3 move;

        if (isDashing)
        {
            float dashSpeed = playerStats != null ? playerStats.dashSpeed : speed * 3f;
            move = dashDirection * dashSpeed;
        }
        else
        {
            float currentSpeed = playerStats != null ? playerStats.CurrentMovementSpeed : speed;
            move = (forward * input.y + right * input.x) * currentSpeed;
        }

        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -1f;
        else
            verticalVelocity += gravity * Time.deltaTime;

        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }

    private void TryDash()
    {
        if (playerStats == null || !playerStats.dashUnlocked) return;
        if (!canDash || isDashing) return;

        Vector3 forward = centerEye.forward;
        Vector3 right = centerEye.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        if (lastInput.sqrMagnitude > 0.01f)
            dashDirection = (forward * lastInput.y + right * lastInput.x).normalized;
        else
            dashDirection = forward;

        StartCoroutine(DashRoutine());

        if (dashSfxSource != null && dashSound != null)
            dashSfxSource.PlayOneShot(dashSound);

        if (dashUI != null) dashUI.StartCooldownUI();
    }

    private IEnumerator DashRoutine()
    {
        canDash = false;
        isDashing = true;
        yield return new WaitForSeconds(playerStats.dashDuration);
        isDashing = false;
        yield return new WaitForSeconds(playerStats.dashCooldown);
        canDash = true;
    }
}