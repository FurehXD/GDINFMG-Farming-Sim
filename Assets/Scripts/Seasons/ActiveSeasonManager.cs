using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;  // Add this for async support

public class ActiveSeasonManager : MonoBehaviour
{
    public static ActiveSeasonManager Instance { get; private set; }
    public Season ActiveSeason;
    public List<Season> Seasons = new();
    private int currentSeasonIndex = 0;
    public bool IsInitialized { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    private async void Start()
    {
        this.Seasons = await DataRetriever.Instance.RetrieveSeasons();
        this.ActiveSeason = Seasons[this.currentSeasonIndex];
        SeasonProgression.Instance.SeasonDurations = this.GetSeasonDurations();
        SeasonProgression.Instance.StartSeasonProgression();
        IsInitialized = true;
    }

    private float updateInterval = 1f;
    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;
            UpdateSeasons();
        }
    }

    private async void UpdateSeasons()
    {
        this.Seasons = await DataRetriever.Instance.RetrieveSeasons();
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