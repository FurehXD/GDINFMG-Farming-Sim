using UnityEngine.UI;

public class CloseInventoryButton : BaseButton
{
    private Image imageComponent;
    protected override void OnEnable()
    {
        base.OnEnable();
        InventoryManager.OnInventoryOpened += EnableImage;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        InventoryManager.OnInventoryOpened -= EnableImage;
    }
    private void EnableImage()
    {
        this.imageComponent.enabled = true;
    }
    private void DisableImage()
    {
        this.imageComponent.enabled = false;
    }
    protected override void Start()
    {
        base.Start();
        this.imageComponent = this.GetComponent<Image>();
        this.DisableImage();
    }
    protected override void PerformClick()
    {
        base.PerformClick();

        InventoryManager.Instance.CloseInventory();
    }
}
