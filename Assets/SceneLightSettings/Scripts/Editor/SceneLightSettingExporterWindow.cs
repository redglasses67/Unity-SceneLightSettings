using System.IO;
using System.Reflection;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
// using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

using Object = UnityEngine.Object;

namespace SceneLightSettings
{
    public class SceneLightSettingExporterWindow : EditorWindow
    {
#region Window 関連 変数
        private static SceneLightSettingExporterWindow window;
        private static readonly Vector2 fixedWindowSize = new Vector2(330f, 310);
        private static Scene currentScene;
        private static string currentSceneName;
        private static string currentSceneFolderPath;

        private static bool isExpandExportLightSetting = true;
        private static string exportDataPath;
        private static bool isExportLightingData = true;
        private static bool isExportLights = true;
        private static bool isExportLightProbes = true;
        private static bool isExportReflectionProbes = true;

        private static bool isExpandImportLightSetting = true;
        private static string importDataPath;
        private static bool isImportLightingData = true;
        private static bool isImportLights = true;
        private static bool isImportLightProbes = true;
        private static bool isImportReflectionProbes = true;
#endregion

#region Window & Console Log の表示メッセージ関連
        private static string message_CreateDir;
        private static string message_NoLightingData;
        private static string message_DoneExpoted;
        private static string message_EmptyImportPath;
        private static string message_NoLoadLightingData;
#endregion


        [MenuItem("Tools/Scene Light Setting")]
        private static void ShowWindow()
        {
            GetSceneInfo(SceneManager.GetActiveScene());
            window = EditorWindow.GetWindow<SceneLightSettingExporterWindow>();
            window.titleContent = new GUIContent("Scene Light Setting Exporter Window");
            var icon = EditorGUIUtility.IconContent("Lightmapping");
            if (icon != null)
            {
                window.titleContent.image = icon.image;
            }
            window.maxSize = window.minSize = fixedWindowSize;
        }

        private static void GetSceneInfo(Scene scene)
        {
            // Debug.Log( "sceneOpened : " + scene.name);
            currentScene           = scene;
            currentSceneName       = currentScene.name;
            currentSceneFolderPath = (scene.path != "") ? Path.GetDirectoryName(scene.path) : "";
        }

        private static void ResetSceneInfo(Scene scene, NewSceneSetup setup, NewSceneMode mode)
        {
            currentScene           = scene;
            currentSceneName       = "";
            currentSceneFolderPath = "";
        }

        private static void SceneOpend(Scene openedScene, OpenSceneMode mode)
        {
            GetSceneInfo(openedScene);
        }

        private void OnEnable()
        {
            EditorSceneManager.sceneOpened     += SceneOpend;
            EditorSceneManager.sceneSaved      += GetSceneInfo; // This is insurance if it is empty scene.
            EditorSceneManager.newSceneCreated += ResetSceneInfo;
            SetupMessages();
        }

        private void OnDisable()
        {
            EditorSceneManager.sceneOpened     -= SceneOpend;
            EditorSceneManager.sceneSaved      -= GetSceneInfo;
            EditorSceneManager.newSceneCreated -= ResetSceneInfo;
        }

        private void SetupMessages()
        {
            message_CreateDir = (Application.systemLanguage == SystemLanguage.Japanese) ?
                "SceneLightingData フォルダを作成しました.\n\n" : "Created SceneLightingData Folder\n\n";

            message_NoLightingData = (Application.systemLanguage == SystemLanguage.Japanese) ?
                "SceneLightingData が取得できませんでした...\n\n" : "Did not get SceneLightingData\n\n";

            message_DoneExpoted = (Application.systemLanguage == SystemLanguage.Japanese) ?
                "SceneLightingData を書き出しました\n\n" : "Created SceneLightingData\n\n";

            message_EmptyImportPath = (Application.systemLanguage == SystemLanguage.Japanese) ?
                "Import File Path が空です.\n\n" : "Import File Path is empty.\n\n";

            message_NoLoadLightingData = (Application.systemLanguage == SystemLanguage.Japanese) ?
                "SceneLightingData が読み込めませんでした...\n\n" : "Did not load SceneLightingData\n\n";
        }

