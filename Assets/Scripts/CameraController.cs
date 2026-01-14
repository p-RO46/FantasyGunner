using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform playerCam;
    public float minXLook = -85f; //최소각
    public float maxXLook = 85f; //최대각
    private float camCurXRot = 0f;
    private float playerCurYRot = 0f; 
    public float lookSensitivity = 3f;

    private Vector3 camOrigin;

    private Vector2 mouseDelta;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camOrigin = playerCam.localPosition;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        CameraLook();
        playerCam.localEulerAngles = new Vector3(camCurXRot, 0, 0); 
        transform.localEulerAngles = new Vector3(0, playerCurYRot, 0);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    void CameraLook()
    {
        float mouseX = mouseDelta.x * lookSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * lookSensitivity * Time.deltaTime;

        camCurXRot -= mouseY;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        //playerCam.localEulerAngles = new Vector3(camCurXRot, 0, 0); 
        
        playerCurYRot += mouseX;
        //transform.Rotate(Vector3.up * mouseX);
    }

    public void CameraRecoil(float recoilXRot, float recoilYRot)
    {
        // 수직 반동
        camCurXRot -= recoilXRot;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        //playerCam.localEulerAngles = new Vector3(camCurXRot, 0, 0); 

        playerCurYRot += recoilYRot;
        //playerCurYRot = Mathf.Clamp(playerCurYRot);
        // 수평 반동
        //transform.Rotate(Vector3.up * recoilYRot);
    }

    public void CameraDown(bool crouch)
    {
        if (crouch)
        {
            playerCam.localPosition += new Vector3(0, 1, 0);
        }
        else
        {
            playerCam.localPosition += new Vector3(0, -1, 0);
        }
    }
}
