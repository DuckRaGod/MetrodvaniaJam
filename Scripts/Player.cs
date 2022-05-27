using UnityEngine;

public class Player : MonoBehaviour{
    public PlayerStats player;
    bool isDashable = false;
    bool isWallJumpable = false;
    bool isRollable = true;

    bool onWall;
    public bool onGround { get; private set; }
    public bool isDash { get; private set; }
    public bool isWallSlide { get; private set; }
    public bool isRoll { get; private set; }

    Rigidbody2D rb;
    BoxCollider2D bc;

    const float thrust = 8;
    const float dashThrust = 10;
    const float maxBufferLength   = .1f;
    const float maxCayoteLength   = .2f;
    const float jumpColdownLength = .4f;
    const float dashFrezze        = .6f;
    const float headPos           = .8f;

    //  Wall
    const float wallCheckDis      = .4f;
    const float wallJumpFrezze    = .2f ;
    const float slideSpeed        = .5f;
    const float maxSlideLength    = 1f;

    //  Movement
    const float moveSpeed         = 9;
    const float moveAcceleration  = 13;
    const float moveDecceleration = 13;
    const float velPower          = .96f;

    const float frictionAmount    = .96f;

    float bufferTime;
    float cayoteTime;
    float nextPosibleJump;
    float frezzeInputTime;
    float slideTime;
    float horizontalInput => Input.GetAxisRaw("Horizontal");
    float verticalInput => Input.GetAxisRaw("Vertical");

    Vector2 groundCheckPos = new Vector2(0, -.5f);
    Vector2 groundCheckSize = new Vector2(.46f, .01f);
    Vector2 wallCheckSize = new Vector2(.3f, .85f);
    Vector2 rollSize = new Vector2(.75f, .5f);
    Vector2 standSize = new Vector2(.5f, 1);

    public LayerMask groundMask;

    const int maxAirJump = 0;
    int avalibaleAirJump;
    const int maxDash = 0;
    int avalibaleDash;

    void Awake(){
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
    }

    void Update(){
        if(frezzeInputTime > 0) {frezzeInputTime -= Time.deltaTime; return;}        // frezze player input
        InputHandler();
        RollHandler();
        if(isRoll) return;
        JumpHandler();
        WallJumpHandler();
        DashHandler();
    }

    void FixedUpdate(){
        MovementHandler();
        ColisionHandler();
        RotatePlayer();
        FrictionHandler();
        WallSlideHandler();
    }

    void MovementHandler(){
        if(frezzeInputTime > 0) return;
        var targetSpeed = horizontalInput * moveSpeed;
        var speedDif = targetSpeed - rb.velocity.x;
        var accelRate = (Mathf.Abs(targetSpeed)>.01f) ? moveAcceleration : moveDecceleration;
        var movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
        rb.AddForce(movement * Vector2.right);
    }

    void FrictionHandler(){
        if(Mathf.Abs(horizontalInput) <= .1f && !onGround || frezzeInputTime > 0) return;
        var amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(frictionAmount));
        amount *= Mathf.Sign(rb.velocity.x);
        rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
    }

    void RotatePlayer(){
        if(rb.velocity.x > 1f && transform.eulerAngles != new Vector3(0,0,0)) transform.eulerAngles = new Vector3(0,0,0);
        else if(rb.velocity.x < -1f && transform.eulerAngles != new Vector3(0,180,0)) transform.eulerAngles = new Vector3(0,180,0);
    }

    void InputHandler(){
        if(Input.GetButtonDown("Jump")) bufferTime = maxBufferLength;
        else if(Input.GetButtonUp("Jump") && rb.velocity.y > 0) rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * .5f);
        else if(bufferTime > 0) bufferTime -= Time.deltaTime;
    }

    void JumpHandler(){
        if(nextPosibleJump > Time.time) cayoteTime = 0;
        if(bufferTime > 0 && cayoteTime > 0 || rb.velocity.y <= 0 && cayoteTime <= 0 && bufferTime > 0 && avalibaleAirJump > 0){
            avalibaleAirJump--;
            nextPosibleJump = Time.time + jumpColdownLength;
            bufferTime = 0;
            cayoteTime = 0;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * thrust, ForceMode2D.Impulse);
        }
    }

    void WallJumpHandler(){
        if(!isWallJumpable || !onWall || cayoteTime > 0) return;
        if(!Input.GetButtonDown("Jump")) return;
        frezzeInputTime = wallJumpFrezze;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(-transform.right.x * moveSpeed , thrust), ForceMode2D.Impulse);
    }

    void WallSlideHandler(){
        if(isRoll){
            if(isWallSlide) isWallSlide = false;
            return;
        }
        if((slideTime <= 0 || !onWall || rb.velocity.y > 0) && isWallSlide) isWallSlide = false; 
        if(!onWall || slideTime <= 0 || rb.velocity.y > 0) return;
        slideTime -= Time.deltaTime;
        isWallSlide = true;
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * slideSpeed);
    }

    void DashHandler(){
        if(!isDashable) return;
        if(!Input.GetKeyDown("j")) return;
        frezzeInputTime = dashFrezze;
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(horizontalInput, verticalInput).normalized * dashThrust, ForceMode2D.Impulse);
    }

    void RollHandler(){
        if(!isRollable) return;
        if(!Input.GetKeyDown("k")) return;
        isRoll = !isRoll;
        if(!isRoll && !isStandable()) isRoll = true;
        if(isRoll) bc.size = rollSize;
        else bc.size = standSize;
    }

    bool isStandable(){
        if(Physics2D.OverlapBox(transform.position + transform.up * headPos, groundCheckSize, 0 ,groundMask)) return false;
        return true;
    }

    void ColisionHandler(){
        //  Wall Check
        if(horizontalInput != 0 && !isRoll) onWall = Physics2D.OverlapBox(transform.position + transform.right * wallCheckDis, wallCheckSize, 0, groundMask);
        else onWall = false;
        onGround = Physics2D.OverlapBox((Vector2)transform.position + groundCheckPos, groundCheckSize, 0, groundMask);
        if(onGround) {
            cayoteTime = maxCayoteLength;
            slideTime = maxSlideLength;
            avalibaleAirJump = maxAirJump;
            avalibaleDash = maxDash;
            return;
        }
        if(onWall){
            avalibaleAirJump = maxAirJump;
        }
        if(cayoteTime > 0 ) cayoteTime -= Time.deltaTime;
    }

    public void Hit(int damage){
    }

    void OnTriggerEnter2D(Collider2D coll){
        var col = coll.GetComponent<IInteractable>();
        if(col==null) return;
        col.Interact();
    }

    void OnCollisionEnter2D(Collision2D coll){
        var col = coll.transform.GetComponent<IInteractable>();
        if(col==null) return;
        col.Interact();
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)transform.position + groundCheckPos, groundCheckSize);
        Gizmos.DrawWireCube(transform.position + transform.right * wallCheckDis, wallCheckSize);
        Gizmos.DrawWireCube(transform.position + transform.up * headPos, groundCheckSize);
    }
}