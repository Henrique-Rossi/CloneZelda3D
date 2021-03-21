using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{   public ParticleSystem fxGrass;
    private bool isCut;

    void GetHit(int amount){
        if (isCut == false)
        {
            isCut=true;
            transform.localScale = new Vector3(1f,1f,1f);
            fxGrass.Emit(100);
            
        }
        //Desafio-Fazer grama Crescer de novo(Curritine)

        
    }
}
