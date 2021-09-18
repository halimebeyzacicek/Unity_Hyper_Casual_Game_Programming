using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float limitX;//platfor dýþýna karkateretin çýkmasýný istemiyoz.

    public float runningSpeed;//karakterin max hýzý.
    public float xSpeed;//karakter saða sola ne kadar hýz ile kayacak.
    private float _currentRunningSpeed; //þuanki hýz

    public GameObject ridingCylinderPrefab;
    public List<RidingCylinder> cylinders;  //ayaðýmýz altýndaki silindirleri tutmak için liste

    void Start()
    {
        _currentRunningSpeed = runningSpeed;
    }

    
    void Update()
    {
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
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="AddCylinder")//silindiri büyüt ve çarpýþtýðýmýzý yok et.
        {
            IncrementCylinderVolume(0.2f); 
            Destroy(other.gameObject);
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
}
