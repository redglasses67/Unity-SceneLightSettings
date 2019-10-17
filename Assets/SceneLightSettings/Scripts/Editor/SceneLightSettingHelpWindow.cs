using UnityEngine;
using UnityEditor;

namespace SceneLightSettings
{
    public class SceneLightSettingHelpWindow : EditorWindow
    {
		public static readonly Vector2 windowSize = new Vector2(300, 200);

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
				message_Help    = "SceneLightingData フォルダを作成しました.\n\n" + 
									"SceneLightingData フォルダを作成しました.\n\n" +
									"SceneLightingData フォルダを作成しました.\n\n" +
									"SceneLightingData フォルダを作成しました.\n\n";
			}
			else
			{
				message_Author  = "Author";
				message_Version = "Version";
				message_Help    = "Created SceneLightingData Folder\n\n" +
									"Created SceneLightingData Folder\n\n" +
									"Created SceneLightingData Folder\n\n" +
									"Created SceneLightingData Folder\n\n";
			}
		}


        void OnGUI()
        {
			labelStyle            = new GUIStyle();
            labelStyle.richText   = true;
            labelStyle.font       = GUI.skin.font;
            labelStyle.alignment  = TextAnchor.MiddleLeft;
			labelStyle.wordWrap   = true;

            EditorGUILayout.LabelField(message_Author, "redglasses67");
			EditorGUILayout.LabelField(message_Version, "1.0");

			using (new EditorGUILayout.VerticalScope("Box"))
            {
				EditorGUILayout.LabelField(message_Help, labelStyle);
			}

			using (new EditorGUILayout.HorizontalScope())
            {
				EditorGUILayout.Space();
				if (GUILayout.Button("Close", GUILayout.Width(200)))
				{
					this.Close();
				}
				EditorGUILayout.Space();
			}
        }
    }
}