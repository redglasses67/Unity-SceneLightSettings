using System.IO;

using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Reflection;

namespace SceneLightSettings
{
    public class SceneLightSettingExporterWindow : EditorWindow, IHasCustomMenu
    {
#region Window 関連 変数
        private static SceneLightSettingExporterWindow window;
        private static SceneLightSettingHelpWindow helpWindow;
        private static readonly Vector2 windowMinSize        = new Vector2(340, 125);
        private static readonly Vector2 windowSizeFull       = new Vector2(340, 620);
        private static readonly Vector2 windowSizeExportOnly = new Vector2(340, 380);
        private static readonly Vector2 windowSizeImportOnly = new Vector2(340, 420);
        private static readonly Vector2 windowSizeEmpty      = new Vector2(340, 190);
        private static Vector2 scrollPos;

        private static readonly Color titleBgColor      = new Color(0.1f, 0.15f, 0.35f, 1f);
        private static readonly Color exportGroupColor  = new Color(1f, 0.8f, 0.9f, 1f);
        private static readonly Color exportButtonColor = new Color(0.9f, 0.6f, 0.7f, 1f);
        private static readonly Color importGroupColor  = new Color(0.7f, 0.9f, 1f, 1f);
        private static readonly Color importButtonColor = new Color(0.5f, 0.7f, 0.9f, 1f);

        private static string exportTextColorHex;
        private static string importTextColorHex;

        private static GUIStyle labelStyle;
        private static GUIStyle textStyle;
        private static GUIStyle buttonStyle;
        private static GUIStyle titleStyle;


        private static Scene currentScene;
        private static string currentSceneName;
        private static string currentSceneFolderPath;

        private static bool isExpandedExportLightSetting;
        private static string exportDataFolderPath;
        private static string exportDataFileName;
        private static bool doExportLightingData;
        private static bool doExportLights;
        private static bool doExportLightProbeGroups;
        private static bool doExportReflectionProbes;

        private static bool isExpandedImportLightSetting;
        private static string importDataPath;
        private static bool doImportLightingData;
        private static bool doImportLightingData_ENV;
        private static bool doImportLightingData_RML;
        private static bool doImportLightingData_LMS;
        private static bool doImportLightingData_OTS;
        private static bool doImportLights;
        private static bool doImportLightProbeGroups;
        private static bool doImportReflectionProbes;
        private static bool doDeleteExistingLights;

        private Rect tempLastRect;
#endregion

#region Window & Console Log の表示メッセージ関連
        public const string label_title = "Scene Light Setting Exporter / Importer";
        private static GUIContent label_SceneName        = new GUIContent();
        private static GUIContent label_ScenePath        = new GUIContent();

        private static GUIContent label_LightingData     = new GUIContent();
        private static readonly string label_ENV         = "Enviroment";
        private static readonly string label_RML         = "Realtime / Mixed Lighting";
        private static readonly string label_LMS         = "Lightmapping Settings";
        private static readonly string label_OTS         = "Other Settings";
        private static GUIContent label_Lights           = new GUIContent();
        private static GUIContent label_LightProbeGroups = new GUIContent();
        private static GUIContent label_ReflectionProbes = new GUIContent();
        private static GUIContent label_ExistingLights   = new GUIContent();
        private static GUIContent label_ExportFolderPath = new GUIContent();
        private static GUIContent label_ExportFileName   = new GUIContent();
        private static GUIContent label_ImportFilePath   = new GUIContent();


        private static string message_Playing;
        private static string message_CreateDir;
        private static string message_NoLightingData;
        private static string message_DoneExpoted;
        private static string message_EmptyImportPath;
        private static string message_NoLoadLightingData;
        private static string message_DoneImpoted;
        private static string message_ExportSerializedPropText;
#endregion

#region Window の設定関連
        private const string prefsKey_isExpandedExportLightSetting = "SceneLightSetting isExpandedExportLightSetting";
        private const string prefsKey_doExportLightingData         = "SceneLightSetting doExportLightingData";
        private const string prefsKey_doExportLights               = "SceneLightSetting doExportLights";
        private const string prefsKey_doExportLightProbeGroups     = "SceneLightSetting doExportLightProbeGroups";
        private const string prefsKey_doExportReflectionProbes     = "SceneLightSetting doExportReflectionProbes";

