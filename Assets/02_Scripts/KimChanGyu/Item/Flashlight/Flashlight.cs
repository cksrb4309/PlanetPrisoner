using UnityEngine;

public class Flashlight : Item
{
    Light spotLight = null;

    private void Awake()
    {
        spotLight = GetComponentInChildren<Light>();

        spotLight.enabled = false;
    }

    public void ToggleFlashlight()
    {
        spotLight.enabled = !spotLight.enabled;
    }
    public override void DisableInHand()
    {
        base.DisableInHand();

        spotLight.enabled = false;
    }
}
