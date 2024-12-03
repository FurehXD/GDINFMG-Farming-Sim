using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Events;

public class CropProducer : MonoBehaviour
{
    [SerializeField]
    private float baseGrowthRate = 20.0f;
    private float growthRate;

    private float countDownPrecision = 0.1f;
    private Pulser pulserReference;

    private Crop cropComponentReference;

    private RaritySetter raritySetter;
    private CropIconEnabler cropIconEnabler;
    private EmptyTextEnabler emptyTextEnabler;
    private PlotButton plotButton;

    public UnityEvent OnReadyForHarvest;
    public static event Action OnProductionStarted;
    public void StartProduction(Crop crop)
    {
        this.pulserReference = this.GetComponentInChildren<Pulser>();
        this.raritySetter = this.GetComponentInChildren<RaritySetter>();
        this.cropIconEnabler = this.GetComponentInChildren<CropIconEnabler>();
        this.emptyTextEnabler = this.GetComponentInChildren<EmptyTextEnabler>();
        
        this.plotButton = this.GetComponent<PlotButton>();
        this.plotButton.DisableButton();

        this.cropIconEnabler.EnableCropIcon(true);
        this.emptyTextEnabler.DontSetToEmpty();

        OnProductionStarted?.Invoke();

        this.cropComponentReference = this.GetComponentInChildren<Crop>();
        this.cropComponentReference.CropID = crop.CropID;

        if (pulserReference == null)
            Debug.LogError("PULSER REFERENCE OF " + this.name + " DNE");

        this.growthRate = crop.GrowthRate;

        StartCoroutine(this.Produce());
    }
    private IEnumerator Produce()
    {
        float growthTime = this.CalculateGrowthTime();
        float baseGrowthTime = growthTime;

        this.pulserReference.StartPulsing();

        while (growthTime > 0.0f)
        {
            yield return new WaitForSecondsRealtime(this.countDownPrecision);

            growthTime -= this.countDownPrecision;
        }

        this.pulserReference.StopPulsing();
        this.cropComponentReference.RarityID = this.raritySetter.DetermineRarity(this.cropComponentReference);

        Logger.Instance.LogMessage(this.cropComponentReference.CropName + " finished production at " + baseGrowthTime + "s");
        OnReadyForHarvest.Invoke();
    }

    private float CalculateGrowthTime()
    {
        float growthTime = this.baseGrowthRate;
        //Debug.Log("in here: " + PlotAreaRetriever.Instance.CurrentPlotArea.PlotName);
        float plotAreaGrowthFactor = PlotAreaRetriever.Instance.CurrentPlotArea.GrowthBuffPercentage - 1;

        growthTime = this.baseGrowthRate / this.growthRate;
        float timeLost = growthTime * plotAreaGrowthFactor;
        growthTime -= timeLost;

        Debug.Log("GROWTH TIME Of " + this.cropComponentReference.CropName + " IS " + growthTime + "s");
        return growthTime;
    }
    public void UpdateGrowthTime(float growthTimeFactor)
    {
        this.growthRate *= growthTimeFactor;
    }
}
