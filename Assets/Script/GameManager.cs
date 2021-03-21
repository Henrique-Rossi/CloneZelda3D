using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

//Acesso pra todos os outros scripts
public enum enemyState
{
    IDLE,
    ALERT,
    PATROL,
    FOLLOW,
    FURY,
    DIE
    
}

public enum GameState{
    GAMEPLAY,DIE
}

public class GameManager : MonoBehaviour
{
    public GameState gameState;

   
    [Header("Info Player")]
    public Transform player;//para seguir o player 
    public int gems;

    [Header("UI")]
    public Text txtGem;


    [Header("Drops")]
    public GameObject gemPrefab;
    public int percDrop=25;


    [Header("Slime IA")]
    public  float slimeIdleWaitTime;  //tempo que vai ficar parado para executrar uma nova ação
    public Transform[] slimeWayPoints;//array pra armazaenaar as localizações dep onto de destiono
    public float slimeDistanceAttack=2.3f;
    public float slimeAlertTime=2f;
    public float slimeAttackDelay=2f;
    public float slimeLookAtSpeed=1f;//velocidade de rotação para atacar
    public float SlimeStopDistance=1f;
    
    [Header("Rain Manager")]
    public PostProcessVolume postB;
    public ParticleSystem rainParticles;
    private ParticleSystem.EmissionModule rainModule;
    public float rainRateOverTime;
    public int rainIncrement;
    public float rainIncrementDelay;


    private void Start() {
        rainModule = rainParticles.emission;
        txtGem.text= gems.ToString();
    }

    public void OnOffRain(bool isRain){
        StopCoroutine("RainManager");
        StopCoroutine("PostBManager");
        StartCoroutine("RainManager", isRain);
        StartCoroutine("PostBManager", isRain);
    }

    IEnumerator RainManager(bool isRain){
        switch (isRain)
        {
            case true://aumenta a chuva
            for (float r = rainModule.rateOverTime.constant; r < rainRateOverTime; r += rainIncrement)
            {
                rainModule.rateOverTime = r;
                yield return new WaitForSeconds(rainIncrementDelay);
            }

            rainModule.rateOverTime = rainRateOverTime;
            break;

            case false://diminui a chuva
            for (float r = rainModule.rateOverTime.constant; r > 0; r -= rainIncrement)
            {
                rainModule.rateOverTime = r;
                yield return new WaitForSeconds(rainIncrementDelay);
            }

            rainModule.rateOverTime = 0;
            break;
        }

    }

    IEnumerator PostBManager(bool isRain){
         switch (isRain)
        {
            case true://aumenta a chuva
            for (float w= postB.weight; w<1; w+= 1 * Time.deltaTime )
            {
                postB.weight = w;
                yield return new WaitForEndOfFrame();
            }
             postB.weight=1;
            
            break;

            case false://diminui a chuva
            for (float w= postB.weight; w> 0; w-= 1 * Time.deltaTime )
            {
                postB.weight = w;
                yield return new WaitForEndOfFrame();
            }
             postB.weight=0;

            
            break;
        }
    }

    public void ChangeGameState(GameState newState){
        gameState=newState;

    }

    public void SetGem( int amount){
       gems+=amount;
        txtGem.text= gems.ToString();
    }

    public bool Perc(int p){
        int temp= Random.Range(0,100);
        bool retorno=temp <=p?true :false;   
        return retorno;

    }
}
