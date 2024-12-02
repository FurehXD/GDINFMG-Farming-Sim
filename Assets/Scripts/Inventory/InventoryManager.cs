using UnityEngine;
using UnityEngine.UI;
using System;
using NUnit.Framework;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    private VerticalLayoutGroup inventoryLayoutGroup;

    private PlotButton selectedPlotReference;
    public PlotButton SelectedPlotReference { get {  return selectedPlotReference; } }

    private CropProducer cropProducerReference;
    public CropProducer CropProducerReference { get { return cropProducerReference; } }

    //Crop Inventory Allocation Stuff
    [SerializeField]
    private CropStorageCellButton cropStorageTemplate;
    [SerializeField]    
    private VerticalLayoutGroup cropStorageVerticalGroup;
    private List<CropStorageCellButton> cropStorageReferences = new();

    public static event Action OnInventoryOpened;
    public static event Action OnInventoryClosed;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance.gameObject);

        this.inventoryLayoutGroup = this.GetComponentInChildren<VerticalLayoutGroup>();
        this.AllocateExistingCrops();
    }
    private void Update()
    {
        //this.AllocateExistingCrops();
    }
    private void OnEnable()
    {
        PlotButton.OnPlotSelected += this.LogSelectedPlot;
        CropProducer.OnProductionStarted += this.CloseInventory;
    }
    private void OnDisable()
    {
        PlotButton.OnPlotSelected -= this.LogSelectedPlot;
        CropProducer.OnProductionStarted -= this.CloseInventory;
    }
    public void OpenInventory()
    {
        this.GetComponent<Enbiggener>().Enbiggen();
        OnInventoryOpened?.Invoke();
    }
    public void CloseInventory()
    {
        this.GetComponent<Enbiggener>().ResetSize();
        OnInventoryClosed?.Invoke();
    }
    public void LogSelectedPlot(PlotButton selectedPlot)
    {
        Debug.Log(selectedPlot.gameObject.name + " logged!");

        this.selectedPlotReference = selectedPlot;
        this.cropProducerReference = this.selectedPlotReference.GetComponent<CropProducer>();

        if (this.cropProducerReference == null)
            this.cropProducerReference = this.selectedPlotReference.transform.parent.GetComponent<CropProducer>();
    }

    private async void AllocateExistingCrops()
    {
        Transform cropStorageParentTransform = this.cropStorageVerticalGroup.transform;
        int cropCount = await DataRetriever.Instance.RetrieveAvailableCropCount();
        this.cropStorageReferences.Clear();
        for (int i = 0; i < cropCount; i++)
        {
            this.cropStorageReferences.Add(Instantiate(this.cropStorageTemplate, cropStorageParentTransform));
        }
    }

    public void StoreBoughtItem(int itemID, QualityData qualityData)
    {
        if(this.cropStorageReferences.Count > 0)
        {
            CropStorageCellButton cropStorageCellButton = this.cropStorageReferences.Find(cropStorageButton => cropStorageButton.CropIDItStores == itemID);

            cropStorageCellButton.StoreBoughtItem(qualityData);
        }
    }
}
