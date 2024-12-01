using TMPro;
using UnityEngine;

public class PlotAreaLabelSetter : MonoBehaviour
{
    private TextMeshProUGUI currentPlotLabel;
    private void Start()
    {
        this.currentPlotLabel = this.GetComponent<TextMeshProUGUI>();
    }
    private void Update()
    {
        if (this.currentPlotLabel != null)
            this.currentPlotLabel.text = PlotAreaRetriever.Instance.CurrentPlotName;
    }
}
