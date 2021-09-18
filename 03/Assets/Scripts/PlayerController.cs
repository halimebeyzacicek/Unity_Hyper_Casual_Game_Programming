using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float limitX;//platfor d���na karkateretin ��kmas�n� istemiyoz.

    public float runningSpeed;//karakterin max h�z�.
    public float xSpeed;//karakter sa�a sola ne kadar h�z ile kayacak.
    private float _currentRunningSpeed; //�uanki h�z

    public GameObject ridingCylinderPrefab;
    public List<RidingCylinder> cylinders;  //aya��m�z alt�ndaki silindirleri tutmak i�in liste

    void Start()
    {
        _currentRunningSpeed = runningSpeed;
    }

    
    void Update()
    {
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
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="AddCylinder")//silindiri b�y�t ve �arp��t���m�z� yok et.
        {
            IncrementCylinderVolume(0.2f); 
            Destroy(other.gameObject);
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
}
