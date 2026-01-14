using System.Collections;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int maxHp = 1000;
    [SerializeField] private int curHp;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float crouchSpeed = 3f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.8f;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 velocity;

    public Slider hpSlider;

    private Animator playerAnimator;
    private InputAction moveAction;
    private InputAction runAction;
    private InputAction crouchAction;
    private InputActionMap gp;

    public InputActionAsset ipa;

    private CameraController cam;
    //private GunSetting weapon;

    private bool crouch = false;

    private FootstepSound fs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        gp = ipa.FindActionMap("GamePlay");

        moveAction = gp.FindAction("Move");
        runAction = gp.FindAction("Run");
        crouchAction = gp.FindAction("Crouch");

        moveAction.started += OnMove;
        moveAction.canceled += StopMove;
    }

    void Start()
    {
        cam = GetComponent<CameraController>();
        fs = GetComponentInChildren<FootstepSound>(); //발소리 전용

        playerAnimator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        curHp = maxHp;
        hpSlider.value = 1;
    }

    public void changeHP(int damage)
    {
        hpSlider.value = (float)(curHp - damage) / (float)maxHp;
        curHp -= damage;
    }
    private void OnMove(InputAction.CallbackContext context)
    {
        fs.isMoving = true;
        moveInput = context.ReadValue<Vector2>();
        //Debug.Log($"Move Input: {moveInput}");
    }

    private void StopMove(InputAction.CallbackContext context)
    {
        fs.isMoving = false;
        //playerAnimator.SetBool("isMove", false);
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            cam.CameraDown(crouch);
            crouch = !crouch;
            fs.isCrouching = crouch;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            fs.isRunning = false;
            fs.isJumping = true;
        }
        if (crouch) return;
        //Debug.Log($"Jumping {context.performed} - Is Grounded: {controller.isGrounded}");
        if(context.performed && controller.isGrounded)
        {
            //Debug.Log("We are supposed to jump");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(curHp == 0)
        {
            // 게임 오버
        }

        if (crouch)
        {
            //Debug.Log("웅크리는 중...");
            fs.isRunning = false;
            fs.isCrouching = true;
            speed = crouchSpeed;
        }
        else if (runAction.IsPressed())
        {
            if (!crouch)
            {
                speed = runSpeed;
                fs.isRunning = true;
            }
            //Debug.Log("뛰는 중...");
        }
        else
        {
            fs.isRunning = false;
            speed = walkSpeed;
        }
        //Vector3 inputDir = new Vector3(moveInput.x, 0, moveInput.y);
        Vector3 move = (transform.right * moveInput.x) + (transform.forward * moveInput.y);
        
        controller.Move(move * speed * Time.deltaTime);

        ApplyGravity();
    }

    void ApplyGravity()
    {
        if(controller.isGrounded && velocity.y < 0) 
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

}
