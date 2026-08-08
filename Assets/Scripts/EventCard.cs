using UnityEngine;

[CreateAssetMenu(fileName = "EventCard", menuName = "Scriptable Objects/EventCard")]
public class EventCard : ScriptableObject
{
    [TextArea(3,10)]
    public string story;
    [TextArea(2,5)]
    public string lButtonText;
    [TextArea(2,5)]
    public string rButtonText;

    public int l_population;
    public int l_supply;
    public int l_motivation;
    public int l_rebellion;

    public int r_population;
    public int r_supply;
    public int r_motivation; 
    public int r_rebellion;



}   
