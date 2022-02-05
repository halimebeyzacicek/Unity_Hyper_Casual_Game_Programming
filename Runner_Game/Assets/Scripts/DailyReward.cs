using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyReward : MonoBehaviour
{
    public bool initialized;//Bu s�n�f�n tan�n�p tan�nmad���n� tutan de�i�ken //initialized=tan�mlanm�� //otomatik false tan�mlanaca�� i�in zaten s�n�f tan�mlanmam�� say�lacak.
    public long rewardGivingTimeTicks; //�d�l� alma zaman tiki.//bir sonraki �d�l� ne zaman alaca��m�z� tutan tick biriminde long tipinde bir de�i�ken olacak.
    public GameObject rewardMenu;//�d�l menusunu tutacak de�i�ken.
    public Text remainingTimeText; //kalan zaman yaz�s�


    public void InitializeDailyReward()//g�nl�k �d�l� tan�mla.
    {
        // PlayerPrefs.SetString("lastDailyReward", (System.DateTime.Now.Ticks-864000000000 +10 * 10000000).ToString());//bir g�n geri git 10 saniye iler git.//deneme
        //kullan�c�n� �nceki �d�l� ne zaman ald���n� unity haf�za biriminde tutmal�y�z. haf�za biriminde �d�l� ald��� son tarihi tik biriminde tutaca��z.
        //PlayerPrefs s�n�f�n�n setint fonksiyonu ile bunu yapamayaca��z ��nk� tick birimi long de�i�kenlerde tutuluyor interger a s��m�yor. bu y�zden bu tikleri string de saklayaca��z.
        //g�nl�k �d�l�m�z� ilk tan�mlarken �unu kontrol etmeliyiz, oyuncu daha �nce hi� g�nl�k �d�l alm�� m�? haf�za biriminde buna dair bir kay�t var m�?
        if (PlayerPrefs.HasKey("lastDailyReward")) // HasKey=daha �nce b�yle bir giri� yap�lm�� m�?
        {
            rewardGivingTimeTicks = long.Parse(PlayerPrefs.GetString("lastDailyReward")) + 864000000000; //bir sonraki �d�l tarihini tik cinsinden bulmam�z gerekiyor.yani �st�ne bir g�n eklicez.
            long currentTime = System.DateTime.Now.Ticks;//�imdi �uanki zamana ihtiyac�m var.
            if (currentTime >= rewardGivingTimeTicks)
            {
                GiveReward();//�d�l� ver.
            }

        }
        else//direkt kullan�c�ya �d�l� verece�iz.
        {

        }


        initialized = true;
    }

    public void GiveReward()//�d�l� ver.
    {
        LevelController.Current.GiveMoneyToPlayer(100); //kullan�c�ya paras�n� ver.
        rewardMenu.SetActive(true); //g�nl�k �d�l menusunu a�
        PlayerPrefs.SetString("lastDailyReward",System.DateTime.Now.Ticks.ToString()); //son �d�l al�m tarihini g�ncelle. //�uanki zaman� son �d�l al�m tarihi yap.
        rewardGivingTimeTicks = long.Parse(PlayerPrefs.GetString("lastDailyReward")) + 864000000000; //bir sonraki �d�l al�m tarihinide g�ncellemeliyiz.
    }

    void Update()
    {
        //geri say�m i�lemini ger�ekle�tirmemiz gerekiyor.
        if (initialized)//e�er g�nl�k �d�l sistemi tan�mlanm��sa 
        {
            if (LevelController.Current.startMenu.activeInHierarchy)//oyuncu level ba�lang�� menusunde ise //startMenu.activeInHierarchy=start menusu hiyerar��de aktifse.
            {
                long currentTime = System.DateTime.Now.Ticks;
                long remainingTime = rewardGivingTimeTicks - currentTime; //kalan zaman=bir sonraki kalan �d�l tarihi-�imdiki zman.
                if (remainingTime <= 0)//e�er kalan zaman s�f�ra e�it veya s�f�rdan k���kse o zman �d�l al�m zamn� gelmi� demektir.
                {
                    GiveReward();
                }
                else//ekrandaki kalan zaman yaz�s�n� g�ncellemeliyiz.
                {
                    System.TimeSpan timespan = System.TimeSpan.FromTicks(remainingTime);
                    //bu de�i�ken �zerinde kalan zman�m� ka� aya ka� g�ne ka� saate ka� saniyeye denk geldi�ini bulabilirim.
                    remainingTimeText.text = string.Format("{0}:{1}:{2}",timespan.Hours.ToString("D2"), timespan.Minutes.ToString("D2"), timespan.Seconds.ToString("D2"));//format fonksiyonu bizim belli de�i�kenleri belli formatlarda yazabilmemizi sa�l�yor.
                    //d2 �u i�e yar�yor=burda verdi�im de�er 2 basamaktan k���k bile olsa onu 05,06 �eklinde d�nd�recek.                
                }
            }
        }
    }
    public void TapToReturnButtom()//yan�ld���nda geri d�nme tu�u anlam�nda.//a��lan �d�l menusunu kapatmak i�i
    {
        rewardMenu.SetActive(false);
    }
}
