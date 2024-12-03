using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.Events;
using Image = UnityEngine.UI.Image;

public class CropButton : BaseButton{
    [SerializeField]
    private string cropName;
    public string CropName {
        get { return this.cropName; }
    }

    [SerializeField]
    private int cropID;
    public int CropID {
        get { return this.cropID; }
        set { this.cropID = value; }
    }

    [SerializeField]
    private Image _cropIcon;

    protected override void Start() {
        base.Start();
        this.SetCropImage();
    }

    private async void SetCropImage() {
        string cropAssetDirectory = await DataRetriever.Instance.RetrieveCropAssetDirectory(cropID);
        this._cropIcon.sprite = Resources.Load<Sprite>(cropAssetDirectory);
    }

    public void BuyThingy() {
        ShopManager.Instance.BuySeed(this.cropID);
        Debug.Log("Thingy bought.");
    }

    protected override void Update()
    {
        base.Update();
        #if UNITY_EDITOR
        // Remove any existing persistent calls to avoid duplication
        UnityEditor.Events.UnityEventTools.RemovePersistentListener(this.onClick, BuyThingy);

        UnityEventTools.AddPersistentListener(this.onClick, BuyThingy);
        #endif
    }

    public void SelectCrop() {
        //get crop form database using crop id
        //add crop to inventory
        Debug.Log("Success");
    }
}
