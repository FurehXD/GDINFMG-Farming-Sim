using UnityEngine;
using UnityEngine.UI;

public class CropIconEnabler : MonoBehaviour
{
    private Image imageComponent;

    private void Start()
    {
        if (this.GetComponent<Image>() && this.GetComponent<Crop>())
            this.imageComponent = GetComponent<Image>();
        else
            Debug.LogError("Crop Icon Enabler Component does not have a Crop MonoBehavior Attached To It");
    }
    private void DisableCropIcon()
    {
        if(this.imageComponent)
            this.imageComponent.enabled = false;
    }
    private void EnableCropIcon()
    {
        if (this.imageComponent)
            this.imageComponent.enabled = true;
    }
    public void EnableCropIcon(bool enabled)
    {
        if (this.imageComponent)
            this.imageComponent.enabled = enabled;
    }
}
