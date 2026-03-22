using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class VirtualJoystick : MonoBehaviour
{
    [Header("조이스틱 반경 (픽셀)")]
    public float handleRange = 80f;

    public Vector2 Direction { get; private set; }

    RectTransform _bg;
    RectTransform _handle;

    bool    _active;
    int     _touchId = -1;
    Vector2 _origin;

    void Awake()
    {
        _bg     = GetComponent<RectTransform>();
        _handle = transform.GetChild(0).GetComponent<RectTransform>();
        EnhancedTouchSupport.Enable();
    }

    void OnDestroy()
    {
        EnhancedTouchSupport.Disable();
    }

    void Update()
    {
        _origin = GetScreenCenter();

#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouse();
#endif
        HandleTouch();
    }

    // ── 마우스 (PC 테스트용) ──────────────────────────────────
    void HandleMouse()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        Vector2 mPos = mouse.position.ReadValue();

        if (mouse.leftButton.wasPressedThisFrame && IsInsideBackground(mPos))
            _active = true;

        if (_active)
        {
            if (mouse.leftButton.isPressed)
                Apply(mPos);
            else
                Release();
        }
    }

    // ── 터치 (모바일) ─────────────────────────────────────────
    void HandleTouch()
    {
        foreach (var t in Touch.activeTouches)
        {
            Vector2 pos = t.screenPosition;

            if (t.phase == UnityEngine.InputSystem.TouchPhase.Began
                && _touchId == -1 && IsInsideBackground(pos))
            {
                _touchId = t.touchId;
                _active  = true;
            }

            if (_active && t.touchId == _touchId)
            {
                if (t.phase == UnityEngine.InputSystem.TouchPhase.Ended
                    || t.phase == UnityEngine.InputSystem.TouchPhase.Canceled)
                    Release();
                else
                    Apply(pos);
            }
        }

        if (Touch.activeTouches.Count == 0 && _touchId != -1)
            Release();
    }

    // ── 공통 ──────────────────────────────────────────────────
    void Apply(Vector2 screenPos)
    {
        // 핸들이 배경 원 안에서만 움직이도록 최대 범위를 크기 기반으로 계산
        float scale    = GetCanvasScale();
        float maxDist  = (_bg.sizeDelta.x * 0.5f - _handle.sizeDelta.x * 0.5f) * scale;

        Vector2 delta   = screenPos - _origin;
        Vector2 clamped = Vector2.ClampMagnitude(delta, maxDist);
        _handle.anchoredPosition = clamped / scale;
        Direction = clamped / maxDist;
    }

    void Release()
    {
        _active  = false;
        _touchId = -1;
        _handle.anchoredPosition = Vector2.zero;
        Direction = Vector2.zero;
    }

    bool IsInsideBackground(Vector2 screenPos)
    {
        float radius = (_bg.sizeDelta.x * 0.5f) * GetCanvasScale();
        return (screenPos - _origin).sqrMagnitude <= radius * radius;
    }

    Vector2 GetScreenCenter()
    {
        Vector3[] corners = new Vector3[4];
        _bg.GetWorldCorners(corners);
        return (corners[0] + corners[2]) * 0.5f;
    }

    float GetCanvasScale()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        return canvas != null ? canvas.scaleFactor : 1f;
    }
}
