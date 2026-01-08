using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform playerCam;
    public float minXLook = -85f; //최소각
    public float maxXLook = 85f; //최대각
    private float camCurXRot = 0f;  
    public float lookSensitivity = 3f;  

    private Vector2 mouseDelta;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        CameraLook();
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
        playerCam.localEulerAngles = new Vector3(camCurXRot, 0, 0); 

        transform.Rotate(Vector3.up * mouseX);
    }

    public void CameraRecoil(float recoilXRot, float recoilYRot)
    {
        // 수직 반동
        camCurXRot -= recoilXRot;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        playerCam.localEulerAngles = new Vector3(camCurXRot, 0, 0); 

        // 수평 반동
        transform.Rotate(Vector3.up * recoilYRot);
    }


}
