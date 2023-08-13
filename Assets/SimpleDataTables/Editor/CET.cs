﻿using UnityEngine;
using UnityEditor;

namespace CustomEditorTools
{
    /// <summary>
    /// A set of editor tools used to speed up custom editor creation
    /// </summary>
    static public class CET
    {
        /// <summary>
        /// Creates a 1X1 Images with the input color. Use to color editor backgrounds 
        /// </summary>
        /// <param name="Color">Color of background image</param>
        static public Texture2D MakeEditorBackgroundColor (Color Color)
        {
            Texture2D t = new Texture2D(1, 1);
            t.SetPixel(0, 0, Color);
            t.Apply();
            return t;
        }

        /// <summary>
        /// Draw a simple dividing line on an custom editor 
        /// </summary>
        static public void HorizontalLine ()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }
    }
}