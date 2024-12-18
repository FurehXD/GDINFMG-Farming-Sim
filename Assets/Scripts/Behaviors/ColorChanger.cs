using UnityEngine;
using UnityEngine.UI;

public class ColorChanger : MonoBehaviour
{
    [SerializeField]
    private Color targetColor = Color.white;
    private Color defaultColor;

    private void OnEnable()
    {
        InventoryManager.OnInventoryClosed += this.ResetColor;
    }
    private void OnDisable()
    {
        InventoryManager.OnInventoryClosed -= this.ResetColor;
    }
    private void Start()
    {
        this.defaultColor = this.GetComponent<Image>().color;
    }
    public void SetColor()
    {
        this.GetComponent<Image>().color = targetColor;
    }
    public void ResetColor()
    {
        this.GetComponent<Image>().color = defaultColor;
    }
}
