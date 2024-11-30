using UnityEngine;

public class CropButton : BaseButton{
    [SerializeField]
    private string cropName;
    public string CropName {
        get { return this.cropName; }
    }

    public string SelectCrop() {
        return this.cropName;
    }
}
