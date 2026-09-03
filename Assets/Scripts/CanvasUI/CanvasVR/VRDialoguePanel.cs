using UnityEngine;

public class VRDialoguePanel : MonoBehaviour
{
    [SerializeField] private Transform head;
    [SerializeField] private float distance = 2f;
    [SerializeField] private float panelHeight = 2.5f;

    [Header("Seguimiento")]
    [SerializeField] private float deadZoneAngle = 25f;
    [SerializeField] private float followSpeed = 2.5f;
    [SerializeField] private float stopAngle = 3f;

    private bool isFollowing;

    public void PlaceInFrontOfPlayer()
    {
        if (head == null) return;

        Vector3 forward = GetFlatForward();
        if (forward == Vector3.zero) return;

        transform.position = GetTargetPosition(forward);
        transform.rotation = Quaternion.LookRotation(forward);
        isFollowing = false;
    }

    private void LateUpdate()
    {
        if (head == null) return;
        if (!gameObject.activeInHierarchy) return;

        Vector3 forward = GetFlatForward();
        if (forward == Vector3.zero) return;

        Vector3 toPanel = transform.position - head.position;
        toPanel.y = 0f;
        if (toPanel.sqrMagnitude < 0.001f) return;

        float angle = Vector3.Angle(forward, toPanel.normalized);

        if (angle > deadZoneAngle) isFollowing = true;
        else if (angle < stopAngle) isFollowing = false;

        if (!isFollowing) return;

        Vector3 target = GetTargetPosition(forward);
        float t = followSpeed * Time.unscaledDeltaTime;

        transform.position = Vector3.Lerp(transform.position, target, t);

        Vector3 lookDir = transform.position - head.position;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(lookDir.normalized);
    }

    private Vector3 GetFlatForward()
    {
        Vector3 forward = head.forward;
        forward.y = 0f;
        return forward.sqrMagnitude < 0.001f ? Vector3.zero : forward.normalized;
    }

    private Vector3 GetTargetPosition(Vector3 forward)
    {
        Vector3 position = head.position + forward * distance;
        position.y = panelHeight;
        return position;
    }
}