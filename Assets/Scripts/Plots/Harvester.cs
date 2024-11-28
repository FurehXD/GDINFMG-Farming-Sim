using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Harvester : MonoBehaviour
{
    public void HarvestCrop()
    {
        Crop cropReference = this.GetComponentInChildren<Crop>();

        cropReference.GetComponent<Image>().enabled = false;
        this.GetComponentInChildren<RaritySetter>().GetComponent<Image>().enabled = false;
        this.GetComponentInChildren<TextMeshProUGUI>().enabled = false;

        Money.Instance.Sell(cropReference.SellingPrice);
    }
}
