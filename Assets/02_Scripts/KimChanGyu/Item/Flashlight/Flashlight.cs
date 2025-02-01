using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Flashlight : Item
{
    bool isFlashlightOn = false;
    Light spotLight = null;

    private void Awake()
    {
        spotLight = GetComponentInChildren<Light>();

        spotLight.enabled = false;
    }

    public void ToggleFlashlight()
    {
        Debug.Log("ToggleFlashlight");

        spotLight.enabled = !isFlashlightOn;
        
        isFlashlightOn = !isFlashlightOn;


        Debug.Log("LightEnabled : " + spotLight.enabled.ToString() + " / TriggerBool : " + isFlashlightOn.ToString());
    }
}
