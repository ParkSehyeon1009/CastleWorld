using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class JoystickSetup : Editor
{
    [MenuItem("Tools/Setup Virtual Joystick")]
    public static void Setup()
    {
        // ── Canvas ────────────────────────────────────────────
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGO.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
            canvasGO.AddComponent<GraphicRaycaster>();

            GameObject esGO = new GameObject("EventSystem");
            esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGO.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        // ── Joystick Background ───────────────────────────────
        GameObject bgGO = new GameObject("Joystick");
        bgGO.transform.SetParent(canvas.transform, false);

        RectTransform bgRect = bgGO.AddComponent<RectTransform>();
        bgRect.anchorMin = bgRect.anchorMax = bgRect.pivot = new Vector2(0f, 0f);
        bgRect.anchoredPosition = new Vector2(180f, 180f);
        bgRect.sizeDelta = new Vector2(240f, 240f);

        Image bgImg = bgGO.AddComponent<Image>();
        bgImg.color = new Color(1f, 1f, 1f, 0.25f);
        bgImg.sprite = CreateCircleSprite();
        bgImg.raycastTarget = true;

        VirtualJoystick vj = bgGO.AddComponent<VirtualJoystick>();
        vj.handleRange = 80f;

        // ── Joystick Handle ───────────────────────────────────
        GameObject handleGO = new GameObject("Handle");
        handleGO.transform.SetParent(bgGO.transform, false);

        RectTransform handleRect = handleGO.AddComponent<RectTransform>();
        handleRect.anchorMin = handleRect.anchorMax = handleRect.pivot = new Vector2(0.5f, 0.5f);
        handleRect.anchoredPosition = Vector2.zero;
        handleRect.sizeDelta = new Vector2(120f, 120f);

        Image handleImg = handleGO.AddComponent<Image>();
        handleImg.color = new Color(1f, 1f, 1f, 0.6f);
        handleImg.sprite = CreateCircleSprite();
        handleImg.raycastTarget = false;

        // ── Player에 조이스틱 연결 ────────────────────────────
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            PlayerMovement pm = player.GetComponent<PlayerMovement>();
            if (pm != null) pm.joystick = vj;
        }

        EditorUtility.SetDirty(canvas.gameObject);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        Debug.Log("[JoystickSetup] 가상 조이스틱 UI 생성 완료!");
    }

    static Sprite CreateCircleSprite()
    {
        // 유니티 내장 원형 스프라이트 사용
        return Resources.GetBuiltinResource<Sprite>("UI/Skin/Knob.psd");
    }
}
