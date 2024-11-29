using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UGUIMiniMap;

public class bl_IconItem : MonoBehaviour{

    [Separator("SETTINGS")]
    public float DestroyIn = 5f;
    [Separator("REFERENCES")]
    public Image TargetGraphic;
    public Sprite DeathIcon = null;
    public CanvasGroup m_CanvasGroup;

    private Animator Anim;
    private float delay = 0.1f;
    private bl_MiniMapItem miniMapItem;
    public RectTransform textRect { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        //Get the canvas group or add one if nt have.
        if(m_CanvasGroup == null)
        {
            m_CanvasGroup = GetComponent<CanvasGroup>();
        }
        if(GetComponent<Animator>() != null)
        {
            Anim = GetComponent<Animator>();
        }
        if(Anim != null) { Anim.enabled = false; }
        m_CanvasGroup.alpha = 0;
    }

    public void SetUp(bl_MiniMapItem item)
    {
        miniMapItem = item;
    }

    /// <summary>
    /// When player or the target die,desactive,remove,etc..
    /// call this for remove the item UI from Map
    /// for change to other icon and desactive in certain time
    /// or destroy immediate
    /// </summary>
    public void DestroyIcon(Sprite death)
    {
         TargetGraphic.sprite = death;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ico"></param>
    public void SetIcon(Sprite ico)
    {
        TargetGraphic.sprite = ico;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator FadeIcon()
    {
        yield return new WaitForSeconds(delay);
        while(m_CanvasGroup.alpha < 1)
        {
            m_CanvasGroup.alpha += Time.deltaTime * 2;
            yield return null;
        }
        if (Anim != null) { Anim.enabled = true; }
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetVisibleAlpha()
    {
        m_CanvasGroup.alpha = 1;
    }

    public void DelayStart(float v) { delay = v; StartCoroutine(FadeIcon()); }
}