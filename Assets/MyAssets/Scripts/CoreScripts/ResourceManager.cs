using UnityEngine;
using System;

public class ResourceManager : MonoBehaviour
{
    public static event Action<int, int, int, float> OnResourceChanged;
    public static event Action<GameResult> OnGameResult;
    public static ResourceManager Instance { get; private set; }

    public enum GameResult
    {
        None,
        Won,
        Lost
    }

    [Header("Stats")]
    int motivation=50;
    int supply=50;
    int population=50;
    float rebellion=0f;

    void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        GameManager.OnStatsChangeRequested += ChangeStats;
        GameManager.OnStatsResetRequested += StatReset;
    }

    private void OnDisable()
    {
        GameManager.OnStatsChangeRequested -= ChangeStats;
        GameManager.OnStatsResetRequested -= StatReset;
    }

    

    public void ChangeStats(int _population,int _supply,int _motivation,float _rebellion)
    {
        population += _population;
        supply += _supply;
        motivation += _motivation;
        rebellion += _rebellion;

        population = Mathf.Clamp(population, 0, 100);
        supply = Mathf.Clamp(supply, 0, 100);
        motivation = Mathf.Clamp(motivation, 0, 100);
        rebellion = Mathf.Clamp(rebellion, 0f, 100f);

        OnResourceChanged?.Invoke(population, supply, motivation, rebellion);
        OnGameResult?.Invoke(isLose() ? GameResult.Lost : isWin() ? GameResult.Won : GameResult.None);
    }

    public void SetStats(int _population, int _supply, int _motivation, float _rebellion)
    {
        population = Mathf.Clamp(_population, 0, 100);
        supply = Mathf.Clamp(_supply, 0, 100);
        motivation = Mathf.Clamp(_motivation, 0, 100);
        rebellion = Mathf.Clamp(_rebellion, 0f, 100f);

        OnResourceChanged?.Invoke(population, supply, motivation, rebellion);
        OnGameResult?.Invoke(isLose() ? GameResult.Lost : isWin() ? GameResult.Won : GameResult.None);
    }




    public bool isWin()
    {
        if (population >= 100)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public bool isLose()
    {
        if(population<=0 || motivation<=0 || supply<=0 || rebellion >= 100)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void StatReset()
    {
        SetStats(50, 50, 50, 0);
    }

    public int getPopulation()
    {
        return population;
    }

    public int getSupply()
    {
        return supply;
    }

    public int getMotivation()
    {
        return motivation;
    }

    public float getRebellion()
    {
        return rebellion;
    }

}
