using UnityEngine;
using UnityEditor;

public class CameraFollowSetup : Editor
{
    [MenuItem("Tools/Setup Camera Follow")]
    public static void Setup()
    {
        GameObject camGO = GameObject.FindWithTag("MainCamera");
        if (camGO == null) { Debug.LogError("Main Camera를 찾을 수 없습니다."); return; }

        CameraFollow cf = camGO.GetComponent<CameraFollow>();
        if (cf == null) cf = camGO.AddComponent<CameraFollow>();

        GameObject player = GameObject.Find("Player");
        if (player == null) { Debug.LogError("Player를 찾을 수 없습니다."); return; }

        cf.target = player.transform;

        EditorUtility.SetDirty(camGO);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        Debug.Log("[CameraFollowSetup] 완료: Main Camera → Player 타겟 설정됨");
    }
}
