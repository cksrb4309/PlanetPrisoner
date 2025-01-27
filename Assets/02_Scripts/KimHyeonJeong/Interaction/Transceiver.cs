using UnityEngine;

public class Transceiver : MonoBehaviour
{
    private Renderer objectRenderer;
    private Material objectMaterials;

    //private Color OriginalEmissionColor;

    void Start()
    {
        objectRenderer=GetComponent<Renderer>();
        objectMaterials=objectRenderer.materials[1]; // index = 1 or 2 

        //OriginalEmissionColor = objectMaterials.GetColor("_EmissionColor");
    }

    void Update()
    {
        
    }

    public void TurnOffEmission()
    {
        //objectMaterials.SetColor("_EmissionColor",Color.black);
        objectMaterials.DisableKeyword("_EMISSION");
    }

    public void TurnOnEmission()
    {
        objectMaterials.EnableKeyword("_EMISSION");
    }
}
