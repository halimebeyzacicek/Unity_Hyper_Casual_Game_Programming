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

    public Animator animator; 


    void Start()
    {
        Current = this;
        //_currentRunningSpeed = runningSpeed;
    }

    
    void Update()
    {
        
        if (LevelController.Current==null || !LevelController.Current.gameActive )//oyun ba�lamad�ysa ne ko�acak ne de sa�a sola gidecek //suan herhangi bir level kontroller yoksa veya oyun aktif de�ilse
        {
            return;//update fonksiyonunu burada bitir.
        }

        float newX = 0;//karakterin x de ki yeni pozisyonu.
        float touchXDelta = 0;//ne kadar kayd�rd�k sa�a sola
        if (Input.touchCount > 0 && Input.GetTouch(0).phase==TouchPhase.Moved){//ekrana dokunmu� //ilk dokunu�un bilgilerini ald�k. //�uan ekrana dokunulmu� parmak hareket halinde ise

                touchXDelta = Input.GetTouch(0).deltaPosition.x / Screen.width;  //ilk dokunan parma��n o anki pozisyonundan �uanki pozisyonun fark�n� alabilmek i�in deltapozition bilgisine eri�iyoruz.
                //x dedik yani oyuncunun parma��n� sa�a sola ne kadar kayd�rd���n� alm�� oluyoruz. iyi bir oran almak i�in ekran�n geni�lik de�erine b�l�yoruz.
        }else if(Input.GetMouseButton(0)) //oyunuc telefonda de�il ise //0=sol tu�a bas�yosa
         {
            touchXDelta = Input.GetAxis("Mouse X");//x d�zleminde mouse ne kadar hareket etti.
         }

        newX = transform.position.x + xSpeed * touchXDelta * Time.deltaTime;
        newX = Mathf.Clamp(newX, -limitX, limitX);//newX de�i�kenini s�n�rlan�r�lm�� haline e�itleyece�iz.

        Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z + _currentRunningSpeed * Time.deltaTime); //karakter yava� yava� ilerliyor.
        transform.position = newPosition; //karakterin ilerlemesini sa�lamak i�in. 

        if (_spawningBridge==true) //k�pr� yarat�p yaratmad���n� kontrol edece�im
        {
            //nesneleri her kare yaratmayaca��z. belli bir s�re bekleyece�iz
            _creatingBridgeTimer -= Time.deltaTime; //ve k�pr�y� yatatt���m�z her karede time.deltatime i ��karaca��z.
            if (_creatingBridgeTimer<0)//yeni k�pr� par�as�n� yarataca��z ve _creatingBridgeTimer i g�ncelle.
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
            IncrementCylinderVolume(0.1f); 
            Destroy(other.gameObject);
        }else if (other.tag=="SpawnBridge")//�arpt��� nesne k�pr� olu�turmaya ba�layan nesene ise.
        {
            StartSpawningBridge(other.transform.parent.GetComponent<BridegeSpawner>());
        }else if (other.tag=="StopSpawnBridge")
        {
            StopSpawingBridge();
        }
    }

    private void OnTriggerStay(Collider other)//karakter objemizin kolayd�r�nda istrigger olan bir objeye �arp��t��� her s�re boyunca. her �arp��ma an�nda
    {
        if (other.tag == "Trap")
        {
            IncrementCylinderVolume(-Time.fixedDeltaTime);//ontrigger fonksiyonlar� fizik d�ng�s�nde �al��t�klar� i�in fizikle alakal� olan zaman birimini kullanaca��m.
            //silindirleri azaltmak istedi�im i�im eksi koyuyorum.
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
            }
        }
        else//en alttaki silindir boyutunu g�ncelle.
        {
            cylinders[cylinders.Count - 1].IncrementCylinderVolume(value); //son index e -1 ile eri�iyoruz.
        }
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

}
