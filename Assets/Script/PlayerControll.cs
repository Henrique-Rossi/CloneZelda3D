using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    private GameManager _GameManager;
    private CharacterController controller;
    private Animator anim;

    [Header("Config Player")]
    public float movementSpeed=5f;
    private Vector3 direction;
    private bool isWalk;

    [Header("Config Life")]
    public float HP=3f;


    //INPUTS
    private float horizontal;
    private float vertical;

    [Header("Config Attack")]
    public ParticleSystem fxAttack;
    public Transform hitBox;

    [Range(0.2f,1f)]
    public float hitRange=0.5f;
    public LayerMask hitmask;//o que posso atingir

    public bool isAttack;
    public Collider[] hitinfo;
    public int amountDmg; //quantidade de dano
    



//Desafios-Criar reação ao sofrer dano 
//-Particulas dos slimes ao sofrer hits

    void Start()
    {
        _GameManager=FindObjectOfType(typeof(GameManager)) as GameManager;
        controller= GetComponent<CharacterController>();
        anim=GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_GameManager.gameState != GameState.GAMEPLAY){return;}
        
       Inputs();
       MoveChar();
       UpdateAnimator();
        
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag =="TakeDamage")
        {
            GetHit(1);//dano recebido
        }
    }

#region Meus Metodos

        void Inputs()
        {
            //controles para movimentar o player
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
            
            //se apertar mause esquerdo faça
            /*if (Input.GetButtonDown("Fire1") && isAttack ==false)
            {
                Attack();
            }*/
            
            if (Input.GetKeyDown("z") && isAttack ==false)
            {
                Attack();
            }
             
        }


        void Attack()
        {           
                isAttack=true;
                anim.SetTrigger("Attack");//ativa animação
                fxAttack.Emit(1);   //ativa animação de particuals da espada

                hitinfo = Physics.OverlapSphere(hitBox.position,hitRange,hitmask);//registra no array(lista) o que foi atigindo

                foreach (Collider c in hitinfo)
                {
                    c.gameObject.SendMessage("GetHit",amountDmg,SendMessageOptions.DontRequireReceiver);
                }
        }
        void AttackIsDone(){
            isAttack=false;//se o o atck foi realizado,espere um irtevalo para atacar novamente
        }

        void GetHit(float amount){
            HP -= amount;
            Debug.Log(HP);
            if (HP>0)
            {
                
                anim.SetTrigger("Hit");
            }else
            {
                _GameManager.ChangeGameState(GameState.DIE);
                 anim.SetTrigger("Die");
            }
        }

        //MÉTODO MOVIMENTAÇÃO DA CAMERA
        void MoveChar()
        {
            direction = new Vector3(horizontal,0f,vertical).normalized;//normalized ajsuta a velocidade ,pois dependendo do angulo ele fica mais rpaido
            if (direction.magnitude > 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x,direction.z) * Mathf.Rad2Deg;
                transform.rotation=Quaternion.Euler(0,targetAngle,0);
                isWalk=true;
            }else//<
            {
                isWalk=false;
            }
            controller.Move(direction* movementSpeed * Time.deltaTime); //utilizasse o time.deltatime para equuilibrar a taxa de fps,para que um pc melhjor fique maisrapido o personagem
        }


        void UpdateAnimator()
        {
            anim.SetBool("isWalk",isWalk);
        }


#endregion
    
    private void OnDrawGizmosSelected() {
        if (hitBox != null)
        {
            Gizmos.color=Color.red;   
            Gizmos.DrawWireSphere(hitBox.position,hitRange);//cria um gizmo pra simular onde esta o objto
    
        }
     }
}
