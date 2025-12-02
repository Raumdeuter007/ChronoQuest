using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HealHUD : MonoBehaviour
{
    public Image[] healHearts;
    public Sprite fullSprite;
    public Sprite emptySprite;

    public int stacksNeeded = 5;
    public int healOnComplete = 50;

    public PlayerHealth king;
    private int currentStacks = 0;

    private void Start()
    {
        ResetHealHUD();
    }

    public void Heal()
    {
        if (healHearts == null || healHearts.Length == 0)
            return;

        if (currentStacks < healHearts.Length)
        {
            healHearts[currentStacks].sprite = fullSprite;
            currentStacks++;
        }
    }
    public void Heal(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (currentStacks >= stacksNeeded)
            {
                HealKing();
                ResetHealHUD();
            }
        }
    }
    private void HealKing()
    {
        int missing = king.maxHealth - king.currentHealth;

        if (missing > 0)
        {
            int healAmount = Mathf.Min(healOnComplete, missing);
            king.Heal(healAmount);
        }
    }

    public void ResetHealHUD()
    {
        currentStacks = 0;

        for (int i = 0; i < healHearts.Length; i++)
        {
            healHearts[i].sprite = emptySprite;
        }
    }
}