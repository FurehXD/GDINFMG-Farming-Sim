using UnityEngine;

public class DataGrabber : MonoBehaviour
{
    void Start()
    {
        // Get all crops
        StartCoroutine(DatabaseManager.Instance.GetCrops((crops) => {
            if (crops != null)
            {
                foreach (var crop in crops)
                {
                    Debug.Log($"Loaded crop: {crop.CropName}");
                }
            }
        }));

        
    }
}
