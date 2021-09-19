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

    public Animator animator; 


    void Start()
    {
        Current = this;
        //_currentRunningSpeed = runningSpeed;
    }

    
    void Update()
    {
        
        if (LevelController.Current==null || !LevelController.Current.gameActive )//oyun baþlamadýysa ne koþacak ne de saða sola gidecek //suan herhangi bir level kontroller yoksa veya oyun aktif deðilse
        {
            return;//update fonksiyonunu burada bitir.
        }

        float newX = 0;//karakterin x de ki yeni pozisyonu.
        float touchXDelta = 0;//ne kadar kaydýrdýk saða sola
        if (Input.touchCount > 0 && Input.GetTouch(0).phase==TouchPhase.Moved){//ekrana dokunmuþ //ilk dokunuþun bilgilerini aldýk. //þuan ekrana dokunulmuþ parmak hareket halinde ise

                touchXDelta = Input.GetTouch(0).deltaPosition.x / Screen.width;  //ilk dokunan parmaðýn o anki pozisyonundan þuanki pozisyonun farkýný alabilmek için deltapozition bilgisine eriþiyoruz.
                //x dedik yani oyuncunun parmaðýný saða sola ne kadar kaydýrdýðýný almýþ oluyoruz. iyi bir oran almak için ekranýn geniþlik deðerine bölüyoruz.
        }else if(Input.GetMouseButton(0)) //oyunuc telefonda deðil ise //0=sol tuþa basýyosa
         {
            touchXDelta = Input.GetAxis("Mouse X");//x düzleminde mouse ne kadar hareket etti.
         }

        newX = transform.position.x + xSpeed * touchXDelta * Time.deltaTime;
        newX = Mathf.Clamp(newX, -limitX, limitX);//newX deðiþkenini sýnýrlanýrýlmýþ haline eþitleyeceðiz.

        Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z + _currentRunningSpeed * Time.deltaTime); //karakter yavaþ yavaþ ilerliyor.
        transform.position = newPosition; //karakterin ilerlemesini saðlamak için. 

        if (_spawningBridge==true) //köprü yaratýp yaratmadýðýný kontrol edeceðim
        {
            //nesneleri her kare yaratmayacaðýz. belli bir süre bekleyeceðiz
            _creatingBridgeTimer -= Time.deltaTime; //ve köprüyü yatattýðýmýz her karede time.deltatime i çýkaracaðýz.
            if (_creatingBridgeTimer<0)//yeni köprü parçasýný yaratacaðýz ve _creatingBridgeTimer i güncelle.
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
            IncrementCylinderVolume(0.1f); 
            Destroy(other.gameObject);
        }else if (other.tag=="SpawnBridge")//çarptýðý nesne köprü oluþturmaya baþlayan nesene ise.
        {
            StartSpawningBridge(other.transform.parent.GetComponent<BridegeSpawner>());
        }else if (other.tag=="StopSpawnBridge")
        {
            StopSpawingBridge();
        }
    }

    private void OnTriggerStay(Collider other)//karakter objemizin kolaydýrýnda istrigger olan bir objeye çarpýþtýðý her süre boyunca. her çarpýþma anýnda
    {
        if (other.tag == "Trap")
        {
            IncrementCylinderVolume(-Time.fixedDeltaTime);//ontrigger fonksiyonlarý fizik döngüsünde çalýþtýklarý için fizikle alakalý olan zaman birimini kullanacaðým.
            //silindirleri azaltmak istediðim içim eksi koyuyorum.
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
            }
        }
        else//en alttaki silindir boyutunu güncelle.
        {
            cylinders[cylinders.Count - 1].IncrementCylinderVolume(value); //son index e -1 ile eriþiyoruz.
        }
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

}
