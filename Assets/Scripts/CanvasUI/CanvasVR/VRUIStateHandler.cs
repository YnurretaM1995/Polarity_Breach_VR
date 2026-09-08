using UnityEngine;
using PolarityBreach.Player;
using PolarityBreach.UI;

public class VRUIStateHandler : MonoBehaviour
{
    [Header("Ocultar durante menús")]
    [SerializeField] private GameObject weapon;
    [SerializeField] private GameObject reticle;
    [SerializeField] private VRAimReticle aimScript;

    private bool lastState;

    private void Start()
    {
        lastState = !IsMenuOpen();
        Apply(IsMenuOpen());
    }

    private void Update()
    {
        bool menuOpen = IsMenuOpen();

        if (menuOpen == lastState) return;

        Apply(menuOpen);
        lastState = menuOpen;
    }

    private bool IsMenuOpen()
    {
        return PauseMenu.IsPaused || UIQueue.IsBlocking || LevelUpMenu.IsOpen;
    }

    private void Apply(bool menuOpen)
    {
        if (weapon != null) weapon.SetActive(!menuOpen);
        if (reticle != null) reticle.SetActive(!menuOpen);
        if (aimScript != null) aimScript.enabled = !menuOpen;
    }
}