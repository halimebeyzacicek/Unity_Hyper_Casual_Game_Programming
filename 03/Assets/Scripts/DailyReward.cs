using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyReward : MonoBehaviour
{
    public bool initialized;//Bu sýnýfýn tanýnýp tanýnmadýðýný tutan deðiþken //initialized=tanýmlanmýþ //otomatik false tanýmlanacaðý için zaten sýnýf tanýmlanmamýþ sayýlacak.
    public long rewardGivingTimeTicks; //ödülü alma zaman tiki.//bir sonraki ödülü ne zaman alacaðýmýzý tutan tick biriminde long tipinde bir deðiþken olacak.
    public GameObject rewardMenu;//ödül menusunu tutacak deðiþken.
    public Text remainingTimeText; //kalan zaman yazýsý


    public void InitializeDailyReward()//günlük ödülü tanýmla.
    {
        // PlayerPrefs.SetString("lastDailyReward", (System.DateTime.Now.Ticks-864000000000 +10 * 10000000).ToString());//bir gün geri git 10 saniye iler git.//deneme
        //kullanýcýný önceki ödülü ne zaman aldýðýný unity hafýza biriminde tutmalýyýz. hafýza biriminde ödülü aldýðý son tarihi tik biriminde tutacaðýz.
        //PlayerPrefs sýnýfýnýn setint fonksiyonu ile bunu yapamayacaðýz çünkü tick birimi long deðiþkenlerde tutuluyor interger a sýðmýyor. bu yüzden bu tikleri string de saklayacaðýz.
        //günlük ödülümüzü ilk tanýmlarken þunu kontrol etmeliyiz, oyuncu daha önce hiç günlük ödül almýþ mý? hafýza biriminde buna dair bir kayýt var mý?
        if (PlayerPrefs.HasKey("lastDailyReward")) // HasKey=daha önce böyle bir giriþ yapýlmýþ mý?
        {
            rewardGivingTimeTicks = long.Parse(PlayerPrefs.GetString("lastDailyReward")) + 864000000000; //bir sonraki ödül tarihini tik cinsinden bulmamýz gerekiyor.yani üstüne bir gün eklicez.
            long currentTime = System.DateTime.Now.Ticks;//þimdi þuanki zamana ihtiyacým var.
            if (currentTime >= rewardGivingTimeTicks)
            {
                GiveReward();//ödülü ver.
            }

        }
        else//direkt kullanýcýya ödülü vereceðiz.
        {

        }


        initialized = true;
    }

    public void GiveReward()//ödülü ver.
    {
        LevelController.Current.GiveMoneyToPlayer(100); //kullanýcýya parasýný ver.
        rewardMenu.SetActive(true); //günlük ödül menusunu aç
        PlayerPrefs.SetString("lastDailyReward",System.DateTime.Now.Ticks.ToString()); //son ödül alým tarihini güncelle. //þuanki zamaný son ödül alým tarihi yap.
        rewardGivingTimeTicks = long.Parse(PlayerPrefs.GetString("lastDailyReward")) + 864000000000; //bir sonraki ödül alým tarihinide güncellemeliyiz.
    }

    void Update()
    {
        //geri sayým iþlemini gerçekleþtirmemiz gerekiyor.
        if (initialized)//eðer günlük ödül sistemi tanýmlanmýþsa 
        {
            if (LevelController.Current.startMenu.activeInHierarchy)//oyuncu level baþlangýç menusunde ise //startMenu.activeInHierarchy=start menusu hiyerarþýde aktifse.
            {
                long currentTime = System.DateTime.Now.Ticks;
                long remainingTime = rewardGivingTimeTicks - currentTime; //kalan zaman=bir sonraki kalan ödül tarihi-þimdiki zman.
                if (remainingTime <= 0)//eðer kalan zaman sýfýra eþit veya sýfýrdan küçükse o zman ödül alým zamný gelmiþ demektir.
                {
                    GiveReward();
                }
                else//ekrandaki kalan zaman yazýsýný güncellemeliyiz.
                {
                    System.TimeSpan timespan = System.TimeSpan.FromTicks(remainingTime);
                    //bu deðiþken üzerinde kalan zmanýmý kaç aya kaç güne kaç saate kaç saniyeye denk geldiðini bulabilirim.
                    remainingTimeText.text = string.Format("{0}:{1}:{2}",timespan.Hours.ToString("D2"), timespan.Minutes.ToString("D2"), timespan.Seconds.ToString("D2"));//format fonksiyonu bizim belli deðiþkenleri belli formatlarda yazabilmemizi saðlýyor.
                    //d2 þu iþe yarýyor=burda verdiðim deðer 2 basamaktan küçük bile olsa onu 05,06 þeklinde döndürecek.                
                }
            }
        }
    }
    public void TapToReturnButtom()//yanýldýðýnda geri dönme tuþu anlamýnda.//açýlan ödül menusunu kapatmak içi
    {
        rewardMenu.SetActive(false);
    }
}
