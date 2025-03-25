using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiScript : MonoBehaviour
{
    [SerializeField] Image deathScreenImage;

    public void ToggleDeathScreen(bool desiredImageStatus)
    {
        deathScreenImage.gameObject.SetActive(desiredImageStatus);
    }
}
