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
        private static Texture titleIcon;
        private static readonly Vector2 windowSizeFull     = new Vector2(340, 378);
        private static readonly Vector2 windowSizeNoExport = new Vector2(340, 306);
        private static readonly Vector2 windowSizeNoImport = new Vector2(340, 260);
        private static readonly Vector2 windowSizeEmpty    = new Vector2(340, 188);

        private static readonly Color titleBgColor         = new Color(0.1f, 0.15f, 0.35f, 0.5f);
        private static readonly Color exportGroupColor     = new Color(1f, 0.8f, 0.9f, 1f);
        private static readonly Color exportButtonColor    = new Color(0.9f, 0.6f, 0.7f, 1f);
        private static readonly Color importGroupColor     = new Color(0.7f, 0.9f, 1f, 1f);
        private static readonly Color importButtonColor    = new Color(0.5f, 0.7f, 0.9f, 1f);

        private static GUIStyle labelStyle;
        private static GUIStyle textStyle;
        private static GUIStyle buttonStyle;
        private static GUIStyle foldoutStyle;
        private static GUIStyle titleStyle;


        private static Scene currentScene;
        private static string currentSceneName;
        private static string currentSceneFolderPath;

        private static bool isExpandExportLightSetting;
        private static string exportDataPath;
        private static bool isExportLightingData;
        private static bool isExportLights;
        private static bool isExportLightProbes;
        private static bool isExportReflectionProbes;

        private static bool isExpandImportLightSetting;
        private static string importDataPath;
        private static bool isImportLightingData;
        private static bool isImportLights;
        private static bool isImportLightProbes;
        private static bool isImportReflectionProbes;

#endregion

#region Window & Console Log の表示メッセージ関連
        public const string label_title = "Scene Light Setting Exporter / Importer";
        public static GUIContent label_SceneName        = new GUIContent();
        public static GUIContent label_ScenePath        = new GUIContent();

        public static GUIContent label_LightingData     = new GUIContent();
        public static GUIContent label_Lights           = new GUIContent();
        public static GUIContent label_LightProbes      = new GUIContent();
        public static GUIContent label_ReflectionProbes = new GUIContent();
        public static GUIContent label_ImportFilePath   = new GUIContent();


        private static string message_CreateDir;
        private static string message_NoLightingData;
        private static string message_DoneExpoted;
        private static string message_EmptyImportPath;
        private static string message_NoLoadLightingData;
#endregion

