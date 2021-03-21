using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Slime : MonoBehaviour
{
    private GameManager _GameManager;


    public ParticleSystem fxSlime;
    private bool isHit;
    private Animator anim;

    public float hp;
    private bool isDied;

    [Header("AI")]
    public enemyState state;
    
    
    //ia
    private bool isWalk;
    private bool isAlert;
    private bool isPlayerVisible;
    public bool isAttack;
    private NavMeshAgent agent;
    private int idleWaitpoint;
    private Vector3 destination;


  
    void Start()
    {
        _GameManager=FindObjectOfType(typeof(GameManager)) as GameManager;

        anim=GetComponent<Animator>();
        agent=GetComponent<NavMeshAgent>();
        ChangeState(state);
    }

    
    void Update()
    {
        StateManager();
        //se for maior que 0.1 quer dizer que esta moviemntando
        if (agent.desiredVelocity.magnitude >= 0.1f)
        {
            isWalk=true;
        }else{
            isWalk=false;
        }
        anim.SetBool("isWalk",isWalk);
        anim.SetBool("isAlert",isAlert);
    }

    

    IEnumerator Died(){
        isDied=true;
        yield return new WaitForSeconds(2.5f);
        if (_GameManager.Perc(_GameManager.percDrop))
        {
            Instantiate(_GameManager.gemPrefab,transform.position,_GameManager.gemPrefab.transform.rotation);//instanciar a gema onde o mob morreu
        }
        Destroy(this.gameObject);
    }


    private void OnTriggerEnter(Collider other) {

        if(_GameManager.gameState != GameState.GAMEPLAY){return;}

        if (other.gameObject.tag== "Player")
        {
            isPlayerVisible=true;

            if (state == enemyState.IDLE|| state == enemyState.PATROL)
            {
                ChangeState(enemyState.ALERT);
            }
            else if(state == enemyState.FOLLOW)
            {
                StopCoroutine("FOLLOW");
                ChangeState(enemyState.FOLLOW);
            }

        }      
    }


    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player")
        {
            isPlayerVisible=false;
        }
    }


 #region MEUS METODOS 


    void GetHit(int amount){
       
        if (isHit == false)
        { 
            if (isDied ==true){return;}   

            hp -= amount;

           // transform.localScale = new Vector3(1f,1f,1f); diminuir o tamanho do slide a cada hit
           if (hp > 0)
           {
               ChangeState(enemyState.FURY);
                anim.SetTrigger("GetHit");
               
           }else
           {
                ChangeState(enemyState.DIE);
                anim.SetTrigger("Die");
                
                StartCoroutine("Died");
           }
        
           fxSlime.Emit(15);
            
        }

    }

    void StateManager(){
        if(_GameManager.gameState == GameState.DIE && (state ==enemyState.FOLLOW ||state ==enemyState.FURY ||state ==enemyState.ALERT))
        {
        ChangeState(enemyState.IDLE);
        }

        switch (state)
        {
            case enemyState.ALERT:
                 LookAt();
            break;

            //seguir player
            case enemyState.FOLLOW:
                  LookAt();
                 destination=_GameManager.player.position;//captura a posição do player
                 agent.destination=destination;
                
                 //se estiver na posição de atack faça    
                 if (agent.remainingDistance <= agent.stoppingDistance)
                 {
                    Attack();
                 }

            break;
            

            case enemyState.FURY:

                 //LookAt();
                 destination=_GameManager.player.position;//captura a posição do player
                 agent.destination=destination;//seta pra ai seguir ele

                 //se estiver na posição de atack faça    
                 if (agent.remainingDistance <= agent.stoppingDistance)
                 {
                    Attack();
                 }

            break;
        }
    }

    //mudar o estado
    void ChangeState(enemyState newState){

        StopAllCoroutines();//encerra todas as coroutines,para nao gerar comflito
         
        isAlert=false; 
       // isAttack = true;

        switch (newState)
        {
            case enemyState.IDLE:    
             
                agent.stoppingDistance=_GameManager.SlimeStopDistance;
                destination=transform.position;
                agent.destination=destination;

                StartCoroutine("IDLE");

            break;


            case enemyState.ALERT:

                agent.stoppingDistance=_GameManager.SlimeStopDistance;
                destination=transform.position;
                agent.destination=destination;
                isAlert=true;
                StartCoroutine("ALERT");
            break;


            case enemyState.PATROL:

                agent.stoppingDistance=_GameManager.SlimeStopDistance;//distancia pra poder ativar animação de ataque
                idleWaitpoint = Random.Range(0,_GameManager.slimeWayPoints.Length);//va iacessar o tamnho do array das posições e va isortear algum numero que vai representar a posiçã oem qual o slime ira seguir
                destination= _GameManager.slimeWayPoints[idleWaitpoint].position;//pra onde ir
                agent.destination=destination;

                StartCoroutine("PATROL");
                
            break;

            case enemyState.FOLLOW:
                
                agent.stoppingDistance= _GameManager.slimeDistanceAttack;
                StartCoroutine("FOLLOW");
                
            break;


            case enemyState.FURY:

                destination= transform.position;
                agent.stoppingDistance=_GameManager.slimeDistanceAttack;
                agent.destination=destination;
               
            break;

             case enemyState.DIE:

                 destination= transform.position;
                agent.destination=destination;
            break;
        }
        StartCoroutine("ATTACK");
        state=newState;
    }



