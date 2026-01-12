using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int maxHp = 1000;
    [SerializeField] private int curHp;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.8f;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 velocity;

    public Slider hpSlider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        curHp = maxHp;
        hpSlider.value = 1;
    }

    public void changeHP(int damage)
    {
        hpSlider.value = (float)(curHp - damage) / (float)maxHp;
        curHp -= damage;
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        //Debug.Log($"Move Input: {moveInput}");
    }

    public void OnJump(InputAction.CallbackContext context)
    {
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
