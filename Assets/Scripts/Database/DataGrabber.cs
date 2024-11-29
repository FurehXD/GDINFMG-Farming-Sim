using UnityEngine;

public class DataGrabber : MonoBehaviour
{
    private void Start()
    {
        DatabaseManager.Instance.FetchAllDataExample();
    }
}
