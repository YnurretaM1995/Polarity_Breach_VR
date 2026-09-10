using UnityEngine;
using PolarityBreach.UI;

public class VRSimplePause : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Transform head;
    [SerializeField] private MonoBehaviour[] scriptsToDisable;

    [Header("Posición")]
    [SerializeField] private float distance = 2f;
    [SerializeField] private float panelHeight = 1.5f;

    public static bool IsPaused { get; private set; }

    private bool isPaused;

    private void Update()
    {
        if (UIQueue.IsBlocking) return;

        if (OVRInput.GetDown(OVRInput.Button.Start))
            Toggle();
    }

    private void Toggle()
    {
        isPaused = !isPaused;
        IsPaused = isPaused;

        if (isPaused) PlaceInFront();

        if (pausePanel != null)
            pausePanel.SetActive(isPaused);

        for (int i = 0; i < scriptsToDisable.Length; i++)
        {
            if (scriptsToDisable[i] != null)
                scriptsToDisable[i].enabled = !isPaused;
        }

        Time.timeScale = isPaused ? 0f : 1f;
        AudioListener.pause = isPaused;
    }

    private void PlaceInFront()
    {
        if (pausePanel == null || head == null) return;

        Vector3 forward = head.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.001f) return;
        forward.Normalize();

        Vector3 position = head.position + forward * distance;
        position.y = panelHeight;

        pausePanel.transform.position = position;
        pausePanel.transform.rotation = Quaternion.LookRotation(forward);
    }

    private void OnDisable()
    {
        if (!isPaused) return;

        isPaused = false;
        IsPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }
}