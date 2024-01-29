using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraShakeManager : Singleton<CameraShakeManager>
{

    new Transform camera;
    Vector3 originalPosition;
    private void Start()
    {
        camera = Camera.main.transform;
        originalPosition = camera.localPosition;
    }

    public void StartShake(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
    }
    public IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            camera.localPosition = new Vector3(x, y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        camera.localPosition = originalPosition;
    }
}