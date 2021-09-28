using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Current;  //di�er silindirlerin player kontroller s�n�f�na eri�ebilmesi i�in static bir de�i�ken.
                                             //kendi tipinde bir de�i�ken olmal�. //�uanki PlayerControlleri temsil eden bir de�i�ken olu�turduk.

    public float limitX;//platfor d���na karkateretin ��kmas�n� istemiyoz.

    public float runningSpeed;//karakterin max h�z�.
    public float xSpeed;//karakter sa�a sola ne kadar h�z ile kayacak.
    private float _currentRunningSpeed; //�uanki h�z

    public GameObject ridingCylinderPrefab;
    public List<RidingCylinder> cylinders;  //aya��m�z alt�ndaki silindirleri tutmak i�in liste

    private bool _spawningBridge;
    public GameObject bridgePiecePrefab;
    private BridegeSpawner _bridgeSpawner;
    private float _creatingBridgeTimer;//k�pr� yatarma zamanlay�c�s�

    private bool _finished; //finishline e gelip gelmedi�ini tutacak.

    private float _scoreTimer = 0;

    public Animator animator;

    private float _lastTouchedX; //oyuncunun ekrana dokunu� hassasiyetinde bir de�i�iklik yapaca��z.oyuncunun ekrana dokundu�u son yatay pozisyonu bir de�i�kende tutaca��z.
    private float _dropSoundTimer; 

    public AudioSource cylinderAudioSource,triggerAudioSource,itemAudioSource;
    public AudioClip gatherAudioClip, dropAudioClip,coinAudioClip,buyAudioClip,equipItemAudioClip,unequipItemAudioClip;//silindir hacmi b�y�rken,k���l�rken ses.

    public List<GameObject> wearSpots;  //karakterime giyilme alanlar� olu�turmam gerekiyor. bu alanlar� liste olarak olu�turaca��m.

    void Update()
    {
        
        if (LevelController.Current==null || !LevelController.Current.gameActive )//oyun ba�lamad�ysa ne ko�acak ne de sa�a sola gidecek //suan herhangi bir level kontroller yoksa veya oyun aktif de�ilse
        {
            return;//update fonksiyonunu burada bitir.
        }

        float newX = 0;//karakterin x de ki yeni pozisyonu.
        float touchXDelta = 0;//ne kadar kayd�rd�k sa�a sola
        if (Input.touchCount > 0){//ekrana dokunmu� //ilk dokunu�un bilgilerini ald�k. //�uan ekrana dokunulmu� parmak hareket halinde ise

            if (Input.GetTouch(0).phase == TouchPhase.Began)//ekrana ilk defa dokunuyorsa
            {
                _lastTouchedX = Input.GetTouch(0).position.x; //dokunulan pozisyonun x ine e�itle dicem.
            }
            else if(Input.GetTouch(0).phase == TouchPhase.Moved)//ilk defa dokunmuyorsa, parma��n� hareket ettiriyorsa 
            {
                //karakteri hareket ettirmek i�in
                touchXDelta =5* ( _lastTouchedX- Input.GetTouch(0).position.x) / Screen.width;  //fark kadar ilerlemi� olacak karakterimiz.//ilk dokunan parma��n o anki pozisyonundan �uanki pozisyonun fark�n� alabilmek i�in deltapozition bilgisine eri�iyoruz.
                                                                                                //x dedik yani oyuncunun parma��n� sa�a sola ne kadar kayd�rd���n� alm�� oluyoruz. iyi bir oran almak i�in ekran�n geni�lik de�erine b�l�yoruz.
                                                                                                //daha hassas bir dokunu� i�in ramam� 5 ile �arp�cam.
                _lastTouchedX = Input.GetTouch(0).position.x;//son dokunulmu� x i g�ncellemem laz�m.
            }

        }
        else if(Input.GetMouseButton(0)) //oyunuc telefonda de�il ise //0=sol tu�a bas�yosa
         {
            touchXDelta = Input.GetAxis("Mouse X");//x d�zleminde mouse ne kadar hareket etti.
         }

        newX = transform.position.x + xSpeed * touchXDelta * Time.deltaTime;
        newX = Mathf.Clamp(newX, -limitX, limitX);//newX de�i�kenini s�n�rlan�r�lm�� haline e�itleyece�iz.

        Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z + _currentRunningSpeed * Time.deltaTime); //karakter yava� yava� ilerliyor.
        transform.position = newPosition; //karakterin ilerlemesini sa�lamak i�in. 

        if (_spawningBridge==true) //k�pr� yarat�p yaratmad���n� kontrol edece�im
        {
            PlayDropSound();
            //nesneleri her kare yaratmayaca��z. belli bir s�re bekleyece�iz
            _creatingBridgeTimer -= Time.deltaTime; //ve k�pr�y� yatatt���m�z her karede time.deltatime i ��karaca��z.
            if (_creatingBridgeTimer<0)//yeni k�pr� par�as�n� yarataca��z ve _creatingBridgeTimer i g�ncelle.  //k�pr�y� kurup karakter alt�ndaki silindirleri azalt�yorduk
            {
                _creatingBridgeTimer = 0.01f;
                IncrementCylinderVolume(-0.01f); //ayn� zamanda k�pr�y� yarataca��m zaman alt�ndaki silindirlerin hacmini de k���ltemem gerekiyor. //bu parametriyide bir �steki gibi k���lt�p 0.01 yap�yoruz ��nk�
                //art�k k�pr� par�as� yaratma h�zland��� i�in ayn� zamanda silindirimizinde de�erinin azalmas� h�zlanacak.  
                GameObject createdBridgePiece = Instantiate(bridgePiecePrefab); //yeni k�pr� par�as�n� olu�turaca��z.
                //ayn� zamanda do�ru konum ve do�ru �ekilde d�nmeli
                Vector3 direction = _bridgeSpawner.endReference.transform.position - _bridgeSpawner.startReference.transform.position;//iki nokta aras�ndaki y�n vekt�r�n� elde etmi� oluyorum.
                float distance = direction.magnitude; //iki nokta aras�ndaki mesafe //bu de�i�keni y�n vekt�r�m�n a��rl���na e�itleyece�im
                direction = direction.normalized; //y�n vekt�r�n� i�lemlerde kullanabilmek i�in birim vekt�re d�n��t�rmeliyim.
                createdBridgePiece.transform.forward = direction; //�imdi yaratt���m�z objenin y�n�n� do�ru y�ne �evirebiliriz.
                //sonras�nda ise iki referans noktas� aras�nda karakterimizin ne kadar ilerledi�ini pozisyonun z de�erinden bulaca��z.
                //ve ona g�re yaratt���m�z objeyi ba�lang�� referans�ndan o kadar ilerletece�iz.
                float characterDistance = transform.position.z - _bridgeSpawner.startReference.transform.position.z; //�ncelikle karakterimizin ba�lang�� referans noktas�ndan ne kadar ileride oldu�unu bulal�m.
                characterDistance = Mathf.Clamp(characterDistance, 0, distance); //s�n�rland�rma yap�yorum characterdistance i�in max ve min. distance=ba�lang�� referans�n biti� referansa uzakl���
                Vector3 newPiecePosition = _bridgeSpawner.startReference.transform.position + direction * characterDistance; //yaratt���m�z objenin yeni pozisyonunu tutmas� i�in vekt�r3 tipinde bir de�i�ken olu�turuyorum.//ba�lang�� ref den karakterin uzakl��� kadar ilerleticez demi�tik.
                newPiecePosition.x = transform.position.x; //karakter sa�a sola ne kadar gitti ise yeni par�ada sa�a sola o kadar gitsin.           
                createdBridgePiece.transform.position = newPiecePosition; //son olarak yaratt���m par�an�n pozisyonunu bu yeni vekt�re e�itliyorum

                if (_finished)//finish �izgisine geldiyse belli s�reler i�inde skor kazan
                {
                    //e�er k�pr� yarat�rken oyunu bitirmi�sen skor timer dan geri saymaya ba�la
                    _scoreTimer -= Time.deltaTime;
                    if (_scoreTimer<0)//veya skor timer s�f�r a ula�t�ysa ve ya k���k olmu�sa �nce skortimeri g�ncelle 
                    {
                        _scoreTimer = 0.3f;//er bir b�l� 3 saniyede skor kazans�n.
                        LevelController.Current.ChangeScore(1); //skor u bir artt�r�cam.
                    }
                }
            }
        }
    }
    public void ChangeSpeed(float value) //levelconrollerin, caraktercontrollerin h�z�n� de�itirebilmesi i�in de�i�ken.
    {
        _currentRunningSpeed = value;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="AddCylinder")//silindiri b�y�t ve �arp��t���m�z� yok et.
        {
            cylinderAudioSource.PlayOneShot(gatherAudioClip,0.1f);//sadece bir defa �al. parametre:hangi clipi �alacak,ne kadar y�kses ses ile �alay�m.//1 en y�kses se, 0 en d���k

            IncrementCylinderVolume(0.1f); 
            Destroy(other.gameObject);
        }else if (other.tag=="SpawnBridge")//�arpt��� nesne k�pr� olu�turmaya ba�layan nesene ise.
        {
            StartSpawningBridge(other.transform.parent.GetComponent<BridegeSpawner>());
        }else if (other.tag=="StopSpawnBridge")
        {
            StopSpawingBridge();
            if (_finished)//e�er biti� �izgisinde ise 
            {
                LevelController.Current.FinishGame();
            }
        }else if (other.tag == "Finish")
        {
            _finished = true;
            StartSpawningBridge(other.transform.parent.GetComponent<BridegeSpawner>());
        }else if (other.tag == "Coin")
        {
            triggerAudioSource.PlayOneShot(coinAudioClip,0.1f);
            other.tag = "Untagged";//iki silindir �arparsa e�er iki kere puan kazanmamams� i�in.
            LevelController.Current.ChangeScore(10);
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)//karakter objemizin kolayd�r�nda istrigger olan bir objeye �arp��t��� her s�re boyunca. her �arp��ma an�nda
    {
        
        if (LevelController.Current.gameActive)//�uanki oyun aktif mi.o zman tuzakla �arp���p �arp��mad���n� kontrol et. yoksa tuzak �st�nde �l�nce yine o silindir azalma sesi gelir.
        {
            if (other.tag == "Trap")//e�er tuza�a �arparsa silindiri azalt.
            {
                PlayDropSound();
                IncrementCylinderVolume(-Time.fixedDeltaTime);//ontrigger fonksiyonlar� fizik d�ng�s�nde �al��t�klar� i�in fizikle alakal� olan zaman birimini kullanaca��m.
                                                              //silindirleri azaltmak istedi�im i�im eksi koyuyorum.
            }
        }
        
    }

    public void IncrementCylinderVolume(float value)//Silindir hacmini artt�r value=art�� de�eri
    {
        if (cylinders.Count == 0)//aya��m�n alt�nda silindir yok.
        {
            if (value > 0)//yani silindir hacmi azalt�lmaya �al���lm�yprsa bir tane slindir yarat
            {
                CreateCylinder(value);
            }
            else
            {
                //Gameover

                if (_finished)//e�er karakter biti� �izgisine ula�t�ysa game ove olmayacakleveli bitiricez.
                {
                    LevelController.Current.FinishGame();
                }
                else//�izgiye ula�madan silindirleri bittiyse.
                {
                    Die(); 
                }
            }
        }
        else//en alttaki silindir boyutunu g�ncelle.
        {
            cylinders[cylinders.Count - 1].IncrementCylinderVolume(value); //son index e -1 ile eri�iyoruz.
        }
    }

    public void Die()//karakter bu fonksiyonu �a��rd���nda �lecek.
    {
        animator.SetBool("dead", true);
        gameObject.layer = 6; //karakter layer ini 6 yapaca��m.(caracterlayer.)
        Camera.main.transform.SetParent(null); //karakter a�a�� d��ebilece�i i�in kamara onu takip etmesin diye kamaran�n parentini null yap�cam.
        LevelController.Current.GameOver();
    }



    public void CreateCylinder(float value)//yeni silindir yaratmak i�in
    {
        RidingCylinder createdCylinder = Instantiate(ridingCylinderPrefab, transform).GetComponent<RidingCylinder>(); //yarat�lan adam�n �ocu�u oluyor.
        cylinders.Add(createdCylinder);
        createdCylinder.IncrementCylinderVolume(value);//boyunu g�ncelledik.
    }
    public void destroyCylinder(RidingCylinder cylinder)
    {
        cylinders.Remove(cylinder); //�nce listeden ��karmal�y�z.
        Destroy(cylinder.gameObject); //�imdi oyun sahnesinden yok etmeliyiz.
    }

    public void StartSpawningBridge(BridegeSpawner spawner)
    {
        _bridgeSpawner = spawner;
        _spawningBridge = true;
    }
    public void StopSpawingBridge()
    {
        _spawningBridge = false;
    }

    public void PlayDropSound()//silindir azal�rken ses
    {
        _dropSoundTimer -= Time.deltaTime;//saniye gibi geri say�yorum
        //s�f�rdan k���kse sesi �al diyece�im. ve zamanlay�c�y� s�f�rla.
        if (_dropSoundTimer<0)
        {
            _dropSoundTimer = 0.15f;
            cylinderAudioSource.PlayOneShot(dropAudioClip,0.1f);
        }
    }

}
