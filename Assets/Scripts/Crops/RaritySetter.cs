using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaritySetter : MonoBehaviour
{
    private List<Rarity> rarities = new(); //MySQL

    private Crop cropComponentReference;
    private IconApplier iconApplier;
    private Image imageComponentReference;

    private void Start()
    {
        this.imageComponentReference = this.GetComponent<Image>();
        this.iconApplier = this.GetComponent<IconApplier>();
        this.imageComponentReference.enabled = false;
    }
    private void Update()
    {
        this.rarities = DataRetriever.Instance.RetrieveRarities();
    }
    public int DetermineRarity(Crop cropComponentReference)
    {
        float randomNumber = Random.Range(0.01f, 1);

        this.cropComponentReference = cropComponentReference;

        foreach (Rarity rarity in rarities)
        {
            if (randomNumber >= rarity.RarityProbability)
            {
                this.DisplayRarity(rarity.RarityID);
                Debug.Log(rarity.RarityType + ", Random Number = " + randomNumber + " vs. " + rarity.RarityProbability);
                return rarity.RarityID;
            }
        }
        Debug.Log("ERROR NO RARITY ID WAS RETURNED");
        return 0;
    }
    public void DisplayRarity(int rarityID)
    {
        Rarity rarity = this.rarities.Find(rarity => rarity.RarityID == rarityID);

        string rarityIndicatorDirectory = this.cropComponentReference.CropAssetDirectory;

        rarityIndicatorDirectory += " [Rarity Indicator]";

        Sprite rarityIndicator = Resources.Load<Sprite>(rarityIndicatorDirectory);

        if (rarityIndicator != null)
        {
            this.iconApplier.ApplyIcon(rarityIndicator);
            this.imageComponentReference.enabled = true;
            this.imageComponentReference.color = rarity.RarityColor / 255.0f;
        }
        else
            Debug.LogError("RARITY INDICATOR WAS NOT APPLIED");
    }
}
