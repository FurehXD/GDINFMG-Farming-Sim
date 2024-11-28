using System.Data.Common;
using System.Xml.Linq;
using UnityEngine;

/*
 * Retrives Data from the database then assigns it to any respective attribute
 */
public class DataRetriever : MonoBehaviour
{
    public static DataRetriever Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance.gameObject);
    }

    private void Update()
    {
        
    }
}
