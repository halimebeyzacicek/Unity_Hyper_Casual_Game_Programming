using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridegeSpawner : MonoBehaviour
{
    public GameObject startReference, endReference;
    public BoxCollider hiddenPlatform;//boxcolider i boyutland�rmak ve konumland�rmak i�in.
    void Start()
    {
        Vector3 direction = endReference.transform.position - startReference.transform.position;//iki nokta aras�ndaki y�n vekt�r�n� elde etmi� oluyorum.
        float distance = direction.magnitude; //iki nokta aras�ndaki mesafe //bu de�i�keni y�n vekt�r�m�n a��rl���na e�itleyece�im
        direction = direction.normalized; //y�n vekt�r�n� i�lemlerde kullanabilmek i�in birim vekt�re d�n��t�rmeliyim.
        hiddenPlatform.transform.forward = direction; //iki referans noktam�n aras�ndaki y�n de�i�irse g�r�nmez colayd�r�m�nda y�n�n�n de�i�mesi gerekiyor. //�n�n� bu y�ne e�itliyorum yani d�nm�� oluyor. 
        hiddenPlatform.size = new Vector3(hiddenPlatform.size.x, hiddenPlatform.size.y,distance);//kolayd�r� boyutland�rd�k.

        hiddenPlatform.transform.position = startReference.transform.position + (direction * distance / 2)+(new Vector3(0,-direction.z,direction.y)*hiddenPlatform.size.y/2 ); //�imdi konumland�rmas�n� yap�caz.//kolayd�r� iki referans noktas�n�n ortas�na getiriyoruz.

    }

}
