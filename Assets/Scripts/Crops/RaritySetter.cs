using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;


public class RaritySetter : MonoBehaviour
{
    private List<Rarity> rarities;

    private Crop cropComponentReference;
    private IconApplier iconApplier;
    private Image imageComponentReference;

    private bool isLoading = false;

    private async void Start()
    {
        this.imageComponentReference = this.GetComponent<Image>();
        this.iconApplier = this.GetComponent<IconApplier>();
        this.imageComponentReference.enabled = false;
        await LoadRarities();
    }


    private async void Update()
    {
        if (rarities == null && !isLoading)
        {
            isLoading = true;
            try
            {
                rarities = await DataRetriever.Instance.RetrieveRarities();
                if (rarities != null)
                {
                    Debug.Log($"[RARITY SETTER]: Loaded {rarities.Count} rarities");
                }
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

    private async Task LoadRarities()
    {
        if (!isLoading)
        {
            isLoading = true;
            try 
            {
                rarities = await DataRetriever.Instance.RetrieveRarities();
                if (rarities == null || rarities.Count == 0)
                {
                    Debug.LogError("Failed to load rarities from database");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading rarities: {e.Message}");
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
        foreach (Rarity rarity in rarities)
        {
            Debug.Log(rarity.RarityID);
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
    private void DisplayRarity(int rarityID)
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
    private bool IsFertileInCurrentSeason()
    {
        bool isFertile = false;

        if (ActiveSeasonManager.Instance.ActiveSeason.FertileCrops.Exists(fertileCrop => fertileCrop == this.cropComponentReference.CropID))
        {
            isFertile = true;
        }

        return isFertile;
    }

    private bool IsInfertileInCurrentSeason()
    {
        bool isInfertile = false;

        if (ActiveSeasonManager.Instance.ActiveSeason.InfertileCrops.Exists(infertileCrop => infertileCrop == this.cropComponentReference.CropID))
        {
            isInfertile = true;
        }

        return isInfertile;
    }
}
