using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketItem : MonoBehaviour
{
    // Sat�n alma, giyme, ��karmaburada olacak.
    public int itemId, wearId;//itemId=e�yan�n kendisine ait id'si, bu id yi kullanarak unityin haf�za biriminde e�yalar�m�z�n son durumlar�n� tutucaz.//wearId=e�yan�n hangi kategoriye ait oldu�unu tutacak.

    public bool HasItem()//kullan�c�n� bu e�yay� daha �nceden sat�n al�p almad���n� kontrol eden foksiyon.
    {
        //0=daha sat�n al�nmam��.
        //sat�n al�nm�� ama giyilmemi�
        //2=sat�n al�nm�� giyilmi�.
        bool hasItem = PlayerPrefs.GetInt("item" + itemId.ToString())!=0;//0 a e�itse false yap.
        return hasItem;
    }






}
