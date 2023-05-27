 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Coin : MonoBehaviour
{
    public static Coin instance;
     
    public TMP_Text coinText;
    public int currentCoins = 0;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        coinText.text = "Coins: " + currentCoins.ToString();
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    public void IncreaseCoins(int i)
    {
        currentCoins += i;
        coinText.text = "Coins: " + currentCoins.ToString();
    }
    public void DecreaseCoins(int i)
    {
        currentCoins -= i;
        coinText.text = "Coins: " + currentCoins.ToString();
    }
}
