using UnityEngine;

public class DataGrabber : MonoBehaviour
{
    void Start()
    {
        DatabaseManager.Instance.FetchAllDataExample();
    }
}