//função para definir se vai ou nao ficar parado
    void StayStill(int yes)
    {
        if (Rand() <= yes)
        {
            ChangeState(enemyState.IDLE);
        }
        else
        {
            ChangeState(enemyState.PATROL);
        }
    }

    IEnumerator FOLLOW(){
         //vai verificar se o player ainda esta no campo de visao
         yield return new WaitUntil(() => !isPlayerVisible);
        //se nao ele vai chamar função pra ver se fica parado ou volta patrulhar
         yield return new WaitForSeconds(_GameManager.slimeAlertTime);
         StayStill(50);
    }

    IEnumerator IDLE()
    {
        yield return new WaitForSeconds(_GameManager.slimeIdleWaitTime);
        StayStill(50);//50% de chance de ficar parado ou entrar em patrulha
    }
     


    IEnumerator PATROL()
    {
        yield return new WaitUntil(() => agent.remainingDistance <= 0);//espere até uma condição acontecer
        StayStill(30);//30% de chance de ficar parado e 70% de ficar em patrulha
    }


    IEnumerator ALERT(){

        yield return new WaitForSeconds(_GameManager.slimeAlertTime);

        if (isPlayerVisible == true)
        {
            ChangeState(enemyState.FOLLOW);
        }else
        {
            StayStill(10);
        }
    }
    IEnumerator ATTACK(){
        yield return new WaitForSeconds(_GameManager.slimeAttackDelay);
        isAttack=false;
    }

    int Rand(){
      
        int rand=Random.Range(0,100);
        return rand;
    }


    void Attack(){

        if (isAttack==false && isPlayerVisible == true)
        {
             isAttack=true;
             anim.SetTrigger("Attack");
        }
       
    }

/*
    void AttackIsDone(){
      StartCoroutine("ATTACK");

    }*/

    void LookAt(){
        
        Vector3 lookDirection = (_GameManager.player.position = transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        
        transform.rotation= Quaternion.Slerp(transform.rotation,lookRotation,_GameManager.slimeLookAtSpeed * Time.deltaTime);
    }


 #endregion

       
/*public int slimeLife;
    public Renderer slimeRender;
if ( slimeLife > 0)
      {
        
          slimeLife-= Time.deltaTime;
          
          slimeRender.material.SetColor("_Color",new Color(cor.r,cor.g,cor.b,slimeLife));
          
      }
      if (slimeLife <= 0)
      {   
          
            Destroy(this.gameObject);
           
      }
*/

}
