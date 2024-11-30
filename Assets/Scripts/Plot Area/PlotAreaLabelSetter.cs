using TMPro;
using UnityEngine;

public class PlotAreaLabelSetter : MonoBehaviour
{
    private TextMeshProUGUI currentPlotLabel;
    [SerializeField]
    private TMP_Dropdown plotDropdown;
    private void Start()
    {
        this.currentPlotLabel = this.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if(this.currentPlotLabel != null && this.plotDropdown != null) 
            this.currentPlotLabel.text = this.plotDropdown.options[plotDropdown.value].text;
    }
}