        private const string prefsKey_isExpandedImportLightSetting = "SceneLightSetting isExpandedImportLightSetting";
        private const string prefsKey_doImportLightingData         = "SceneLightSetting doImportLightingData";
        private const string prefsKey_doImportLightingData_ENV     = "SceneLightSetting doImportLightingData Enviroment";
        private const string prefsKey_doImportLightingData_RML     = "SceneLightSetting doImportLightingData Realtime Mixed Lighting";
        private const string prefsKey_doImportLightingData_LMS     = "SceneLightSetting doImportLightingData Lightmappong Settings";
        private const string prefsKey_doImportLightingData_OTS     = "SceneLightSetting doImportLightingData Other Settings";
        private const string prefsKey_doImportLights               = "SceneLightSetting doImportLights";
        private const string prefsKey_doImportLightProbeGroups     = "SceneLightSetting doImportLightProbeGroups";
        private const string prefsKey_doImportReflectionProbes     = "SceneLightSetting doImportReflectionProbes";
        private const string prefsKey_doDeleteExistingLights       = "SceneLightSetting doDeleteExistingLights";

#endregion


        [MenuItem("Tools/Scene Light Settings")]
        private static void ShowWindow()
        {
            GetSceneInfo(SceneManager.GetActiveScene());
            window = EditorWindow.GetWindow<SceneLightSettingExporterWindow>();
            window.titleContent = new GUIContent("Scene Light Setting Exporter Window");
            var titleIcon = EditorGUIUtility.IconContent("Lightmapping");
            if (titleIcon != null)
            {
                window.titleContent.image = titleIcon.image;
            }
            window.minSize = windowMinSize;
            ChangeWindowMaxSize();
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Check Serialized Properties"), false, CheckSerializedProperties);
        }

        void CheckSerializedProperties()
        {
            var so_lightmapSettings        = SceneLightSettingExporter.GetSerializedLightmapSettingsObject();
            var sp_lightmapSettingsTxtPath = Application.dataPath + "/sp_lightmapSettings.txt";
            var sw_lightmapSettings        = new StreamWriter(sp_lightmapSettingsTxtPath, false, System.Text.Encoding.GetEncoding("shift_jis"));
            DataUtility.GetSerializedProperties(so_lightmapSettings, ref sw_lightmapSettings);
            sw_lightmapSettings.Close();

            var so_renderSettings        = SceneLightSettingExporter.GetSerializedRenderSettingsObject();
            var sp_renderSettingsTxtPath = Application.dataPath + "/sp_renderSettings.txt";
            var sw_renderSettings        = new StreamWriter(sp_renderSettingsTxtPath, false, System.Text.Encoding.GetEncoding("shift_jis"));
            DataUtility.GetSerializedProperties(so_renderSettings, ref sw_renderSettings);
            sw_renderSettings.Close();

            Debug.Log(message_ExportSerializedPropText);
            EditorUtility.DisplayDialog(
                    "Scene Light Setting Export / Import",
                    message_ExportSerializedPropText,
                    "OK");
        }

        private static void ChangeWindowMaxSize()
        {
            if (isExpandedExportLightSetting == false && isExpandedImportLightSetting == false)
            {
                window.maxSize = windowSizeEmpty;
            }
            else if (isExpandedImportLightSetting == false)
            {
                window.maxSize = windowSizeExportOnly;
            }
            else if (isExpandedExportLightSetting == false)
            {
                window.maxSize = windowSizeImportOnly;
            }
            else
            {
                window.maxSize = windowSizeFull;
            }
        }

        private static void GetSceneInfo()
        {
            GetSceneInfo(EditorSceneManager.GetActiveScene());
        }
        private static void GetSceneInfo(Scene scene)
        {
            currentScene           = scene;
            currentSceneName       = (currentScene.name != "") ? currentScene.name : " < Untitled > ";
            currentSceneFolderPath = (currentScene.path != "") ? Path.GetDirectoryName(currentScene.path) : "";
        }

        private static void ResetSceneInfo(Scene scene, NewSceneSetup setup, NewSceneMode mode)
        {
            currentScene           = scene;
            currentSceneName       = " < Untitled > ";
            currentSceneFolderPath = "";
            SetDefaultExportPath();
        }

        private static void SceneOpend(Scene openedScene, OpenSceneMode mode)
        {
            GetSceneInfo(openedScene);
            SetDefaultExportPath();
        }

