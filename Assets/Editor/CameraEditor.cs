using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraController)), CanEditMultipleObjects]
public class CameraEditor : Editor {

	public SerializedProperty
		cameraMode_Prop,
		mat_Prop,
		scrollerSpeed_Prop;

	void OnEnable() {
		cameraMode_Prop = serializedObject.FindProperty("cameraType");
		mat_Prop = serializedObject.FindProperty("gridMaterial");
		scrollerSpeed_Prop = serializedObject.FindProperty("scrollerSpeed");
	}

	public override void OnInspectorGUI() {
		serializedObject.Update();

		EditorGUILayout.PropertyField(cameraMode_Prop);
		EditorGUILayout.PropertyField(mat_Prop);

		//Get which enum is selected.
		CameraController.CameraType cameraMode = (CameraController.CameraType)cameraMode_Prop.enumValueIndex;

		if(cameraMode == CameraController.CameraType.Scroller) {
			GUIStyle tStyle = GUI.skin.GetStyle("Label");
			tStyle.alignment = TextAnchor.UpperCenter;

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			EditorGUILayout.LabelField("Scroller Options", tStyle);
			EditorGUILayout.PropertyField(scrollerSpeed_Prop);
		}

		serializedObject.ApplyModifiedProperties();
	}
}