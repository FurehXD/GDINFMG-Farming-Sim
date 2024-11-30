using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaritySetter : MonoBehaviour
{
    private List<Rarity> rarities = new(); //MySQL

    private Crop cropComponentReference;
    private IconApplier iconApplier;
    private Image imageComponentReference;

    private bool isLoading = false;

    private void Start()
    {
        this.imageComponentReference = this.GetComponent<Image>();
        this.iconApplier = this.GetComponent<IconApplier>();
        this.imageComponentReference.enabled = false;
    }
    private async void Update()
    {
        if (rarities == null && !isLoading)
        {
            isLoading = true;
            try
            {
                rarities = await DataRetriever.Instance.RetrieveRarities();
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to retrieve rarities: {e.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }
    }
    public int DetermineRarity(Crop cropComponentReference)
    {
        float randomNumber = UnityEngine.Random.Range(0.01f, 1);
        this.cropComponentReference = cropComponentReference;
        float cumulativeProbability = 0;

        foreach (Rarity rarity in rarities)
        {
            cumulativeProbability += rarity.RarityProbability;
            if (randomNumber <= cumulativeProbability)
            {
                this.DisplayRarity(rarity.RarityID);
                Debug.Log($"{rarity.RarityType}, Random Number = {randomNumber} vs. Cumulative Probability {cumulativeProbability}");
                return rarity.RarityID;
            }
        }

        Debug.Log("ERROR NO RARITY ID WAS RETURNED");
        return 1; // Return Common rarity as fallback
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
