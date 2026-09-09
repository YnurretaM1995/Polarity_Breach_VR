using UnityEngine;

public class VRAimReticle : MonoBehaviour
{
    public Transform muzzle;
    public Transform reticle;
    public float maxDistance = 50f;
    public LayerMask hitMask = ~0;
    public float sizeAtOneMeter = 0.02f;

    void Update()
    {
        Vector3 point;

        if (Physics.Raycast(muzzle.position, muzzle.forward, out RaycastHit hit, maxDistance, hitMask))
            point = hit.point;
        else
            point = muzzle.position + muzzle.forward * maxDistance;

        reticle.position = point;

        //float distance = Vector3.Distance(muzzle.position, point);
        //reticle.localScale = Vector3.one * sizeAtOneMeter * distance;
    }
}