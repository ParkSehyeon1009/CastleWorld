using UnityEngine;

/// <summary>
/// Android 환경 전용 카메라 초기 설정.
/// 카메라 이동은 별도의 CameraFollow 스크립트에서 처리.
/// </summary>
[RequireComponent(typeof(Camera))]
public class AndroidCameraSetup : MonoBehaviour
{
    [Tooltip("모바일 화면에서 보여줄 세로 타일 수의 절반 (orthographicSize)")]
    public float orthographicSize = 7f;

    void Awake()
    {
        Camera cam = GetComponent<Camera>();
        cam.orthographic     = true;
        cam.orthographicSize = orthographicSize;
        cam.nearClipPlane    = 0.3f;
        cam.farClipPlane     = 100f;

#if UNITY_ANDROID
        Screen.orientation  = ScreenOrientation.LandscapeLeft;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif
        QualitySettings.antiAliasing = 0;  // 픽셀 아트 선명도
        Application.targetFrameRate  = 60;
    }
}
