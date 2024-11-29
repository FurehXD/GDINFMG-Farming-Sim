using TMPro;
using UnityEngine;

public class QualitySetter : MonoBehaviour
{
    //you can apply colors here if you want
    public void SetQuality(int qualityID)
    {
        string qualityText = "";
        for(int i = 0; i < qualityID; i++) 
            qualityText += "$";

        this.GetComponent<TextMeshProUGUI>().text = qualityText;
    }
}
