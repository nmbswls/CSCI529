using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomPropertyDrawer(typeof(CardEffectBranch))]
public class CardEffectDrawer : PropertyDrawer
{


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        using (new EditorGUI.PropertyScope(position, label, property))
        {

            EditorGUIUtility.labelWidth = 60;
            position.height = EditorGUIUtility.singleLineHeight;
            //EditorGUILayout.Space();
            Rect r1 = new Rect(position)
            {
                width = 100,
                height = 60
            };

            SerializedProperty nameProperty = property.FindPropertyRelative("effect");
            nameProperty.stringValue = EditorGUI.TextField(r1, nameProperty.displayName, nameProperty.stringValue);

            //EditorGUI.PrefixLabel(r1,new GUIContent("name"));
            //EditorGUI.PropertyField(r1, nameProperty,);

        }

    }
}
