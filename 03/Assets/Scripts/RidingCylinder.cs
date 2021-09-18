using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidingCylinder : MonoBehaviour
{
    private bool _filled;//silindir tam doldu mu
    private float _value; //silindirin sayýsal olarak ne kadar dolduðu.
    
    public void IncrementCylinderVolume(float value)
    {
        _value += value;
        if (_value > 1)
        {
            //silindirin boyutunu tam olarak bir yap
            //1'den ne kadar büyükse o büyüklükte yeni bir silindir yarat
        }else if (_value<0)
        {
            //karakterimize bu silindiri yok etmesini söyleyeceðiz.
        }
        else
        {
            //silindir boyutunu güncelle
        }
    }

}
