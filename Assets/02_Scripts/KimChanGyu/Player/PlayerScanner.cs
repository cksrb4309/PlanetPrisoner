using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScanner : MonoBehaviour
{
    [SerializeField] InputActionReference scanInputAction;

    [SerializeField] float range;
    [SerializeField] float speed;
    [SerializeField] float cooltime;

    [SerializeField] Transform scanEffectTransform;

    [SerializeField] Material effectMaterial;

    [SerializeField] LayerMask layerMask;

    Coroutine cooltimeCoroutine = null;
    Coroutine scanCoroutine = null;

    private void OnEnable()
    {
        scanInputAction.action.Enable();
    }
    private void OnDisable()
    {
        scanInputAction.action.Disable();
    }
    private void Update()
    {
        if (scanInputAction.action.WasPressedThisFrame())
        {
            if (cooltimeCoroutine == null)
            {
                cooltimeCoroutine = StartCoroutine(CooltimeCoroutine());

                Scan(range);
            }
        }
    }
    IEnumerator CooltimeCoroutine()
    {
        yield return new WaitForSeconds(cooltime);

        cooltimeCoroutine = null;
    }
    public void Scan(float range)
    {
        if (scanCoroutine != null) StopCoroutine(scanCoroutine);

        scanCoroutine = StartCoroutine(ScanCoroutine(range));
    }
    IEnumerator ScanCoroutine(float range)
    {
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

                distances[currIndex].Item2.GetComponentInChildren<ScanResultDisplay>().OnDisplay();
            }

            effectMaterial.SetFloat("_Alpha", 1 - (currRange / range));

            scanEffectTransform.localScale = currRange * 2f * Vector3.one;

            yield return null;
        }

        effectMaterial.SetFloat("_Alpha", 0);
    }
}
