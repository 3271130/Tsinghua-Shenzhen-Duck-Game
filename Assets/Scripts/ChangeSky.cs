using UnityEngine;
using System.Collections;

public class ToggleSkyboxOnTrigger : MonoBehaviour
{
    public Material skybox1;
    public GameObject particlesRain;

    private Coroutine exposureCoroutine;
    private float defaultExposure = 1.53f;
    private float triggeredExposure = 0.5f;
    private float transitionDuration = 2.0f;

    private void Start()
    {
        if (skybox1 == null)
        {
            Debug.LogError("Skybox1 Material not existing!");
            return;
        }

        RenderSettings.skybox = skybox1;
        skybox1.SetFloat("_Exposure", defaultExposure);

        if (particlesRain == null)
        {
            particlesRain = GameObject.Find("ParticlesRain");
        }

        if (particlesRain != null)
        {
            particlesRain.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Enter Trigger");

            StartExposureTransition(triggeredExposure);

            if (particlesRain != null)
            {
                particlesRain.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Exit Trigger");

            StartExposureTransition(defaultExposure);

            if (particlesRain != null)
            {
                particlesRain.SetActive(false);
            }
        }
    }

    private void StartExposureTransition(float targetExposure)
    {
        if (exposureCoroutine != null)
        {
            StopCoroutine(exposureCoroutine);
        }

        exposureCoroutine = StartCoroutine(TransitionExposure(targetExposure));
    }

    private IEnumerator TransitionExposure(float targetExposure)
    {
        float currentExposure = skybox1.GetFloat("_Exposure"); 
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            float newExposure = Mathf.Lerp(currentExposure, targetExposure, elapsedTime / transitionDuration);
            skybox1.SetFloat("_Exposure", newExposure);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        skybox1.SetFloat("_Exposure", targetExposure);
    }
}
