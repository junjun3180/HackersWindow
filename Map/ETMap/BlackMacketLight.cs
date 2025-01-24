using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BlackMacketLight : MonoBehaviour
{
    public Light2D mapLight;
    public Light2D globalLight;
    public GameObject Capet;
    public GameObject Shadow;

    public float logicInterval;
    public float resetDelay = 7.5f;
    public float targetIntensity = 1.75f;
    public Color MaptargetColor = new Color(0.1898f, 0.2817f, 0.4716f, 1.0f);
    private float originalIntensity;
    private Color originalMapColor;

    public Color GlobaltargetColor = new Color(0.5094f, 0.5094f, 0.5094f, 1.0f);
    private Color originalGlobalColor;
    private Coroutine logicCoroutine;

    private void Start()
    {
        if (GlobalLightManager.Instance != null)
        {
            globalLight = GlobalLightManager.Instance.globalLight;
        }
        else
        {
            Debug.LogError("GlobalLightManager not found!");
        }

        if (mapLight != null)
        {
            originalIntensity = mapLight.intensity;
            originalMapColor = mapLight.color;
        }
        if (globalLight != null)
        {
            originalGlobalColor = globalLight.color;
        }
    }

    private void OnEnable()
    {
        logicCoroutine = StartCoroutine(LogicRoutine());
    }

    private void OnDisable()
    {
        if (logicCoroutine != null)
        {
            StopCoroutine(logicCoroutine);
            logicCoroutine = null;
        }
    }

    private IEnumerator LogicRoutine()
    {
        while (true)
        {
            logicInterval = Random.Range(30f, 90f);
            yield return new WaitForSeconds(logicInterval);
            ActivateLogic();
            yield return new WaitForSeconds(0.1f);
            ResetLogic();
            yield return new WaitForSeconds(0.1f);
            ActivateLogic();
            yield return new WaitForSeconds(0.1f);
            ResetLogic();
            yield return new WaitForSeconds(0.1f);
            ActivateLogic();
            yield return new WaitForSeconds(resetDelay);
            ResetLogic();
            yield return new WaitForSeconds(0.1f);
            ActivateLogic();
            yield return new WaitForSeconds(0.1f);
            ResetLogic();
            yield return new WaitForSeconds(0.1f);
            ActivateLogic();
            yield return new WaitForSeconds(0.1f);
            ResetLogic();
        }
    }

    private void ActivateLogic()
    {
        if (mapLight != null)
        {
            mapLight.intensity = targetIntensity;
            mapLight.color = MaptargetColor;
        }
        if (globalLight != null)
        {
            globalLight.color = GlobaltargetColor;
        }
        if (Capet != null)
        {
            Capet.SetActive(true);
        }
        if (Shadow != null)
        {
            Shadow.SetActive(false);
        }
    }

    private void ResetLogic()
    {
        if (mapLight != null)
        {
            mapLight.intensity = originalIntensity;
            mapLight.color = originalMapColor;
        }
        if (globalLight != null)
        {
            globalLight.color = originalGlobalColor;
        }
        if (Capet != null)
        {
            Capet.SetActive(false);
        }
        if (Shadow != null)
        {
            Shadow.SetActive(true);
        }
    }
}