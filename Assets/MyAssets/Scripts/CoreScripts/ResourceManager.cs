using UnityEngine;
using System;

public class ResourceManager : MonoBehaviour
{
    public static event Action<int, int, int, float> OnResourceChanged;
    public static ResourceManager Instance { get; private set; }
    [Header("Stats")]
    int motivation=50;
    int supply=50;
    int population=50;
    float rebellion=0f;

    void Awake()
    {
        Instance = this;
    }

    

    public void ChangeStats(int _population,int _supply,int _motivation,float _rebellion)
    {
        population += _population;
        supply += _supply;
        motivation += _motivation;
        rebellion += _rebellion;

        if (isWin())
        {
            GameManager.Instance.WinState();
            
        }
        else if(isLose())
        {
            GameManager.Instance.LoseState();
            
        }

        if (OnResourceChanged != null)
        {
            OnResourceChanged.Invoke(population, supply, motivation, rebellion);
        }
    }

    public void SetStats(int _population, int _supply, int _motivation, float _rebellion)
    {
        population = _population;
        supply = _supply;
        motivation = _motivation;
        rebellion = _rebellion;

        if (isWin())
        {
            GameManager.Instance.WinState();

        }
        else if (isLose())
        {
            GameManager.Instance.LoseState();

        }

        if (OnResourceChanged != null)
        {
            OnResourceChanged.Invoke(population, supply, motivation, rebellion);
        }
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
