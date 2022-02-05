using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketController : MonoBehaviour
{
    public static MarketController Current; //dýþardan eriþilebilmesi için statik bir deðiþkeni oluþturacaðým.

    public List<MarketItem> items; //satýn alýnmýþ-alýnmamýþ tüm eþyalarý listeleyecek.
    public List<Item> equippedItems; //giyilmiþ eþyalarý tutacak
    public GameObject marketMenu; //market munusune eriþmek

    
    public void InitializeMarketController()//tanýmlama fonksiyonunda currenti tanýmlayacaðým
    {
        Current=this;
        foreach (MarketItem item in items)//eþyalarýn içinde dolan hepsini teker teker tanýmla.
        {
            item.InitializeItem();
        }
    }
    public void ActivateMarketMenu(bool active)//oyun menulerinde market butonuna týkladýðýmýzda marketin açýlmasý ve market menusundeykende geri tuþuna bastýðýmýzda market menusunun kapanmasý için bir fonksiyon yazýcaz.
    {
        marketMenu.SetActive(active);
    } 
}
