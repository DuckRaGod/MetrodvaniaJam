using UnityEngine;

public class Player : MonoBehaviour{
    [Header("Jump")]
    public float thrust;
    public float cayoteLength;
    public float bufferLength;
    public float  maxJumpLength;
    float cayoteTime;
    float bufferTime;

    [Space(5)]
    public float moveSpeed;
    public float groundDisCheck;
    public float fallGravity;

    [Space(5)]
    public LayerMask groundMask;
    [Space(5)]
    public Vector2 groundBoxSize;

    [HideInInspector] public Rigidbody2D rb;
    float xInput => Input.GetAxisRaw("Horizontal");

    void Awake(){
        rb = GetComponent<Rigidbody2D>();
    }
    
    void RotatePlayer(){
        if(xInput > 0 && transform.eulerAngles.y != 0) transform.eulerAngles = new Vector3(0,0,0); 
        else if(xInput < 0 && transform.eulerAngles.y != 180) transform.eulerAngles = new Vector3(0,180,0); 
    }
    
    void GroundCheck(){
        var groundColl = Physics2D.OverlapBox(transform.position - new Vector3(0,groundDisCheck,0), groundBoxSize, 0, groundMask);
        if(groundColl){
            cayoteTime = cayoteLength;
            return;
        }     
        if(cayoteTime > 0) cayoteTime -= Time.deltaTime;
    }   

    void Jump(){
        if(bufferTime > 0) bufferTime -= Time.deltaTime;
        if(Input.GetButtonDown("Jump")){
            bufferTime = bufferLength;
        }
        if(bufferTime > 0 && cayoteTime > 0){
            rb.AddForce(transform.up * thrust, ForceMode2D.Impulse);
            bufferTime = 0;
            cayoteTime = 0;
        }
    }

    void GravityHandler(){
        if(rb.velocity.y < 0) rb.gravityScale = fallGravity;
        else rb.gravityScale = 1;
    }

    void Update(){
        Jump();
    }

    void FixedUpdate(){
        RotatePlayer();
        GroundCheck();
        GravityHandler();
        rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
    }
    
    void OnDrawGizmosSelected(){
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position - new Vector3(0,groundDisCheck,0), groundBoxSize);
    }
}