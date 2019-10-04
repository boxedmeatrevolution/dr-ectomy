/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PieceComponent))]
public class PieceEditor : Editor {
    
    bool showShape;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        PieceComponent piece = (PieceComponent)target;

        showShape = EditorGUILayout.Foldout(showShape, "Shape");
        if (showShape) {
            EditorGUI.indentLevel++;
            Vector2Int shapeBounds = new Vector2Int(piece.shape.GetLength(1), piece.shape.GetLength(0));
            shapeBounds = EditorGUILayout.Vector2IntField("Size", shapeBounds);

            bool[,] oldShape = piece.shape;
            piece.shape = new bool[shapeBounds.y, shapeBounds.x];

            for (int i = 0; i < Math.Min(oldShape.GetLength(0), piece.shape.GetLength(0)); ++i) {
                for (int j = 0; j < Math.Min(oldShape.GetLength(1), piece.shape.GetLength(1)); ++j) {
                    piece.shape[i, j] = oldShape[i, j];
                }
            }
            
            EditorGUILayout.BeginVertical();
            for (int i = 0; i < piece.shape.GetLength(0); ++i) {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < piece.shape.GetLength(1); ++j) {
                    piece.shape[i, j] = EditorGUILayout.Toggle(piece.shape[i, j]);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            
            EditorGUI.indentLevel--;
        }
    }

}
*/