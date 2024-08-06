using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class Utilities : MonoBehaviour
{
    public static Utilities Instance { get; private set; }

    private Coroutine ControllerShakeCoroutine;
    private Coroutine ControllerFlashCoroutine;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            Debug.LogError("More than one instance of Utilities in scene." +
                "\n Located on " + this.name + " and " + Instance.name);
        }

        Instance = this;
    }

    public void ShakeCamera(float intensity = 0.5f)
    {
        if (TryGetComponent<CinemachineImpulseSource>(out CinemachineImpulseSource impulseSource))
        {
            impulseSource.GenerateImpulseWithForce(intensity);
        }
    }

    public void ShakeController(float low = 0.5f, float high = 0.5f, float timeTillStop = 0.1f)
    {
        if (ControllerShakeCoroutine != null) StopCoroutine(ControllerShakeCoroutine);

        ControllerShakeCoroutine = StartCoroutine(ShakingController(low, high, timeTillStop));

    }

    private IEnumerator ShakingController(float low, float high, float timeTillStop)
    {
        Gamepad.current.SetMotorSpeeds(low, high);

        float initLow = low;
        float initHigh = high;

        float shakeTimer = timeTillStop;
        while (shakeTimer > 0) 
        {
            shakeTimer -= Time.deltaTime;

            low = initLow * (shakeTimer / timeTillStop);
            high = initHigh * (shakeTimer / timeTillStop);

            Gamepad.current.SetMotorSpeeds(low, high);

            yield return null;
        }

        Gamepad.current.SetMotorSpeeds(0, 0);
       

        yield break;
    }

    public void FlashControllerColor(Color flashColor, float flashTime = 0.5f)
    {
        if (ControllerFlashCoroutine != null) StopCoroutine(ControllerFlashCoroutine);

        ControllerFlashCoroutine = StartCoroutine(FlashingControllerColor(flashColor, flashTime));
    }

    private IEnumerator FlashingControllerColor(Color flashColor, float flashTime)
    {
        if (Gamepad.current is not DualShockGamepad psController) yield break;

        Color initColor = flashColor;
        psController.SetLightBarColor(initColor);

        float elapsedTime = 0f;

        while (elapsedTime < flashTime)
        {
            elapsedTime += Time.deltaTime;
            flashColor = Color.Lerp(initColor, Color.clear, elapsedTime / flashTime);
            psController.SetLightBarColor(flashColor);
            yield return null;
        }

        yield break;
    }
}
