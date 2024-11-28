using UnityEngine;
using UnityEngine.UI;

public class IconApplier : MonoBehaviour
{
    private void Update()
    {
        if(this.GetComponent<Crop>())
            this.GetComponent<Image>().sprite = this.GetComponent<Crop>().CropIcon;
    }

    public void ApplyThisIcon(IconApplier iconToApply)
    {
        this.GetComponent<Image>().sprite = iconToApply.GetComponent<Image>().sprite;
    }
}
