using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class SetNativeSizeHelper
{
    [MenuItem("Tools/Set Native Size - PoolNameText")]
public static void SetNativeSize() { var all = Resources.FindObjectsOfTypeAll<UnityEngine.UI.Image>(); foreach (var img in all) { if (img.gameObject.name == "PoolNameText") { img.SetNativeSize(); EditorUtility.SetDirty(img.gameObject); Debug.Log($"[SetNativeSizeHelper] SetNativeSize 완료: {img.rectTransform.sizeDelta}"); return; } } Debug.LogError("PoolNameText Image를 찾을 수 없습니다."); }


[MenuItem("Tools/Import Wood Card As Sprite")] public static void ImportWoodCardAsSprite() { string path = "Assets/Image/Wood Card Image.png"; var importer = UnityEditor.AssetImporter.GetAtPath(path) as UnityEditor.TextureImporter; if (importer == null) { Debug.LogError("TextureImporter not found"); return; } importer.textureType = UnityEditor.TextureImporterType.Sprite; importer.spriteImportMode = UnityEditor.SpriteImportMode.Single; UnityEditor.AssetDatabase.ImportAsset(path, UnityEditor.ImportAssetOptions.ForceUpdate); Debug.Log("[SetNativeSizeHelper] Wood Card Image → Sprite 변환 완료"); }


[MenuItem("Tools/Apply Wood Card Sprite")] public static void ApplyWoodCardSprite() { var sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Image/Wood Card Image.png"); if (sprite == null) { Debug.LogError("Sprite 로드 실패"); return; } var go = GameObject.Find("GachaCard"); if (go == null) { Debug.LogError("GachaCard를 찾을 수 없습니다."); return; } var img = go.GetComponent<UnityEngine.UI.Image>(); if (img == null) { Debug.LogError("Image 컴포넌트 없음"); return; } img.sprite = sprite; img.SetNativeSize(); EditorUtility.SetDirty(go); Debug.Log($"[SetNativeSizeHelper] Wood Card 스프라이트 적용 완료: {img.rectTransform.sizeDelta}"); }
}
