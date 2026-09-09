using UnityEngine;

public class VRLocomotion : MonoBehaviour
{
    [Header("Referencias")]
    public Transform centerEye;
    public CharacterController controller;
    public PolarityBreach.Player.PlayerStatsData playerStats;

    [Header("Movimiento")]
    public float speed = 2.5f;
    public float gravity = -9.81f;

    private float verticalVelocity;

    void Update()
    {
        Vector2 input = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

        Vector3 forward = centerEye.forward;
        Vector3 right = centerEye.right;
        forward.y = 0f;
        right.y = 0f;

        if (forward.sqrMagnitude < 0.001f) return;

        forward.Normalize();
        right.Normalize();

        float currentSpeed = playerStats != null ? playerStats.CurrentMovementSpeed : speed;
        Vector3 move = (forward * input.y + right * input.x) * currentSpeed;

        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -1f;
        else
            verticalVelocity += gravity * Time.deltaTime;

        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }
}