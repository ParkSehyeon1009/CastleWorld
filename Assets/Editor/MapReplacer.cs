using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapReplacer
{
    [MenuItem("Tools/Replace MainScene Map with SC Demo")]
    public static void ReplaceMap()
    {
        string mainScenePath = "Assets/Scenes/MainScene.unity";
        string demoScenePath = "Assets/Cainos/Pixel Art Top Down - Basic/Scene/SC Demo.unity";

        // 현재 씬 저장
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            Debug.Log("Map replacement cancelled.");
            return;
        }

        // MainScene 로드
        Scene mainScene = EditorSceneManager.OpenScene(mainScenePath, OpenSceneMode.Single);

        // SC Demo 추가 로드
        Scene demoScene = EditorSceneManager.OpenScene(demoScenePath, OpenSceneMode.Additive);

        // SC Demo에서 SCENE 루트 찾기
        GameObject sceneRoot = null;
        foreach (GameObject go in demoScene.GetRootGameObjects())
        {
            if (go.name == "SCENE")
            {
                sceneRoot = go;
                break;
            }
        }

        if (sceneRoot == null)
        {
            Debug.LogError("[MapReplacer] 'SCENE' root not found in SC Demo!");
            EditorSceneManager.CloseScene(demoScene, true);
            return;
        }

        // MainScene의 기존 Grid 삭제
        foreach (GameObject go in mainScene.GetRootGameObjects())
        {
            if (go.name == "Grid")
            {
                Object.DestroyImmediate(go);
                Debug.Log("[MapReplacer] Old 'Grid' removed from MainScene.");
                break;
            }
        }

        // SCENE 오브젝트를 MainScene으로 이동
        SceneManager.MoveGameObjectToScene(sceneRoot, mainScene);
        Debug.Log("[MapReplacer] 'SCENE' moved to MainScene.");

        // SC Demo 씬 닫기 (저장 안 함)
        EditorSceneManager.CloseScene(demoScene, false);

        // MainScene 저장
        EditorSceneManager.SaveScene(mainScene);

        Debug.Log("[MapReplacer] Done! MainScene saved with SC Demo map.");
    }
}
