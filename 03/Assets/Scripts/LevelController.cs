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
    public Text scoreText, finishScoreText, currentLevelText, nextLevelText; //yaz� objelerini tutmak i�in

    void Start()
    {
        Current = this;
        int currentLevel = PlayerPrefs.GetInt("currentLevel"); //oyuncu hangi levelde kalm�� de�i�keni //bu oyunun haf�zas�nda bana int bit de�i�ken �ek diyorum
       
        //�uanki level in ismi level+current de�ilse o leveli y�kle dicem ama o levelse o level ile ilgili ayarlamalar yap�lacak.
        if (SceneManager.GetActiveScene().name != "Level " + currentLevel)//a��k olan level e eri�iyoruz //e�it de�ilse do�ru sahneye y�klemesini yaapcakt�k
        {
            SceneManager.LoadScene("Level" + currentLevel); //level y�kle
        }
        else //do�ru levelde isek //levelcontrollerin aray�zlere ei�mesi gerekiyor.
        {
            currentLevelText.text = (currentLevel + 1).ToString();   //s�f�nr�nc� b�l�mde isek 1----2 yazmal�
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
