using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(bl_MiniMapItem))]
public class bl_MiniMapItemEditor : Editor
{


    public override void OnInspectorGUI()
    {
        bl_MiniMapItem script = (bl_MiniMapItem)target;
        bool allowSceneObjects = !EditorUtility.IsPersistent(target);
        serializedObject.Update();

        GUILayout.BeginVertical("box");
        script.Target = EditorGUILayout.ObjectField("Target", script.Target, typeof(Transform), allowSceneObjects) as Transform;
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Icon Settings", EditorStyles.toolbarButton);
        GUILayout.Space(4);
        script.Icon = EditorGUILayout.ObjectField("Icon", script.Icon, typeof(Sprite), allowSceneObjects) as Sprite;
        script.DeathIcon = EditorGUILayout.ObjectField("Death Icon", script.DeathIcon, typeof(Sprite), allowSceneObjects) as Sprite;
        script.Size = EditorGUILayout.Slider("Icon Size", script.Size, 1, 100);
        script.RenderDelay = EditorGUILayout.Slider("Start render delay", script.RenderDelay, 0, 3);
        script.IconColor = EditorGUILayout.ColorField("Icon Color", script.IconColor);
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Settings", EditorStyles.toolbarButton);
        script.OffScreen = EditorGUILayout.ToggleLeft("Show icon off screen", script.OffScreen, EditorStyles.toolbarButton);
        if (script.OffScreen)
        {
            script.BorderOffScreen = EditorGUILayout.Slider("Border Off set", script.BorderOffScreen, 0, 5);
            script.OffScreenSize = EditorGUILayout.Slider("Off screen icon size", script.OffScreenSize, 1, 50);
        }
        GUILayout.Space(5);
        script.m_Effect = (ItemEffect)EditorGUILayout.EnumPopup("Loop Effect", script.m_Effect,EditorStyles.toolbarDropDown);
        script.DestroyWithObject = EditorGUILayout.ToggleLeft("Destroy with Object", script.DestroyWithObject, EditorStyles.toolbarButton);
        script.useCustomIconPrefab = EditorGUILayout.ToggleLeft("Use Custom Icon Prefab", script.useCustomIconPrefab, EditorStyles.toolbarButton);
        if (script.useCustomIconPrefab)
        {
            script.CustomIconPrefab = EditorGUILayout.ObjectField("Custom Icon Prefab", script.CustomIconPrefab, typeof(GameObject), false) as GameObject;
        }
        GUILayout.EndVertical();

        if (GUI.changed)
            EditorUtility.SetDirty(script);
    }
}