        private void OnGUI()
        {
            var labelStyle        = new GUIStyle();
            labelStyle.richText   = true;
            labelStyle.font       = GUI.skin.font;
            labelStyle.alignment  = TextAnchor.MiddleLeft;

            var textStyle         = GUI.skin.textField;
            textStyle.richText    = true;
            textStyle.alignment   = TextAnchor.MiddleCenter;

            var buttonStyle       = GUI.skin.button;
            buttonStyle.richText  = true;
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            var foldoutStyle       = GUI.skin.FindStyle("Foldout");
            foldoutStyle.richText  = true;
            foldoutStyle.alignment = TextAnchor.MiddleCenter;

            // EditorGUI.DropShadowLabel だとRichTextが使えなそうなので、色違いをズラして2個描画させる
            EditorGUI.LabelField(new Rect(9, 1, 300, 20), "<size=14><b><color=black>Scene Light Setting Exporter / Importer</color></b></size>", labelStyle);
            EditorGUI.LabelField(new Rect(8, 0, 300, 20), "<size=14><b><color=#ffd700>Scene Light Setting Exporter / Importer</color></b></size>", labelStyle);
            GUILayout.Space(25);

            EditorGUI.indentLevel++;
            currentSceneName = (currentScene != null) ? currentScene.name : "...";
            EditorGUIUtility.labelWidth = 90;
            EditorGUILayout.LabelField("Scene Name", ": " + currentSceneName);
            EditorGUILayout.LabelField("Scene Path", ": " + currentSceneFolderPath);
            EditorGUIUtility.labelWidth = 0;
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            using (new EditorGUILayout.VerticalScope("Box"))
            {
                if (isExpandExportLightSetting = EditorGUILayout.Foldout(isExpandExportLightSetting, "<b>Export Light Setting</b>", foldoutStyle))
                {
                    EditorGUI.indentLevel++;

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        using (new EditorGUILayout.VerticalScope())
                        {
                            isExportLightingData     = EditorGUILayout.ToggleLeft("Export Lighting Data", isExportLightingData, GUILayout.Width(180));
                            isExportLights           = EditorGUILayout.ToggleLeft("Export Lights", isExportLights, GUILayout.Width(180));
                            isExportLightProbes      = EditorGUILayout.ToggleLeft("Export LightProbes", isExportLightProbes, GUILayout.Width(180));
                            isExportReflectionProbes = EditorGUILayout.ToggleLeft("Export ReflectionProbes", isExportReflectionProbes, GUILayout.Width(180));
                        }
                        using (new EditorGUILayout.VerticalScope())
                        {
                            if (GUILayout.Button("Export", GUILayout.Height(70)))
                            {
                                ExportSceneLightingData();
                            }
                        }
                        GUILayout.Space(10);
                    }
                    EditorGUI.indentLevel--;
                }
            }