#region Window の設定関連
        private const string prefsKey_isExpandExportLightSetting = "SceneLightSetting isExpandExportLightSetting";
        private const string prefsKey_isExportLightingData       = "SceneLightSetting isExportLightingData";
        private const string prefsKey_isExportLights             = "SceneLightSetting isExportLights";
        private const string prefsKey_isExportLightProbes        = "SceneLightSetting isExportLightProbes";
        private const string prefsKey_isExportReflectionProbes   = "SceneLightSetting isExportReflectionProbes";

        private const string prefsKey_isExpandImportLightSetting = "SceneLightSetting isExpandImportLightSetting";
        private const string prefsKey_isImportLightingData       = "SceneLightSetting isImportLightingData";
        private const string prefsKey_isImportLights             = "SceneLightSetting isImportLights";
        private const string prefsKey_isImportLightProbes        = "SceneLightSetting isImportLightProbes";
        private const string prefsKey_isImportReflectionProbes   = "SceneLightSetting isImportReflectionProbes";

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
            ReseizeWindow();
        }

        private static void ReseizeWindow()
        {
            if (isExpandExportLightSetting == false && isExpandImportLightSetting == false)
            {
                window.maxSize = window.minSize = windowSizeEmpty;
            }
            else if (isExpandImportLightSetting == false)
            {
                window.maxSize = window.minSize = windowSizeNoImport;
            }
            else if (isExpandExportLightSetting == false)
            {
                window.maxSize = window.minSize = windowSizeNoExport;
            }
            else
            {
                window.maxSize = window.minSize = windowSizeFull;
            }
        }

        private static void GetSceneInfo(Scene scene)
        {
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

            GetEditorIcons();
            GetEditorPrefs();
            SetupMessages();
        }


        private void OnDisable()
        {
            EditorSceneManager.sceneOpened     -= SceneOpend;
            EditorSceneManager.sceneSaved      -= GetSceneInfo;
            EditorSceneManager.newSceneCreated -= ResetSceneInfo;
        }

        private void GetEditorIcons()
        {
            var sceneIcon = EditorGUIUtility.IconContent("SceneAsset Icon");
            if (sceneIcon != null)
            {
                label_SceneName.image = sceneIcon.image;
            }

            var folderIcon = EditorGUIUtility.IconContent("Folder Icon");
            if (folderIcon != null)
            {
                label_ScenePath.image      = folderIcon.image;
                label_ImportFilePath.image = folderIcon.image;
            }

            var lightingDataIcon = EditorGUIUtility.IconContent("LightmapParameters Icon");
            if (lightingDataIcon != null)
            {
                label_LightingData.image = lightingDataIcon.image;
            }

            var lightIcon = EditorGUIUtility.IconContent("Light Icon");
            if (lightIcon != null)
            {
                label_Lights.image = lightIcon.image;
            }

            var lightProbeIcon = EditorGUIUtility.IconContent("LightProbeGroup Icon");
            if (lightProbeIcon != null)
            {
                label_LightProbes.image = lightProbeIcon.image;
            }

            var reflectionProbeIcon = EditorGUIUtility.IconContent("ReflectionProbe Icon");
            if (reflectionProbeIcon != null)
            {
                label_ReflectionProbes.image = reflectionProbeIcon.image;
            }
        }

        private void GetEditorPrefs()
        {
            isExpandExportLightSetting = EditorPrefs.GetBool(prefsKey_isExpandExportLightSetting ,true);
            isExportLights             = EditorPrefs.GetBool(prefsKey_isExportLights ,true);
            isExportLightingData       = EditorPrefs.GetBool(prefsKey_isExportLightingData ,true);
            isExportLightProbes        = EditorPrefs.GetBool(prefsKey_isExportLightProbes ,true);
            isExportReflectionProbes   = EditorPrefs.GetBool(prefsKey_isExportReflectionProbes ,true);

            isExpandImportLightSetting = EditorPrefs.GetBool(prefsKey_isExpandImportLightSetting ,true);
            isImportLightingData       = EditorPrefs.GetBool(prefsKey_isImportLightingData ,true);
            isImportLights             = EditorPrefs.GetBool(prefsKey_isImportLights ,true);
            isImportLightProbes        = EditorPrefs.GetBool(prefsKey_isImportLightProbes ,true);
            isImportReflectionProbes   = EditorPrefs.GetBool(prefsKey_isImportReflectionProbes ,true);
        }

        private void SetupMessages()
        {
            label_SceneName.text        = "Current Scene Name";
            label_ScenePath.text        = "Current Scene Path";
            label_LightingData.text     = "Lighting Data";
            label_Lights.text           = "Light Objects";
            label_LightProbes.text      = "LightProbes";
            label_ReflectionProbes.text = "ReflectionProbes";
            label_ImportFilePath.text   = "Import File Path";


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
            labelStyle            = new GUIStyle();
            labelStyle.richText   = true;
            labelStyle.font       = GUI.skin.font;
            labelStyle.alignment  = TextAnchor.MiddleLeft;

            textStyle             = GUI.skin.textField;
            textStyle.richText    = true;
            textStyle.alignment   = TextAnchor.MiddleCenter;

            buttonStyle           = GUI.skin.button;
            buttonStyle.richText  = true;
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            titleStyle            = GUI.skin.GetStyle("IN TitleText");
            titleStyle.alignment  = TextAnchor.UpperCenter;

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    EditorGUI.DrawRect(new Rect(0, 0, 340, 30), titleBgColor);
                    // EditorGUI.DropShadowLabel だとRichTextが使えなそうなので、色違いをズラして2個描画させる
                    EditorGUI.LabelField(new Rect(14, 8, 300, 20), "<size=13><b><color=#888888>" + label_title + "</color></b></size>", labelStyle);
                    EditorGUI.LabelField(new Rect(13, 7, 300, 20), "<size=13><b><color=#444444>" + label_title + "</color></b></size>", labelStyle);
                    EditorGUI.LabelField(new Rect(12, 6, 300, 20), "<size=13><b><color=#222222>" + label_title + "</color></b></size>", labelStyle);
                    EditorGUI.LabelField(new Rect(11, 5, 300, 20), "<size=13><b><color=#000000>" + label_title + "</color></b></size>", labelStyle);
                    EditorGUI.LabelField(new Rect(10, 4, 300, 20), "<size=13><b><color=#ffd700>" + label_title + "</color></b></size>", labelStyle);
                }

                var lastRect = GUILayoutUtility.GetLastRect();
                if (GUI.Button(new Rect(lastRect.width - 28, lastRect.y + 10, 20, 20), EditorGUIUtility.IconContent("_Help"), titleStyle))
                {
                    var helpWindowSize = SceneLightSettingHelpWindow.windowSize;
                    var helpWindowPosX = (Screen.currentResolution.width - helpWindowSize.x) / 2;
                    var helpWindowPosY = (Screen.currentResolution.height - helpWindowSize.y) / 2;

                    var helpWindow = EditorWindow.GetWindow<SceneLightSettingHelpWindow>(false, "", true);
                    helpWindow.titleContent = new GUIContent("Scene Light Setting Help Window");
                    var icon = EditorGUIUtility.IconContent("Lightmapping");
                    if (icon != null)
                    {
                        helpWindow.titleContent.image = icon.image;
                    }
                    helpWindow.position = new Rect(helpWindowPosX, helpWindowPosY, helpWindowSize.x, helpWindowSize.y);
                    helpWindow.maxSize = helpWindow.minSize = helpWindowSize;
                }
            }

            GUILayout.Space(30);

            currentSceneName = (currentScene != null) ? currentScene.name : "...";

            EditorGUILayout.LabelField(label_SceneName);
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.Space();
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField(currentSceneName, GUILayout.Width(320));
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.Space();
            }

            EditorGUILayout.LabelField(label_ScenePath);
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.Space();
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField(currentSceneFolderPath, GUILayout.Width(320));
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.Space();
            }

            GUILayout.Space(10);

            using (new BackgroundColorScope(exportGroupColor))
            {
                using (new EditorGUILayout.VerticalScope("Box"))
                {
                    ExportLightSettingGroup();
                }
            }

            GUILayout.Space(6);

            using (new BackgroundColorScope(importGroupColor))
            {
                using (new EditorGUILayout.VerticalScope("Box"))
                {
                    ImportLightSettingGroup();
                }
            }
        }

        private static void ExportLightSettingGroup()
        {
            EditorGUI.BeginChangeCheck();
            isExpandExportLightSetting = CustomFoldout(isExpandExportLightSetting, "<color=#8b0000><b>Export Light Setting</b></color>");
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(prefsKey_isExpandExportLightSetting, isExpandExportLightSetting);
                ReseizeWindow();
                GUI.changed = false;
            }

            if (isExpandExportLightSetting == true)
            {
                EditorGUI.indentLevel++;

                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUILayout.VerticalScope())
                    {
                        EditorGUI.BeginChangeCheck();
                        isExportLightingData     = EditorGUILayout.ToggleLeft(label_LightingData, isExportLightingData, GUILayout.Width(180));
                        isExportLights           = EditorGUILayout.ToggleLeft(label_Lights, isExportLights, GUILayout.Width(180));
                        isExportLightProbes      = EditorGUILayout.ToggleLeft(label_LightProbes, isExportLightProbes, GUILayout.Width(180));
                        isExportReflectionProbes = EditorGUILayout.ToggleLeft(label_ReflectionProbes, isExportReflectionProbes, GUILayout.Width(180));
                        if (EditorGUI.EndChangeCheck())
                        {
                            EditorPrefs.SetBool(prefsKey_isExportLightingData, isExportLightingData);
                            EditorPrefs.SetBool(prefsKey_isExportLights, isExportLights);
                            EditorPrefs.SetBool(prefsKey_isExportLightProbes, isExportLightProbes);
                            EditorPrefs.SetBool(prefsKey_isExportReflectionProbes, isExportReflectionProbes);
                        }
                    }

                    using (new BackgroundColorScope(exportButtonColor))
                    {
                        using (new EditorGUILayout.VerticalScope())
                        {
                            if (GUILayout.Button("<color=#8b0000><size=15><b>Export</b></size></color>", buttonStyle, GUILayout.Height(60)))
                            {
                                ExportSceneLightingData();
                            }
                        }
                    }
                    GUILayout.Space(10);
                }
                EditorGUI.indentLevel--;
            }
        }

        private static void ImportLightSettingGroup()
        {
            EditorGUI.BeginChangeCheck();
            isExpandImportLightSetting = CustomFoldout(isExpandImportLightSetting, "<color=#191970><b>Import Light Setting</b></color>");
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(prefsKey_isExpandImportLightSetting, isExpandImportLightSetting);
                ReseizeWindow();
                GUI.changed = false;
            }

            if (isExpandImportLightSetting == true)
            {
                EditorGUI.indentLevel++;

                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUILayout.VerticalScope())
                    {
                        EditorGUI.BeginChangeCheck();
                        isImportLightingData     = EditorGUILayout.ToggleLeft(label_LightingData, isImportLightingData, GUILayout.Width(180));
                        isImportLights           = EditorGUILayout.ToggleLeft(label_Lights, isImportLights, GUILayout.Width(180));
                        isImportLightProbes      = EditorGUILayout.ToggleLeft(label_LightProbes, isImportLightProbes, GUILayout.Width(180));
                        isImportReflectionProbes = EditorGUILayout.ToggleLeft(label_ReflectionProbes, isImportReflectionProbes, GUILayout.Width(180));
                        if (EditorGUI.EndChangeCheck())
                        {
                            EditorPrefs.SetBool(prefsKey_isImportLightingData, isImportLightingData);
                            EditorPrefs.SetBool(prefsKey_isImportLights, isImportLights);
                            EditorPrefs.SetBool(prefsKey_isImportLightProbes, isImportLightProbes);
                            EditorPrefs.SetBool(prefsKey_isImportReflectionProbes, isImportReflectionProbes);
                        }
                    }

                    using (new BackgroundColorScope(importButtonColor))
                    {
                        using (new EditorGUILayout.VerticalScope())
                        {
                            if (GUILayout.Button("<color=#191970><size=15><b>Import</b></size></color>", buttonStyle, GUILayout.Height(60)))
                            {
                                ImportSceneLightingData();
                            }
                        }
                    }
                    GUILayout.Space(10);
                }

                EditorGUILayout.Space();

                EditorGUILayout.LabelField(label_ImportFilePath, labelStyle);
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Space(15);

                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.TextField("", importDataPath, textStyle);
                    EditorGUI.EndDisabledGroup();

                    using (new BackgroundColorScope(importButtonColor))
                    {
                        if (GUILayout.Button("<color=#191970><b>...</b></color>", buttonStyle, GUILayout.Width(30)))
                        {
                            var openFolderPath = (currentSceneFolderPath != "") ? currentSceneFolderPath : Application.dataPath;
                            var selectedImportDataPath = EditorUtility.OpenFilePanelWithFilters(
                                                            "Select Scene Light Setting Data to Import",
                                                            openFolderPath,
                                                            new string[]{"SceneLightingData", "asset"});
                            // 相対パスに変換
                            importDataPath = selectedImportDataPath.Replace(Application.dataPath, "Assets");
                        }
                    }
                    GUILayout.Space(10);
                }
                EditorGUI.indentLevel--;
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

