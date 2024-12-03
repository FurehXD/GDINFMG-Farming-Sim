using UnityEngine;

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

    public void SelectCrop() {
        //get crop form database using crop id
        //add crop to inventory
        Debug.Log("Success");
    }
}
