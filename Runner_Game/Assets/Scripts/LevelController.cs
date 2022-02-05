using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    public static LevelController Current; //di�er s�n�flar�n bu objeye eri�mesi i�in bu s�n�f i�in static bir de�i�ken olu�turaca��m.
    public bool gameActive = false; //oyun aktif mi de�il mi

    public GameObject startMenu, gameMenu, gameOverMenu, finishMenu; //men�lere eri�mek
    public Text scoreText, finishScoreText, currentLevelText, nextLevelText,startingMenuMoneyText, gameOverMenuMoneyText, finishGameMenuMoneyText; //yaz� objelerini tutmak i�in
    public Slider levelProgressBar;
    public float maxDistance; //oyun ilk ba�lad��� anda karakterimizin biti� �izgisine olan uzakl���n� tutmam�z gerekiyor.
    public GameObject finishLine; //biti� �izgimizi tutacak.

    public AudioSource gameMusicAudioSource;
    public AudioClip victoryAudioClip, gameOverAudioClip;

    public DailyReward dailyReward; //levelcontroler s�n�f�m�n i�inde g�nl�k �d�l sistemimi tan�mlamam gerekiyordu.

    int currentLevel;
    int score; //skorlar� tutan de�i�ken


    void Start()
    {
        Current = this;
        currentLevel = PlayerPrefs.GetInt("currentLevel"); //oyuncu hangi levelde kalm�� de�i�keni //bu oyunun haf�zas�nda bana int bit de�i�ken �ek diyorum
       
        //�uanki level in ismi level+current de�ilse o leveli y�kle dicem ama o levelse o level ile ilgili ayarlamalar yap�lacak.
        if (SceneManager.GetActiveScene().name != "Level " + currentLevel)//a��k olan level e eri�iyoruz //e�it de�ilse do�ru sahneye y�klemesini yaapcakt�k
        {
            Debug.Log("girdi"+ currentLevel);
            SceneManager.LoadScene("Level " + currentLevel); //level y�kle
        }
        else //do�ru levelde isek //levelcontrollerin aray�zlere ei�mesi gerekiyor.
        {
            PlayerController.Current = GameObject.FindObjectOfType<PlayerController>();//burada player control u yazd�k. ��nk� maketten player a eri�mek gerekebilir ve marketten �nce player kontrol bulunmu� olmal�.  
            GameObject.FindObjectOfType<MarketController>().InitializeMarketController();

            dailyReward.InitializeDailyReward(); //do�ru levelde isek o b�l�mdede �d�l sistemimizi tan�ml�yorum
            Debug.Log("girdi2"+ currentLevel);
            currentLevelText.text = (currentLevel + 1).ToString();   //s�f�nr�nc� b�l�mde isek 1----2 yazmal�
            nextLevelText.text = (currentLevel + 2).ToString();
            UpdateMoneyTexts();
            //GiveMoneyToPlayer(3000);
        }
        gameMusicAudioSource = Camera.main.GetComponent<AudioSource>();//kameradan sesi �ekecek. ba�lang�� sesini(oyun sesini)
    }

    
    void Update()
    {
        //e�er oyun aktif ise slider i s�rekli g�ncellemeliyim.
        if (gameActive)
        {
            PlayerController player = PlayerController.Current; //�ncelikle playercontrollerimi tutmas� i�in bir de�i�ken olu�turuyorum.
            float distance= finishLine.transform.position.z - PlayerController.Current.transform.position.z;  //karakterin �uan �izgiye ne kadar uzak oldu�unu buluyorum.
            levelProgressBar.value = 1 - (distance / maxDistance); //o silider imin ne kadar dolup dolmad���n� da bu de�erde tutucaz. //slider 1-0 aras� olacak.
        }
    }

    public void StartLevel()
    {
        maxDistance = finishLine.transform.position.z - PlayerController.Current.transform.position.z;

        PlayerController.Current.ChangeSpeed(PlayerController.Current.runningSpeed);

        startMenu.SetActive(false);
        gameMenu.SetActive(true);
        PlayerController.Current.animator.SetBool("running",true);

        gameActive = true;
    }

    public void RestartLevel() //�l�nce leveli tekrar y�kleyecek.
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //level y�kle ve verdi�im parametre �uanki levelin ismi oluyor.
    }

    public void LoadNextLevel() //bir sonraki leveli y�klemek i�in. 
    {
        SceneManager.LoadScene( "Level " + (currentLevel+1));
    } 

    public void GameOver()
    {
        UpdateMoneyTexts();

        gameMusicAudioSource.Stop();
        gameMusicAudioSource.PlayOneShot(gameOverAudioClip);//burada ikinci bir parametre vermiyorum ��nk� otomatik olarak m�zik kayna��m�n kendi ses y�ksekli�ini alacak.
        gameMenu.SetActive(false); //yan�nca ilk ba�ta oyun menusunu kapatmam�z gerekiyor
        gameOverMenu.SetActive(true); //game over menumuzu true yapmam�z gerekiyor.
        gameActive = false; //son olarak oyunda yand���m�z i�in o y�zden game activi de false yapmam�z laz�m.
    }
    public void FinishGame()//oyunu bitirdi�imiz zaman yazacak fonksiyon.
    {

        GiveMoneyToPlayer(score);
        gameMusicAudioSource.Stop();
        gameMusicAudioSource.PlayOneShot(victoryAudioClip);
        PlayerPrefs.SetInt("currentLevel",currentLevel+1); //�ncelikle unityin haf�za biriminde hangi levelde kald���m�z� g�ncellememiz gerekiyor.
        //parantez i�i= �ncelikle hangi haf�za birimini de�i�tirece�iz, bu sayede bir sonraki level e kaydedece�iz oyunu.
        finishScoreText.text = score.ToString(); //oyun bitince skor yaz�s�n� g�ncellememiz laz�m ilk olarak.
        gameMenu.SetActive(false);
        finishMenu.SetActive(true);
        gameActive = false;
    }
    //oyuncu skor kazan�nca ekranda g�z�ken skorun de�i�mesi gerekiyor.
    public void ChangeScore(int increment)//art�� miktar�n� parametre olarak alacak.
    {
        score += increment;
        scoreText.text = score.ToString();
    }

    
    public void UpdateMoneyTexts()//para yaz�lar�n� g�ncelle. o anki param�z� haf�za biriminden �ekecek ve oyunda param�z�n yazd��� ne kadar yaz� varsa hepsini g�ncelleyecek. bu �ekilde tek fonksiyon ile para yaz�lar�m�z� g�ncelleyece�iz.
    {
        int money = PlayerPrefs.GetInt("money");//ilk ihtiyac�m�z olan �ey ne kadar pam�z oldu�u.
        startingMenuMoneyText.text = money.ToString();
        gameOverMenuMoneyText.text = money.ToString();
        finishGameMenuMoneyText.text = money.ToString();
    } 
    
    public void GiveMoneyToPlayer(int increment)//oyuncuya para ver. //increment azalma anlam�nada geliyor olabilir. biz bunu s�f�rdan a�a�� d���rmemeiz laz�m.
    {
        int money = PlayerPrefs.GetInt("money"); //para yaz�s�n� g�ncellemeden �nce payay� g�ncellemem gerekiyor
        money = Mathf.Max(0,money + increment);  //mathf max fonsiyonu ald��� iki parametre aras�ndaki en b�y�k de�eri bize d�nd�r�yordu.
        PlayerPrefs.SetInt("money", money);//money in yeni de�erini g�ncelledik
        UpdateMoneyTexts();//bu sayede haf�za b�l�m�ndeki param�z� g�ncelledikten sonra oyun menulerimizdeki para yaz�lar�m�z� da g�ncellemi� oluyoruz.
    }
}
