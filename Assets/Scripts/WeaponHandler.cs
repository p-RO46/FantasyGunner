using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class WeaponHandler : MonoBehaviour
{
    public WeaponData currentWeaponData;
    public TextMeshProUGUI gunName;
    public TextMeshProUGUI maxAmmo;
    public TextMeshProUGUI curAmmo;

    public int curBullet;

    public Camera cam;
    public float MaxDistance; //Ray의 사거리
    public GameObject fireParticle; // 할당할 파티클 프리팹

    void Start()
    {
        curBullet = currentWeaponData.maxAmmo;
        maxAmmo.text = currentWeaponData.maxAmmo.ToString();
        curAmmo.text = currentWeaponData.maxAmmo.ToString();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ShootRay();
            curBullet--;
            curAmmo.text = curBullet.ToString();
        }
    }

    void ShootRay()
    {
        //1.화면의 정중앙 좌표 설정 (Screen Space)
        //Screen.width / 2, Screen.height / 2가 화면의 정중앙
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        //2.화면 좌표를 Ray로 변환
        //카메라에서 화면 중앙을 통과하는 선(Ray)를 생성
        Ray ray = cam.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        //3.Ray 발사 (물리 충돌 체크)
        if(Physics.Raycast(ray,out hit, MaxDistance))
        {
            GameObject hitObject = hit.collider.gameObject;
            GameObject effect = Instantiate(fireParticle, hit.point, Quaternion.LookRotation(hit.normal));
            
            Debug.Log($"적중 대상: {hitObject.name}");
            Debug.Log($"적중 지점 좌표: {hit.point}");

            if (hitObject.CompareTag("Enemy"))
            {
                hitObject.GetComponent<CharacterHandler>().attacked(currentWeaponData.damage);
            }
            Destroy(effect, 2.0f);
        }

        //시각적으로 확인하기 위한 디버그 선 (Scene 뷰에서만 보임)
        Debug.DrawRay(ray.origin, ray.direction * MaxDistance, Color.red, 1.0f);
    }
}
