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
        if (ActiveSeasonManager.Instance == null || !ActiveSeasonManager.Instance.IsInitialized)
            return;

        if (ActiveSeasonManager.Instance.ActiveSeason == null)
            return;

        string assetPath = ActiveSeasonManager.Instance.ActiveSeason.AssetPath;
        if (string.IsNullOrEmpty(assetPath))
            return;

        this.imageComponent.sprite = Resources.Load<Sprite>(assetPath);
    }
}