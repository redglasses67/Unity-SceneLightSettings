using UnityEngine;
using UnityEditor;

namespace SceneLightSettings
{
    public class SceneLightSettingHelpWindow : EditorWindow
    {
        public static readonly Vector2 windowSize = new Vector2(330, 280);

        private static GUIStyle labelStyle;

        private static string message_Author;
        private static string message_Version;
        private static string message_Help;

        private void OnEnable()
        {
            if (Application.systemLanguage == SystemLanguage.Japanese)
            {
                message_Author  = "作者";
                message_Version = "バージョン";
                message_Help    = "このツールは下記のデータを ScriptableObject で出力します.\n" +
                                    "\n" +
                                    "    ● シーンの Lighting ウィンドウ内の設定\n" +
                                    "    ● シーン内の Light オブジェクト\n" +
                                    "    ● シーン内の LightProbeGroup オブジェクト\n" +
                                    "    ● シーン内の ReflectionProbe オブジェクト\n" +
                                    "\n" +
                                    "データは,現在のシーンの保存先のパスに\n" +
                                    "『SceneLightingData』というフォルダを作成し,\n" +
                                    "SceneLightingData+シーン名 のファイル名で出力されます.\n" +
                                    "\n" +
                                    "他のシーンで保存したデータをImportする場合は\n" +
                                    "出力した ScriptableObject のパスを\n" +
                                    "『Import File Path』に設定して下さい.";
            }
            else
            {
                message_Author  = "Author";
                message_Version = "Version";
                message_Help    = "This tool exports data below as ScriptableObject.\n" +
                                    "\n" +
                                    "    - Lighting window settings in the current scene\n" +
                                    "    - Lights objects in the current scene\n" +
                                    "    - LightProbeGroup objects in the current scene\n" +
                                    "    - ReflectionProbe objects in the current scene\n" +
                                    "\n" +
                                    "The data name is 'SceneLightingData' + 'scene name',\n" +
                                    "and a 'SceneLightingData' folder is created\n" +
                                    "and exported to that location.\n" +
                                    "\n" +
                                    "When importing data exported from other scene,\n" +
                                    "please set the exported ScriptableObject path\n" +
                                    "to 'Import File Path'.";
            }
        }


        void OnGUI()
        {
            labelStyle            = new GUIStyle();
            labelStyle.richText   = true;
            labelStyle.font       = GUI.skin.font;
            labelStyle.alignment  = TextAnchor.MiddleLeft;
            labelStyle.wordWrap   = true;

            EditorGUILayout.LabelField("<size=13><b>" + SceneLightSettingExporterWindow.label_title + " Help</b></size>", labelStyle);

            EditorGUIUtility.labelWidth = 80;
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField(message_Author, "redglasses67", GUILayout.Width(200));
            EditorGUILayout.LabelField(message_Version, "1.0", GUILayout.Width(200));
            EditorGUI.indentLevel--;
            EditorGUIUtility.labelWidth = 0;

            using (new EditorGUILayout.VerticalScope("Box"))
            {
                EditorGUILayout.LabelField(message_Help, labelStyle);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.Space();
                if (GUILayout.Button("Close", GUILayout.Height(30), GUILayout.Width(200)))
                {
                    this.Close();
                }
                EditorGUILayout.Space();
            }
        }
    }
}