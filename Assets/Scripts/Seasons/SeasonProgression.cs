using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SeasonProgression : MonoBehaviour
{
    public static SeasonProgression Instance;

    private List<int> seasonDurations = new(); //IN ORDER
    public List<int> SeasonDurations{ set { this.seasonDurations = value; } } //IN ORDER

    [SerializeField]
    private float countdownPrecision = 0.1f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }
    public void StartSeasonProgression()
    {
        Debug.Log("Season Count: " + this.seasonDurations.Count);
        StartCoroutine(this.ProgressSeasons());
    }
    private IEnumerator ProgressSeasons()
    {
        foreach (int duration in this.seasonDurations)
        {
            float seasonDuration = duration;

            while (seasonDuration > 0.0f)
            {
                yield return new WaitForSecondsRealtime(this.countdownPrecision);

                seasonDuration -= this.countdownPrecision;
            }

            ActiveSeasonManager.Instance.MoveToNextSeason();
            continue;
        }
    }
}
