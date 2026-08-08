using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        
        Instance = this;
    }


    [SerializeField] private GameObject left_button;
    [SerializeField] private GameObject right_button;

    [SerializeField] private TextMeshProUGUI population_label;
    [SerializeField] private TextMeshProUGUI supply_label;
    [SerializeField] private TextMeshProUGUI motivation_label;
    [SerializeField] private Slider rebellion_bar;
    [SerializeField] private TextMeshProUGUI chatBubble;

    [SerializeField] private TextMeshProUGUI lButtonText;
    [SerializeField] private TextMeshProUGUI rButtonText;

    [SerializeField] private GameObject restart_button;



    public void OnEnable()
    {
        ResourceManager.OnResourceChanged += UpdateUIStats;
    }

    public void OnDisable()
    {
        ResourceManager.OnResourceChanged -= UpdateUIStats;
    }


    public void UpdateUIStats(int population,int supply,int motivation,float rebellion)
    {
        population_label.text = "Nüfus: " + population.ToString();
        motivation_label.text = "Moral: " + motivation.ToString();
        supply_label.text = "Erzak: " + supply.ToString();
        rebellion_bar.value = rebellion;
    }



    public void disableButtons()
    {
        left_button.SetActive(false);
        right_button.SetActive(false);
    }

    public void activateButtons()
    {
        left_button.SetActive(true);
        right_button.SetActive(true);
    }

    public void SetButtons(string lbuttonText,string rbuttonText)
    {
        lButtonText.text = lbuttonText;
        rButtonText.text = rbuttonText;
    }

    public void SetStory(string _story)
    {
        chatBubble.text = _story;
    }

    public IEnumerator SetCard(string _story,string lbuttonText,string rbuttonText)
    {
        chatBubble.text = "";
        disableButtons();
        foreach(char c in _story)
        {
            chatBubble.text += c;
            yield return new WaitForSeconds(0.05f);
        }
        lButtonText.text = lbuttonText;
        rButtonText.text = rbuttonText;

        activateButtons();
    }

    public IEnumerator WinMessage()
    {
        
        string winMessage = "EFENDİM ARTIK KOCAMAN BİR ORDUMUZ VAR!!!!";
        chatBubble.text = "";
        disableButtons();
        foreach(char C in winMessage)
        {
            chatBubble.text += C;
            yield return new WaitForSeconds(0.05f);
        }
    }


    public IEnumerator LoseMessage()
    {
        string winMessage = "efendim üzgünüm ama bütün birlikler toplandı ve üstümüze doğru geliyorlar...";
        chatBubble.text = "";
        disableButtons();
        foreach (char C in winMessage)
        {
            chatBubble.text += C;
            yield return new WaitForSeconds(0.05f);
        }
    }


    public void SetResButton(bool state)
    {
        if (state)
        {
            restart_button.SetActive(true);
        }
        else
        {
            restart_button.SetActive(false);
        }
    }

    
    public void UIReset()
    {
        
        StartCoroutine(SetCard("", "", ""));
    }


}
