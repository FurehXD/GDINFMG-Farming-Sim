using UnityEngine;

public class Crop : MonoBehaviour
{
    [SerializeField]
    [Range(1, 50)]
    [Tooltip("Getting this set correctly will retrieve all its necessary attributes from the Database")]
    protected int cropID;

    protected Sprite cropIcon;
    public Sprite CropIcon { get { return cropIcon; } }

    protected string cropAssetDirectory;//MYSQL
    protected string cropName;//MYSQL

    public int CropID { set { this.cropID = value; } }

    protected float growthRate;//MYSQL
    protected int growthID; //MYSQL

    protected virtual void Update()
    {
        this.cropName = DataRetriever.Instance.RetrieveCropName(this.cropID);
        this.cropAssetDirectory = DataRetriever.Instance.RetrieveCropAssetDirectory(this.cropID);

        this.cropIcon = Resources.Load<Sprite>(this.cropAssetDirectory);

        this.growthID = DataRetriever.Instance.RetrieveGrowthID(this.cropID);
        this.growthRate = DataRetriever.Instance.RetrieveCropGrowthRate(this.growthID);
    }
}
