using UnityEngine;

namespace PolarityBreach.Enemy
{
    public class EnemyFloat : MonoBehaviour
    {
        [SerializeField] private float amplitude = 0.15f;
        [SerializeField] private float speed = 2f;
        [SerializeField] private bool randomizeStart = true;

        private Vector3 startPosition;
        private float offset;

        private void Start()
        {
            startPosition = transform.localPosition;

            if (randomizeStart)
                offset = Random.Range(0f, Mathf.PI * 2f);
        }

        private void OnEnable()
        {
            if (startPosition != Vector3.zero)
                transform.localPosition = startPosition;
        }

        private void Update()
        {
            float y = Mathf.Sin(Time.time * speed + offset) * amplitude;
            transform.localPosition = startPosition + Vector3.up * y;
        }
    }
}