        private static void SetDefaultExportPath()
        {
            exportDataFolderPath = (currentSceneFolderPath != "") ? currentSceneFolderPath : "Assets";
            exportDataFolderPath = Path.Combine(exportDataFolderPath, "SceneLightSettingData");

            exportDataFileName   = "SceneLightSettingData_" + ((currentSceneName != " < Untitled > ") ? currentSceneName : "UntitledScene");
        }

        private void OnEnable()
        {
            EditorSceneManager.sceneOpened     += SceneOpend;
            EditorSceneManager.sceneSaved      += GetSceneInfo; // This is insurance if it is empty scene.
            EditorSceneManager.newSceneCreated += ResetSceneInfo;

            GetEditorIcons();
            GetEditorPrefs();
            SetMessages();
            SetTextColors();

            GetSceneInfo();
            SetDefaultExportPath();
        }

        private void OnFocus()
        {
            GetSceneInfo();
        }

        private void OnDisable()
        {
            EditorSceneManager.sceneOpened     -= SceneOpend;
            EditorSceneManager.sceneSaved      -= GetSceneInfo;
            EditorSceneManager.newSceneCreated -= ResetSceneInfo;

            if (helpWindow != null)
            {
                helpWindow.Close();
                helpWindow = null;
            }
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
                label_ScenePath.image        = folderIcon.image;
                label_ExportFolderPath.image = folderIcon.image;
                label_ImportFilePath.image   = folderIcon.image;
            }

            var scriptableIcon = EditorGUIUtility.IconContent("ScriptableObject Icon");
            if (scriptableIcon != null)
            {
                label_ExportFileName.image = scriptableIcon.image;
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
                label_LightProbeGroups.image = lightProbeIcon.image;
            }

            var reflectionProbeIcon = EditorGUIUtility.IconContent("ReflectionProbe Icon");
            if (reflectionProbeIcon != null)
            {
                label_ReflectionProbes.image = reflectionProbeIcon.image;
            }

            var deleteIcon = EditorGUIUtility.IconContent("CollabDeleted Icon");
            if (deleteIcon != null)
            {
                label_ExistingLights.image = deleteIcon.image;
            }
        }

        private void GetEditorPrefs()
        {
            isExpandedExportLightSetting = EditorPrefs.GetBool(prefsKey_isExpandedExportLightSetting, true);
            doExportLights               = EditorPrefs.GetBool(prefsKey_doExportLights, true);
            doExportLightingData         = EditorPrefs.GetBool(prefsKey_doExportLightingData, true);
            doExportLightProbeGroups     = EditorPrefs.GetBool(prefsKey_doExportLightProbeGroups, true);
            doExportReflectionProbes     = EditorPrefs.GetBool(prefsKey_doExportReflectionProbes, true);

            isExpandedImportLightSetting = EditorPrefs.GetBool(prefsKey_isExpandedImportLightSetting, true);
            doImportLightingData         = EditorPrefs.GetBool(prefsKey_doImportLightingData, true);
            doImportLightingData_ENV     = EditorPrefs.GetBool(prefsKey_doImportLightingData_ENV, true);
            doImportLightingData_RML     = EditorPrefs.GetBool(prefsKey_doImportLightingData_RML, true);
            doImportLightingData_LMS     = EditorPrefs.GetBool(prefsKey_doImportLightingData_LMS, true);
            doImportLightingData_OTS     = EditorPrefs.GetBool(prefsKey_doImportLightingData_OTS, true);
            doImportLights               = EditorPrefs.GetBool(prefsKey_doImportLights, true);
            doImportLightProbeGroups     = EditorPrefs.GetBool(prefsKey_doImportLightProbeGroups, true);
            doImportReflectionProbes     = EditorPrefs.GetBool(prefsKey_doImportReflectionProbes, true);
            doDeleteExistingLights       = EditorPrefs.GetBool(prefsKey_doDeleteExistingLights, true);
        }

