using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("타겟")]
    public Transform target;

    [Header("부드러움 (높을수록 즉각 반응)")]
    public float smoothSpeed = 5f;

    [Header("맵 크기 (타일 단위)")]
    public float mapWidth  = 40f;
    public float mapHeight = 30f;

    Camera _cam;

    void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 플레이어 위치로 이동
        Vector3 desired = new Vector3(target.position.x, target.position.y, transform.position.z);
        Vector3 smoothed = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);

        // 카메라 뷰 반절 크기 계산
        float halfH = _cam.orthographicSize;
        float halfW = halfH * _cam.aspect;

        // 맵 밖을 비추지 않도록 클램프
        smoothed.x = Mathf.Clamp(smoothed.x, halfW,          mapWidth  - halfW);
        smoothed.y = Mathf.Clamp(smoothed.y, halfH,          mapHeight - halfH);

        transform.position = smoothed;
    }
}
