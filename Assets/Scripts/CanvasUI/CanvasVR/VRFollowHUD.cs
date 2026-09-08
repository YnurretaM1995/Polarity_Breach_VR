using UnityEngine;

public class VRFollowHUD : MonoBehaviour
{
    [SerializeField] private Transform head;
    [SerializeField] private float distance = 2f;
    [SerializeField] private float heightOffset = -0.3f;
    [SerializeField] private float followSpeed = 4f;
    [SerializeField] private float deadZoneAngle = 25f;
    [SerializeField] private float distanceTolerance = 0.4f;

    private bool isFollowing;

    private void LateUpdate()
    {
        if (head == null) return;

        Vector3 headForward = head.forward;
        headForward.y = 0f;
        if (headForward.sqrMagnitude < 0.001f) return;
        headForward.Normalize();

        Vector3 toPanel = transform.position - head.position;
        toPanel.y = 0f;

        float currentDistance = toPanel.magnitude;

        float angle = currentDistance > 0.001f
            ? Vector3.Angle(headForward, toPanel.normalized)
            : 0f;

        float distanceError = Mathf.Abs(currentDistance - distance);

        if (angle > deadZoneAngle || distanceError > distanceTolerance)
            isFollowing = true;
        else if (angle < 2f && distanceError < 0.1f)
            isFollowing = false;

        Vector3 target = head.position + headForward * distance + Vector3.up * heightOffset;

        if (isFollowing)
            transform.position = Vector3.Lerp(transform.position, target, followSpeed * Time.unscaledDeltaTime);

        transform.rotation = Quaternion.LookRotation(transform.position - head.position);
    }
}