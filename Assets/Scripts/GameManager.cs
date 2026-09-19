
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Reflection.Metadata.Ecma335;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public GameState currentState;
    public static GameManager Instance { get; private set;}

    public List<EventCard> eventList = new List<EventCard>();
    private List<EventCard> pastEventList = new List<EventCard>(); 
    void Start()
    {
        Instance = this;
        SkipNextEvent();
    }


    EventCard currentEvent;
    
    public void DrawRandomCard()
    {
        
        int randomIndex = Random.Range(0, eventList.Count);
        currentEvent = eventList[randomIndex];
        if (pastEventList.Contains(currentEvent))
        {
            DrawRandomCard();
            return;
        }
        pastEventList.Add(currentEvent);

        if (eventList.Count == pastEventList.Count)
        {
            pastEventList.Clear();
        }
        UIManager.Instance.activateButtons();
        StartCoroutine(UIManager.Instance.SetCard(currentEvent.story, currentEvent.lButtonText, currentEvent.rButtonText));

        
        
        
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

        Debug.Log("currentEvent durumu: " + (currentEvent == null ? "BOŞ" : "DOLU"));
        Debug.Log("ResourceManager durumu: " + (ResourceManager.Instance == null ? "BOŞ" : "DOLU"));
        ResourceManager.Instance.ChangeStats(currentEvent.l_population, currentEvent.l_supply, currentEvent.l_motivation, currentEvent.l_rebellion);
        if (currentState != GameState.ended)
        {
            SkipNextEvent();
        }

        UIManager.Instance.disableButtons();


    }

    public void OnRightButtonAction()
    {

        Debug.Log("currentEvent durumu: " + (currentEvent == null ? "BOŞ" : "DOLU"));
        Debug.Log("ResourceManager durumu: " + (ResourceManager.Instance == null ? "BOŞ" : "DOLU"));
        ResourceManager.Instance.ChangeStats(currentEvent.r_population, currentEvent.r_supply, currentEvent.r_motivation, currentEvent.r_rebellion);
        if (currentState != GameState.ended)
        {
            SkipNextEvent();
        }

        UIManager.Instance.disableButtons();

    }
    
   
    
    public void WinState()
    {
        currentState = GameState.ended;
        StopAllCoroutines();
        StartCoroutine(UIManager.Instance.WinMessage());
        UIManager.Instance.SetResButton(true);
    }

    public void LoseState()
    {
        currentState = GameState.ended;
        StopAllCoroutines();
        StartCoroutine(UIManager.Instance.LoseMessage());
        UIManager.Instance.SetResButton(true);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ResourceManager.Instance.ChangeStats(5, 0, 0, 0);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ResourceManager.Instance.ChangeStats(-5, 0, 0, 0);
        }
    }


    public void StartTheGame()
    {
        GameReset();
        UIManager.Instance.SetResButton(false);
        SkipNextEvent();
    }


    public void GameReset()
    {
        ResourceManager.Instance.StatReset();
        UIManager.Instance.UIReset();
        currentState = GameState.working;
    }

    public EventCard getCurrentCard()
    {
        return currentEvent;
    }

}
