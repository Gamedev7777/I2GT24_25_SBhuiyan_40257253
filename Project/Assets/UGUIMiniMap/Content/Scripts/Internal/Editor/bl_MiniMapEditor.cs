using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine.UI;
using UnityEditor.SceneManagement;
using UGUIMiniMap;

[CustomEditor(typeof(bl_MiniMap))]
public class bl_MiniMapEditor : Editor
{
    AnimBool GeneralAnim;
    protected static bool ShowGeneral = false;
    AnimBool ZoomAnim;
    protected static bool ShowZoom = false;
    AnimBool RotationAnim;
    protected static bool ShowRotation = false;
    AnimBool GripAnim;
    protected static bool ShowGrip = false;
    AnimBool PositionAnim;
    protected static bool ShowPosition = false;
    AnimBool AnimationsAnim;
    protected static bool ShowAnimation = false;
    AnimBool DragAnim;
    protected static bool ShowDrag = false;
    AnimBool RenderAnim;
    protected static bool ShowRender = false;
    AnimBool ReferencesAnim;
    protected static bool ShowReferences = false;
    AnimBool MarksAnim;
    protected static bool ShowMarks = false;

    private void OnEnable()
    {
        GeneralAnim = new AnimBool(ShowGeneral);
        GeneralAnim.valueChanged.AddListener(Repaint);
        ZoomAnim = new AnimBool(ShowZoom);
        ZoomAnim.valueChanged.AddListener(Repaint);
        RotationAnim = new AnimBool(ShowRotation);
        RotationAnim.valueChanged.AddListener(Repaint);
        GripAnim = new AnimBool(ShowGrip);
        GripAnim.valueChanged.AddListener(Repaint);
        PositionAnim = new AnimBool(ShowPosition);
        PositionAnim.valueChanged.AddListener(Repaint);
        AnimationsAnim = new AnimBool(ShowAnimation);
        AnimationsAnim.valueChanged.AddListener(Repaint);
        DragAnim = new AnimBool(ShowDrag);
        DragAnim.valueChanged.AddListener(Repaint);
        RenderAnim = new AnimBool(ShowRender);
        RenderAnim.valueChanged.AddListener(Repaint);
        ReferencesAnim = new AnimBool(ShowReferences);
        ReferencesAnim.valueChanged.AddListener(Repaint);
        MarksAnim = new AnimBool(ShowMarks);
        MarksAnim.valueChanged.AddListener(Repaint);
    }

    void CheckLayer(bl_MiniMap script)
    {
        string layer = LayerMask.LayerToName(script.MiniMapLayer);
        if (string.IsNullOrEmpty(layer))
        {
            CreateLayer("MiniMap");
            int layerID = LayerMask.NameToLayer("MiniMap");
            script.MiniMapLayer = layerID;
        }
    }

