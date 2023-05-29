using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoWidget : MonoBehaviour
{
   
    public TMPro.TMP_Text ammoText;
    public TMPro.TMP_Text totalAmmoText;
    public void Refresh(int ammoCount, int totalAmmoAmount)
    {
        ammoText.text = ammoCount.ToString();
        totalAmmoText.text = totalAmmoAmount.ToString();
    }
}
