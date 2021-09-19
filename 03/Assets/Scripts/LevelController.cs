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

    void Start()
    {
        Current = this;
        int currentLevel = PlayerPrefs.GetInt("currentLevel"); //oyuncu hangi levelde kalmýþ deðiþkeni //bu oyunun hafýzasýnda bana int bit deðiþken çek diyorum
       
        //þuanki level in ismi level+current deðilse o leveli yükle dicem ama o levelse o level ile ilgili ayarlamalar yapýlacak.
        if (SceneManager.GetActiveScene().name != "Level " + currentLevel)//açýk olan level e eriþiyoruz //eþit deðilse doðru sahneye yüklemesini yaapcaktýk
        {
            SceneManager.LoadScene("Level" + currentLevel); //level yükle
        }
        else //doðru levelde isek //levelcontrollerin arayüzlere eiþmesi gerekiyor.
        {
            currentLevelText.text = (currentLevel + 1).ToString();   //sýfýnrýncý bölümde isek 1----2 yazmalý
            nextLevelText.text = (currentLevel + 2).ToString();
        }
    }

    
    void Update()
    {
        
    }

    public void StartLevel()
    { 
        PlayerController.Current.ChangeSpeed(PlayerController.Current.runningSpeed);

        startMenu.SetActive(false);
        gameMenu.SetActive(true);
        PlayerController.Current.animator.SetBool("running",true);

        gameActive = true;
    }
}
