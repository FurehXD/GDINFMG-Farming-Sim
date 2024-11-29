using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Harvester : MonoBehaviour
{
    public void HarvestCrop()
    {
        Crop cropReference = this.GetComponentInChildren<Crop>();

        this.GetComponentInChildren<CropIconEnabler>().EnableCropIcon(false);
        this.GetComponentInChildren<RaritySetter>().GetComponent<Image>().enabled = false;
        this.GetComponentInChildren<EmptyTextEnabler>().SetToEmpty();

        Money.Instance.Sell(cropReference.SellingPrice);
    }
}