        private void SetMessages()
        {
            label_SceneName.text        = "Current Scene Name";
            label_ScenePath.text        = "Current Scene Path";
            label_LightingData.text     = "Lighting Data";
            label_Lights.text           = "Light Objects";
            label_LightProbeGroups.text = "LightProbeGroups";
            label_ReflectionProbes.text = "ReflectionProbes";
            label_ExistingLights.text   = "Delete Existing Lights";
            label_ExportFolderPath.text = "Export Folder Path";
            label_ExportFileName.text   = "Export File Name";
            label_ImportFilePath.text   = "Import File Full Path";

            message_Playing = (Application.systemLanguage == SystemLanguage.Japanese) ?
                "再生中は処理を実行できません." : "It cannot be processed while the scene is playing.";

            message_CreateDir = (Application.systemLanguage == SystemLanguage.Japanese) ?
                "SceneLightSettingData フォルダを作成しました." : "Created SceneLightSettingData Folder.";

            message_NoLightingData = (Application.systemLanguage == SystemLanguage.Japanese) ?
                "SceneLightSettingData が取得できませんでした..." : "Did not get SceneLightSettingData ...";

            message_DoneExpoted = (Application.systemLanguage == SystemLanguage.Japanese) ?
                "SceneLightSettingData を書き出しました!" : "Created SceneLightSettingData";

            message_EmptyImportPath = (Application.systemLanguage == SystemLanguage.Japanese) ?
                "Import File Path が空です..." : "Import File Path is empty ...";

            message_NoLoadLightingData = (Application.systemLanguage == SystemLanguage.Japanese) ?
                "SceneLightSettingData が読み込めませんでした..." : "Did not load SceneLightSettingData ...";

            message_DoneImpoted = (Application.systemLanguage == SystemLanguage.Japanese) ?
                "SceneLightSettingData を読み込みました!" : "Created SceneLightSettingData.";

            message_ExportSerializedPropText = (Application.systemLanguage == SystemLanguage.Japanese) ?
                "lightmapSettings と renderSettings の SerializedProperty のリストをテキストとして Assets フォルダ直下に保存しました。" :
                "Saved 2 serializedProperty list about lightmapSettings and renderSettings as text to Assets folder.";
        }

        private void SetTextColors()
        {
            exportTextColorHex = (EditorGUIUtility.isProSkin == true) ? "<color=#ff7f7f>" : "<color=#8b0000>";
            importTextColorHex = (EditorGUIUtility.isProSkin == true) ? "<color=#7fbfff>" : "<color=#191970>";
        }

        private void GetGUIStyles()
        {
            if (labelStyle == null)
            {
                labelStyle            = new GUIStyle(GUI.skin.label);
                labelStyle.richText   = true;
                labelStyle.alignment  = TextAnchor.MiddleLeft;
            }

            if (textStyle == null)
            {
                textStyle             = new GUIStyle(GUI.skin.textField);
                textStyle.alignment   = TextAnchor.MiddleLeft;
                textStyle.wordWrap    = true;
            }

            if (buttonStyle == null)
            {
                buttonStyle           = new GUIStyle(GUI.skin.button);
                buttonStyle.richText  = true;
                buttonStyle.alignment = TextAnchor.MiddleCenter;
            }

            if (titleStyle == null)
            {
                titleStyle            = new GUIStyle(GUI.skin.GetStyle("IN TitleText"));
                titleStyle.alignment  = TextAnchor.UpperCenter;
            }
        }

        private void OnGUI()
        {
            GetGUIStyles();

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    EditorGUI.DrawRect(new Rect(0, 0, position.width, 30), titleBgColor);
                    // EditorGUI.DropShadowLabel だとRichTextが使えなそうなので、色違いをズラして描画させる
                    EditorGUI.LabelField(new Rect(14, 8, 300, 20), "<size=13><b><color=#444444>" + label_title + "</color></b></size>", labelStyle);
                    EditorGUI.LabelField(new Rect(13, 7, 300, 20), "<size=13><b><color=#333333>" + label_title + "</color></b></size>", labelStyle);
                    EditorGUI.LabelField(new Rect(12, 6, 300, 20), "<size=13><b><color=#222222>" + label_title + "</color></b></size>", labelStyle);
                    EditorGUI.LabelField(new Rect(11, 5, 300, 20), "<size=13><b><color=#000000>" + label_title + "</color></b></size>", labelStyle);
                    EditorGUI.LabelField(new Rect(10, 4, 300, 20), "<size=13><b><color=#ffd700>" + label_title + "</color></b></size>", labelStyle);
                }

