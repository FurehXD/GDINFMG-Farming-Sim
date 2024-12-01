using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSeasonManager : MonoBehaviour
{
    public static ActiveSeasonManager Instance { get; private set; }

    public Season ActiveSeason;

    public List<Season> Seasons = new();
    private int currentSeasonIndex = 0;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }
    private void Start()
    {
        this.Seasons = DataRetriever.Instance.RetrieveSeasons();

        this.ActiveSeason = Seasons[this.currentSeasonIndex];
        SeasonProgression.Instance.SeasonDurations = this.GetSeasonDurations();

        SeasonProgression.Instance.StartSeasonProgression();
    }
    private void Update()
    {
        this.Seasons = DataRetriever.Instance.RetrieveSeasons();
        SeasonProgression.Instance.SeasonDurations = this.GetSeasonDurations();
    }
    private List<int> GetSeasonDurations()
    {
        List<int> tempSeasonDurations = new();

        if (Seasons != null && Seasons.Count > 0)
        {
            foreach (var season in Seasons)
                tempSeasonDurations.Add(season.SeasonDuration);
        }

        return tempSeasonDurations;
    }
    public void MoveToNextSeason()
    {
        this.currentSeasonIndex++;

        if (this.currentSeasonIndex > this.Seasons.Count - 1)
        {
            this.currentSeasonIndex = 0;
            SeasonProgression.Instance.StartSeasonProgression();
        }

        this.ActiveSeason = Seasons[this.currentSeasonIndex];
    }
}
