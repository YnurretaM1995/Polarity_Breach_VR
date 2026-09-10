using System.Collections;
using UnityEngine;

public class VREndTransition : MonoBehaviour
{
    [SerializeField] private VRScreenFade screenFade;
    [SerializeField] private VRLocomotion playerLocomotion;
    [SerializeField] private GameObject[] levelObjects;

    public IEnumerator Play()
    {
        if (playerLocomotion != null)
            playerLocomotion.enabled = false;

        if (screenFade != null)
            yield return screenFade.FadeOut();

        for (int i = 0; i < levelObjects.Length; i++)
        {
            if (levelObjects[i] != null)
                levelObjects[i].SetActive(false);
        }

        if (screenFade != null)
            yield return screenFade.FadeIn();
    }
}