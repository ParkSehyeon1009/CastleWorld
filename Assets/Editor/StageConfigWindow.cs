using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// 스테이지 설정 에디터 창.
/// 메뉴: Defence > Stage Config Window
/// </summary>
public class StageConfigWindow : EditorWindow
{
    private StageManager _stageManager;
    private Vector2 _scrollPos;
    private int _selectedStage = -1;
    private int _selectedWave = -1;

    [MenuItem("Defence/Stage Config Window")]
    public static void Open()
    {
        var win = GetWindow<StageConfigWindow>("Stage Config");
        win.minSize = new Vector2(500, 600);
        win.Show();
    }

    void OnEnable()
    {
        _stageManager = FindFirstObjectByType<StageManager>();
    }

    void OnGUI()
    {
        // 헤더
        EditorGUILayout.Space(6);
        GUILayout.Label("스테이지 설정", EditorStyles.boldLabel);
        EditorGUILayout.Space(4);

        // StageManager 참조
        var newSM = (StageManager)EditorGUILayout.ObjectField(
            "StageManager", _stageManager, typeof(StageManager), true);
        if (newSM != _stageManager)
        {
            _stageManager = newSM;
            _selectedStage = -1;
            _selectedWave = -1;
        }

        if (_stageManager == null)
        {
            EditorGUILayout.HelpBox("씬에서 StageManager를 찾지 못했습니다.", MessageType.Warning);
            if (GUILayout.Button("씬에서 자동 탐색"))
                _stageManager = FindFirstObjectByType<StageManager>();
            return;
        }

        EditorGUILayout.Space(6);

        // 스테이지 목록 + 편집 패널을 좌우 분할
        EditorGUILayout.BeginHorizontal();
        {
            // ── 왼쪽: 스테이지 목록 ──────────────────────────
            EditorGUILayout.BeginVertical(GUILayout.Width(160));
            GUILayout.Label("스테이지 목록", EditorStyles.boldLabel);

            SerializedObject so = new SerializedObject(_stageManager);
            SerializedProperty stagesProp = so.FindProperty("stages");
            so.Update();

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.ExpandHeight(true));
            for (int i = 0; i < stagesProp.arraySize; i++)
            {
                var stageProp = stagesProp.GetArrayElementAtIndex(i);
                StageData stageAsset = stageProp.objectReferenceValue as StageData;
                string label = stageAsset != null ? $"{i + 1}. {stageAsset.stageName}" : $"{i + 1}. (없음)";

                bool selected = _selectedStage == i;
                GUI.color = selected ? new Color(0.6f, 0.9f, 1f) : Color.white;
                if (GUILayout.Button(label, GUILayout.Height(26)))
                {
                    _selectedStage = i;
                    _selectedWave = -1;
                }
                GUI.color = Color.white;
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(4);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+ 스테이지 추가"))
            {
                so.Update();
                stagesProp.arraySize++;
                stagesProp.GetArrayElementAtIndex(stagesProp.arraySize - 1).objectReferenceValue = null;
                so.ApplyModifiedProperties();
                _selectedStage = stagesProp.arraySize - 1;
            }
            if (_selectedStage >= 0 && _selectedStage < stagesProp.arraySize)
            {
                GUI.color = new Color(1f, 0.5f, 0.5f);
                if (GUILayout.Button("삭제"))
                {
                    so.Update();
                    stagesProp.DeleteArrayElementAtIndex(_selectedStage);
                    so.ApplyModifiedProperties();
                    _selectedStage = Mathf.Clamp(_selectedStage - 1, -1, stagesProp.arraySize - 1);
                }
                GUI.color = Color.white;
            }
            EditorGUILayout.EndHorizontal();

            so.ApplyModifiedProperties();
            EditorGUILayout.EndVertical();

            // ── 오른쪽: 스테이지 상세 편집 ────────────────────
            EditorGUILayout.BeginVertical();
            if (_selectedStage >= 0 && _selectedStage < stagesProp.arraySize)
            {
                DrawStageDetail(_stageManager, _selectedStage);
            }
            else
            {
                EditorGUILayout.HelpBox("왼쪽에서 스테이지를 선택하세요.", MessageType.Info);
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
    }

    void DrawStageDetail(StageManager sm, int stageIdx)
    {
        SerializedObject so = new SerializedObject(sm);
        so.Update();
        var stagesProp = so.FindProperty("stages");
        var stageProp = stagesProp.GetArrayElementAtIndex(stageIdx);
        StageData stageAsset = stageProp.objectReferenceValue as StageData;

        GUILayout.Label($"스테이지 {stageIdx + 1} 편집", EditorStyles.boldLabel);
        EditorGUILayout.Space(4);

        // StageData 에셋 연결
        var newAsset = (StageData)EditorGUILayout.ObjectField(
            "StageData 에셋", stageAsset, typeof(StageData), false);
        if (newAsset != stageAsset)
        {
            stageProp.objectReferenceValue = newAsset;
            so.ApplyModifiedProperties();
            stageAsset = newAsset;
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("새 StageData 생성", GUILayout.Width(160)))
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "StageData 저장 위치", $"Stage{stageIdx + 1}", "asset",
                "StageData 에셋을 저장할 위치를 선택하세요.",
                "Assets/Data/Stages");
            if (!string.IsNullOrEmpty(path))
            {
                var newSD = CreateInstance<StageData>();
                newSD.stageName = $"Stage {stageIdx + 1}";
                newSD.waves = new WaveData[0];
                AssetDatabase.CreateAsset(newSD, path);
                AssetDatabase.SaveAssets();
                stageProp.objectReferenceValue = newSD;
                so.ApplyModifiedProperties();
                stageAsset = newSD;
            }
        }
        EditorGUILayout.EndHorizontal();

        so.ApplyModifiedProperties();

        if (stageAsset == null) return;

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // StageData 직접 편집
        SerializedObject stageSO = new SerializedObject(stageAsset);
        stageSO.Update();

        EditorGUILayout.PropertyField(stageSO.FindProperty("stageName"), new GUIContent("스테이지 이름"));
        EditorGUILayout.Space(6);

        // 웨이브 목록
        SerializedProperty wavesProp = stageSO.FindProperty("waves");
        GUILayout.Label($"웨이브 ({wavesProp.arraySize}개)", EditorStyles.boldLabel);

        for (int w = 0; w < wavesProp.arraySize; w++)
        {
            var waveProp = wavesProp.GetArrayElementAtIndex(w);
            WaveData waveAsset = waveProp.objectReferenceValue as WaveData;
            string wLabel = waveAsset != null ? $"Wave {w + 1}: {waveAsset.name}" : $"Wave {w + 1}: (없음)";

            bool wSelected = _selectedWave == w;
            GUI.color = wSelected ? new Color(1f, 1f, 0.7f) : Color.white;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(wSelected ? "▾ " + wLabel : "▸ " + wLabel, EditorStyles.label))
                _selectedWave = wSelected ? -1 : w;
            GUI.color = Color.white;

            var newWave = (WaveData)EditorGUILayout.ObjectField(
                waveAsset, typeof(WaveData), false, GUILayout.Width(160));
            if (newWave != waveAsset)
                waveProp.objectReferenceValue = newWave;

            // 새 WaveData 생성 버튼
            if (GUILayout.Button("생성", GUILayout.Width(40)))
            {
                string path = EditorUtility.SaveFilePanelInProject(
                    "WaveData 저장", $"Stage{stageIdx + 1}_Wave{w + 1}", "asset",
                    "WaveData를 저장할 위치를 선택하세요.", "Assets/Data/Waves");
                if (!string.IsNullOrEmpty(path))
                {
                    var newWD = CreateInstance<WaveData>();
                    newWD.enemyGroups = new WaveData.EnemyGroup[0];
                    AssetDatabase.CreateAsset(newWD, path);
                    AssetDatabase.SaveAssets();
                    waveProp.objectReferenceValue = newWD;
                }
            }

            GUI.color = new Color(1f, 0.5f, 0.5f);
            if (GUILayout.Button("X", GUILayout.Width(22)))
            {
                wavesProp.DeleteArrayElementAtIndex(w);
                stageSO.ApplyModifiedProperties();
                stageSO.Update();
                if (_selectedWave >= wavesProp.arraySize) _selectedWave = -1;
                break;
            }
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();

            // 선택된 웨이브 상세
            if (wSelected && waveAsset != null)
                DrawWaveDetail(waveAsset);
        }

