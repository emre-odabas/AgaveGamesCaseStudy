/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AgaveGames.Data;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{
    private int fixedEnumWidth = 85;
    private int rowHeight = 25;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LevelData data = (LevelData)target;

        if (data.Grid != Vector2Int.zero)
        {
            EditorGUILayout.Space();

            EditorGUI.indentLevel = 0;

            GUIStyle boardStyle = new GUIStyle("box");
            boardStyle.padding = new RectOffset(5, 5, 5, 5);
            boardStyle.margin.left = 16;

            GUIStyle headerY = new GUIStyle();
            headerY.fixedHeight = 35;

            GUIStyle colStyle = new GUIStyle();
            colStyle.fixedWidth = fixedEnumWidth;

            GUIStyle rowStyle = new GUIStyle();
            rowStyle.fixedHeight = rowHeight;

            GUIStyle rowHeaderStyle = new GUIStyle();
            rowHeaderStyle.fixedWidth = colStyle.fixedWidth - 1;

            GUIStyle colHeaderStyle = new GUIStyle();
            colHeaderStyle.fixedWidth = 32;
            colHeaderStyle.fixedHeight = rowHeight;

            GUIStyle colLabelStyle = new GUIStyle();
            colLabelStyle.fixedWidth = rowHeaderStyle.fixedWidth - 6;
            colLabelStyle.alignment = TextAnchor.MiddleCenter;
            colLabelStyle.fontStyle = FontStyle.Bold;

            //Define the label of the corner. It will read (Row, Col)
            GUIStyle cornerLabelStyle = new GUIStyle();
            cornerLabelStyle.fixedWidth = 64;
            cornerLabelStyle.alignment = TextAnchor.UpperLeft;
            cornerLabelStyle.fontStyle = FontStyle.BoldAndItalic;
            cornerLabelStyle.fontSize = 12;
            cornerLabelStyle.padding.top = -5;

            GUIStyle rowLabelStyle = new GUIStyle();
            rowLabelStyle.fixedWidth = 15;
            rowLabelStyle.alignment = TextAnchor.MiddleLeft;
            rowLabelStyle.fontStyle = FontStyle.Bold;

            GUIStyle enumStyle = new GUIStyle("popup");
            enumStyle.fixedWidth = fixedEnumWidth;

            EditorGUILayout.BeginHorizontal(boardStyle);
            for (int col = -1; col < data.Grid.y; col++)
            {
                EditorGUILayout.BeginVertical((col == -1) ? headerY : colStyle);
                for (int row = -1; row < data.Grid.x; row++)
                {
                    //First, draw corner with to display row and col
                    if (col == -1 && row == -1)
                    {
                        EditorGUILayout.BeginVertical(rowHeaderStyle);
                        EditorGUILayout.LabelField("Row,Col", cornerLabelStyle);
                        EditorGUILayout.EndHorizontal();
                    }
                    else if (col == -1)
                    {
                        EditorGUILayout.BeginVertical(colHeaderStyle);
                        EditorGUILayout.LabelField(row.ToString(), rowLabelStyle);
                        EditorGUILayout.EndHorizontal();
                    }
                    else if (row == -1)
                    {
                        EditorGUILayout.BeginVertical(rowHeaderStyle);
                        EditorGUILayout.LabelField(col.ToString(), colLabelStyle);
                        EditorGUILayout.EndHorizontal();
                    }

                    if (col >= 0 && row >= 0)
                    {
                        EditorGUILayout.BeginHorizontal(rowStyle);
                        //redo grid if the size changed
                        if (data.Grid.x < row || data.Grid.y < col || data.boardInfo.Length != (data.Grid.x * data.Grid.y))
                            data.SetBoard(data.Grid);
                        int index = data.GetIndex(row, col);
                        data.boardInfo[index] = (GridInfo)EditorGUILayout.EnumPopup(data.boardInfo[index], enumStyle);
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        } //end if data grid != 0

        


    }
}
*/

