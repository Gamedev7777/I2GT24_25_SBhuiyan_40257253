using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UGUIMiniMap;
using System;
using UnityEngine.Serialization;

public class bl_MiniMap : MonoBehaviour
{
    #region Public members
   
    public GameObject m_Target;
    public int MiniMapLayer = 10;
    public bool useNonRenderLayer = false;
    public int nonRenderLayer = 10;
    [FormerlySerializedAs("MMCamera")] public Camera miniMapCamera = null;
    [FormerlySerializedAs("m_Type")] public RenderType renderType = RenderType.Picture;
    public RenderMode canvasRenderMode = RenderMode.Mode2D;
    [FormerlySerializedAs("m_MapType")] public MapType mapMode = MapType.Local;
    public int UpdateRate = 5;
    public Color playerColor = Color.white;
    [Range(0.05f, 2)] public float IconMultiplier = 1;
    [Range(1, 10)] public int scrollSensitivity = 3;
    public float DefaultHeight = 30;
    public bool saveZoomInRuntime = false;
    public float MaxZoom = 80;
    public float MinZoom = 5;
    public float LerpHeight = 8;
    public Sprite PlayerIconSprite;
    public bool iconsAlwaysFacingUp = true;
    public bool DynamicRotation = true;
    public bool SmoothRotation = true;
    public float LerpRotation = 8;
    public bool ShowAreaGrid = true;
    [Range(1, 20)] public float AreasSize = 4;
    public Material AreaMaterial;
    public float LerpTransition = 7;
    public Texture MapTexture = null;
    public float planeSaturation = 1.4f;    
    public RectTransform WorldSpace = null;
    public Canvas m_Canvas = null;
    public RectTransform MiniMapUIRoot = null;
    public RectTransform IconsParent;
    public Image PlayerIcon = null;
    public CanvasGroup RootAlpha;
    public GameObject ItemPrefabSimple = null;
    public GameObject HoofdPuntPrefab;
    #endregion

    #region Public properties
    public bool hasError { get; set; }
    public float Zoom { get; set; }
    #endregion

    #region Private members
    private Vector3 MiniMapPosition = Vector2.zero;
    private Vector3 MiniMapRotation = Vector3.zero;
    private Vector2 MiniMapSize = Vector2.zero;
    private Vector3 DragOffset = Vector3.zero;
    private bool DefaultRotationMode = false;
    private Vector3 DeafultMapRot = Vector3.zero;
    const string MMHeightKey = "MinimapCameraHeight";
    private bool isAlphaComplete = false;
    private bool isPlanedCreated = false;
    private RectTransform PlayerIconTransform;
    private List<bl_MiniMapItem> miniMapItems = new List<bl_MiniMapItem>();
    private Vector3 playerPosition, targetPosition;
    private Vector3 playerRotation;
    private bool isUpdateFrame = false;
    private bl_MiniMapPlane miniMapPlane;
    #endregion

    public static bl_MiniMap instance;

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        instance = this;
       
        GetMiniMapSize();
        DefaultRotationMode = DynamicRotation;
        DeafultMapRot = m_Transform.eulerAngles;
        PlayerIcon.sprite = PlayerIconSprite;
        PlayerIcon.color = playerColor;
        PlayerIconTransform = PlayerIcon.GetComponent<RectTransform>();
        SetHoofdPunt();
        if (hasError) return;

