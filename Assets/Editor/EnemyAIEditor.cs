using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyAI)), CanEditMultipleObjects]
public class EnemyAIEditor : Editor {

	/*public SerializedProperty
		lookAtHero_Prop,        //Type of Behavior ( LeftRight, TopDown, WaveLR )
		abBool_Prop,        //Type of Behavior ( LeftRight, TopDown, WaveLR )
		abList_Prop,		//Type of Behavior ( LeftRight, TopDown, WaveLR )
		behaviorType_Prop,	//Type of Behavior ( LeftRight, TopDown, WaveLR )
		flip_Prop,			//Does the model flip vertically when reversing direction on Y axis.
		distance_Prop,		//Set a max distance ( from Character's origin ) the character can move to ( reverse direction when hit ).
		iWalls_Prop,        //Does the character ignore walls collisions ( Only usable when a max distance is set ).
		iHoles_Prop,        //Does the character ignore holes ( Only usable when a max distance is set ).
		movSpeed_Prop,		//Speed at which the character moves.
		angleSpeed_Prop,	//Speed at which the angle changes ( used for the Wave type behavior ).
		maxAngle_Prop;		//Max angle the character goes to before reversing anglespeed ( used for Wave type behavior ).

	void OnEnable() {
		lookAtHero_Prop = serializedObject.FindProperty("lookAtHero");
		abBool_Prop = serializedObject.FindProperty("useAdvancedBehaviors");
		abList_Prop = serializedObject.FindProperty("abList");
		behaviorType_Prop = serializedObject.FindProperty("basicBehavior");
		flip_Prop = serializedObject.FindProperty("flipVertically");
		distance_Prop = serializedObject.FindProperty("distance");
		iWalls_Prop = serializedObject.FindProperty("ignoreWalls");
		iHoles_Prop = serializedObject.FindProperty("ignoreHoles");
		movSpeed_Prop = serializedObject.FindProperty("movementSpeed");
		angleSpeed_Prop = serializedObject.FindProperty("angleSpeed");
		maxAngle_Prop = serializedObject.FindProperty("maxAngle");
	}

	public override void OnInspectorGUI() {
		serializedObject.Update();

		//Shows advanced behavior list in the inspector.
		EditorGUILayout.PropertyField(abBool_Prop, new GUIContent("Use AB", "Does this AI use Advanced Behaviors?"));
		EditorGUILayout.PropertyField(lookAtHero_Prop, new GUIContent("lookAtHero"));
		EditorGUILayout.PropertyField(behaviorType_Prop);       //Shows basic behavior type in the inspector.
		if(abBool_Prop.boolValue) {
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			EditorGUILayout.LabelField("Advanced Behaviors : ");
			abList_Prop.arraySize = 4;

			//Cannot use dash attack and jump attack behaviors at the same time.
			if(abList_Prop.GetArrayElementAtIndex(1).boolValue)
				abList_Prop.GetArrayElementAtIndex(2).boolValue = false;

			EditorGUILayout.PropertyField(abList_Prop.GetArrayElementAtIndex(0), new GUIContent("Avoid Hero", "Run away from hero if within set radius."));
			EditorGUILayout.PropertyField(abList_Prop.GetArrayElementAtIndex(1), new GUIContent("Radius Jump Attack", "Jump torwards the hero if within set radius."));
			EditorGUILayout.PropertyField(abList_Prop.GetArrayElementAtIndex(2), new GUIContent("Radius Dash ATtack", "Charge hero at current location in a straight line if within set radius than come back to original position."));
			EditorGUILayout.PropertyField(abList_Prop.GetArrayElementAtIndex(3), new GUIContent("Follow Hero", "Follow hero if within set radius. Can also modify movement speed while following."));
		}
		else
			abList_Prop.arraySize = 0;                          //Makes sure the list is null if not used.
					

		//Get which enum is selected.
		EnemyAI.BasicBehavior behaviorType = (EnemyAI.BasicBehavior)behaviorType_Prop.enumValueIndex;

		//Get chosen distance.
		float distance = distance_Prop.floatValue;

		//Shows distance in the inspector if behavior type isn't Motionless.
		if(behaviorType != EnemyAI.BasicBehavior.Motionless) {
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			EditorGUILayout.PropertyField(distance_Prop);
			
			//Only shows ignoreWalls option in inspector if a distance is set.
			if(distance != 0.0f)
				EditorGUILayout.PropertyField(iWalls_Prop, new GUIContent("ignoreWalls"));
		}

		//Shows certain variables depending on which behavior type is selected.
		switch(behaviorType) {
			case EnemyAI.BasicBehavior.LeftRight:
				if(distance != 0.0f)
					EditorGUILayout.PropertyField(iHoles_Prop, new GUIContent("ignoreHoles"));

				EditorGUILayout.PropertyField(movSpeed_Prop, new GUIContent("movementSpeed"));
			break;
			
			case EnemyAI.BasicBehavior.TopDown:
				EditorGUILayout.PropertyField(movSpeed_Prop, new GUIContent("movementSpeed"));
				EditorGUILayout.PropertyField(flip_Prop, new GUIContent("flipVertically"));
			break;

			case EnemyAI.BasicBehavior.WaveLR:
				EditorGUILayout.PropertyField(movSpeed_Prop, new GUIContent("movementSpeed"));
				EditorGUILayout.PropertyField(angleSpeed_Prop, new GUIContent("angleSpeed"));
				EditorGUILayout.PropertyField(maxAngle_Prop, new GUIContent("maxAngle"));
			break;

			default:
			break;
		}

		serializedObject.ApplyModifiedProperties();
	}*/
}