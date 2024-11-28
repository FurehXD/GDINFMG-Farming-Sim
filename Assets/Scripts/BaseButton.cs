using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor.Events; // Required for adding persistent calls
#endif
[Serializable]
public class BaseButton : Button, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Button Behavior Settings")]
    [SerializeField]
    protected EHoverType[] hoverTypes;
    [SerializeField]
    protected EButtonType buttonType = EButtonType.NO_CONFIRM;

    [Header("Button Visibility Settings")]
    private float baseAlpha;

    [Header("Hover Marker Settings")]
    [SerializeField]
    private Image markerAsset;
    private bool markerCreated = false;
    private Image markerAssetReference;

    [Header("Enbiggen Settings")]
    private Enbiggener enbiggenerComponent;

    [Header("Bubble Descriptor Settings")]
    [SerializeField]
    protected string bubbleText = "";
    private Image bubbleBorder;
    [SerializeField]
    private float bubbleRelativeScaleFactor = 0.5f;
    private bool bubbleCreated = false;
    private Image bubbleReference;
    [SerializeField]
    private ECardinalDirections positionRelativeToMouse = ECardinalDirections.SOUTH;

    [Header("New Asset Settings")]
    [SerializeField]
    private bool hasUniqueAlpha = true;

    [Header("Button Transform Settings")]
    [SerializeField]
    private bool hasUniqueSize = false;
    public bool HasUniqueSize {  get { return hasUniqueSize; } set { this.hasUniqueSize = value; } }

    [SerializeField]
    private bool overrideDefault = false;
    public bool OverrideDefault { set  { this.overrideDefault = value; } get { return this.overrideDefault; } }

    [Tooltip("If 0, it's top most, 1 is below that, so on and so forth")]
    protected int buttonNumber = 0;
    public int ButtonNumber { get { return buttonNumber; } }

    [SerializeField]
    private float buttonWidth;
    [SerializeField]
    private float buttonHeight;

    [SerializeField]
    private Sprite newSpriteDefault;
    [SerializeField]
    private Sprite newSpriteHover;
    [SerializeField]
    private Sprite newSpritePressed;

    [Header("SFX Settings")]
    [SerializeField]
    private bool hasUniqueSFX = false;
    public bool HasUniqueSFX { get { return hasUniqueSFX; } set { this.hasUniqueSFX = value; } }
    [SerializeField]
    private AudioSource hoverSFX;
    public AudioSource HoverSFX{ set { this.hoverSFX = value; } get { return this.hoverSFX; } }
    [SerializeField]
    private AudioSource clickSFX;
    public AudioSource ClickSFX { set { this.clickSFX = value; } get { return this.clickSFX; } }

    protected bool buttonIsPressed = false;

    protected RectTransform thisRectTransform; 
    public void EnableSemSagaButton(bool enabled)
    {
        if (!enabled && Array.Exists(this.hoverTypes, hoverType => hoverType == EHoverType.BUBBLE_DESCRIPTOR))
            this.DeleteBubbleDescriptor();
        this.interactable = enabled;
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!this.buttonIsPressed && this.interactable)
        {
            if (this.hoverSFX != null) hoverSFX.Play();

            this.PerformHover();
        }
    }
    protected virtual void PerformHover()
    {
        foreach (EHoverType hoverType in this.hoverTypes) 
        {
            switch (hoverType)
            {
                case EHoverType.ENBIGGEN:
                    this.enbiggenerComponent.Enbiggen();
                    break;
                case EHoverType.BUBBLE_DESCRIPTOR:
                    this.CreateBubbleDescriptor();
                    break;
                case EHoverType.MARKER:
                    this.CreateMarker();
                    break;
                case EHoverType.NEW_ASSET:
                    this.ApplyButtonSprite(this.newSpriteHover, false);
                    break;
            }
        }
    }
    protected void SetBubbleText()
    {
        if (this.bubbleText != null)    
            this.bubbleReference.GetComponentInChildren<TextMeshProUGUI>().text = this.bubbleText;
    }
    public Vector3 GetBubbleMouseRelativePosition()
    {
        float distanceFromMouse = 75.0f;
        Vector3 mouseOffset = this.positionRelativeToMouse switch
        {
            ECardinalDirections.SOUTH_EAST => new Vector3(Mathf.Cos(-45.0f * Mathf.Deg2Rad) * distanceFromMouse, Mathf.Sin(-45.0f * Mathf.Deg2Rad) * distanceFromMouse, 0.0f),
            ECardinalDirections.SOUTH_WEST => new Vector3(distanceFromMouse * Mathf.Cos(-135.0f * Mathf.Deg2Rad), distanceFromMouse * Mathf.Sin(-135.0f * Mathf.Deg2Rad), 0.0f),
            ECardinalDirections.NORTH_EAST => new Vector3(Mathf.Cos(45.0f * Mathf.Deg2Rad) * distanceFromMouse, Mathf.Sin(45.0f * Mathf.Deg2Rad) * distanceFromMouse, 0.0f),
            ECardinalDirections.NORTH_WEST => new Vector3(distanceFromMouse * Mathf.Cos(135.0f * Mathf.Deg2Rad), distanceFromMouse * Mathf.Sin(135.0f * Mathf.Deg2Rad), 0.0f),
            ECardinalDirections.WEST => new Vector3(-distanceFromMouse, 0.0f, 0.0f),
            ECardinalDirections.EAST => new Vector3(distanceFromMouse, 0.0f, 0.0f),
            ECardinalDirections.NORTH => new Vector3(0.0f, distanceFromMouse, 0.0f),
            _ => new Vector3(0.0f, -distanceFromMouse, 0.0f),
        };

        return mouseOffset;
    }
    protected void CreateBubbleDescriptor()
    {
        if (!this.bubbleCreated)
        {
            Vector3 mouseOffset = this.GetBubbleMouseRelativePosition();
            Vector3 screenPosition = Input.mousePosition + mouseOffset;

            float widthScaleFactor = this.bubbleRelativeScaleFactor;
            float heightScaleFactor = widthScaleFactor;

            Image bubble = Instantiate(this.bubbleBorder, Vector3.zero, Quaternion.identity);
            RectTransform bubbleRectTransform = bubble.GetComponent<RectTransform>();
            RectTransform bubbleTextRectTransform = bubble.GetComponentInChildren<TextMeshProUGUI>().GetComponent<RectTransform>();

            this.bubbleReference = bubble;

            this.SetBubbleText();

            bubble.transform.SetParent(this.transform, false);

            bubbleRectTransform.position = screenPosition;

            bubbleRectTransform.sizeDelta *= new Vector2(widthScaleFactor, heightScaleFactor);
            bubbleTextRectTransform.sizeDelta *= new Vector2(widthScaleFactor, heightScaleFactor);

            this.bubbleCreated = true;
        }
    }
    private void DeleteBubbleDescriptor()
    {
        if (this.bubbleReference != null)
        {
            Destroy(this.bubbleReference.gameObject);
            this.bubbleCreated = false;
        }
    }
    protected virtual void PerformPointerExit()
    {
        foreach (EHoverType hoverType in this.hoverTypes)
        {
            switch (hoverType)
            {
                case EHoverType.ENBIGGEN:
                    this.enbiggenerComponent.ResetSize();
                    break;
                case EHoverType.BUBBLE_DESCRIPTOR:
                    this.DeleteBubbleDescriptor();
                    break;
                case EHoverType.MARKER:
                    this.DeleteMarker();
                    break;
                case EHoverType.NEW_ASSET:
                    this.ApplyButtonSprite(this.newSpriteDefault, true);
                    break;
            }
        }
    }

    private void ApplyButtonSprite(Sprite spriteToApply, bool isDefault)
    {
        Image thisImage = this.targetGraphic.GetComponent<Image>();

        if (spriteToApply)
        {
            thisImage.sprite = spriteToApply;
            if (this.hasUniqueAlpha)
            {
                Color uniqueAlphaColor = thisImage.color;

                if(isDefault) uniqueAlphaColor.a = this.baseAlpha;
                else uniqueAlphaColor.a = 1.0f;

                thisImage.color = uniqueAlphaColor;
            }
        }
    }
    private void CreateMarker()
    {
        if(!this.markerCreated)
        {
            Image marker = Instantiate(this.markerAsset, this.transform);
            RectTransform rectTransform = marker.GetComponent<RectTransform>();
            float markerAssetOffset = -50.0f;

            this.markerAssetReference = marker;

            marker.transform.SetParent(this.transform, false);

            rectTransform.anchoredPosition = Vector3.zero;
            rectTransform.anchoredPosition += new Vector2(markerAssetOffset, 0.0f);
            marker.transform
                    .DOMoveX(1.0f, 0.50f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);

            this.markerCreated = true;
        }
    }
    private void DeleteMarker()
    {
        if(this.markerAssetReference != null)
        {
            Destroy(this.markerAssetReference.gameObject);
            this.markerCreated = false;
        }
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!this.buttonIsPressed && this.interactable)
            this.PerformPointerExit();
    }
    protected virtual void OnClicked()
    {
        if (this.interactable)
        {
            this.enbiggenerComponent.ResetSize();

            switch (this.buttonType)
            {
                case EButtonType.NEEDS_CONFIRM:
                    this.buttonIsPressed = true;
                    break;
                case EButtonType.NO_CONFIRM:
                    this.buttonIsPressed = false;
                    break;
            }
            if (this.newSpritePressed) this.targetGraphic.GetComponent<Image>().sprite = this.newSpritePressed;
            if (this.clickSFX != null) this.clickSFX.Play();

            this.onClick.Invoke();
        }
    }
    public virtual void ResetButton()
    {
        this.buttonIsPressed = false;

        if (this.newSpriteDefault)
            this.targetGraphic.GetComponent<Image>().sprite = this.newSpriteDefault;
        this.enbiggenerComponent.ResetSize();
    }
    protected virtual void InitializeBubbleDescriptor()
    {
        string omittedString = " Button";
        if (this.bubbleText == "")
            this.bubbleText = this.name.Replace(omittedString, "");
    }
    private void Awake()
    {
        this.bubbleBorder = Resources.Load<GameObject>("Prefabs/UI/Bubble Template").GetComponent<Image>();
        this.thisRectTransform = this.GetComponent<RectTransform>();
        this.enbiggenerComponent = this.GetComponent<Enbiggener>();

        this.InitializeBubbleDescriptor();

        if (!this.newSpriteDefault)
            this.newSpriteDefault = this.targetGraphic.GetComponent<Image>().sprite;


        if (this.buttonWidth == 0)
        {
            this.buttonWidth = 75.0f;
        }
        if (this.buttonHeight == 0)
        {
            this.buttonHeight = 75.0f;
        }

        if(this.overrideDefault)
            this.thisRectTransform.sizeDelta = new Vector2(this.buttonWidth, this.buttonHeight);

        this.baseAlpha = this.targetGraphic.GetComponent<Image>().color.a;
    }
    public void SetButtonNumber(int buttonNumber)
    {
        this.buttonNumber = buttonNumber;
    }

    public void SetOwnSize()
    {
        this.thisRectTransform.sizeDelta = new Vector2(this.buttonWidth, this.buttonHeight);
    }

    protected virtual void Update()
    {
        this.GetComponent<Button>().interactable = this.interactable;
    }
}
