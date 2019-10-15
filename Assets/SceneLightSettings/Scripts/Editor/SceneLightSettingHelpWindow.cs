using UnityEngine;
using UnityEditor;

namespace SceneLightSettings
{
	public class SceneLightSettingHelpWindow : EditorWindow
	{

		void OnGUI()
		{
			EditorGUILayout.LabelField("Help");

			if (GUILayout.Button("Close"))
			{
				this.Close();
			}
		}
	}
}