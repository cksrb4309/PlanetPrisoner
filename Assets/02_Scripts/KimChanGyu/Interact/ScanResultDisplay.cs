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

    [SerializeField] float duration = 3f;

    [SerializeField] float testAlpha = 0f;

    [SerializeField] Color mainColor = Color.red;

    [SerializeField] bool isExplain = false;

    Coroutine displayCoroutine = null;

    bool isDisplay = true;

    float t = 0;
    float alpha = 0;

    private void Awake()
    {
        if (cameraTransform == null)
        {
            cameraTransform = GameObject.Find("ScanResultCamera").transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("확 인 " + Random.value.ToString());

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
        if (mainGroup != null) mainGroup.alpha = testAlpha;
        if (explainGroup != null) explainGroup.alpha = testAlpha;

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
    private void OnEnable()
    {
        isDisplay = true;
    }
    public void EnableDisplay() => isDisplay = true;
    public void DisableDisplay() => isDisplay = false;
    public void OnDisplay()
    {
        Debug.Log("OnDisplay A :" + transform.parent.gameObject.name);

        if (!isDisplay) return;

        Debug.Log("OnDisplay B : " + transform.parent.gameObject.name);

        t = 0;

        if (displayCoroutine == null)
        {

            Debug.Log("OnDisplay C : " + transform.parent.gameObject.name);

            displayCoroutine = StartCoroutine(DisplayCoroutine());
        }
    }

    IEnumerator DisplayCoroutine()
    {
        alpha = mainGroup.alpha;

        for (; t < duration; t += Time.deltaTime)
        {
            transform.rotation = Quaternion.LookRotation(cameraTransform.forward, cameraTransform.up);

            alpha = Mathf.Clamp(alpha + Time.deltaTime * 3f, 0.0f, 1.0f);

            mainGroup.alpha = alpha;

            yield return null;

            if (!isDisplay)
            {
                mainGroup.alpha = 0f;

                yield break;
            }
        }

        mainGroup.alpha = 0;

        displayCoroutine = null;

        Debug.Log("OnDisplay D : " + transform.parent.gameObject.name);
    }
}
