
using UnityEngine;

public class Rarity
{
    private int rarityID;
    public int RarityID {  get { return rarityID; } }   
    private string rarityType;
    public string RarityType {  get { return rarityType; } }    
    private float priceBuffPercentage;
    public float PriceBuffPercentage { get { return this.priceBuffPercentage; } }
    private float rarityProbability;
    public float RarityProbability { get { return this.rarityProbability; } }

    private Color rarityColor;
    public Color RarityColor { get { return this.rarityColor; } }   
    public Rarity(int rarityID, string rarityType, float priceBuffPercentage, float rarityProbability, Color rarityColor)
    {
        this.rarityID = rarityID;
        this.rarityType = rarityType;
        this.priceBuffPercentage = priceBuffPercentage;
        this.rarityProbability = rarityProbability;
        this.rarityColor = rarityColor;
    }
}