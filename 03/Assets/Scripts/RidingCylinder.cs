using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidingCylinder : MonoBehaviour
{
    private bool _filled;//silindir tam doldu mu
    private float _value; //silindirin say�sal olarak ne kadar doldu�u.
    
    public void IncrementCylinderVolume(float value)
    {
        _value += value;
        if (_value > 1)
        {
            float leftValue = _value  - 1; //geriye kalan de�er
                                          //
            //silindirin boyutunu tam olarak bir yap
            int cylinderCount = PlayerController.Current.cylinders.Count;
            transform.localPosition = new Vector3(transform.localPosition.x, -0.5f * (cylinderCount - 1) - 0.25f, transform.localPosition.z);
            transform.localScale = new Vector3(0.5f, transform.localScale.y, 0.5f);  //silindirin boyutunu hesaplamak

            PlayerController.Current.CreateCylinder(leftValue); //1'den ne kadar b�y�kse o b�y�kl�kte yeni bir silindir yarat
        }else if (_value<0)//silindirin boyutu s�f�rdan k���kse.
        {
            PlayerController.Current.destroyCylinder(this); //karakterimize bu silindiri yok etmesini s�yleyece�iz.
        }
        else
        {
            //silindir boyutunu g�ncelle
            int cylinderCount = PlayerController.Current.cylinders.Count;
            transform.localPosition = new Vector3(transform.localPosition.x, -0.5f * (cylinderCount - 1) - 0.25f*_value, transform.localPosition.z);
            transform.localScale = new Vector3(0.5f * _value, transform.localScale.y, 0.5f * _value);  //silindirin boyutunu hesaplamak

        }
    }

}
