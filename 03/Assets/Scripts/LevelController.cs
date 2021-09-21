using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    public static LevelController Current; //diðer sýnýflarýn bu objeye eriþmesi için bu sýnýf için static bir deðiþken oluþturacaðým.
    public bool gameActive = false; //oyun aktif mi deðil mi

    public GameObject startMenu, gameMenu, gameOverMenu, finishMenu; //menülere eriþmek
    public Text scoreText, finishScoreText, currentLevelText, nextLevelText; //yazý objelerini tutmak için
    public Slider levelProgressBar;
    public float maxDistance; //oyun ilk baþladýðý anda karakterimizin bitiþ çizgisine olan uzaklýðýný tutmamýz gerekiyor.
    public GameObject finishLine; //bitiþ çizgimizi tutacak.

    public AudioSource gameMusicAudioSource;
    public AudioClip victoryAudioClip, gameOverAudioClip;

    int currentLevel;
    int score; //skorlarý tutan deðiþken


    void Start()
    {
        Current = this;
        currentLevel = PlayerPrefs.GetInt("currentLevel"); //oyuncu hangi levelde kalmýþ deðiþkeni //bu oyunun hafýzasýnda bana int bit deðiþken çek diyorum
       
        //þuanki level in ismi level+current deðilse o leveli yükle dicem ama o levelse o level ile ilgili ayarlamalar yapýlacak.
        if (SceneManager.GetActiveScene().name != "Level " + currentLevel)//açýk olan level e eriþiyoruz //eþit deðilse doðru sahneye yüklemesini yaapcaktýk
        {
            Debug.Log("girdi"+ currentLevel);
            SceneManager.LoadScene("Level " + currentLevel); //level yükle
        }
        else //doðru levelde isek //levelcontrollerin arayüzlere eiþmesi gerekiyor.
        {
            Debug.Log("girdi2"+ currentLevel);
            currentLevelText.text = (currentLevel + 1).ToString();   //sýfýnrýncý bölümde isek 1----2 yazmalý
            nextLevelText.text = (currentLevel + 2).ToString();
        }
        gameMusicAudioSource = Camera.main.GetComponent<AudioSource>();//kameradan sesi çekecek. baþlangýç sesini(oyun sesini)
    }

    
    void Update()
    {
        //eðer oyun aktif ise slider i sürekli güncellemeliyim.
        if (gameActive)
        {
            PlayerController player = PlayerController.Current; //öncelikle playercontrollerimi tutmasý için bir deðiþken oluþturuyorum.
            float distance= finishLine.transform.position.z - PlayerController.Current.transform.position.z;  //karakterin þuan çizgiye ne kadar uzak olduðunu buluyorum.
            levelProgressBar.value = 1 - (distance / maxDistance); //o silider imin ne kadar dolup dolmadýðýný da bu deðerde tutucaz. //slider 1-0 arasý olacak.
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

    public void RestartLevel() //ölünce leveli tekrar yükleyecek.
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //level yükle ve verdiðim parametre þuanki levelin ismi oluyor.
    }

    public void LoadNextLevel() //bir sonraki leveli yüklemek için. 
    {
        SceneManager.LoadScene( "Level " + (currentLevel+1));
    } 

    public void GameOver()
    {
        gameMusicAudioSource.Stop();
        gameMusicAudioSource.PlayOneShot(gameOverAudioClip);//burada ikinci bir parametre vermiyorum çünkü otomatik olarak müzik kaynaðýmýn kendi ses yüksekliðini alacak.
        gameMenu.SetActive(false); //yanýnca ilk baþta oyun menusunu kapatmamýz gerekiyor
        gameOverMenu.SetActive(true); //game over menumuzu true yapmamýz gerekiyor.
        gameActive = false; //son olarak oyunda yandýðýmýz için o yüzden game activi de false yapmamýz lazým.
    }
    public void FinishGame()//oyunu bitirdiðimiz zaman yazacak fonksiyon.
    {
        gameMusicAudioSource.Stop();
        gameMusicAudioSource.PlayOneShot(victoryAudioClip);
        PlayerPrefs.SetInt("currentLevel",currentLevel+1); //öncelikle unityin hafýza biriminde hangi levelde kaldýðýmýzý güncellememiz gerekiyor.
        //parantez içi= öncelikle hangi hafýza birimini deðiþtireceðiz, bu sayede bir sonraki level e kaydedeceðiz oyunu.
        finishScoreText.text = score.ToString(); //oyun bitince skor yazýsýný güncellememiz lazým ilk olarak.
        gameMenu.SetActive(false);
        finishMenu.SetActive(true);
        gameActive = false;
    }
    //oyuncu skor kazanýnca ekranda gözüken skorun deðiþmesi gerekiyor.
    public void ChangeScore(int increment)//artýþ miktarýný parametre olarak alacak.
    {
        score += increment;
        scoreText.text = score.ToString();
    }
}
