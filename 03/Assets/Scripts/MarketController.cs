using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketController : MonoBehaviour
{
    public static MarketController Current; //d��ardan eri�ilebilmesi i�in statik bir de�i�keni olu�turaca��m.

    public List<MarketItem> items; //sat�n al�nm��-al�nmam�� t�m e�yalar� listeleyecek.
    public List<Item> equippedItems; //giyilmi� e�yalar� tutacak
    public GameObject marketMenu; //market munusune eri�mek

    
    public void InitializeMarketController()//tan�mlama fonksiyonunda currenti tan�mlayaca��m
    {
        Current=this;
        foreach (MarketItem item in items)//e�yalar�n i�inde dolan hepsini teker teker tan�mla.
        {
            item.InitializeItem();
        }
    }
    public void ActivateMarketMenu(bool active)//oyun menulerinde market butonuna t�klad���m�zda marketin a��lmas� ve market menusundeykende geri tu�una bast���m�zda market menusunun kapanmas� i�in bir fonksiyon yaz�caz.
    {
        marketMenu.SetActive(active);
    } 
}
