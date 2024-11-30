using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CropStorageCellButton : BaseButton
{
    bool subStorageIsOpen = false;

    [SerializeField]
    private SubCropStorageCellButton subCellPrefabTemplate;

    private VerticalLayoutGroup childLayoutGroup;
    private List<SubCropStorageCellButton> childButtonReferences = new();

    [SerializeField]
    [Header("CropStorageCellButton")]
    [UnityEngine.Range(1, 50)]
    private int cropIDItStores;
    public int CropIDItStores {  get { return this.cropIDItStores; } set { this.cropIDItStores = value; } }

    private Crop cropItStores;

    public static event Action<CropStorageCellButton> OnCropStorageCellButtonClicked;
    protected override void OnEnable()
    {
        base.OnEnable();
        InventoryManager.OnInventoryOpened += this.EnableButton;
        InventoryManager.OnInventoryClosed += this.DisableButton;

        OnCropStorageCellButtonClicked += this.CloseSubStorageCells;
        CropProducer.OnProductionStarted += this.CloseSubStorageCells;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        InventoryManager.OnInventoryOpened -= this.EnableButton;
        InventoryManager.OnInventoryClosed -= this.DisableButton;

        OnCropStorageCellButtonClicked -= this.CloseSubStorageCells;
        CropProducer.OnProductionStarted -= this.CloseSubStorageCells;
    }
    protected override void Start()
    {
        base.Start();
        this.childLayoutGroup = this.transform.GetComponentInChildren<VerticalLayoutGroup>();   
        this.interactable = false;
        this.cropItStores = this.GetComponentInChildren<Crop>();
    }
    protected override void Update()
    {
#if UNITY_EDITOR
        // Remove any existing persistent calls to avoid duplication
        UnityEditor.Events.UnityEventTools.RemovePersistentListener(this.onClick, OpenSubStorageCells);
        // Remove any existing persistent calls to avoid duplication
        UnityEditor.Events.UnityEventTools.RemovePersistentListener(this.onClick, CloseSubStorageCells);

        if (this.subStorageIsOpen)
        {
            // Add persistent listeners for the button's OnClick event
            UnityEventTools.AddPersistentListener(this.onClick, CloseSubStorageCells);
        }
        else
        {
            // Add persistent listeners for the button's OnClick event
            UnityEventTools.AddPersistentListener(this.onClick, OpenSubStorageCells);
        }
#endif
    }
    public async void OpenSubStorageCells()
    {
        List<QualityData> availableQualities = await DataRetriever.Instance.RetrieveQualities();
        if (availableQualities != null)  // Add null check for safety
        {
            foreach (QualityData qualityData in availableQualities)
            {
                SubCropStorageCellButton subCropStorageCellButton = Instantiate(this.subCellPrefabTemplate, this.childLayoutGroup.transform);
                subCropStorageCellButton.Initialize(qualityData, this.cropItStores);
                this.childButtonReferences.Add(subCropStorageCellButton);
            }
            OnCropStorageCellButtonClicked?.Invoke(this);
            this.subStorageIsOpen = true;
        }
        else
        {
            Debug.LogError("Failed to retrieve qualities from database");
        }
    }
    public void CloseSubStorageCells()
    {
        foreach (SubCropStorageCellButton childButton in this.childButtonReferences)
        {
            Destroy(childButton.gameObject);
        }
        this.childButtonReferences.Clear();
        this.subStorageIsOpen = false;
    }
    private void CloseSubStorageCells(CropStorageCellButton eventCaller)
    {
        if(eventCaller != this)
        {
            foreach (SubCropStorageCellButton childButton in this.childButtonReferences)
            {
                Destroy(childButton.gameObject);
            }
            this.childButtonReferences.Clear();
            this.subStorageIsOpen = false;
        }
    }
}
