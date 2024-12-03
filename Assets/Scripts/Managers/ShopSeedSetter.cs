using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSeedSetter : MonoBehaviour
{
    [SerializeField]
    private CropButton cropTemplate;
    [SerializeField]
    private GridLayoutGroup seedArea;

    private List<CropButton> seedReferences = new();

    private TMP_Dropdown areaSelectorDropdown;
    private int maxSeedID = 12;//the last num for seeds in the market table

    private void Start()
    {
        this.areaSelectorDropdown = this.GetComponentInChildren<TMP_Dropdown>();
        //this.plots = DataRetriever.Instance.RetrievePlotAreas();
        this.UpdateGameLocation();
    }
    private void Update()
    {
        //
    }
    public void UpdateGameLocation()
    {
        this.ResetPlots();

        //string currentPlotAreaString = this.areaSelectorDropdown.options[this.areaSelectorDropdown.value].text;
    
        if (this.cropTemplate != null && this.seedArea != null)
        {
            int areaSize = maxSeedID;  // Consider getting this from database instead

            Debug.Log($"Starting seed instantiation with areaSize: {areaSize}");

            for(int i = 1; i <= areaSize; i++)
            {
                if(this.seedArea)
                {
                    this.seedReferences.Add(Instantiate(this.cropTemplate, this.seedArea.transform));
                    this.seedReferences[i-1].CropID = i;
                    Debug.Log($"Instantiating seed number: {i}");
                }
            }
        }
        else
        {
            Debug.LogWarning("cropTemplate or seedArea not initialized");
        }
    }
    private void ResetPlots()
    {
        foreach(CropButton cropButton in this.seedReferences)
            Destroy(cropButton.gameObject);

        this.seedReferences.Clear();
    }

}
