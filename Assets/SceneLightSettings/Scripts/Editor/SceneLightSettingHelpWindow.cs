using UnityEngine;
using UnityEditor;

namespace SceneLightSettings
{
    public class SceneLightSettingHelpWindow : EditorWindow
    {
        public static readonly Vector2 windowSize = new Vector2(316, 320);

        private static string textBaseColorHex;
        private static GUIStyle labelStyle;
        private static GUIStyle textFieldStyle;
        private static GUIStyle messageStyle;

        private static string message_Author;
        private const string authorName = "redglasses67";
        private static string message_Version;
        private const string version = "1.0";
        private static string message_Help;

        private static Vector2 scrollPos;

        private void OnEnable()
        {
            textBaseColorHex = (EditorGUIUtility.isProSkin == true) ? "<color=silver>" : "<color=black>";

            if (Application.systemLanguage == SystemLanguage.Japanese)
            {
                message_Author  = "作者";
                message_Version = "バージョン";
                message_Help    = textBaseColorHex + "このツールは、チェックを付けられた下記のデータを\n" +
                                    "ScriptableObject でエクスポート/インポートします。\n" +
                                    "    - シーンの Lighting ウィンドウ内の設定\n" +
                                    "    - シーン内の Light オブジェクト\n" +
                                    "    - シーン内の LightProbeGroup オブジェクト\n" +
                                    "    - シーン内の ReflectionProbe オブジェクト\n" +
                                    "    - ( インポート時のみ ) シーン内の既存の\n" +
                                    "         Light関連オブジェクトを削除するかどうか\n" +
                                    "\n" +
                                    "<size=12><b>Export</b></size>\n" +
                                    "    現在開いているシーンの保存先のパスに\n" +
                                    "    SceneLightSettingData フォルダを作成し、\n" +
                                    "    そこに </color><color=red>SceneLightSettingData + シーン名</color>" + textBaseColorHex + " の\n" +
                                    "    ファイル名で出力されます。\n" +
                                    "\n" +
                                    "<size=12><b>Import</b></size>\n" +
                                    "    他のシーンで読み込む場合は\n" +
                                    "    出力した ScriptableObject のパスを\n" +
                                    "    <color=blue>Import File Path</color> に設定して下さい。\n" +
                                    "\n" +
                                    " ( このスクリプトの動作確認は\n" +
                                    "    Unityの <size=12><b>2017.4 以降</b></size> で行っています。 )\n" +
                                    "================================\n" +
                                    "\n" +
                                    "更新履歴\n" +
                                    "    Ver 1.0 リリース" +
                                    "</color>";
            }
            else
            {
                message_Author  = "Author";
                message_Version = "Version";
                message_Help    = textBaseColorHex + "This tool export and import \n" +
                                    "checked data below as ScriptableObject.\n" +
                                    "\n" +
                                    "    - Lighting window settings in the current scene\n" +
                                    "    - Light objects in the current scene\n" +
                                    "    - LightProbeGroup objects in the current scene\n" +
                                    "    - ReflectionProbe objects in the current scene\n" +
                                    "    - ( Import only ) Whether to delete \n" +
                                    "         existing light objects in the current scene\n" +
                                    "\n" +
                                    "<size=12><b>Export</b></size>\n" +
                                    "    The data name is </color><color=red>'SceneLightSettingData' + </color>\n" +
                                    "    <color=red>'scene name'</color>" + textBaseColorHex + ", and a SceneLightSettingData folder\n" +
                                    "    is created in saving path in the current scene.\n" +
                                    "    Then it exports to that location.\n" +
                                    "\n" +
                                    "<size=12><b>Import</b></size>\n" +
                                    "    When importing data exported\n" +
                                    "    from other scene, please set the exported\n" +
                                    "    ScriptableObject path to <color=blue>Import File Path</color>.\n" +
                                    "\n" +
                                    " ( The operation check of this script was done\n" +
                                    "    with Unity version <size=12><b>2017.4 or newer</b></size>. )\n" +
                                    "================================\n" +
                                    "\n" +
                                    "Change Log\n" +
                                    "    Ver 1.0 release" +
                                    "</color>";
            }
        }


        void OnGUI()
        {
            if (labelStyle == null)
            {
                labelStyle           = new GUIStyle();
                labelStyle.richText  = true;
                labelStyle.font      = GUI.skin.font;
                labelStyle.alignment = TextAnchor.MiddleCenter;
            }

            if (textFieldStyle == null)
            {
                textFieldStyle           = new GUIStyle(GUI.skin.textField);
                textFieldStyle.richText  = true;
                textFieldStyle.font      = GUI.skin.font;
                textFieldStyle.alignment = TextAnchor.MiddleCenter;
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
                EditorGUILayout.LabelField(
                    textBaseColorHex + "<b><size=11>" + SceneLightSettingExporterWindow.label_title + " Help</size></b></color>",
                    labelStyle,
                    GUILayout.Width(300));
                EditorGUILayout.Space();
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(message_Author, GUILayout.Width(40));
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField(authorName, textFieldStyle, GUILayout.Width(100));
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(message_Version, GUILayout.Width(60));
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField(version, textFieldStyle, GUILayout.Width(40));
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.Space();
            }

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUI.skin.box, GUILayout.Height(240));
            {
                EditorGUILayout.LabelField(message_Help, messageStyle);
            }
            EditorGUILayout.EndScrollView();

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