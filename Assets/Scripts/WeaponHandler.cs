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
    public float MaxDistance; //Ray�� ��Ÿ�
    public GameObject fireParticle; // �Ҵ��� ��ƼŬ ������

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
        //1.ȭ���� ���߾� ��ǥ ���� (Screen Space)
        //Screen.width / 2, Screen.height / 2�� ȭ���� ���߾�
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        //2.ȭ�� ��ǥ�� Ray�� ��ȯ
        //ī�޶󿡼� ȭ�� �߾��� ����ϴ� ��(Ray)�� ����
        Ray ray = cam.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        //3.Ray �߻� (���� �浹 üũ)
        if(Physics.Raycast(ray,out hit, MaxDistance))
        {
            GameObject hitObject = hit.collider.gameObject;
            GameObject effect = Instantiate(fireParticle, hit.point, Quaternion.LookRotation(hit.normal));
            
            //Debug.Log($"���� ���: {hitObject.name}");
            //Debug.Log($"���� ���� ��ǥ: {hit.point}");

            if (hitObject.CompareTag("Enemy"))
            {
                hitObject.GetComponent<CharacterHandler>().attacked(currentWeaponData.damage);
            }
            Destroy(effect, 2.0f);
        }

        //�ð������� Ȯ���ϱ� ���� ����� �� (Scene �信���� ����)
        Debug.DrawRay(ray.origin, ray.direction * MaxDistance, Color.red, 1.0f);
    }
}
