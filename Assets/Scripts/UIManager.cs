using System.Collections;
using System.Resources;
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

    [SerializeField] private Slider preview_rebellion_bar;
    [SerializeField] private Image preview_fill_image;


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
        HidePreviewStats();
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


    public void OnHoverL()
    {
        EventCard activeCard = GameManager.Instance.getCurrentCard();

        if (activeCard == null) return;

        ShowPreviewStats(activeCard.l_population, activeCard.l_supply, activeCard.l_motivation, activeCard.l_rebellion);
    }

    public void OnHoverR()
    {
        EventCard activeCard = GameManager.Instance.getCurrentCard();

        if (activeCard == null) return;

        ShowPreviewStats(activeCard.r_population, activeCard.r_supply, activeCard.r_motivation, activeCard.r_rebellion);
    }




    public void ShowPreviewStats(int prePopulation,int preSupply,int preMotivation,int preRebellion)
    {

        // Önce mevcut değerleri çekiyoruz
        int currentPopulation = ResourceManager.Instance.getPopulation();
        int currentSupply = ResourceManager.Instance.getSupply();
        int currentMotivation = ResourceManager.Instance.getMotivation();
        float currentRebellion = ResourceManager.Instance.getRebellion();

        // Label'ları tek satırda yardımcı metot ile güncelliyoruz
        population_label.text = "Nüfus: " + FormatStat(currentPopulation, prePopulation);
        supply_label.text = "Erzak: " + FormatStat(currentSupply, preSupply);
        motivation_label.text = "Moral: " + FormatStat(currentMotivation, preMotivation);

        //Rebellion part is a little bit more complicated
        if (preRebellion > 0)
        {
            // İsyan artacaksa: Ana bar sabit, saydam bar ileride. Renk: Saydam Kırmızı
            rebellion_bar.value = currentRebellion;
            preview_rebellion_bar.value = currentRebellion + preRebellion;
            preview_fill_image.color = new Color(1f, 0f, 0f, 0.5f);
        }
        else if (preRebellion < 0)
        {
            // İsyan azalacaksa: Ana bar geriler, saydam bar eski konumda kalır. Renk: Saydam Yeşil
            rebellion_bar.value = currentRebellion + preRebellion;
            preview_rebellion_bar.value = currentRebellion;
            preview_fill_image.color = new Color(0f, 1f, 0f, 0.5f);
        }
        else
        {
            // Değişim yoksa barları eşitle ve saydam barı tamamen görünmez yap (Alpha = 0)
            rebellion_bar.value = currentRebellion;
            preview_rebellion_bar.value = currentRebellion;
            preview_fill_image.color = new Color(1f, 1f, 1f, 0f);
        }


    }
    // Rengi ve işareti ayarlayan yardımcı metot
    private string FormatStat(int currentValue, int preValue)
    {
        if (preValue > 0)
        {
            return  " <color=green>+"+ (currentValue + preValue).ToString() + "</color>";
        }
        else if (preValue < 0)
        {
            return " <color=red>-" + (currentValue + preValue).ToString() + "</color>";
        }
        else
        {
            // Değişim 0 ise sadece mevcut değeri döndür
            return currentValue.ToString();
        }
    }

    public void HidePreviewStats()
    {
        int currentPopulation = ResourceManager.Instance.getPopulation();
        int currentSupply = ResourceManager.Instance.getSupply();
        int currentMotivation = ResourceManager.Instance.getMotivation();
        float currentRebellion = ResourceManager.Instance.getRebellion();

        population_label.text = "Nüfus: " + currentPopulation.ToString();
        motivation_label.text = "Moral: " + currentMotivation.ToString();
        supply_label.text = "Erzak: " + currentSupply.ToString();
        //Rebellion part    
        rebellion_bar.value = currentRebellion;
        preview_rebellion_bar.value = currentRebellion;
        preview_fill_image.color = new Color(1f, 1f, 1f, 0f); // Alpha'yı sıfırla
    }

}