        EditorGUILayout.Space(4);
        if (GUILayout.Button("+ 웨이브 추가"))
        {
            wavesProp.arraySize++;
            wavesProp.GetArrayElementAtIndex(wavesProp.arraySize - 1).objectReferenceValue = null;
        }

        stageSO.ApplyModifiedProperties();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(stageAsset);
        }
    }

    void DrawWaveDetail(WaveData wave)
    {
        SerializedObject wSO = new SerializedObject(wave);
        wSO.Update();

        EditorGUI.indentLevel++;
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.PropertyField(wSO.FindProperty("delayAfterWave"),
            new GUIContent("다음 웨이브 대기(초)"));

        SerializedProperty groupsProp = wSO.FindProperty("enemyGroups");
        GUILayout.Label($"  적 그룹 ({groupsProp.arraySize}개)");

        for (int g = 0; g < groupsProp.arraySize; g++)
        {
            var gProp = groupsProp.GetArrayElementAtIndex(g);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"그룹 {g + 1}", GUILayout.Width(50));
            EditorGUILayout.PropertyField(gProp.FindPropertyRelative("enemyData"),
                GUIContent.none, GUILayout.Width(130));
            EditorGUILayout.LabelField("수:", GUILayout.Width(20));
            EditorGUILayout.PropertyField(gProp.FindPropertyRelative("count"),
                GUIContent.none, GUILayout.Width(40));
            EditorGUILayout.LabelField("간격:", GUILayout.Width(35));
            EditorGUILayout.PropertyField(gProp.FindPropertyRelative("spawnInterval"),
                GUIContent.none, GUILayout.Width(45));

            GUI.color = new Color(1f, 0.5f, 0.5f);
            if (GUILayout.Button("X", GUILayout.Width(22)))
            {
                groupsProp.DeleteArrayElementAtIndex(g);
                wSO.ApplyModifiedProperties();
                wSO.Update();
                break;
            }
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("  + 적 그룹 추가"))
            groupsProp.arraySize++;

        EditorGUILayout.EndVertical();
        EditorGUI.indentLevel--;

        wSO.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(wave);
    }
}
