using UnityEngine;

public class CharacterAnimController : MonoBehaviour
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Animator animator;
    

    void Awake() // Start yerine Awake yapıyoruz
    {
        animator = GetComponent<Animator>();
    }
    // Oyun içindeki kart veya diyalog olayları tetiklendiğinde bu metodu çağır
    public void StartTalking()
    {
        animator.SetBool("isTalking", true);
    }

    // Liderin cümlesi/etkileşimi bittiğinde bu metodu çağır
    public void StopTalking()
    {
        animator.SetBool("isTalking", false);
    }

    public void StartWalking()
    {
        animator.SetBool("isWalking", true);
    }

    public void StopWalking()
    {
        animator.SetBool("isWalking", false);
    }
}