        CreateMapPlane(renderType == RenderType.RealTime);
        if (canvasRenderMode == RenderMode.Mode3D) { ConfigureCamera3D(); }
        if (mapMode == MapType.Local)
        {
            //Get Save Height
            if (saveZoomInRuntime) Zoom = PlayerPrefs.GetFloat(MMHeightKey, DefaultHeight);
            else Zoom = DefaultHeight;
        }
        else
        {
            ConfigureWorlTarget();
            Zoom = DefaultHeight;
            PlayerIcon.gameObject.SetActive(false);
        }
        if (RootAlpha != null) { StartCoroutine(StartFade(0)); }
    }

    /// <summary>
    /// 
    /// </summary>
    void OnEnable()
    {
        if (!isAlphaComplete)
        {
            if (RootAlpha != null) { StartCoroutine(StartFade(0)); }
        }
    }

    /// <summary>
    /// Create a Plane with Map Texture
    /// MiniMap Camera will be renderer only this plane.
    /// This is more optimizing that RealTime type.
    /// </summary>
    void CreateMapPlane(bool realTime)
    {
        if (isPlanedCreated) return;
        if (MapTexture == null && !realTime)
        {
            return;
        }
        if (realTime || ShowAreaGrid)
        {
            GameObject plane = Instantiate(bl_MiniMapData.Instance.mapPlane.gameObject) as GameObject;
            miniMapPlane = plane.GetComponent<bl_MiniMapPlane>();
            miniMapPlane.Setup(this);
        }
        isPlanedCreated = true;
    }

    /// <summary>
    /// Avoid to UI world space collision with other objects in scene.
    /// </summary>
    public void ConfigureCamera3D()
    {
        Camera cam = (Camera.main != null) ? Camera.main : Camera.current;
        if (cam == null)
        {
            return;
        }
        m_Canvas.worldCamera = cam;
        //Avoid to 3D UI transferred other objects in the scene.
        cam.nearClipPlane = 0.015f;
        m_Canvas.planeDistance = 0.1f;
    }

    /// <summary>
    /// 
    /// </summary>
    public void ConfigureWorlTarget()
    {
        if (m_Target == null)
            return;

        bl_MiniMapItem mmi = m_Target.AddComponent<bl_MiniMapItem>();
        mmi.Icon = PlayerIcon.sprite;
        mmi.IconColor = PlayerIcon.color;
        mmi.Target = m_Target.transform;
        mmi.Size = PlayerIcon.rectTransform.sizeDelta.x + 2;
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (hasError) return;
        if (m_Target == null || miniMapCamera == null)
            return;
        isUpdateFrame = (Time.frameCount % UpdateRate) == 0;

        //controlled that minimap follow the target
        PositionControll();
        //Apply rotation settings
        RotationControll();
        //for minimap and world map control
        MapSize();
        //update all items (icons)
        UpdateItems();
    }

    /// <summary>
    /// Minimap follow the target.
    /// </summary>
    void PositionControll()
    {
        if (mapMode == MapType.Local)
        {
            if (isUpdateFrame)
            {
                playerPosition = m_Transform.position;
                targetPosition = Target.position;
                // Update the transformation of the camera as per the target's position.
                playerPosition.x = targetPosition.x;
                playerPosition.z = targetPosition.z;
                playerPosition += DragOffset;

                //Calculate player position
                if (Target != null)
                {
                    Vector3 pp = miniMapCamera.WorldToViewportPoint(TargetPosition);
                    PlayerIconTransform.anchoredPosition = bl_MiniMapUtils.CalculateMiniMapPosition(pp, MiniMapUIRoot);
                }

                // For this, we add the predefined (but variable, see below) height var.
                playerPosition.y = Target.TransformPoint(Vector3.up * 200).y;
            }
            //Camera follow the target
            m_Transform.position = Vector3.Lerp(m_Transform.position, playerPosition, Time.deltaTime * 10);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void RotationControll()
    {
        // If the minimap should rotate as the target does, the rotateWithTarget var should be true.
        // An extra catch because rotation with the full screen map is a bit weird.       
        if (DynamicRotation && mapMode != MapType.Global)
        {
            if (isUpdateFrame)
            {
                //get local reference.
                playerRotation = m_Transform.eulerAngles;
                playerRotation.y = Target.eulerAngles.y;
            }
            if (SmoothRotation)
            {
                if (isUpdateFrame)
                {
                    if (canvasRenderMode == RenderMode.Mode2D)
                    {
                        //For 2D Mode
                        PlayerIconTransform.eulerAngles = Vector3.zero;
                    }
                    else
                    {
                        //For 3D Mode
                        PlayerIconTransform.localEulerAngles = Vector3.zero;
                    }

                    if (m_Transform.eulerAngles.y != playerRotation.y)
                    {
                        //calculate the difference 
                        float d = playerRotation.y - m_Transform.eulerAngles.y;
                        //avoid lerp from 360 to 0 or reverse
                        if (d > 180 || d < -180)
                        {
                            m_Transform.eulerAngles = playerRotation;
                        }
                    }
                }
                //Lerp rotation of map
                m_Transform.eulerAngles = Vector3.Lerp(this.transform.eulerAngles, playerRotation, Time.deltaTime * LerpRotation);
            }
            else
            {
                m_Transform.eulerAngles = playerRotation;
            }
        }
        else
        {
            m_Transform.eulerAngles = DeafultMapRot;
            if (canvasRenderMode == RenderMode.Mode2D)
            {
                //When map rotation is static, only rotate the player icon
                Vector3 e = Vector3.zero;
                //get and fix the correct angle rotation of target
                e.z = -Target.eulerAngles.y;
                PlayerIconTransform.eulerAngles = e;
            }
            else
            {
                //Use local rotation in 3D mode.
                Vector3 tr = Target.localEulerAngles;
                Vector3 r = Vector3.zero;
                r.z = -tr.y;
                PlayerIconTransform.localEulerAngles = r;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void UpdateItems()
    {
        if (!isUpdateFrame) return;
        if (miniMapItems == null || miniMapItems.Count <= 0) return;
        for (int i = 0; i < miniMapItems.Count; i++)
        {
            if (miniMapItems[i] == null) { miniMapItems.RemoveAt(i); continue; }
            miniMapItems[i].OnUpdateItem();
        }
    }

    /// <summary>
    /// Map FullScreen or MiniMap
    /// Lerp all transition for smooth effect.
    /// </summary>
    void MapSize()
    {
        float delta = Time.deltaTime;
        if (DynamicRotation != DefaultRotationMode) 
        { 
            DynamicRotation = DefaultRotationMode;
        }
        MiniMapUIRoot.sizeDelta = Vector2.Lerp(MiniMapUIRoot.sizeDelta, MiniMapSize, delta * LerpTransition);
        MiniMapUIRoot.anchoredPosition = Vector3.Lerp(MiniMapUIRoot.anchoredPosition, MiniMapPosition, delta * LerpTransition);
        MiniMapUIRoot.localEulerAngles = Vector3.Lerp(MiniMapUIRoot.localEulerAngles, MiniMapRotation, delta * LerpTransition);
        
        float zoom = Mathf.Lerp(miniMapCamera.orthographicSize, Zoom, delta * LerpHeight);
        zoom = Mathf.Max(1, zoom);
        miniMapCamera.orthographicSize = zoom;
    }
    /// <summary>
    /// 
    /// </summary>
    public void GoToTarget()
    {
        StopCoroutine("ResetOffset");
        StartCoroutine("ResetOffset");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator ResetOffset()
    {
        while (Vector3.Distance(DragOffset, Vector3.zero) > 0.2f)
        {
            DragOffset = Vector3.Lerp(DragOffset, Vector3.zero, Time.deltaTime * LerpTransition);
            yield return null;
        }
        DragOffset = Vector3.zero;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="b"></param>
    public void ChangeHeight(bool b)
    {
        if (mapMode == MapType.Global)
            return;

        if (b) Zoom -= scrollSensitivity;
        else Zoom += scrollSensitivity;

        Zoom = Mathf.Clamp(Zoom, MinZoom, MaxZoom);
        if(saveZoomInRuntime) PlayerPrefs.SetFloat(MMHeightKey, Zoom);
    }

    /// <summary>
    /// Create a new icon without reference in runtime.
    /// see all structure options in bl_MMItemInfo.
    /// </summary>
    public bl_MiniMapItem CreateNewItem(bl_MMItemInfo item)
    {
        if (hasError) return null;

        GameObject newItem = Instantiate(ItemPrefabSimple, item.Position, Quaternion.identity) as GameObject;
        bl_MiniMapItem mmItem = newItem.GetComponent<bl_MiniMapItem>();
        if (item.Target != null) { mmItem.Target = item.Target; }
        mmItem.Size = item.Size;
        mmItem.IconColor = item.Color;
        mmItem.m_Effect = item.Effect;
        if (item.Sprite != null) { mmItem.Icon = item.Sprite; }

        return mmItem;
    }

    /// <summary>
    /// Add an 'N' icon inside the minimap to reference the North direction
    /// </summary>
    public void SetHoofdPunt()
    {
        //Verify is MiniMap Layer Exist in Layer Mask List.
        string layer = LayerMask.LayerToName(MiniMapLayer);
        //If not exist.
        if (string.IsNullOrEmpty(layer))
        {
            MiniMapUIRoot.gameObject.SetActive(false);
            hasError = true;
            enabled = false;
            return;
        }
        if (HoofdPuntPrefab == null || mapMode == MapType.Global) return;

        GameObject newItem = Instantiate(HoofdPuntPrefab, new Vector3(0, 0, 100), Quaternion.identity) as GameObject;
        bl_MiniMapItem mmItem = newItem.GetComponent<bl_MiniMapItem>();
        mmItem.Target = newItem.transform;
    }

    /// <summary>
    /// Reset this transform rotation helper.
    /// </summary>
    void ResetMapRotation() { m_Transform.eulerAngles = new Vector3(90, 0, 0); }

    /// <summary>
    /// Call this fro change the mode of rotation of map
    /// Static or dynamic
    /// </summary>
    /// <param name="mode">static or dynamic</param>
    /// <returns></returns>
    public void RotationMap(bool mode) 
    {
        DynamicRotation = mode; 
        DefaultRotationMode = DynamicRotation;
    }

    /// <summary>
    /// Set target in runtime
    /// </summary>
    /// <param name="t"></param>
    public void SetTarget(GameObject t)
    {
        m_Target = t;
    }

    /// <summary>
    /// Set Map Texture in Runtime
    /// </summary>
    /// <param name="t"></param>
    public void SetMapTexture(Texture2D newTexture)
    {
        if (renderType != RenderType.Picture)
        {
            return;
        }
        miniMapPlane.SetMapTexture(newTexture);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (miniMapCamera != null)
        {
            miniMapCamera.orthographicSize = DefaultHeight;
        }
        if (AreaMaterial != null)
        {
            Vector2 r = AreaMaterial.GetTextureScale("_MainTex");
            r.x = AreasSize;
            r.y = AreasSize;
            AreaMaterial.SetTextureScale("_MainTex", r);
        }
        if (PlayerIcon != null)
        {
            PlayerIcon.sprite = PlayerIconSprite;
            PlayerIcon.color = playerColor;
        }
    }
#endif

    /// <summary>
    /// 
    /// </summary>
    public void SetGridSize(float value)
    {
        if (miniMapPlane == null) return;

        miniMapPlane.SetGridSize(value);
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetActiveGrid(bool active)
    {
        if (miniMapPlane == null) return;

        miniMapPlane.gridPlane.SetActive(active);
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetMapRotation(bool dynamic)
    {
        DynamicRotation = dynamic;
        DefaultRotationMode = dynamic;
        m_Transform.eulerAngles = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// 
    /// </summary>
    void GetMiniMapSize()
    {
        MiniMapSize = MiniMapUIRoot.sizeDelta;
        MiniMapPosition = MiniMapUIRoot.anchoredPosition;
        MiniMapRotation = MiniMapUIRoot.eulerAngles;
    }

    public void RegisterItem(bl_MiniMapItem item) { if (miniMapItems.Contains(item)) return; miniMapItems.Add(item); }
    public void RemoveItem(bl_MiniMapItem item) { miniMapItems.Remove(item); }

    IEnumerator StartFade(float delay)
    {
        RootAlpha.alpha = 0;
        yield return new WaitForSeconds(delay);
        while (RootAlpha.alpha < 1)
        {
            RootAlpha.alpha += Time.deltaTime;
            yield return null;
        }
        isAlphaComplete = true;
    }

    public Transform Target
    {
        get
        {
            if (m_Target != null)
            {
                return m_Target.GetComponent<Transform>();
            }
            return this.GetComponent<Transform>();
        }
        set
        {
            m_Target = value.gameObject;
        }
    }
    public Vector3 TargetPosition
    {
        get
        {
            Vector3 v = Vector3.zero;
            if (m_Target != null)
            {
                v = m_Target.transform.position;
            }
            return v;
        }
    }


    //Get Transform
    private Transform t;
    private Transform m_Transform
    {
        get
        {
            if (t == null)
            {
                t = this.GetComponent<Transform>();
            }
            return t;
        }
    }
    //Get Mask Helper (if exist one)for management of texture mask
    private bl_MaskHelper _maskHelper = null;
    private bl_MaskHelper m_maskHelper
    {
        get
        {
            if (_maskHelper == null)
            {
                _maskHelper = this.transform.root.GetComponentInChildren<bl_MaskHelper>();
            }
            return _maskHelper;
        }
    }

    [System.Serializable]
    public enum RenderType
    {
        RealTime,
        Picture,
    }

    [System.Serializable]
    public enum RenderMode
    {
        Mode2D,
        Mode3D,
    }

    [System.Serializable]
    public enum MapType
    {
        Local,
        Global,
    }
}