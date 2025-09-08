using System;
using UnityEngine;

public class CanvasLightSetup : MonoBehaviour
{
    private void Start()
    {
        GameObject uiLight = new GameObject("UI Light");
        Light light = uiLight.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1f;
        light.color = Color.white;
        light.cullingMask = 1 << LayerMask.NameToLayer("UI"); // Seulement l'UI

        uiLight.transform.rotation = Quaternion.Euler(50, -30, 0);
    }
}
