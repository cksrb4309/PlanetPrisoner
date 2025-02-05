using System.Collections;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField] CanvasGroup interactableGroup;
    [SerializeField] CanvasGroup alphaControlGroup;

    [SerializeField] float gameStartDelay = 1f;
    [SerializeField] float fadeSpeed = 5f;

    bool isSceneMove = false;

    public void Start()
    {
        StartCoroutine(GameStartDelayCoroutine());
    }
    IEnumerator GameStartDelayCoroutine()
    {
        interactableGroup.interactable = false;

        alphaControlGroup.alpha = 0;

        yield return new WaitForSeconds(gameStartDelay);

        float alpha = 0;

        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fadeSpeed;

            alphaControlGroup.alpha = alpha;

            yield return null;
        }
        alphaControlGroup.alpha = 1f;

        interactableGroup.interactable = true;
    }
    
    public void SceneLoad()
    {
        if (isSceneMove) return;

        isSceneMove = true;

        NextDayController.SceneStart();
    }
    IEnumerator SceneLoadCoroutine(string sceneName)
    {
        interactableGroup.interactable = false;

        float alpha = 1;

        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeSpeed;

            alphaControlGroup.alpha = alpha;

            yield return null;
        }
        alphaControlGroup.alpha = 0;

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(sceneName);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
