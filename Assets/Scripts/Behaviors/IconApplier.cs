using UnityEngine;
using UnityEngine.UI;

public class IconApplier : MonoBehaviour
{
    private void Update()
    {
        if(this.GetComponent<Crop>())
            this.GetComponent<Image>().sprite = this.GetComponent<Crop>().CropIcon;
    }
    public void ApplyIcon(Sprite iconToApply)
    {
        this.GetComponent<Image>().sprite = iconToApply;
    }

    //This is specifically for calling it in events outside of itself
    public void ApplyThisIcon(IconApplier iconToApply)
    {
        this.GetComponent<Image>().sprite = iconToApply.GetComponent<Image>().sprite;
    }
}
