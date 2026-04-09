using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// SPUM 프리팹을 오프스크린에서 캡처해 RenderTexture로 반환하는 싱글톤.
/// 반환된 RenderTexture는 사용 후 Release() 책임은 호출자(GachaResultUI)에게 있음.
/// </summary>
public class GachaPreviewCapture : MonoBehaviour
{
    public static GachaPreviewCapture Instance { get; private set; }

    [Header("캡처 설정")]
    [Tooltip("캡처용 카메라 (비워두면 자동 생성)")]
    [SerializeField] Camera previewCamera;
    [Tooltip("SPUM 캐릭터 크기에 맞춰 조정 (기본 1.2)")]
    [SerializeField] float orthographicSize = 1.2f;
    [SerializeField] Color backgroundColor = new Color(0.08f, 0.06f, 0.12f, 1f);

    private const int RT_SIZE = 256;
    private static readonly Vector3 PreviewPos = new Vector3(-1000f, 0f, 0f);

    private RenderTexture _sharedRT;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        InitCamera();
    }

    void InitCamera()
    {
        if (previewCamera == null)
        {
            var go = new GameObject("_GachaPreviewCam");
            go.transform.SetParent(transform);
            previewCamera = go.AddComponent<Camera>();
        }

        previewCamera.orthographic     = true;
        previewCamera.orthographicSize = orthographicSize;
        previewCamera.transform.position = PreviewPos + Vector3.back * 10f;
        previewCamera.clearFlags       = CameraClearFlags.SolidColor;
        previewCamera.backgroundColor  = backgroundColor;
        previewCamera.nearClipPlane    = 0.1f;
        previewCamera.farClipPlane     = 100f;
        previewCamera.enabled          = false; // 평소엔 비활성

        _sharedRT = new RenderTexture(RT_SIZE, RT_SIZE, 24, RenderTextureFormat.ARGB32);
        previewCamera.targetTexture = _sharedRT;
    }

    /// <summary>
    /// SPUM 프리팹을 오프스크린에서 렌더 후 스냅샷 RenderTexture 반환.
    /// onComplete(null) 이면 캡처 실패.
    /// </summary>
    public IEnumerator CaptureUnit(GameObject spumPrefab, Action<RenderTexture> onComplete)
    {
        if (spumPrefab == null) { onComplete?.Invoke(null); yield break; }

        // 오프스크린에 스폰 (Animator 비활성 – 정적 포즈 캡처)
        var instance = Instantiate(spumPrefab, PreviewPos, Quaternion.identity);
        var anim = instance.GetComponentInChildren<Animator>();
        if (anim != null) anim.enabled = false;

        // SpriteRenderer 초기화 대기 (1 프레임)
        yield return null;

        // URP가 이 카메라를 렌더링하도록 활성화 후 EndOfFrame 대기
        previewCamera.enabled = true;
        yield return new WaitForEndOfFrame();
        previewCamera.enabled = false;

        // 공유 RT → 개별 스냅샷 RT 복사
        var snapshot = new RenderTexture(RT_SIZE, RT_SIZE, 0, RenderTextureFormat.ARGB32);
        Graphics.Blit(_sharedRT, snapshot);

        Destroy(instance);
        onComplete?.Invoke(snapshot);
    }

    void OnDestroy()
    {
        if (_sharedRT != null) _sharedRT.Release();
    }
}
