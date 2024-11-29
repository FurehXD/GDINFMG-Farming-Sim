using TMPro;
using UnityEngine;

public class EmptyTextEnabler : MonoBehaviour
{
    private TextMeshProUGUI emptyText;
    private void Start()
    {
        this.emptyText = this.GetComponent<TextMeshProUGUI>();
    }
    //private void OnEnable()
    //{
    //    CropProducer.OnProductionStarted += this.DontSetToEmpty;
    //}
    //private void OnDisable()
    //{
    //    CropProducer.OnProductionStarted -= this.DontSetToEmpty;
    //}
    public void SetToEmpty()
    {
        this.emptyText.enabled = true;
    }
    public void DontSetToEmpty()
    {
        this.emptyText.enabled = false;
    }
}
