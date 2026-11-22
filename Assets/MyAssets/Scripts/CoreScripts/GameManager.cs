using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Reflection.Metadata.Ecma335;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public GameState currentState;
    public static GameManager Instance { get; private set;}
    public static event Action<EventCard> OnCardChanged;
    public static event Action OnGameReset;
    public static event Action<int, int, int, float> OnStatsChangeRequested;
    public static event Action OnStatsResetRequested;

    public List<EventCard> eventList = new List<EventCard>();
    private List<EventCard> pastEventList = new List<EventCard>(); 
    

    public CharacterAnimController sadFolk;
    public CharacterAnimController angryFolk;
    
 

    void Start()
    {
        Instance = this;
        StartTheGame();
    }

    private void OnEnable()
    {
        ResourceManager.OnGameResult += HandleGameResult;
    }

    private void OnDisable()
    {
        ResourceManager.OnGameResult -= HandleGameResult;
    }

    

    EventCard currentEvent;
    public void DrawRandomCard()
    {
        if (eventList == null || eventList.Count == 0)
        {
            return;
        }

        int startIndex = UnityEngine.Random.Range(0, eventList.Count);
        EventCard selectedCard = null;

        for (int offset = 0; offset < eventList.Count; offset++)
        {
            int candidateIndex = (startIndex + offset) % eventList.Count;
            EventCard candidate = eventList[candidateIndex];

            if (candidate != null && !pastEventList.Contains(candidate))
            {
                selectedCard = candidate;
                break;
            }
        }

        if (selectedCard == null)
        {
            pastEventList.Clear();
            for (int i = 0; i < eventList.Count; i++)
            {
                int candidateIndex = (startIndex + i) % eventList.Count;
                if (eventList[candidateIndex] != null)
                {
                    selectedCard = eventList[candidateIndex];
                    break;
                }
            }
        }

        if (selectedCard == null)
        {
            return;
        }

        currentEvent = selectedCard;
        pastEventList.Add(currentEvent);
        OnCardChanged?.Invoke(currentEvent);
        Debug.Log("Current Event Emotion: " + currentEvent.emotion.ToString() + "Current Event messageSource: " + currentEvent.messenger.ToString());
        
        
        
    }

    
    public enum GameState
    {
        choosing,
        working,
        ended,
    }



    public void SkipNextEvent()
    {

        DrawRandomCard();
    }

    
    public void OnLeftButtonAction()
    {
        if (currentState == GameState.ended || currentEvent == null)
        {
            return;
        }

        OnStatsChangeRequested?.Invoke(currentEvent.l_population, currentEvent.l_supply, currentEvent.l_motivation, currentEvent.l_rebellion);
        if (currentState != GameState.ended)
        {
            SkipNextEvent();
        }

    }

    public void OnRightButtonAction()
    {
        if (currentState == GameState.ended || currentEvent == null)
        {
            return;
        }

        OnStatsChangeRequested?.Invoke(currentEvent.r_population, currentEvent.r_supply, currentEvent.r_motivation, currentEvent.r_rebellion);
        if (currentState != GameState.ended)
        {
            SkipNextEvent();
        }

    }
    
   
    
    public void WinState()
    {
        currentState = GameState.ended;
    }

    public void LoseState()
    {
        currentState = GameState.ended;
    }

    private void HandleGameResult(ResourceManager.GameResult result)
    {
        if (result == ResourceManager.GameResult.Won)
        {
            WinState();
        }
        else if (result == ResourceManager.GameResult.Lost)
        {
            LoseState();
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnStatsChangeRequested?.Invoke(5, 0, 0, 0);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            OnStatsChangeRequested?.Invoke(-5, 0, 0, 0);
        }
    }


    public void StartTheGame()
    {
        GameReset();
        pastEventList.Clear();
        SkipNextEvent();
    }


    public void GameReset()
    {
        OnStatsResetRequested?.Invoke();
        currentState = GameState.working;
        OnGameReset?.Invoke();
    }

    public EventCard getCurrentCard()
    {
        return currentEvent;
    }

}
