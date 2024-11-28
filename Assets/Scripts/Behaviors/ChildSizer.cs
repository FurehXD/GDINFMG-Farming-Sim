using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChildSizer : MonoBehaviour
{
    private void Start()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            if(this.transform.GetChild(i).GetComponent<RectTransform>())
                this.transform.GetChild(i).GetComponent<RectTransform>().sizeDelta /= (new Vector2(100.0f, 100.0f) / this.transform.parent.GetComponent<GridLayoutGroup>().cellSize);
            if(this.transform.GetChild(i).GetComponent<TextMeshProUGUI>())
                this.transform.GetChild(i).GetComponent<TextMeshProUGUI>().fontSizeMin /= (new Vector2(100.0f, 100.0f) / this.transform.parent.GetComponent<GridLayoutGroup>().cellSize).x;
        }
    }
}
