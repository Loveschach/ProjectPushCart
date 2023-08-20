using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditorInternal;

public enum ShelfType
{
    Top,
    Middle,
    Bottom,
}

[Serializable]
public class StoreShelfData
{
    public Vector3 Position = Vector3.zero;
    public ShelfType Enum = ShelfType.Middle;
    public int UnitLength = 1;
    public int UnitHeight = 1;
    public int UnitDepth = 1;
    public bool IsClone = false;
}
public class StoreShelf : MonoBehaviour
{
    public List<StoreShelfData> ShelfData = new List<StoreShelfData>();

    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Vector3 tRight = transform.InverseTransformDirection(transform.right);
        Vector3 tForward = transform.InverseTransformDirection(transform.forward);
        Vector3 tUp = transform.InverseTransformDirection(transform.up);

        foreach (var shelf in ShelfData)
        {
            Gizmos.color = Color.black;
            Vector3 gridPos = shelf.Position;
            Gizmos.DrawCube(gridPos, new Vector3(0.01f, 0.01f, 0.01f));

            switch (shelf.Enum)
			{
                // _____________________
                case ShelfType.Top:
                {
                    Gizmos.color = Color.red;
                    break;
                }
                // _____________________
                case ShelfType.Middle:
                {
                    Gizmos.color = Color.green;
                    break;
                }
                // _____________________
                case ShelfType.Bottom:
                {
                    Gizmos.color = Color.blue;
                    break;
                }
			}

            if (shelf.IsClone)
                Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.5f);

            gridPos.x -= StoreCreator.GridScale * 0.5f;
            gridPos.y += StoreCreator.GridScale * 0.5f;
            gridPos.z -= StoreCreator.GridScale * 0.5f;

            Vector3 startGridPos = gridPos;

            Gizmos.DrawCube(startGridPos, new Vector3(StoreCreator.GridScale, StoreCreator.GridScale, StoreCreator.GridScale));
            for(int i = 1; i < shelf.UnitLength; ++i)
            {
                gridPos += tRight * -StoreCreator.GridScale;
                Gizmos.DrawWireCube(gridPos, new Vector3(StoreCreator.GridScale, StoreCreator.GridScale, StoreCreator.GridScale));
            }

            gridPos = startGridPos;
            for (int i = 1; i < shelf.UnitDepth; ++i)
            {
                gridPos += tForward * -StoreCreator.GridScale;
                Gizmos.DrawWireCube(gridPos, new Vector3(StoreCreator.GridScale, StoreCreator.GridScale, StoreCreator.GridScale));
            }

            gridPos = startGridPos;
            for (int i = 1; i < shelf.UnitHeight; ++i)
            {
                gridPos += tUp * StoreCreator.GridScale;
                Gizmos.DrawWireCube(gridPos, new Vector3(StoreCreator.GridScale, StoreCreator.GridScale, StoreCreator.GridScale));
            }
        }
    }
}

[CustomEditor(typeof(StoreShelf))]
public class StoreShelfClassEditor : Editor
{
    // This will be the serialized "copy" of YourOtherClass.ShelfData
    private SerializedProperty ShelfData;

    private ReorderableList YourReorderableList;

    private void OnEnable()
    {
        // Step 1 "link" the SerializedProperties to the properties of YourOtherClass
        ShelfData = serializedObject.FindProperty("ShelfData");

        // Step 2 setup the ReorderableList
        YourReorderableList = new ReorderableList(serializedObject, ShelfData)
        {
            draggable = true,
            displayAdd = true,
            displayRemove = true,
            drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Shelf Data");
            },

            // Now to the interesting part: Here you setup how elements look like
            drawElementCallback = (rect, index, active, focused) =>
            {
                // Get the currently to be drawn element from ShelfData
                var element = ShelfData.GetArrayElementAtIndex(index);

                // Get the elements Properties into SerializedProperties
                var Position = element.FindPropertyRelative("Position");
                var Enum = element.FindPropertyRelative("Enum");

                // only show Step field if selected "seconds"
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), Position);
                rect.y += EditorGUIUtility.singleLineHeight;

                // Draw the Enum field
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), Enum);
                rect.y += EditorGUIUtility.singleLineHeight;

                // Draw the Unit fields
                var UnitLength = element.FindPropertyRelative("UnitLength");
                var UnitHeight = element.FindPropertyRelative("UnitHeight");
                var UnitDepth = element.FindPropertyRelative("UnitDepth");
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), UnitLength);
                rect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), UnitHeight);
                rect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), UnitDepth);
                rect.y += EditorGUIUtility.singleLineHeight;

                // Draw the IsClone field
                var IsClone = element.FindPropertyRelative("IsClone");
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), IsClone);
                rect.y += EditorGUIUtility.singleLineHeight;

            },

            // And since we have more than one line (default) you'll have to configure 
            // how tall your elements are. Luckyly in your example it will always be exactly
            // 3 Lines in each case. If not you would have to change this.
            // In some cases it becomes also more readable if you use one more Line as spacer between the elements
            elementHeight = EditorGUIUtility.singleLineHeight * 6,

            // optional: Set default Values when adding a new element
            // (otherwise the values of the last list item will be copied)
            onAddCallback = list =>
            {
                // The new index will be the current List size ()before adding
                var index = list.serializedProperty.arraySize;

                // Since this method overwrites the usual adding, we have to do it manually:
                // Simply counting up the array size will automatically add an element
                list.serializedProperty.arraySize++;
                list.index = index;
                var element = list.serializedProperty.GetArrayElementAtIndex(index);

                // again link the properties of the element in SerializedProperties
                var Position = element.FindPropertyRelative("Position");
                var Enum = element.FindPropertyRelative("Enum");
                var UnitLength = element.FindPropertyRelative("UnitLength");
                var UnitHeight = element.FindPropertyRelative("UnitHeight");
                var UnitDepth = element.FindPropertyRelative("UnitDepth");
                var IsClone = element.FindPropertyRelative("IsClone");

                // and set default values
                Enum.intValue = (int)ShelfType.Middle;
                UnitLength.intValue = 1;
                UnitHeight.intValue = 1;
                UnitDepth.intValue = 1;
                IsClone.boolValue = false;
                Position.vector3Value = Vector3.zero;
            }
        };
    }

    public override void OnInspectorGUI()
    {
        // copy the values of the real Class to the linked SerializedProperties
        serializedObject.Update();

        // print the reorderable list
        YourReorderableList.DoLayoutList();

        // apply the changed SerializedProperties values to the real class
        serializedObject.ApplyModifiedProperties();
    }
}
