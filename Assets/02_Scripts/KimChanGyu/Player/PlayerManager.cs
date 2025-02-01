using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] AnimationCurve fadeCurve;
    [SerializeField] float speed;

    
    public void Start()
    {
        canvasGroup.alpha = 1f;

        StartCoroutine(FadeInOutCoroutine());
    }
    IEnumerator FadeInOutCoroutine()
    {
        yield return new WaitForSeconds(1f);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * speed;

            if (t > 1f) t = 1f;

            canvasGroup.alpha = fadeCurve.Evaluate(t);

            yield return null;
        }
    }
}
