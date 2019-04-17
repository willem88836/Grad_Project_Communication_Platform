#if UNITY_EDITOR
using UnityEngine;
using Framework.Utils;

namespace Framework.Variables
{
	using System;
	using System.Reflection;
	using UnityEditor;

	[CustomPropertyDrawer(typeof(RangeAttribute))]
	public sealed class Int2RangeDrawer : PropertyDrawer
	{
		const int BOXHEIGHT = 16;
		const int BOXINTERVAL = 18;
		const string X = "X";
		const string Y = "Y";
		const string NOTSUPPORTEDTEXT = "({0})\tobject of type: ({1}) is not supported by the Range attribute.";
		const string TAB = "     ";


		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			object targetObject = property.serializedObject.targetObject;
			object o = targetObject.GetType().GetField(property.propertyPath).GetValue(targetObject);

			return o.GetType() == typeof(Int2)
				? base.GetPropertyHeight(property, label) * 2 + 20
				: base.GetPropertyHeight(property, label);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			RangeAttribute rangeAttribute = (RangeAttribute)attribute;
			float min = rangeAttribute.min;
			float max = rangeAttribute.max;

			object targetObject = property.serializedObject.targetObject;
			FieldInfo fi = targetObject.GetType().GetField(property.propertyPath);
			if (fi == null)
				return;

			object o = fi.GetValue(targetObject);

			Type t = o.GetType();

			if (t == typeof(Int2))
			{
				EditorGUI.LabelField(position, label.text);
				DrawIntSlider(1, position, property.FindPropertyRelative(X), (int)min, (int)max, TAB);
				DrawIntSlider(2, position, property.FindPropertyRelative(Y), (int)min, (int)max, TAB);
			}
			else if (o.IsDecimalNumber())
			{
				EditorGUI.Slider(position, property, min, max);
			}
			else if (o.IsNonDecimalNumber())
			{
				DrawIntSlider(0, position, property, (int)min, (int)max);
			}
			else
			{
				EditorGUI.LabelField(position, string.Format(NOTSUPPORTEDTEXT, label.text, t));
			}
		}

		private void DrawIntSlider(int i, Rect position, SerializedProperty property, int min, int max, string labelPrefix = "")
		{
			Rect pos = new Rect(position.x, position.y + i * BOXINTERVAL, position.width, BOXHEIGHT);
			EditorGUI.IntSlider(pos, property, min, max, labelPrefix + property.name);
		}
	}
}
#endif