                tempLastRect = GUILayoutUtility.GetLastRect();
                if (GUI.Button(new Rect(tempLastRect.width - 28, tempLastRect.y + 10, 20, 20), EditorGUIUtility.IconContent("_Help"), titleStyle))
                {
                    var helpWindowSize = SceneLightSettingHelpWindow.windowSize;
                    var helpWindowPosX = (Screen.currentResolution.width - helpWindowSize.x) / 2;
                    var helpWindowPosY = (Screen.currentResolution.height - helpWindowSize.y) / 2;

                    helpWindow = EditorWindow.GetWindow<SceneLightSettingHelpWindow>(false, "", true);
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

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));

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
                EditorGUILayout.TextField(currentSceneFolderPath, textStyle, GUILayout.Width(320), GUILayout.ExpandHeight(true));
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.Space();
            }

            DrawUILine(Color.yellow);

            using (new BackgroundColorScope(exportGroupColor))
            {
                using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                {
                    ExportLightSettingGroup();
                }
            }

            GUILayout.Space(5);

            using (new BackgroundColorScope(importGroupColor))
            {
                using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                {
                    ImportLightSettingGroup();
                }
            }

            EditorGUILayout.EndScrollView();
            GUILayout.Space(5);
        }

        private void ExportLightSettingGroup()
        {
            EditorGUI.BeginChangeCheck();
            {
                isExpandedExportLightSetting = CustomFoldout(
                    isExpandedExportLightSetting,
                    exportTextColorHex + "<b>Export Light Setting</b></color>");
            }
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(prefsKey_isExpandedExportLightSetting, isExpandedExportLightSetting);
                ChangeWindowMaxSize();
                GUI.changed = false;
            }

            if (isExpandedExportLightSetting == false) { return; }

            EditorGUI.indentLevel++;

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    EditorGUI.BeginChangeCheck();
                    {
                        doExportLightingData     = EditorGUILayout.ToggleLeft(label_LightingData, doExportLightingData);
                        doExportLights           = EditorGUILayout.ToggleLeft(label_Lights, doExportLights);
                        doExportLightProbeGroups = EditorGUILayout.ToggleLeft(label_LightProbeGroups, doExportLightProbeGroups);
                        doExportReflectionProbes = EditorGUILayout.ToggleLeft(label_ReflectionProbes, doExportReflectionProbes);
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        EditorPrefs.SetBool(prefsKey_doExportLightingData, doExportLightingData);
                        EditorPrefs.SetBool(prefsKey_doExportLights, doExportLights);
                        EditorPrefs.SetBool(prefsKey_doExportLightProbeGroups, doExportLightProbeGroups);
                        EditorPrefs.SetBool(prefsKey_doExportReflectionProbes, doExportReflectionProbes);
                    }
                }

                using (new BackgroundColorScope(exportButtonColor))
                {
                    using (new EditorGUILayout.VerticalScope())
                    {
                        EditorGUILayout.Space();
                        if (GUILayout.Button(exportTextColorHex + "<size=15><b>Export</b></size></color>",
                            buttonStyle,
                            GUILayout.Height(60),
                            GUILayout.Width(90)))
                        {
                            ExportSceneLightSettingData();
                        }
                        EditorGUILayout.Space();
                    }
                }
                EditorGUILayout.Space();
            }

            EditorGUILayout.LabelField(label_ExportFolderPath);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(15);

                EditorGUILayout.TextField(exportDataFolderPath, textStyle, GUILayout.Width(260), GUILayout.ExpandHeight(true));

                using (new BackgroundColorScope(exportButtonColor))
                {
                    if (GUILayout.Button(exportTextColorHex + "<b>...</b></color>", buttonStyle, GUILayout.Width(30)))
                    {
                        var selectedExportDataPath = EditorUtility.SaveFolderPanel(
                                                        "Select Scene Light Setting Data for Export",
                                                        "",
                                                        "SceneLightSettingData");
                        // 相対パスに変換
                        exportDataFolderPath = selectedExportDataPath.Replace(Application.dataPath, "Assets");
                        Debug.Log("exportDataFolderPath = " + exportDataFolderPath);
                    }
                }
                EditorGUILayout.Space();
            }

            EditorGUILayout.LabelField(label_ExportFileName);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(15);
                exportDataFileName = EditorGUILayout.TextField(exportDataFileName, textStyle, GUILayout.Width(260), GUILayout.ExpandHeight(true));
                GUILayout.Label("<color=grey><size=9>.asset</size></color>", labelStyle, GUILayout.Width(35));

                EditorGUILayout.Space();
            }
            EditorGUILayout.Space();

            EditorGUI.indentLevel--;
        }


        private void ImportLightSettingGroup()
        {
            EditorGUI.BeginChangeCheck();
            {
                isExpandedImportLightSetting = CustomFoldout(
                    isExpandedImportLightSetting,
                    importTextColorHex + "<b>Import Light Setting</b></color>");
            }
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(prefsKey_isExpandedImportLightSetting, isExpandedImportLightSetting);
                ChangeWindowMaxSize();
                GUI.changed = false;
            }

            if (isExpandedImportLightSetting == false) { return; }

            EditorGUI.indentLevel++;

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    EditorGUI.BeginChangeCheck();
                    {
                        doImportLightingData     = EditorGUILayout.ToggleLeft(label_LightingData, doImportLightingData);
                        EditorGUI.indentLevel++;
                        EditorGUI.BeginDisabledGroup(!doImportLightingData);
                        {
                            doImportLightingData_ENV = EditorGUILayout.ToggleLeft(label_ENV, doImportLightingData_ENV);
                            doImportLightingData_RML = EditorGUILayout.ToggleLeft(label_RML, doImportLightingData_RML);
                            doImportLightingData_LMS = EditorGUILayout.ToggleLeft(label_LMS, doImportLightingData_LMS);
                            doImportLightingData_OTS = EditorGUILayout.ToggleLeft(label_OTS, doImportLightingData_OTS);
                        }
                        EditorGUI.EndDisabledGroup();
                        EditorGUI.indentLevel--;
                        doImportLights           = EditorGUILayout.ToggleLeft(label_Lights, doImportLights);
                        doImportLightProbeGroups = EditorGUILayout.ToggleLeft(label_LightProbeGroups, doImportLightProbeGroups);
                        doImportReflectionProbes = EditorGUILayout.ToggleLeft(label_ReflectionProbes, doImportReflectionProbes);
                        doDeleteExistingLights   = EditorGUILayout.ToggleLeft(label_ExistingLights, doDeleteExistingLights);
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        EditorPrefs.SetBool(prefsKey_doImportLightingData, doImportLightingData);
                        EditorPrefs.SetBool(prefsKey_doImportLights, doImportLights);
                        EditorPrefs.SetBool(prefsKey_doImportLightProbeGroups, doImportLightProbeGroups);
                        EditorPrefs.SetBool(prefsKey_doImportReflectionProbes, doImportReflectionProbes);
                        EditorPrefs.SetBool(prefsKey_doDeleteExistingLights, doDeleteExistingLights);
                    }
                }

                using (new BackgroundColorScope(importButtonColor))
                {
                    using (new EditorGUILayout.VerticalScope())
                    {
                        EditorGUILayout.Space();
                        if (GUILayout.Button(importTextColorHex + "<size=15><b>Import</b></size></color>",
                            buttonStyle,
                            GUILayout.Height(150),
                            GUILayout.Width(90)))
                        {
                            ImportSceneLightSettingData();
                        }
                        EditorGUILayout.Space();
                    }
                }

                EditorGUILayout.Space();
            }

            EditorGUILayout.LabelField(label_ImportFilePath);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(15);

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField(importDataPath, textStyle, GUILayout.Width(260), GUILayout.ExpandHeight(true));
                EditorGUI.EndDisabledGroup();

                using (new BackgroundColorScope(importButtonColor))
                {
                    if (GUILayout.Button(importTextColorHex + "<b>...</b></color>", buttonStyle, GUILayout.Width(30)))
                    {
                        var openFolderPath = (currentSceneFolderPath != "") ? currentSceneFolderPath : Application.dataPath;
                        var selectedImportDataPath = EditorUtility.OpenFilePanelWithFilters(
                                                        "Select Scene Light Setting Data to Import",
                                                        openFolderPath,
                                                        new string[]{"SceneLightSettingData", "asset"});
                        // 相対パスに変換
                        importDataPath = selectedImportDataPath.Replace(Application.dataPath, "Assets");
                    }
                }

                EditorGUILayout.Space();
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }


        private static void ExportSceneLightSettingData()
        {
            if (EditorApplication.isPlaying == true)
            {
                Debug.LogWarning(message_Playing);
                EditorUtility.DisplayDialog(
                    "Scene Light Setting Export / Import",
                    message_Playing,
                    "OK");
                return;
            }

            if (Directory.Exists(exportDataFolderPath) == false)
            {
                Directory.CreateDirectory(exportDataFolderPath);
                Debug.Log(message_CreateDir);
            }

            var exportDataPath = Path.Combine(exportDataFolderPath, exportDataFileName) + ".asset";
            if (File.Exists(exportDataPath) == true)
            {
                exportDataPath = exportDataPath.Replace(".asset", "_1.asset");
            }
            exportDataPath = AssetDatabase.GenerateUniqueAssetPath(exportDataPath);

            var lightingData = SceneLightSettingExporter.GetSceneLightSettingData(
                                doExportLightingData,
                                doExportLights,
                                doExportLightProbeGroups,
                                doExportReflectionProbes);
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
            Debug.Log(message_DoneExpoted + "  :  exportDataPath = " + exportDataPath, lightingData);

            var displayResult = message_DoneExpoted + "\n\nExportDataPath\n    " + exportDataPath;
            if (lightingData.exportWarningMessages.Length > 0)
            {
                Debug.LogWarning("Scene Light Setting Export Warnings\n" + lightingData.exportWarningMessages);
                displayResult += "\n\nExport Warnings\n    " + lightingData.exportWarningMessages;
            }
            EditorUtility.DisplayDialog("Scene Light Setting Export / Import", displayResult, "OK");
        }

        private static void ImportSceneLightSettingData()
        {
            if (EditorApplication.isPlaying == true)
            {
                Debug.LogWarning(message_Playing);
                EditorUtility.DisplayDialog(
                    "Scene Light Setting Export / Import",
                    message_Playing,
                    "OK");
                return;
            }

            if (importDataPath == "")
            {
                Debug.LogWarning(message_EmptyImportPath);
                EditorUtility.DisplayDialog(
                    "Scene Light Setting Export / Import",
                    message_EmptyImportPath,
                    "OK");
                return;
            }

            var lightingData = AssetDatabase.LoadAssetAtPath<SceneLightSettingData>(importDataPath);
            if (lightingData == null)
            {
                Debug.LogWarning(message_NoLoadLightingData + "  :  importDataPath = " + importDataPath);
                EditorUtility.DisplayDialog(
                    "Scene Light Setting Export / Import",
                    message_NoLoadLightingData + "\n\nImportDataPath\n    " + importDataPath,
                    "OK");
                return;
            }

            // doDeleteExistingLights がtrueだったら、
            // 読み込んだデータを復元する前に既存のライト関係のオブジェクトを削除しておく
            if (doDeleteExistingLights == true)
            {
                SceneLightSettingExporter.DeleteExistingLights(doImportLights, doImportLightProbeGroups, doImportReflectionProbes);
            }

            if (doImportLights == true)
            {
                SceneLightSettingExporter.SetSceneLights(lightingData);
            }

            if (doImportLightProbeGroups == true)
            {
                SceneLightSettingExporter.SetSceneLightProbeGroups(lightingData);
            }

            if (doImportReflectionProbes == true)
            {
                SceneLightSettingExporter.SetSceneReflectionProbes(lightingData);
            }

            if (doImportLightingData == true)
            {
                SceneLightSettingExporter.SetSceneLightSettingData(
                    lightingData,
                    doImportLightingData_ENV,
                    doImportLightingData_RML,
                    doImportLightingData_LMS,
                    doImportLightingData_OTS);
            }

            EditorSceneManager.MarkSceneDirty(currentScene);
            Debug.Log(message_DoneImpoted + "  :  importDataPath = " + importDataPath, lightingData);
            EditorUtility.DisplayDialog(
                "Scene Light Setting Export / Import",
                message_DoneImpoted + "\n\nImportDataPath\n    " + importDataPath,
                "OK");
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
        private static bool CustomFoldout(bool foldout, string content, int width = -1, int height = -1)
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
            if (width > 0)
            {
                toggleRect.width = width;
            }
            if (height > 0)
            {
                toggleRect.height = height;
            }
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

/*
参考サイト
Horizontal Line in Editor Window - Unity Forum
https://forum.unity.com/threads/horizontal-line-in-editor-window.520812/
*/
        public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
        {
            var rect = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
            rect.height = thickness;
            rect.y += padding/2;
            rect.x -= 2;
            rect.width += 6;
            EditorGUI.DrawRect(rect, color);
        }
    }
}