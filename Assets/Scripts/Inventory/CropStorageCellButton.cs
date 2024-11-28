using TMPro;
using UnityEditor.Events;
using UnityEngine;

public class CropStorageCellButton : BaseButton
{
    [SerializeField]
    [Header("CropStorageCellButton")]
    [Range(1, 50)]
    private int cropIDItStores;
    public int CropIDItStores {  get { return this.cropIDItStores; } set { this.cropIDItStores = value; } }

    private Crop cropItStores;

    private int quantity = 1;
    private TextMeshProUGUI quantityTMPDisplay;

    private BaseButton selectedPlotReference;

    private void Start()
    {
        this.interactable = false;
        this.cropItStores = this.GetComponentInChildren<Crop>();
        this.quantityTMPDisplay = this.GetComponentInChildren<TextMeshProUGUI>();

#if UNITY_EDITOR
        //// Remove any existing persistent calls to avoid duplication
        //UnityEditor.Events.UnityEventTools.RemovePersistentListener(myButton.onClick, ExampleMethod1);
        //UnityEditor.Events.UnityEventTools.RemovePersistentListener(myButton.onClick, ExampleMethod2);

        //// Add persistent listeners for the button's OnClick event
        //UnityEventTools.AddPersistentListener(myButton.onClick, ExampleMethod1);
        //UnityEventTools.AddPersistentListener(myButton.onClick, ExampleMethod2);
#endif
    }
    protected override void Update()
    {
        base.Update();
        this.cropItStores.CropID = this.cropIDItStores;
        this.quantityTMPDisplay.text = "x" + this.quantity;
    }

    public void LogSelectedPlot(BaseButton selectedPlot)
    {
        this.selectedPlotReference = selectedPlot;
    }
}
