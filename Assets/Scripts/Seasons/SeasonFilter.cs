using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeasonFilter : MonoBehaviour
{
    [SerializeField]
    [UnityEngine.Range(0f, 255f)]
    private float baseAlpha = 158f;
    private Image imageComponent;

    private void Start()
    {
        imageComponent = GetComponent<Image>();
    }
    private void Update()
    {
        Color newColor = ActiveSeasonManager.Instance.ActiveSeason.SeasonColor;
        float normalizedAlpha = this.baseAlpha / 255f;

        newColor.a = normalizedAlpha;
        imageComponent.color = newColor;
    }
}
