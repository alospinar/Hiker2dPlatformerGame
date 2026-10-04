using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpSpeed = 2f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2 (10f,20f);
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;
    
    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;

    bool isAlive = true; 
    
    float gravityScaleAtStart;
    
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!isAlive){ return; }
        Run();
        FlipSprite();
        ClimbLadder();
        Die();
    }

    void OnMove(InputValue value)
    {
        if(!isAlive){ return; }
        moveInput = value.Get<Vector2>();
        
        
    }
    void OnJump(InputValue value)
    {
        if(!isAlive){ return; }
        if(!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))){return;}
        if (value.isPressed) 
        {
            myRigidbody.linearVelocity += new Vector2(0f, jumpSpeed);
        }
    }
    void Run()
    {
        Vector2 playerVelovity = new Vector2(moveInput.x * moveSpeed, myRigidbody.linearVelocity.y);
        myRigidbody.linearVelocity = playerVelovity;
        bool hasHorizontalSpeed = Mathf.Abs(myRigidbody.linearVelocity.x)> Mathf.Epsilon;
        myAnimator.SetBool("isRunning", hasHorizontalSpeed);
    }
    void FlipSprite()
    {
        bool hasHorizontalSpeed = Mathf.Abs(myRigidbody.linearVelocity.x)> Mathf.Epsilon;
        if (hasHorizontalSpeed)
        {
            transform.localScale = new Vector2 (Mathf.Sign(myRigidbody.linearVelocity.x), 1f);
        }
    }
    void ClimbLadder()
    {
        if(!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            myRigidbody.gravityScale = gravityScaleAtStart;
            myAnimator.SetBool("isClimbing", false);
            return;
        }
        myRigidbody.gravityScale = 0f;
        Vector2 climbVelovity = new Vector2( myRigidbody.linearVelocity.x,moveInput.y *climbSpeed);
        myRigidbody.linearVelocity = climbVelovity;
        bool hasVerticalSpeed = Mathf.Abs(myRigidbody.linearVelocity.y)> Mathf.Epsilon;
        myAnimator.SetBool("isClimbing", hasVerticalSpeed);

    }

    void OnAttack(InputValue value)
    {
        if(!isAlive) { return; }
        Instantiate(bullet,gun.position, transform.rotation);
    }
    void Die()
    {
        if(myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies","Hazards")))
        {
            isAlive = false;
            myAnimator.SetTrigger("Dying");
            myRigidbody.linearVelocity = deathKick;
            FindAnyObjectByType<GameSession>().ProcessPlayerDeath();
        }
    }
}
