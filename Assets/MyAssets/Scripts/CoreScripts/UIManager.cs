using System.Collections;
using System.Resources;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public CharacterAnimController leader;
    
    public CharacterAnimController tenet;
    public CharacterAnimController angryFolk;

    public CharacterAnimController sadFolk;

    public EventCard emptyEvent;

    public Image chatBubbleBG;

    public static event Action UIManagerEvent;

    private float delay=0.05f;
    private int currentPopulation;
    private int currentSupply;
    private int currentMotivation;
    private float currentRebellion;
    private EventCard currentEvent;
    private Coroutine cardCoroutine;
    private Coroutine resultCoroutine;

    private void Awake()
    {
        Instance = this;
        UIManagerEvent?.Invoke();
    }

    void Start()
    {
    
    }

    public void Update(){
        if (Input.GetKeyDown(KeyCode.W))
        {
           delay=0.000005f;
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
           delay=0.05f;
        }
    }


    [SerializeField] private GameObject left_button;
    [SerializeField] private GameObject right_button;

    [SerializeField] private TextMeshProUGUI population_label;
    [SerializeField] private TextMeshProUGUI population_preview_label;
    [SerializeField] private TextMeshProUGUI supply_label;
    [SerializeField] private TextMeshProUGUI supply_preview_label;
    [SerializeField] private TextMeshProUGUI motivation_label;
    [SerializeField] private TextMeshProUGUI motivation_preview_label;
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
        GameManager.OnCardChanged += SetCardFromEvent;
        GameManager.OnGameReset += UIReset;
        ResourceManager.OnGameResult += HandleGameResult;
    }

    public void OnDisable()
    {
        ResourceManager.OnResourceChanged -= UpdateUIStats;
        GameManager.OnCardChanged -= SetCardFromEvent;
        GameManager.OnGameReset -= UIReset;
        ResourceManager.OnGameResult -= HandleGameResult;
    }


    public void UpdateUIStats(int population,int supply,int motivation,float rebellion)
    {
        currentPopulation = population;
        currentSupply = supply;
        currentMotivation = motivation;
        currentRebellion = rebellion;
        population_label.text = "Nüfus: " + population.ToString();
        motivation_label.text = "Moral: " + motivation.ToString();
        supply_label.text = "Erzak: " + supply.ToString();
        rebellion_bar.value = rebellion;
        if (population_preview_label != null) population_preview_label.text = "";
        if (supply_preview_label != null) supply_preview_label.text = "";
        if (motivation_preview_label != null) motivation_preview_label.text = "";
    }

    private void SetCardFromEvent(EventCard card)
    {
        currentEvent = card;
        if (cardCoroutine != null)
        {
            StopCoroutine(cardCoroutine);
        }
        if (resultCoroutine != null)
        {
            StopCoroutine(resultCoroutine);
            resultCoroutine = null;
        }
        HidePreviewStats();
        disableButtons();
        cardCoroutine = StartCoroutine(SetCard(card));
    }

    private void HandleGameResult(ResourceManager.GameResult result)
    {
        if (result == ResourceManager.GameResult.Won)
        {
            if (cardCoroutine != null)
            {
                StopCoroutine(cardCoroutine);
                cardCoroutine = null;
            }
            if (resultCoroutine != null)
            {
                StopCoroutine(resultCoroutine);
            }
            resultCoroutine = StartCoroutine(WinMessage());
            SetResButton(true);
        }
        else if (result == ResourceManager.GameResult.Lost)
        {
            if (cardCoroutine != null)
            {
                StopCoroutine(cardCoroutine);
                cardCoroutine = null;
            }
            if (resultCoroutine != null)
            {
                StopCoroutine(resultCoroutine);
            }
            resultCoroutine = StartCoroutine(LoseMessage());
            SetResButton(true);
        }
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

    public IEnumerator SetCard(EventCard currentEvent)
    {
        
        if (sadFolk.transform.localPosition.x != 10f)
    {
        yield return StartCoroutine(sadFolkWalk(10f));
    }
    if (angryFolk.transform.localPosition.x != 10f)
    {
        yield return StartCoroutine(angryFolkWalk(10f));
    }

        ResetCharacterPositions();
        chatBubble.text = "";
        disableButtons();
        HidePreviewStats();
        
        tenet.StopTalking();

        

        if (currentEvent.messenger == EventCard.messageSource.tenet)
        {
            chatBubbleBG.rectTransform.localScale = new Vector3(1, 1, 1); // Flip the chat bubble background for tenet
            tenet.StartTalking();
        }
        else if (currentEvent.messenger == EventCard.messageSource.folk)
        {
            chatBubbleBG.rectTransform.localScale = new Vector3(-1, 1, 1);
            if (currentEvent.emotion == EventCard.messageEmotion.angry)
            {
                yield return StartCoroutine(angryFolkWalk(7f));
                angryFolk.StartTalking();
            }
            else if (currentEvent.emotion == EventCard.messageEmotion.sad)
            {
                
                yield return StartCoroutine(sadFolkWalk(7f));
                sadFolk.StartTalking();
            }
        }
       
        foreach (char c in currentEvent.story)
        {
            chatBubble.text += c;
            yield return new WaitForSeconds(delay);
        }

        if(currentEvent.messenger == EventCard.messageSource.tenet)
        {
            tenet.StopTalking();
        }
        else if (currentEvent.messenger == EventCard.messageSource.folk)
        {
            if (currentEvent.emotion == EventCard.messageEmotion.angry)
            {
                angryFolk.StopTalking();
            }
            else
            {
                sadFolk.StopTalking();
            }
        }
        lButtonText.text = currentEvent.lButtonText;
        rButtonText.text = currentEvent.rButtonText;
        activateButtons();
        cardCoroutine = null;
    }

    public bool isObjectHere(GameObject obj, Vector3 targetPosition, float threshold)
    {
        float distance = Vector3.Distance(obj.transform.localPosition, targetPosition);
        return distance <= threshold;
    }

    public void ResetCharacterPositions()
    {
        angryFolk.transform.localPosition = new Vector3(10f, angryFolk.transform.localPosition.y, angryFolk.transform.localPosition.z);
        sadFolk.transform.localPosition = new Vector3(10f, sadFolk.transform.localPosition.y, sadFolk.transform.localPosition.z);
    }




    float walkDelay=0.1f;
    

     public IEnumerator angryFolkWalk(float targetX)
        {
    angryFolk.StartWalking();
    
    // Karakterin yönünü çevir
    if(targetX < angryFolk.transform.localPosition.x)
    {
        angryFolk.transform.localScale = new Vector3(0.263f, angryFolk.transform.localScale.y, 0.263f); 
    }
    else
    {
        angryFolk.transform.localScale = new Vector3(-0.263f, angryFolk.transform.localScale.y, 0.263f); 
    }

    
    
    // Karakterin mevcut X pozisyonu ile hedef X arasındaki fark 0.01'den büyük olduğu sürece yürü
    while (Mathf.Abs(targetX - angryFolk.transform.localPosition.x) > 0.01f)
    {
        // MoveTowards: Mevcut konumdan hedefe doğru belirlediğin hızda (0.2f) pürüzsüzce ilerler
        // Hedefi asla ıskalamaz, tam hedefe geldiğinde durur.

        Debug.Log("Şu anki X: " + angryFolk.transform.localPosition.x + " | Hedef: " + targetX);

        float yeniX = Mathf.MoveTowards(angryFolk.transform.localPosition.x, targetX, 0.2f);
        
        angryFolk.transform.localPosition = new Vector3(yeniX, angryFolk.transform.localPosition.y, angryFolk.transform.localPosition.z);
        
        yield return new WaitForSeconds(walkDelay);
    }
    
    angryFolk.StopWalking();
}

    public IEnumerator sadFolkWalk(float targetX)
{
    sadFolk.StartWalking();
    
    // Karakterin yönünü çevir
    if(targetX < sadFolk.transform.localPosition.x)
    {
        sadFolk.transform.localScale = new Vector3(0.263f, sadFolk.transform.localScale.y, 0.263f); 
    }
    else
    {
        sadFolk.transform.localScale = new Vector3(-0.263f, sadFolk.transform.localScale.y, 0.263f); 
    }
    
    // Karakterin mevcut X pozisyonu ile hedef X arasındaki fark 0.01'den büyük olduğu sürece yürü
    while (Mathf.Abs(targetX - sadFolk.transform.localPosition.x) > 0.01f)
    {
        // MoveTowards: Mevcut konumdan hedefe doğru belirlediğin hızda (0.2f) pürüzsüzce ilerler
        // Hedefi asla ıskalamaz, tam hedefe geldiğinde durur.

        Debug.Log("Şu anki X: " + sadFolk.transform.localPosition.x + " | Hedef: " + targetX);
        float yeniX = Mathf.MoveTowards(sadFolk.transform.localPosition.x, targetX, 0.2f);
        
        sadFolk.transform.localPosition = new Vector3(yeniX, sadFolk.transform.localPosition.y, sadFolk.transform.localPosition.z);
        
        yield return new WaitForSeconds(walkDelay);
    }
    
    sadFolk.StopWalking();
}

    

    public IEnumerator WinMessage()
    {
        chatBubbleBG.rectTransform.localScale = new Vector3(1, 1, 1);
        string winMessage = "EFENDİM ARTIK KOCAMAN BİR ORDUMUZ VAR!!!!";
        chatBubble.text = "";
        disableButtons();
        foreach(char C in winMessage)
        {
            chatBubble.text += C;
            yield return new WaitForSeconds(0.05f);
        }
        resultCoroutine = null;
    }


    public IEnumerator LoseMessage()
    {
        chatBubbleBG.rectTransform.localScale = new Vector3(1, 1, 1);
        string winMessage = "efendim üzgünüm ama bütün birlikler toplandı ve üstümüze doğru geliyorlar...";
        chatBubble.text = "";
        disableButtons();
        foreach (char C in winMessage)
        {
            chatBubble.text += C;
            yield return new WaitForSeconds(0.05f);
        }
        resultCoroutine = null;
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
        if (cardCoroutine != null)
        {
            StopCoroutine(cardCoroutine);
            cardCoroutine = null;
        }
        if (resultCoroutine != null)
        {
            StopCoroutine(resultCoroutine);
            resultCoroutine = null;
        }
        chatBubble.text = "";
        disableButtons();
        HidePreviewStats();
        ResetCharacterPositions();
        tenet.StopTalking();
        angryFolk.StopTalking();
        sadFolk.StopTalking();
        SetResButton(false);
    }


    public void OnHoverL()
    {
        if (currentEvent == null) return;
        ShowPreviewStats(currentEvent.l_population, currentEvent.l_supply, currentEvent.l_motivation, currentEvent.l_rebellion);
    }

    public void OnHoverR()
    {
        if (currentEvent == null) return;
        ShowPreviewStats(currentEvent.r_population, currentEvent.r_supply, currentEvent.r_motivation, currentEvent.r_rebellion);
    }




    public void ShowPreviewStats(int prePopulation,int preSupply,int preMotivation,int preRebellion)
    {
        population_label.text = "Nüfus: " + currentPopulation.ToString();
        supply_label.text = "Erzak: " + currentSupply.ToString();
        motivation_label.text = "Moral: " + currentMotivation.ToString();

        SetPreviewStat(population_preview_label, currentPopulation, prePopulation);
        SetPreviewStat(supply_preview_label, currentSupply, preSupply);
        SetPreviewStat(motivation_preview_label, currentMotivation, preMotivation);

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
    private void SetPreviewStat(TextMeshProUGUI previewLabel, int currentValue, int change)
    {
        if (previewLabel == null)
        {
            return;
        }

        previewLabel.text = change > 0
            ? "<color=green>+" + change.ToString() + "</color>"
            : change < 0
                ? "<color=red>" + change.ToString() + "</color>"
                : "";
    }

    private void ClearPreviewStats()
    {
        if (population_preview_label != null) population_preview_label.text = "";
        if (supply_preview_label != null) supply_preview_label.text = "";
        if (motivation_preview_label != null) motivation_preview_label.text = "";
    }

    public void HidePreviewStats()
    {
        population_label.text = "Nüfus: " + currentPopulation.ToString();
        motivation_label.text = "Moral: " + currentMotivation.ToString();
        supply_label.text = "Erzak: " + currentSupply.ToString();
        ClearPreviewStats();
        //Rebellion part    
        rebellion_bar.value = currentRebellion;
        preview_rebellion_bar.value = currentRebellion;
        preview_fill_image.color = new Color(1f, 1f, 1f, 0f); // Alpha'yı sıfırla
    }

}