/*
参考サイト
第6章 EditorGUI (EdirotGUILayout) - エディター拡張入門
https://anchan828.github.io/editor-manual/web/part1-editorgui.html
*/
        private class BackgroundColorScope : GUI.Scope
        {
            private readonly Color _color;
            public BackgroundColorScope(Color color)
            {
                this._color = GUI.backgroundColor;
                GUI.backgroundColor = color;
            }

            protected override void CloseScope()
            {
                GUI.backgroundColor = _color;
            }
        }

/*
参考サイト
Unity のエディタ拡張で FoldOut をかっこよくするのをやってみた - 凹みTips
http://tips.hecomi.com/entry/2016/10/15/004144
*/
        public static bool CustomFoldout(bool foldout, string content)
        {
            var style           = new GUIStyle("ShurikenModuleTitle");
            style.font          = new GUIStyle(EditorStyles.label).font;
            style.fontSize      = 12;
            style.border        = new RectOffset(15, 7, 4, 4);
            style.fixedHeight   = 22;
            style.contentOffset = new Vector2(20, -2);
            style.alignment     = TextAnchor.MiddleLeft;
            style.richText      = true;

            var rect = GUILayoutUtility.GetRect(16f, 22f, style);
            GUI.Box(rect, content, style);

            var e = Event.current;

            var toggleRect = new Rect(rect.x + 4, rect.y + 2, 13, 13);
            if (e.type == EventType.Repaint)
            {
                EditorStyles.foldout.Draw(toggleRect, false, false, foldout, false);
            }

            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
            {
                foldout = !foldout;
                e.Use();
                GUI.changed = true;
            }

            return foldout;
        }
    }
}