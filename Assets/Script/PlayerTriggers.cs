using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggers : MonoBehaviour
{  
    private GameManager _GameManager;
    [Header("Camera")]
    public GameObject camB;


    private void Start() {
        _GameManager=FindObjectOfType(typeof(GameManager)) as GameManager;
    }

    private void OnTriggerEnter(Collider other) {
        switch (other.gameObject.tag)
        {
            case "CamTrigger":
                 camB.SetActive(true);
                 break;
            
             case "Coletavel":
                 _GameManager.SetGem(1);
                 Destroy(other.gameObject);
                 break;     
        }
    }
    private void OnTriggerExit(Collider other) {
        switch (other.gameObject.tag)
        {
            case "CamTrigger":
                 camB.SetActive(false);
                 break;
            
        }
    }
}