    public override void OnInspectorGUI()
    {
        bl_MiniMap script = (bl_MiniMap)target;
        bool allowSceneObjects = !EditorUtility.IsPersistent(target);
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        CheckLayer(script);
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical("window");

        EditorGUILayout.BeginVertical("box");
        if (GUILayout.Button("General Settings", EditorStyles.toolbarPopup)) { ShowGeneral = !ShowGeneral; GeneralAnim.target = ShowGeneral; }
        if (EditorGUILayout.BeginFadeGroup(GeneralAnim.faded))
        {
            script.m_Target = EditorGUILayout.ObjectField("Target", script.m_Target, typeof(GameObject), allowSceneObjects) as GameObject;
            script.MiniMapLayer = EditorGUILayout.LayerField("MiniMap Layer", script.MiniMapLayer);
            if(script.renderType == bl_MiniMap.RenderType.RealTime)
            {
                script.useNonRenderLayer = EditorGUILayout.ToggleLeft("Use Not-Render layer", script.useNonRenderLayer, EditorStyles.toolbarButton);
                if(script.useNonRenderLayer)
                script.nonRenderLayer = EditorGUILayout.LayerField("Non Render Layers", script.nonRenderLayer);
            }
            script.renderType = (bl_MiniMap.RenderType)EditorGUILayout.EnumPopup("Render Mode", script.renderType);
            script.canvasRenderMode = (bl_MiniMap.RenderMode)EditorGUILayout.EnumPopup("Draw Mode", script.canvasRenderMode);
            script.mapMode = (bl_MiniMap.MapType)EditorGUILayout.EnumPopup("Map Mode", script.mapMode);
            if (script.renderType == bl_MiniMap.RenderType.Picture)
            {
                script.MapTexture = EditorGUILayout.ObjectField("Map Texture", script.MapTexture, typeof(Texture), allowSceneObjects) as Texture;
            }
            script.UpdateRate = EditorGUILayout.IntSlider("Update Rate", script.UpdateRate, 1, 10);
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");
        if (GUILayout.Button("Zoom Settings", EditorStyles.toolbarPopup)) { ShowZoom = !ShowZoom; ZoomAnim.target = ShowZoom; }
        if (EditorGUILayout.BeginFadeGroup(ZoomAnim.faded))
        {
            EditorGUILayout.LabelField("Zoom MinMax", EditorStyles.label);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(script.MinZoom.ToString("F2"), EditorStyles.toolbarButton);
            EditorGUILayout.MinMaxSlider(ref script.MinZoom, ref script.MaxZoom, 1, 100);
            GUILayout.Label(script.MaxZoom.ToString("F2"), EditorStyles.toolbarButton);
            EditorGUILayout.EndHorizontal();
            script.DefaultHeight = EditorGUILayout.Slider("Default Zoom", script.DefaultHeight, script.MinZoom, script.MaxZoom);
            script.saveZoomInRuntime = EditorGUILayout.ToggleLeft("Save Zoom", script.saveZoomInRuntime, EditorStyles.toolbarButton);
            script.scrollSensitivity = EditorGUILayout.IntSlider("Zoom Scroll Sensitivity", script.scrollSensitivity, 1, 10);
            script.IconMultiplier = EditorGUILayout.Slider("Icon Size Multiplier", script.IconMultiplier, 0.05f, 2);
            script.LerpHeight = EditorGUILayout.Slider("Zoom Speed", script.LerpHeight, 1, 20);
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");
        if (GUILayout.Button("Rotation Settings", EditorStyles.toolbarPopup)) { ShowRotation = !ShowRotation; RotationAnim.target = ShowRotation; }
        if (EditorGUILayout.BeginFadeGroup(RotationAnim.faded))
        {
            script.DynamicRotation = EditorGUILayout.ToggleLeft("Rotate Map with player", script.DynamicRotation, EditorStyles.toolbarButton);
            if (script.canvasRenderMode == bl_MiniMap.RenderMode.Mode2D)
            {
                script.iconsAlwaysFacingUp = EditorGUILayout.ToggleLeft("Icons Always Facing Up", script.iconsAlwaysFacingUp, EditorStyles.toolbarButton);
            }
            else
            {
                script.iconsAlwaysFacingUp = EditorGUILayout.ToggleLeft("Icons Always Facing Up", script.iconsAlwaysFacingUp, EditorStyles.toolbarButton);
                // script.iconsAlwaysFacingUp = false;
            }
            script.SmoothRotation = EditorGUILayout.ToggleLeft("Smooth Rotation", script.SmoothRotation, EditorStyles.toolbarButton);
            if (script.SmoothRotation) { script.LerpRotation = EditorGUILayout.Slider("Rotation Lerp", script.LerpRotation, 1, 20); }
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");
        if (GUILayout.Button("Grid Settings", EditorStyles.toolbarPopup)) { ShowGrip = !ShowGrip; GripAnim.target = ShowGrip; }
        if (EditorGUILayout.BeginFadeGroup(GripAnim.faded))
        {
            script.ShowAreaGrid = EditorGUILayout.ToggleLeft("Show Grid", script.ShowAreaGrid, EditorStyles.toolbarButton);
            if (script.ShowAreaGrid)
            {
                script.AreasSize = EditorGUILayout.Slider("Row Grid Size", script.AreasSize, 1, 25);
            }
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");
        if (GUILayout.Button("Render Settings", EditorStyles.toolbarPopup)) { ShowRender = !ShowRender; RenderAnim.target = ShowRender; }
        if (EditorGUILayout.BeginFadeGroup(RenderAnim.faded))
        {
            script.PlayerIconSprite = EditorGUILayout.ObjectField("Player Icon", script.PlayerIconSprite, typeof(Sprite), false) as Sprite;
            script.playerColor = EditorGUILayout.ColorField("Player Color", script.playerColor);
            script.planeSaturation = EditorGUILayout.Slider("Map Saturation", script.planeSaturation, 0.2f, 2);
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");
        if (GUILayout.Button("References", EditorStyles.toolbarPopup)) { ShowReferences = !ShowReferences; ReferencesAnim.target = ShowReferences; }
        if (EditorGUILayout.BeginFadeGroup(ReferencesAnim.faded))
        {
            script.miniMapCamera = EditorGUILayout.ObjectField("Mini Map Camera", script.miniMapCamera, typeof(Camera), allowSceneObjects) as Camera;
            
            script.HoofdPuntPrefab = EditorGUILayout.ObjectField("Hoof Punt", script.HoofdPuntPrefab, typeof(GameObject), allowSceneObjects) as GameObject;
            script.RootAlpha = EditorGUILayout.ObjectField("Root Alpha", script.RootAlpha, typeof(CanvasGroup), allowSceneObjects) as CanvasGroup;

            script.WorldSpace = EditorGUILayout.ObjectField("Map Bounds Reference", script.WorldSpace, typeof(RectTransform), allowSceneObjects) as RectTransform;
            script.m_Canvas = EditorGUILayout.ObjectField("Canvas", script.m_Canvas, typeof(Canvas), allowSceneObjects) as Canvas;
            script.MiniMapUIRoot = EditorGUILayout.ObjectField("UI Root", script.MiniMapUIRoot, typeof(RectTransform), allowSceneObjects) as RectTransform;
            script.IconsParent = EditorGUILayout.ObjectField("Icons Parent", script.IconsParent, typeof(RectTransform), allowSceneObjects) as RectTransform;
            script.PlayerIcon = EditorGUILayout.ObjectField("Player Icon", script.PlayerIcon, typeof(Image), allowSceneObjects) as Image;
            script.AreaMaterial = EditorGUILayout.ObjectField("Grip Material", script.AreaMaterial, typeof(Material), allowSceneObjects) as Material;
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndVertical();
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(script);

            if(script.miniMapCamera != null)
            {
                script.miniMapCamera.orthographicSize = script.DefaultHeight;
            }

            if (script.PlayerIcon != null)
            {
                script.PlayerIcon.sprite = script.PlayerIconSprite;
                script.PlayerIcon.color = script.playerColor;
            }
        }
    }

    public void CreateLayer(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new System.ArgumentNullException("name", "New layer name string is either null or empty.");

        var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        var layerProps = tagManager.FindProperty("layers");
        var propCount = layerProps.arraySize;

        SerializedProperty firstEmptyProp = null;

        for (var i = 0; i < propCount; i++)
        {
            var layerProp = layerProps.GetArrayElementAtIndex(i);

            var stringValue = layerProp.stringValue;

            if (stringValue == name) return;

            if (i < 8 || stringValue != string.Empty) continue;

            if (firstEmptyProp == null)
                firstEmptyProp = layerProp;
        }

        if (firstEmptyProp == null)
        {
            return;
        }

        firstEmptyProp.stringValue = name;
        tagManager.ApplyModifiedProperties();
    }
}