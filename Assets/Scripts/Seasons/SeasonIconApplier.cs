using UnityEngine;
using UnityEngine.UI;

public class SeasonIconApplier : MonoBehaviour
{
    private Image imageComponent;

    private void Start()
    {
        this.imageComponent = this.GetComponent<Image>();
    }
    private void Update()
    {
        if(ActiveSeasonManager.Instance && ActiveSeasonManager.Instance.ActiveSeason.SeasonIconDirectory != "")
            this.imageComponent.sprite = Resources.Load<Sprite>(ActiveSeasonManager.Instance.ActiveSeason.SeasonIconDirectory);
    }
}
