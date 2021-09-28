using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Current;  //diðer silindirlerin player kontroller sýnýfýna eriþebilmesi için static bir deðiþken.
                                             //kendi tipinde bir deðiþken olmalý. //þuanki PlayerControlleri temsil eden bir deðiþken oluþturduk.

    public float limitX;//platfor dýþýna karkateretin çýkmasýný istemiyoz.

    public float runningSpeed;//karakterin max hýzý.
    public float xSpeed;//karakter saða sola ne kadar hýz ile kayacak.
    private float _currentRunningSpeed; //þuanki hýz

    public GameObject ridingCylinderPrefab;
    public List<RidingCylinder> cylinders;  //ayaðýmýz altýndaki silindirleri tutmak için liste

    private bool _spawningBridge;
    public GameObject bridgePiecePrefab;
    private BridegeSpawner _bridgeSpawner;
    private float _creatingBridgeTimer;//köprü yatarma zamanlayýcýsý

    private bool _finished; //finishline e gelip gelmediðini tutacak.

    private float _scoreTimer = 0;

    public Animator animator;

    private float _lastTouchedX; //oyuncunun ekrana dokunuþ hassasiyetinde bir deðiþiklik yapacaðýz.oyuncunun ekrana dokunduðu son yatay pozisyonu bir deðiþkende tutacaðýz.
    private float _dropSoundTimer; 

    public AudioSource cylinderAudioSource,triggerAudioSource,itemAudioSource;
    public AudioClip gatherAudioClip, dropAudioClip,coinAudioClip,buyAudioClip,equipItemAudioClip,unequipItemAudioClip;//silindir hacmi büyürken,küçülürken ses.

    public List<GameObject> wearSpots;  //karakterime giyilme alanlarý oluþturmam gerekiyor. bu alanlarý liste olarak oluþturacaðým.

    void Update()
    {
        
        if (LevelController.Current==null || !LevelController.Current.gameActive )//oyun baþlamadýysa ne koþacak ne de saða sola gidecek //suan herhangi bir level kontroller yoksa veya oyun aktif deðilse
        {
            return;//update fonksiyonunu burada bitir.
        }

        float newX = 0;//karakterin x de ki yeni pozisyonu.
        float touchXDelta = 0;//ne kadar kaydýrdýk saða sola
        if (Input.touchCount > 0){//ekrana dokunmuþ //ilk dokunuþun bilgilerini aldýk. //þuan ekrana dokunulmuþ parmak hareket halinde ise

            if (Input.GetTouch(0).phase == TouchPhase.Began)//ekrana ilk defa dokunuyorsa
            {
                _lastTouchedX = Input.GetTouch(0).position.x; //dokunulan pozisyonun x ine eþitle dicem.
            }
            else if(Input.GetTouch(0).phase == TouchPhase.Moved)//ilk defa dokunmuyorsa, parmaðýný hareket ettiriyorsa 
            {
                //karakteri hareket ettirmek için
                touchXDelta =5* ( _lastTouchedX- Input.GetTouch(0).position.x) / Screen.width;  //fark kadar ilerlemiþ olacak karakterimiz.//ilk dokunan parmaðýn o anki pozisyonundan þuanki pozisyonun farkýný alabilmek için deltapozition bilgisine eriþiyoruz.
                                                                                                //x dedik yani oyuncunun parmaðýný saða sola ne kadar kaydýrdýðýný almýþ oluyoruz. iyi bir oran almak için ekranýn geniþlik deðerine bölüyoruz.
                                                                                                //daha hassas bir dokunuþ için ramamý 5 ile çarpýcam.
                _lastTouchedX = Input.GetTouch(0).position.x;//son dokunulmuþ x i güncellemem lazým.
            }

        }
        else if(Input.GetMouseButton(0)) //oyunuc telefonda deðil ise //0=sol tuþa basýyosa
         {
            touchXDelta = Input.GetAxis("Mouse X");//x düzleminde mouse ne kadar hareket etti.
         }

        newX = transform.position.x + xSpeed * touchXDelta * Time.deltaTime;
        newX = Mathf.Clamp(newX, -limitX, limitX);//newX deðiþkenini sýnýrlanýrýlmýþ haline eþitleyeceðiz.

        Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z + _currentRunningSpeed * Time.deltaTime); //karakter yavaþ yavaþ ilerliyor.
        transform.position = newPosition; //karakterin ilerlemesini saðlamak için. 

        if (_spawningBridge==true) //köprü yaratýp yaratmadýðýný kontrol edeceðim
        {
            PlayDropSound();
            //nesneleri her kare yaratmayacaðýz. belli bir süre bekleyeceðiz
            _creatingBridgeTimer -= Time.deltaTime; //ve köprüyü yatattýðýmýz her karede time.deltatime i çýkaracaðýz.
            if (_creatingBridgeTimer<0)//yeni köprü parçasýný yaratacaðýz ve _creatingBridgeTimer i güncelle.  //köprüyü kurup karakter altýndaki silindirleri azaltýyorduk
            {
                _creatingBridgeTimer = 0.01f;
                IncrementCylinderVolume(-0.01f); //ayný zamanda köprüyü yaratacaðým zaman altýndaki silindirlerin hacmini de küçültemem gerekiyor. //bu parametriyide bir üsteki gibi küçültüp 0.01 yapýyoruz çünkü
                //artýk köprü parçasý yaratma hýzlandýðý için ayný zamanda silindirimizinde deðerinin azalmasý hýzlanacak.  
                GameObject createdBridgePiece = Instantiate(bridgePiecePrefab); //yeni köprü parçasýný oluþturacaðýz.
                //ayný zamanda doðru konum ve doðru þekilde dönmeli
                Vector3 direction = _bridgeSpawner.endReference.transform.position - _bridgeSpawner.startReference.transform.position;//iki nokta arasýndaki yön vektörünü elde etmiþ oluyorum.
                float distance = direction.magnitude; //iki nokta arasýndaki mesafe //bu deðiþkeni yön vektörümün aðýrlýðýna eþitleyeceðim
                direction = direction.normalized; //yön vektörünü iþlemlerde kullanabilmek için birim vektöre dönüþtürmeliyim.
                createdBridgePiece.transform.forward = direction; //þimdi yarattýðýmýz objenin yönünü doðru yöne çevirebiliriz.
                //sonrasýnda ise iki referans noktasý arasýnda karakterimizin ne kadar ilerlediðini pozisyonun z deðerinden bulacaðýz.
                //ve ona göre yarattýðýmýz objeyi baðlangýç referansýndan o kadar ilerleteceðiz.
                float characterDistance = transform.position.z - _bridgeSpawner.startReference.transform.position.z; //öncelikle karakterimizin baþlangýç referans noktasýndan ne kadar ileride olduðunu bulalým.
                characterDistance = Mathf.Clamp(characterDistance, 0, distance); //sýnýrlandýrma yapýyorum characterdistance için max ve min. distance=baðlangýç referansýn bitiþ referansa uzaklýðý
                Vector3 newPiecePosition = _bridgeSpawner.startReference.transform.position + direction * characterDistance; //yarattýðýmýz objenin yeni pozisyonunu tutmasý için vektör3 tipinde bir deðiþken oluþturuyorum.//baþlangýç ref den karakterin uzaklýðý kadar ilerleticez demiþtik.
                newPiecePosition.x = transform.position.x; //karakter saða sola ne kadar gitti ise yeni parçada saða sola o kadar gitsin.           
                createdBridgePiece.transform.position = newPiecePosition; //son olarak yarattýðým parçanýn pozisyonunu bu yeni vektöre eþitliyorum

                if (_finished)//finish çizgisine geldiyse belli süreler içinde skor kazan
                {
                    //eðer köprü yaratýrken oyunu bitirmiþsen skor timer dan geri saymaya baþla
                    _scoreTimer -= Time.deltaTime;
                    if (_scoreTimer<0)//veya skor timer sýfýr a ulaþtýysa ve ya küçük olmuþsa önce skortimeri güncelle 
                    {
                        _scoreTimer = 0.3f;//er bir bölü 3 saniyede skor kazansýn.
                        LevelController.Current.ChangeScore(1); //skor u bir arttýrýcam.
                    }
                }
            }
        }
    }
    public void ChangeSpeed(float value) //levelconrollerin, caraktercontrollerin hýzýný deþitirebilmesi için deðiþken.
    {
        _currentRunningSpeed = value;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="AddCylinder")//silindiri büyüt ve çarpýþtýðýmýzý yok et.
        {
            cylinderAudioSource.PlayOneShot(gatherAudioClip,0.1f);//sadece bir defa çal. parametre:hangi clipi çalacak,ne kadar yükses ses ile çalayým.//1 en yükses se, 0 en düþük

            IncrementCylinderVolume(0.1f); 
            Destroy(other.gameObject);
        }else if (other.tag=="SpawnBridge")//çarptýðý nesne köprü oluþturmaya baþlayan nesene ise.
        {
            StartSpawningBridge(other.transform.parent.GetComponent<BridegeSpawner>());
        }else if (other.tag=="StopSpawnBridge")
        {
            StopSpawingBridge();
            if (_finished)//eðer bitiþ çizgisinde ise 
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
            other.tag = "Untagged";//iki silindir çarparsa eðer iki kere puan kazanmamamsý için.
            LevelController.Current.ChangeScore(10);
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)//karakter objemizin kolaydýrýnda istrigger olan bir objeye çarpýþtýðý her süre boyunca. her çarpýþma anýnda
    {
        
        if (LevelController.Current.gameActive)//þuanki oyun aktif mi.o zman tuzakla çarpýþýp çarpýþmadýðýný kontrol et. yoksa tuzak üstünde ölünce yine o silindir azalma sesi gelir.
        {
            if (other.tag == "Trap")//eðer tuzaða çarparsa silindiri azalt.
            {
                PlayDropSound();
                IncrementCylinderVolume(-Time.fixedDeltaTime);//ontrigger fonksiyonlarý fizik döngüsünde çalýþtýklarý için fizikle alakalý olan zaman birimini kullanacaðým.
                                                              //silindirleri azaltmak istediðim içim eksi koyuyorum.
            }
        }
        
    }

    public void IncrementCylinderVolume(float value)//Silindir hacmini arttýr value=artýþ deðeri
    {
        if (cylinders.Count == 0)//ayaðýmýn altýnda silindir yok.
        {
            if (value > 0)//yani silindir hacmi azaltýlmaya çalýþýlmýyprsa bir tane slindir yarat
            {
                CreateCylinder(value);
            }
            else
            {
                //Gameover

                if (_finished)//eðer karakter bitiþ çizgisine ulaþtýysa game ove olmayacakleveli bitiricez.
                {
                    LevelController.Current.FinishGame();
                }
                else//çizgiye ulaþmadan silindirleri bittiyse.
                {
                    Die(); 
                }
            }
        }
        else//en alttaki silindir boyutunu güncelle.
        {
            cylinders[cylinders.Count - 1].IncrementCylinderVolume(value); //son index e -1 ile eriþiyoruz.
        }
    }

    public void Die()//karakter bu fonksiyonu çaðýrdýðýnda ölecek.
    {
        animator.SetBool("dead", true);
        gameObject.layer = 6; //karakter layer ini 6 yapacaðým.(caracterlayer.)
        Camera.main.transform.SetParent(null); //karakter aþaðý düþebileceði için kamara onu takip etmesin diye kamaranýn parentini null yapýcam.
        LevelController.Current.GameOver();
    }



    public void CreateCylinder(float value)//yeni silindir yaratmak için
    {
        RidingCylinder createdCylinder = Instantiate(ridingCylinderPrefab, transform).GetComponent<RidingCylinder>(); //yaratýlan adamýn çocuðu oluyor.
        cylinders.Add(createdCylinder);
        createdCylinder.IncrementCylinderVolume(value);//boyunu güncelledik.
    }
    public void destroyCylinder(RidingCylinder cylinder)
    {
        cylinders.Remove(cylinder); //önce listeden çýkarmalýyýz.
        Destroy(cylinder.gameObject); //þimdi oyun sahnesinden yok etmeliyiz.
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

    public void PlayDropSound()//silindir azalýrken ses
    {
        _dropSoundTimer -= Time.deltaTime;//saniye gibi geri sayýyorum
        //sýfýrdan küçükse sesi çal diyeceðim. ve zamanlayýcýyý sýfýrla.
        if (_dropSoundTimer<0)
        {
            _dropSoundTimer = 0.15f;
            cylinderAudioSource.PlayOneShot(dropAudioClip,0.1f);
        }
    }

}
