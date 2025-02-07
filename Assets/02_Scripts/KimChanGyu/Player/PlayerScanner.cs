using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScanner : MonoBehaviour
{
    static PlayerScanner instance = null;

    [SerializeField] InputActionReference scanInputAction;

    [SerializeField] float range;
    [SerializeField] float speed;

    [SerializeField] Transform scanEffectTransform;
    [SerializeField] Transform cameraTransform;

    [SerializeField] Material effectMaterial;

    [SerializeField] LayerMask layerMask;

    bool isEnabled = false;

    Coroutine scanCoroutine = null;
    private void Awake()
    {
        scanEffectTransform.parent = null;

        instance = this;
    }
    private void OnEnable()
    {
        scanInputAction.action.Enable();

        NextDayController.Subscribe(EnableScanner, ActionType.FirstGameFinished);
        NextDayController.Subscribe(EnableScanner, ActionType.NextDayFinished);
        NextDayController.Subscribe(EnableScanner, ActionType.SurviveFinished);

        NextDayController.Subscribe(DisableScanner, ActionType.NextDayReady);
        NextDayController.Subscribe(DisableScanner, ActionType.OnPlayerDie);
    }
    private void OnDisable()
    {
        scanInputAction.action.Disable();

        NextDayController.Unsubscribe(EnableScanner, ActionType.FirstGameFinished);
        NextDayController.Unsubscribe(EnableScanner, ActionType.NextDayFinished);
        NextDayController.Unsubscribe(EnableScanner, ActionType.SurviveFinished);

        NextDayController.Unsubscribe(DisableScanner, ActionType.NextDayReady);
        NextDayController.Unsubscribe(DisableScanner, ActionType.OnPlayerDie);
    }
    void EnableScanner() => isEnabled = true;
    void DisableScanner() => isEnabled = false;
    private void Update()
    {
        if (!isEnabled) return;

        if (scanInputAction.action.WasPressedThisFrame())
        {
            if (scanCoroutine == null)
            {
                Scan(range);
            }
        }
    }
    public void Scan(float range)
    {
        if (scanCoroutine != null)
        {
            StopCoroutine(scanCoroutine);
        }

        scanCoroutine = StartCoroutine(ScanCoroutine(range));
    }
    IEnumerator ScanCoroutine(float range)
    {
        PlayerAudioController.PlayerAudioPlay(AudioName.PlayerScan);

        scanEffectTransform.position = cameraTransform.position;

        yield return null;

        Collider[] searchColliders = Physics.OverlapSphere(scanEffectTransform.position, range, layerMask);

        List<(float, Collider)> distances = new List<(float, Collider)>();

        for (int i = 0; i < searchColliders.Length; i++)
        {
            distances.Add((Vector3.Distance(searchColliders[i].transform.position, scanEffectTransform.position), searchColliders[i]));
        }

        distances.Sort((a, b) => a.Item1.CompareTo(b.Item1));

        int currIndex = 0;

        for (float currRange = 0.01f; currRange < range; currRange += Time.deltaTime * speed)
        {
            for (; currIndex < distances.Count; currIndex++)
            {
                if (currRange < distances[currIndex].Item1) break;

                Debug.Log(distances[currIndex].Item2.gameObject.name);
                Debug.Log(distances[currIndex].Item2.GetComponentInChildren<ScanResultDisplay>() == null ? "NULL" : "Not NULL");

                distances[currIndex].Item2.GetComponentInChildren<ScanResultDisplay>().OnDisplay();
            }
            effectMaterial.SetFloat("_Alpha", 1 - (currRange / range));

            scanEffectTransform.localScale = currRange * 2f * Vector3.one;

            yield return null;
        }
        effectMaterial.SetFloat("_Alpha", 0);

        yield return null;

        scanCoroutine = null;
    }
}
