using UnityEngine;

public class Crop : MonoBehaviour
{
    [SerializeField]
    [Range(1, 50)]
    [Tooltip("Getting this set correctly will retrieve all its necessary attributes from the Database")]
    protected int cropID;
    public int CropID { set { this.cropID = value; } get { return this.cropID; } }

    protected Sprite cropIcon;
    public Sprite CropIcon { get { return cropIcon; } }

    protected string cropAssetDirectory;
    public string CropAssetDirectory { get { return cropAssetDirectory; } }

    protected string cropName;
    public string CropName { get { return cropName; } }
    protected int rarityID;
    public int RarityID { set { this.rarityID = value; } }

    protected float growthRate;
    public float GrowthRate { get { return this.growthRate; } }

    protected int growthID;
    private int marketID;
    private decimal sellingPrice;
    public decimal SellingPrice { get { return this.sellingPrice; } }

    private bool isLoading = false;

    protected virtual async void Update()
    {
        if (!isLoading)
        {
            isLoading = true;
            try
            {
                cropName = await DataRetriever.Instance.RetrieveCropName(cropID);
                cropAssetDirectory = DataRetriever.Instance.RetrieveCropAssetDirectoryTemp(cropID);
                cropIcon = Resources.Load<Sprite>(cropAssetDirectory);
                growthID = await DataRetriever.Instance.RetrieveCropGrowthID(cropID);
                growthRate = await DataRetriever.Instance.RetrieveCropGrowthRate(growthID);
                marketID = await DataRetriever.Instance.RetrieveCropMarketID(cropID);
                sellingPrice = await DataRetriever.Instance.RetrieveCropSellingPrice(marketID);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error updating crop data: {e.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }
    }
}