            using (new EditorGUILayout.VerticalScope("Box"))
            {
                if (isExpandImportLightSetting = EditorGUILayout.Foldout(isExpandImportLightSetting, "<b>Import Light Setting</b>", foldoutStyle))
                {
                    EditorGUI.indentLevel++;

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        using (new EditorGUILayout.VerticalScope())
                        {
                            isImportLightingData     = EditorGUILayout.ToggleLeft("Import Lighting Data", isImportLightingData, GUILayout.Width(180));
                            isImportLights           = EditorGUILayout.ToggleLeft("Import Lights", isImportLights, GUILayout.Width(180));
                            isImportLightProbes      = EditorGUILayout.ToggleLeft("Import LightProbes", isImportLightProbes, GUILayout.Width(180));
                            isImportReflectionProbes = EditorGUILayout.ToggleLeft("Import ReflectionProbes", isImportReflectionProbes, GUILayout.Width(180));
                        }

                        using (new EditorGUILayout.VerticalScope())
                        {
                            if (GUILayout.Button("Import", GUILayout.Height(70)))
                            {
                                ImportSceneLightingData();
                            }
                        }
                        GUILayout.Space(10);
                    }

                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("● Import File Path", labelStyle);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.Space(15);

                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.TextField("", importDataPath, textStyle);
                        EditorGUI.EndDisabledGroup();

                        if (GUILayout.Button("...", GUILayout.Width(30) ))
                        {
							var openFolderPath = (currentSceneFolderPath != "") ? currentSceneFolderPath : Application.dataPath;
                            var selectedImportDataPath = EditorUtility.OpenFilePanelWithFilters(
															"Select Scene Light Setting Data to Import",
															openFolderPath,
															new string[]{"SceneLightingData", "asset"});
							// 相対パスに変換
							importDataPath = selectedImportDataPath.Replace(Application.dataPath, "Assets");
                        }
                        GUILayout.Space(10);
                    }
                    EditorGUI.indentLevel--;
                }
            }
        }


        private static void ExportSceneLightingData()
        {
			var exportFolderParentPath = (currentSceneFolderPath != "") ? currentSceneFolderPath : "Assets";
            var exportFolderPath = Path.Combine(exportFolderParentPath, "SceneLightingData");
            if (Directory.Exists(exportFolderPath) == false)
            {
                Directory.CreateDirectory(exportFolderPath);
                Debug.Log(message_CreateDir);
            }

			var sceneName  = (currentSceneName != "") ? currentScene.name : "UntitledScene";
            var fileName   = "SceneLightingData_" + sceneName + ".asset";
            exportDataPath = Path.Combine(exportFolderPath, fileName);
            exportDataPath = AssetDatabase.GenerateUniqueAssetPath(exportDataPath);

            var lightingData = SceneLightSettingExporter.GetSceneLightingData(
								isExportLightingData,
								isExportLights,
								isExportLightProbes,
								isExportReflectionProbes);
            if (lightingData == null)
            {
                Debug.LogWarning(message_NoLightingData);
                EditorUtility.DisplayDialog(
                    "Scene Light Setting Export / Import",
                    message_NoLightingData,
                    "OK");
                return;
            }

            AssetDatabase.CreateAsset(lightingData, exportDataPath);
            Debug.Log(message_DoneExpoted + "exportDataPath = " + exportDataPath, lightingData);
            EditorUtility.DisplayDialog(
                "Scene Light Setting Export / Import",
                message_DoneExpoted + "exportDataPath\n>>" + exportDataPath,
                "OK");
        }


        private static void ImportSceneLightingData()
        {
            if (importDataPath == "")
            {
                Debug.LogWarning(message_EmptyImportPath);
                EditorUtility.DisplayDialog(
                    "Scene Light Setting Export / Import",
                    message_EmptyImportPath,
                    "OK");
                return;
            }

            var lightingData = AssetDatabase.LoadAssetAtPath<SceneLightingData>(importDataPath);
            if (lightingData == null)
            {
                Debug.LogWarning(message_NoLoadLightingData + "importDataPath = " + importDataPath);
                EditorUtility.DisplayDialog(
                    "Scene Light Setting Export / Import",
                    message_NoLoadLightingData + "importDataPath\n>>" + importDataPath,
                    "OK");
                return;
            }

			if (isImportLightingData == true)
			{
				SceneLightSettingExporter.SetSceneLightingData(lightingData);
			}

			if (isImportLights == true)
			{
				SceneLightSettingExporter.SetSceneLights(lightingData);
			}

			if (isImportLightProbes == true)
			{
				SceneLightSettingExporter.SetSceneLightProbes(lightingData);
			}

			if (isImportReflectionProbes == true)
			{
				SceneLightSettingExporter.SetSceneReflectionProbes(lightingData);
			}

            EditorSceneManager.MarkSceneDirty(currentScene); 
        }
    }
}