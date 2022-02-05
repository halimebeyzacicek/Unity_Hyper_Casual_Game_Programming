using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketItem : MonoBehaviour
{
    // Satýn alma, giyme, çýkarmaburada olacak.
    public int itemId, wearId;//itemId=eþyanýn kendisine ait id'si, bu id yi kullanarak unityin hafýza biriminde eþyalarýmýzýn son durumlarýný tutucaz.//wearId=eþyanýn hangi kategoriye ait olduðunu tutacak.
    public int price;//fiyat deðiþkeni

    public Button butButton, equipButton, unequipButton;
    public Text priceText; //fiyat yazýsý

    public GameObject itemPrefab;//giyilecek olan aksesuarý sahnemde 3 boyutlu obje olarak yaratmam gerekiyor. objeyi yaratabilmek için önce o objeye ulaþabilmem gerekiyor.o objenin prefabýna ulaþmak için bir deðiþken tanýmlýyorum


    public bool HasItem()//kullanýcýný bu eþyayý daha önceden satýn alýp almadýðýný kontrol eden foksiyon.
    {
        //0=daha satýn alýnmamýþ.
        //1=satýn alýnmýþ ama giyilmemiþ
        //2=satýn alýnmýþ giyilmiþ.
        bool hasItem = PlayerPrefs.GetInt("item" + itemId.ToString())!=0;//0 a eþitse false yap.
        return hasItem;
    }
    
    public bool IsEquipped()//Bu eþyanýn giyilip giyilmediðini bize söyleyen bir fonksiyon yazýcaz.
    {
        bool equippedItem = PlayerPrefs.GetInt("item" + itemId.ToString()) == 2;//2=satýn alýnmýþ giyilmiþ.
        return equippedItem;
    } 

    public void InitializeItem()//eþyayý tanýmla
    {
        //önce eþyanýn fiyat bilgisine ihtiyacýmýz var.
        priceText.text = price.ToString(); //fiyat bilgisini güncellemeliyiz.
        if (HasItem())//oyuncu bu eþyayý satýn almýþ mý
        {
            butButton.gameObject.SetActive(false); //eþya satýn alýndýysa satýn alma butonunu deactif etmek olacak.
            if (IsEquipped())//bu eþyanýn giyilip giyilmediði
            {
                EquipItem();
            }
            else//eþya giyipmedi ise giymem butonunu aktif et.
            {
                equipButton.gameObject.SetActive(true);
            }
        }
        else//satýn alma butonunu aktif etmeliyiz.
        {
            butButton.gameObject.SetActive(true);
        } 
    }
    
    public void ButItem()//satýn alma butonu
    {
        if (!HasItem()) //kullanýcýda bu eþya var mý? yok ise bunlarý yap
        {
            //kullanýcýnýn parasý bu eþyanýn fiyatýndan büyük ve ya eþit mi
            int money = PlayerPrefs.GetInt("money"); //paraya eriþiyoruz.
            if (money >=price)
            {
                PlayerController.Current.itemAudioSource.PlayOneShot(PlayerController.Current.buyAudioClip,0.1f);
                LevelController.Current.GiveMoneyToPlayer(-price); //ilk önce kullanýcýdan parayý düþeceðiz
                PlayerPrefs.SetInt("item" + itemId.ToString(), 1); //sonra bu eþyanýn hafýza birimindeki deðerini satýn alýndý diye iþaretlemeliyiz. //1=satýn alýnmýþ ama giyilmemiþ
                butButton.gameObject.SetActive(false);
                equipButton.gameObject.SetActive(true);
            }
        }
    }
    public void EquipItem()//eþyayý giy
    {
        UnequipItem(); //baþka bir eþya giyili ise onu çýkartacaðýz.
        //giyilecek olan aksesuarý sahnemde 3 boyutlu obje olarak yaratmam gerekiyor. objeyi yaratabilmek için önce o objeye ulaþabilmem gerekiyor.o objenin prefabýna ulaþmak için bir deðiþken tanýmlýyorum
        //bu eþyayý karakterimizin doðru pozisyonunda yaratmam gerekiyor. bunun için iskelet sistemindeki doðru objenin çocuðu yapmam yeterli.
        MarketController.Current.equippedItems[wearId]=Instantiate(itemPrefab, PlayerController.Current.wearSpots[wearId].transform).GetComponent<Item>(); //önece bu objeyi yaratmalýyýz. units de prefabdan bir obje yaratmam istediðimiz zaman instantiate fonksiyonun u kullanýyoruz. market controlerda giyilen objelere bunu atamamýz gerekiyor.
        MarketController.Current.equippedItems[wearId].itemId = itemId;//yaratýlan yani giyilmiþ olan objenin idisini bu market itemin idisine eþitlemem gerekiyor.
        equipButton.gameObject.SetActive(false);
        unequipButton.gameObject.SetActive(true);
        PlayerPrefs.SetInt("item" + itemId.ToString(), 2); //hafýzada bu eþyanýn giyili olduðunu kaydetmeliyim.
    
    }
    public void UnequipItem()//eþyayý çýkar
    {
        Item equippedItem = MarketController.Current.equippedItems[wearId]; //eþya giyili mi diye kontrol etmeiyiz.
        if (equippedItem!= null)//eðer bu eleman nulla eþit deðilse --bu giyili eþyanýn id sine karþýlýk gelen market item objesini bulmam gerekiyor.
        {
            MarketItem marketItem = MarketController.Current.items[equippedItem.itemId];//giyili olan eþyanýn hangi marketitem objesinden yaratýldýðýný bulmuþ oluyorum
            PlayerPrefs.SetInt("item" + marketItem.itemId, 1); //hafýz biriminde giyilmemiþ olarak göstermeliyiyi. þuan çýkartýyoruz.
            marketItem.equipButton.gameObject.SetActive(true);
            marketItem.unequipButton.gameObject.SetActive(false);
            Destroy(equippedItem.gameObject); //giyili eþyayý sahneden yok etmeliyiz.
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
