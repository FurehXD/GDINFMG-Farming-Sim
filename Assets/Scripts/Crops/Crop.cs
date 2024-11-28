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

    protected string cropAssetDirectory;//MYSQL
    public string CropAssetDirectory {  get { return cropAssetDirectory; } }    
    protected string cropName;//MYSQL

    protected int rarityID;
    public int RarityID { set { this.rarityID = value; } }

    protected float growthRate;//MYSQL
    public float GrowthRate { get { return this.growthRate; } }
    protected int growthID; //MYSQL

    private int marketID;//MYSQL
    private int sellingPrice;//MYSQL
    public int SellingPrice { get { return this.sellingPrice; } }

    protected virtual void Update()
    {
        this.cropName = DataRetriever.Instance.RetrieveCropName(this.cropID);
        this.cropAssetDirectory = DataRetriever.Instance.RetrieveCropAssetDirectory(this.cropID);

        this.cropIcon = Resources.Load<Sprite>(this.cropAssetDirectory);

        this.growthID = DataRetriever.Instance.RetrieveGrowthID(this.cropID);
        this.growthRate = DataRetriever.Instance.RetrieveCropGrowthRate(this.growthID);

        this.marketID = DataRetriever.Instance.RetrieveMarketID(this.cropID);
        this.sellingPrice = DataRetriever.Instance.RetrieveSellingPrice(this.marketID);
    }

}
