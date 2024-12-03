using TMPro;
using UnityEngine;

public class PlotAreaLabelSetter : MonoBehaviour
{
    private TextMeshProUGUI currentPlotLabel;
    private void Start()
    {
        this.currentPlotLabel = this.GetComponent<TextMeshProUGUI>();

        this.SetPlotAreaName();
    }
    private void Update()
    {
        this.SetPlotAreaName();
    }
    private void SetPlotAreaName()
    {
        if (this.currentPlotLabel != null)
        {
            this.currentPlotLabel.text = PlotAreaRetriever.Instance.CurrentPlotName;
        }
    }
}
