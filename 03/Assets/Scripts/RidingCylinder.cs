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
            //silindirin boyutunu tam olarak bir yap
            //1'den ne kadar b�y�kse o b�y�kl�kte yeni bir silindir yarat
        }else if (_value<0)
        {
            //karakterimize bu silindiri yok etmesini s�yleyece�iz.
        }
        else
        {
            //silindir boyutunu g�ncelle
        }
    }

}
