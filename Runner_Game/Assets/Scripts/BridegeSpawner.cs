using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridegeSpawner : MonoBehaviour
{
    public GameObject startReference, endReference;
    public BoxCollider hiddenPlatform;//boxcolider i boyutlandýrmak ve konumlandýrmak için.
    void Start()
    {
        Vector3 direction = endReference.transform.position - startReference.transform.position;//iki nokta arasýndaki yön vektörünü elde etmiþ oluyorum.
        float distance = direction.magnitude; //iki nokta arasýndaki mesafe //bu deðiþkeni yön vektörümün aðýrlýðýna eþitleyeceðim
        direction = direction.normalized; //yön vektörünü iþlemlerde kullanabilmek için birim vektöre dönüþtürmeliyim.
        hiddenPlatform.transform.forward = direction; //iki referans noktamýn arasýndaki yön deðiþirse görünmez colaydýrýmýnda yönünün deðiþmesi gerekiyor. //önünü bu yöne eþitliyorum yani dönmüþ oluyor. 
        hiddenPlatform.size = new Vector3(hiddenPlatform.size.x, hiddenPlatform.size.y,distance);//kolaydýrý boyutlandýrdýk.

        hiddenPlatform.transform.position = startReference.transform.position + (direction * distance / 2)+(new Vector3(0,-direction.z,direction.y)*hiddenPlatform.size.y/2 ); //þimdi konumlandýrmasýný yapýcaz.//kolaydýrý iki referans noktasýnýn ortasýna getiriyoruz.

    }

}
