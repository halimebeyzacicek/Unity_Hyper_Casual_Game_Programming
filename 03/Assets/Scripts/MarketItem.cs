using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketItem : MonoBehaviour
{
    // Satýn alma, giyme, çýkarmaburada olacak.
    public int itemId, wearId;//itemId=eþyanýn kendisine ait id'si, bu id yi kullanarak unityin hafýza biriminde eþyalarýmýzýn son durumlarýný tutucaz.//wearId=eþyanýn hangi kategoriye ait olduðunu tutacak.

    public bool HasItem()//kullanýcýný bu eþyayý daha önceden satýn alýp almadýðýný kontrol eden foksiyon.
    {
        //0=daha satýn alýnmamýþ.
        //satýn alýnmýþ ama giyilmemiþ
        //2=satýn alýnmýþ giyilmiþ.
        bool hasItem = PlayerPrefs.GetInt("item" + itemId.ToString())!=0;//0 a eþitse false yap.
        return hasItem;
    }






}
