using UnityEngine;
using UnityEngine.UI;

public class healthHUDscript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public PlayerHealth kingHealth;

    public Image[] heart;
    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int healthPerHeart = kingHealth.maxHealth / heart.Length;
        int halfhhp = healthPerHeart / 2;
        int tempHealth = kingHealth.currentHealth;

        for (int i = 0; i < heart.Length; i++)
        {
            if (tempHealth > healthPerHeart - halfhhp || tempHealth == healthPerHeart)
            {
                heart[i].sprite = fullHeart;
                tempHealth -= healthPerHeart;
            }
            else if (tempHealth <= halfhhp && tempHealth > 0)
            {
                heart[i].sprite = halfHeart;
                tempHealth -= halfhhp;
            }
            else
            {
                heart[i].sprite = emptyHeart;
            }
        }
    }

}
