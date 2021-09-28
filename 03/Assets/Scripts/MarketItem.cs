using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketItem : MonoBehaviour
{
    // Sat�n alma, giyme, ��karmaburada olacak.
    public int itemId, wearId;//itemId=e�yan�n kendisine ait id'si, bu id yi kullanarak unityin haf�za biriminde e�yalar�m�z�n son durumlar�n� tutucaz.//wearId=e�yan�n hangi kategoriye ait oldu�unu tutacak.
    public int price;//fiyat de�i�keni

    public Button butButton, equipButton, unequipButton;
    public Text priceText; //fiyat yaz�s�

    public GameObject itemPrefab;//giyilecek olan aksesuar� sahnemde 3 boyutlu obje olarak yaratmam gerekiyor. objeyi yaratabilmek i�in �nce o objeye ula�abilmem gerekiyor.o objenin prefab�na ula�mak i�in bir de�i�ken tan�ml�yorum


    public bool HasItem()//kullan�c�n� bu e�yay� daha �nceden sat�n al�p almad���n� kontrol eden foksiyon.
    {
        //0=daha sat�n al�nmam��.
        //1=sat�n al�nm�� ama giyilmemi�
        //2=sat�n al�nm�� giyilmi�.
        bool hasItem = PlayerPrefs.GetInt("item" + itemId.ToString())!=0;//0 a e�itse false yap.
        return hasItem;
    }
    
    public bool IsEquipped()//Bu e�yan�n giyilip giyilmedi�ini bize s�yleyen bir fonksiyon yaz�caz.
    {
        bool equippedItem = PlayerPrefs.GetInt("item" + itemId.ToString()) == 2;//2=sat�n al�nm�� giyilmi�.
        return equippedItem;
    } 

    public void InitializeItem()//e�yay� tan�mla
    {
        //�nce e�yan�n fiyat bilgisine ihtiyac�m�z var.
        priceText.text = price.ToString(); //fiyat bilgisini g�ncellemeliyiz.
        if (HasItem())//oyuncu bu e�yay� sat�n alm�� m�
        {
            butButton.gameObject.SetActive(false); //e�ya sat�n al�nd�ysa sat�n alma butonunu deactif etmek olacak.
            if (IsEquipped())//bu e�yan�n giyilip giyilmedi�i
            {
                EquipItem();
            }
            else//e�ya giyipmedi ise giymem butonunu aktif et.
            {
                equipButton.gameObject.SetActive(true);
            }
        }
        else//sat�n alma butonunu aktif etmeliyiz.
        {
            butButton.gameObject.SetActive(true);
        } 
    }
    
    public void ButItem()//sat�n alma butonu
    {
        if (!HasItem()) //kullan�c�da bu e�ya var m�? yok ise bunlar� yap
        {
            //kullan�c�n�n paras� bu e�yan�n fiyat�ndan b�y�k ve ya e�it mi
            int money = PlayerPrefs.GetInt("money"); //paraya eri�iyoruz.
            if (money >=price)
            {
                PlayerController.Current.itemAudioSource.PlayOneShot(PlayerController.Current.buyAudioClip,0.1f);
                LevelController.Current.GiveMoneyToPlayer(-price); //ilk �nce kullan�c�dan paray� d��ece�iz
                PlayerPrefs.SetInt("item" + itemId.ToString(), 1); //sonra bu e�yan�n haf�za birimindeki de�erini sat�n al�nd� diye i�aretlemeliyiz. //1=sat�n al�nm�� ama giyilmemi�
                butButton.gameObject.SetActive(false);
                equipButton.gameObject.SetActive(true);
            }
        }
    }
    public void EquipItem()//e�yay� giy
    {
        UnequipItem(); //ba�ka bir e�ya giyili ise onu ��kartaca��z.
        //giyilecek olan aksesuar� sahnemde 3 boyutlu obje olarak yaratmam gerekiyor. objeyi yaratabilmek i�in �nce o objeye ula�abilmem gerekiyor.o objenin prefab�na ula�mak i�in bir de�i�ken tan�ml�yorum
        //bu e�yay� karakterimizin do�ru pozisyonunda yaratmam gerekiyor. bunun i�in iskelet sistemindeki do�ru objenin �ocu�u yapmam yeterli.
        MarketController.Current.equippedItems[wearId]=Instantiate(itemPrefab, PlayerController.Current.wearSpots[wearId].transform).GetComponent<Item>(); //�nece bu objeyi yaratmal�y�z. units de prefabdan bir obje yaratmam istedi�imiz zaman instantiate fonksiyonun u kullan�yoruz. market controlerda giyilen objelere bunu atamam�z gerekiyor.
        MarketController.Current.equippedItems[wearId].itemId = itemId;//yarat�lan yani giyilmi� olan objenin idisini bu market itemin idisine e�itlemem gerekiyor.
        equipButton.gameObject.SetActive(false);
        unequipButton.gameObject.SetActive(true);
        PlayerPrefs.SetInt("item" + itemId.ToString(), 2); //haf�zada bu e�yan�n giyili oldu�unu kaydetmeliyim.
    
    }
    public void UnequipItem()//e�yay� ��kar
    {
        Item equippedItem = MarketController.Current.equippedItems[wearId]; //e�ya giyili mi diye kontrol etmeiyiz.
        if (equippedItem!= null)//e�er bu eleman nulla e�it de�ilse --bu giyili e�yan�n id sine kar��l�k gelen market item objesini bulmam gerekiyor.
        {
            MarketItem marketItem = MarketController.Current.items[equippedItem.itemId];//giyili olan e�yan�n hangi marketitem objesinden yarat�ld���n� bulmu� oluyorum
            PlayerPrefs.SetInt("item" + marketItem.itemId, 1); //haf�z biriminde giyilmemi� olarak g�stermeliyiyi. �uan ��kart�yoruz.
            marketItem.equipButton.gameObject.SetActive(true);
            marketItem.unequipButton.gameObject.SetActive(false);
            Destroy(equippedItem.gameObject); //giyili e�yay� sahneden yok etmeliyiz.
        }
    }

    public void EquipItenButton()
    {
        PlayerController.Current.itemAudioSource.PlayOneShot(PlayerController.Current.equipItemAudioClip, 0.1f);
        EquipItem();
    }
    public void UnEquipItemButton()
    {
        PlayerController.Current.itemAudioSource.PlayOneShot(PlayerController.Current.unequipItemAudioClip, 0.1f);
        UnEquipItemButton();
    }






}
