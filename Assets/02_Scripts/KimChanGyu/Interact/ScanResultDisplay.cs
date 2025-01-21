using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScanResultDisplay : MonoBehaviour
{
    static Transform cameraTransform = null;

    [SerializeField] CanvasGroup mainGroup;
    [SerializeField] CanvasGroup explainGroup;

    [SerializeField] Image iconImage;

    [SerializeField] Image namePanelImage;
    [SerializeField] TMP_Text nameText;

    [SerializeField] Image explainPanelImage;
    [SerializeField] TMP_Text explainText;


    [SerializeField] AnimationCurve curve;

    [SerializeField] float duration = 2f;

    [SerializeField] Color mainColor = Color.red;

    [SerializeField] bool isExplain = false;

    Coroutine displayCoroutine = null;

    private void Awake()
    {
        if (cameraTransform == null)
        {
            cameraTransform = GameObject.Find("ScanResultCamera").transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isExplain)
            explainGroup.alpha = 1f;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isExplain)
            explainGroup.alpha = 0f;
    }
    private void OnValidate()
    {
        if (iconImage != null)
            iconImage.color = new Color(mainColor.r, mainColor.g, mainColor.b, iconImage.color.a);

        if (namePanelImage != null)
            namePanelImage.color = new Color(mainColor.r, mainColor.g, mainColor.b, namePanelImage.color.a);

        if (nameText != null)
            nameText.color = new Color(mainColor.r, mainColor.g, mainColor.b, nameText.color.a);

        if (explainPanelImage != null)
            explainPanelImage.color = new Color(mainColor.r, mainColor.g, mainColor.b, explainPanelImage.color.a);

        if (explainText != null)
            explainText.color = new Color(mainColor.r, mainColor.g, mainColor.b, explainText.color.a);
    }
    public void OnDisplay()
    {
        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
        }

        displayCoroutine = StartCoroutine(DisplayCoroutine());
    }

    IEnumerator DisplayCoroutine()
    {
        float t = 0;

        for (; t < duration; t += Time.deltaTime)
        {
            //transform.forward = (transform.position - cameraTransform.position);

            transform.rotation = Quaternion.LookRotation(cameraTransform.forward, cameraTransform.up);

            mainGroup.alpha = curve.Evaluate(t);

            yield return null;
        }
        mainGroup.alpha = 0;
    }
}
