using UnityEngine;
using UnityEditor;

namespace SceneLightSettings
{
    public class SceneLightSettingHelpWindow : EditorWindow
    {
        public static readonly Vector2 windowSize = new Vector2(310, 330);

        private static GUIStyle labelStyle;
        private static GUIStyle messageStyle;

        private static string message_Author;
        private static string message_Version;
        private static string message_Help;

        private void OnEnable()
        {
            if (Application.systemLanguage == SystemLanguage.Japanese)
            {
                message_Author  = "作者";
                message_Version = "バージョン";
                message_Help    = "このツールは、チェックを付けられた下記のデータを\n" +
                                    "ScriptableObject でエクスポート/インポートします。\n" +
                                    "    - シーンの Lighting ウィンドウ内の設定\n" +
                                    "    - シーン内の Light オブジェクト\n" +
                                    "    - シーン内の LightProbeGroup オブジェクト\n" +
                                    "    - シーン内の ReflectionProbe オブジェクト\n" +
                                    "    - (インポート時のみ) シーン内の\n" +
                                    "         Light関連オブジェクトを削除するかどうか\n" +
                                    "\n" +
                                    "<size=11>Export</size>\n" +
                                    "    現在のシーンの保存先のパスに\n" +
                                    "    SceneLightingData フォルダを作成し、\n" +
                                    "    そこに SceneLightingData + シーン名 の\n" +
                                    "    ファイル名で出力されます。\n" +
                                    "\n" +
                                    "<size=11>Import</size>\n" +
                                    "    他のシーンで読み込む場合は\n" +
                                    "    出力した ScriptableObject のパスを\n" +
                                    "    『Import File Path』に設定して下さい。";
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
            if (labelStyle == null)
            {
                labelStyle           = new GUIStyle();
                labelStyle.richText  = true;
                labelStyle.font      = GUI.skin.font;
            }

            if (messageStyle == null)
            {
                messageStyle           = new GUIStyle();
                messageStyle.richText  = true;
                messageStyle.font      = GUI.skin.font;
                messageStyle.alignment = TextAnchor.MiddleLeft;
                messageStyle.wordWrap  = true;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b><size=11>" + SceneLightSettingExporterWindow.label_title + " Help</size></b>", labelStyle, GUILayout.Width(300));
                EditorGUILayout.Space();
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(message_Author, labelStyle, GUILayout.Width(50));
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField("redglasses67", labelStyle, GUILayout.Width(100));
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(message_Version, labelStyle, GUILayout.Width(60));
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField("1.0", labelStyle, GUILayout.Width(40));
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.Space();
                EditorGUIUtility.labelWidth = 0;
            }

            using (new EditorGUILayout.VerticalScope("Box"))
            {
                EditorGUILayout.LabelField(message_Help, messageStyle);
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