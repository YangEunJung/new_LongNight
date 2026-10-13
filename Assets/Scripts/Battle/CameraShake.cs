using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;
    private Vector3 originalPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
    }

    public void Shake(float duration, float magnitude)
    {
        // 흔들기 직전의 원래 카메라 위치를 저장합니다.
        originalPos = transform.localPosition;
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // 지정한 강도(magnitude) 안에서 랜덤한 x, y 좌표를 뽑아냅니다.
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            // 타임스케일 0.1의 영향을 받지 않는 실제 현실 시간으로 더해줍니다.
            elapsed += Time.unscaledDeltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 지정된 시간이 끝나면 카메라를 원래 위치로 복구합니다.
        transform.localPosition = originalPos;
    }
}
