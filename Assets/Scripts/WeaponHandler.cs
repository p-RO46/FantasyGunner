using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class WeaponHandler : MonoBehaviour
{
    //====지금까지 쏜 총알 수====//
    private int Shooted = 0;
    //=========================//

    public GameObject weapon;
    public Weapon currentWeapon;
    private WeaponData currentWeaponData;
    public Camera cam;
    public float MaxDistance;
    public GameObject fireParticle; 
    public TextMeshProUGUI gunName;
    public TextMeshProUGUI remainAmmo;
    public TextMeshProUGUI curAmmo;

    public bool isReloading = false;
    public bool isReloadingbySwaping = false;
    public Coroutine reloadCoroutine;

    public InputActionAsset ipa;
    public Coroutine shootCoroutine;
    private InputAction shootAction;
    public bool isShooting = false;

    void Awake()
    {
        shootAction = ipa.FindActionMap("GamePlay").FindAction("Shoot");
        shootAction.started += StartFiring;
        shootAction.canceled += StopFiring;
    }

    private void StartFiring(InputAction.CallbackContext context)
    {
        if (!isReloading && !GetComponent<InventoryManager>().isSwapping)
        {
            if (!currentWeapon.hasBullet || currentWeapon.loadedBullet < 1)
            {
                Debug.Log(currentWeaponData.gunName + ":발사불가.남은탄환없음");
                return;
            }

            if (!currentWeaponData.canContinuousFire) //단발
            {
                ShootRay();
                Shooted = 0;
            }
            else //연사
            {
                isShooting = true;
                shootCoroutine = StartCoroutine(AutoShooting());
            }
        }
    }
    private void StopFiring(InputAction.CallbackContext context)
    {
        isShooting = false;
        Shooted = 0;
    }

    private void ShootRay()
    {
        weapon.GetComponent<Animator>().SetTrigger("Fire");

        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        //Vector3 barrelRotation = cam.transform.forward + WeaponShake();
        //Ray ray = new Ray(cam.ScreenToWorldPoint(screenCenter), barrelRotation.normalized);
        Ray ray = cam.ScreenPointToRay(screenCenter);
        ray.direction += WeaponShake();
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, MaxDistance))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (!hitObject.CompareTag("Player"))
            {
                GameObject effect = Instantiate(fireParticle, hit.point, Quaternion.LookRotation(hit.normal));

                if (hitObject.CompareTag("Enemy"))
                {
                    hitObject.GetComponent<Monster>().Attacked(
                        currentWeaponData.damage, currentWeaponData.hasStun, currentWeaponData.stunTime);
                }

                Destroy(effect, 2.0f);
            }
        }

        currentWeapon.loadedBullet = currentWeapon.loadedBullet - 1;
        if(currentWeapon.loadedBullet == 0)
        {
            if(shootCoroutine != null) 
            {
                isShooting = false;
            }
            reloadCoroutine = StartCoroutine(WaitForEndOfReload());
        }
        curAmmo.text = currentWeapon.loadedBullet.ToString();

        GunRecoil();
        Shooted ++;
        Debug.DrawRay(ray.origin, ray.direction * MaxDistance, Color.red, 1.0f);
    }

    private void OnEnable()
    {
        shootAction.Enable(); // Enable the action when the script is enabled
    }

    private void OnDisable()
    {
        shootAction.Disable(); // Disable the action when the script is disabled
        shootAction.started -= StartFiring;
        shootAction.canceled -= StopFiring;
    }

    private IEnumerator AutoShooting()
    {
        while(true)
        {
            if (!isShooting)
            {
                yield break;
            }
            ShootRay();
            yield return Util.WaitForSecond(currentWeaponData.fireRate);
        }
    }

    private void GunReload()
    {
        int toLoad = currentWeaponData.loadAmmo - currentWeapon.loadedBullet;
        if(toLoad == 0)
        {
            Debug.Log("이미 탄환이 가득찬 상태입니다.");
            return;
        }
        if(currentWeapon.remainBullet < toLoad )
        {
            currentWeapon.loadedBullet += currentWeapon.remainBullet;
            currentWeapon.remainBullet = 0;
        }
        else
        {
            currentWeapon.loadedBullet += toLoad;
            currentWeapon.remainBullet -= toLoad;
        }
        remainAmmo.text = currentWeapon.remainBullet.ToString();
        curAmmo.text = currentWeapon.loadedBullet.ToString();
    }

    public void GunReload(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (GetComponent<InventoryManager>().isSwapping)
            {
                Debug.Log("총 교체중엔 장전 불가입니다.");
                return;
            }
            reloadCoroutine = StartCoroutine(WaitForEndOfReload());
        }
    }

    private IEnumerator WaitForEndOfReload()
    {
        if (GetComponent<InventoryManager>().isSwapping)
        {
            Debug.Log("총 교체중엔 장전 불가입니다.");
            yield return null;
        }
        else if (isShooting)
        {
            Debug.Log("총 발사중엔 장전 불가입니다.");
            yield return null;
        }
        else if(currentWeapon.remainBullet > 0)
        {
            if(!isReloading || isReloadingbySwaping)
            {
                Debug.Log("장전 시작합니다..");
                isReloading = true;
                weapon.GetComponent<Animator>().SetTrigger("Reload");
                yield return Util.WaitForSecond(currentWeaponData.reloadTime);
                if (isReloading)
                {
                    GunReload();
                    Debug.Log("장전된 탄환 수:" + currentWeapon.loadedBullet);
                    Debug.Log("장전 끝");
                }
                isReloading = false;
                //gunAnimator.SetBool("OutOfAmmo", false);
            }
            else
            {
                Debug.Log(currentWeaponData.gunName + ":재장전불가. 이미 재장전 중");
                yield return null;
            }
        }
        else
        {
            currentWeapon.hasBullet = false;
            Debug.Log(currentWeaponData.gunName + ":재장전불가.남은탄환없음");
            yield return null;
        }
        isReloadingbySwaping = false;
    }

    public void EquipGun(Weapon w)
    {
        currentWeapon = w;
        currentWeaponData = currentWeapon.weaponData;
        gunName.text = currentWeaponData.gunName;
        remainAmmo.text = currentWeapon.remainBullet.ToString();
        curAmmo.text = currentWeapon.loadedBullet.ToString();
        if(currentWeapon.loadedBullet == 0 && currentWeapon.remainBullet > 0)
        {
            GetComponent<InventoryManager>().isSwapping = false;
            isReloadingbySwaping = true;
            Debug.Log("총 교체 취소, 장전 시작합니다.");
            reloadCoroutine = StartCoroutine(WaitForEndOfReload());
        }
    }

    #region 총기 흔들림
    // 정확도에 의한 차이
    private Vector3 WeaponShake()
    {
        float accuracy = Mathf.Clamp01((99f - currentWeaponData.accuracy) / 98f);
        float shakeAmount = accuracy * 7.5f;
        float shakeSpeed = 0.7f;

        float rotX = (Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f) * shakeAmount * 1f;
        float rotY = (Mathf.PerlinNoise(0f, Time.time * shakeSpeed) - 0.5f) * shakeAmount * 3f;
        float rotZ = (Mathf.PerlinNoise(Time.time * shakeSpeed, Time.time * shakeSpeed) - 0.5f) * shakeAmount;
        
        Vector3 shakeRotation = new Vector3(rotX, rotY, rotZ);
        return shakeRotation;
    }
    
    // 반동에 의한 카메라 이동
    // 총기 자체 흔들림
    // 자연스럽게 움직이려면??
    public void GunRecoil()
    {
        float recoilXRot, recoilYRot;
        //Debug.Log("Shooted : " + Shooted.ToString());
        if(Shooted < 20) //WeaponData에 범위 추가하기
        {
            recoilXRot = currentWeaponData.verticalForce * 0.01f; // 수직 반동
            recoilYRot = 0;
        }
        else
        {
            recoilXRot = 0;

            float rotX = Mathf.PerlinNoise(Time.time, 0f) - 0.5f;
            float rotY = (Mathf.PerlinNoise(0f, Time.time) - 0.5f) * currentWeaponData.horizontalForce * 3f;
            float rotZ = Mathf.PerlinNoise(Time.time, Time.time - 0.5f);

            //recoilYRot = new Vector3(rotX, rotY, rotZ);
            recoilYRot = currentWeaponData.horizontalForce * 0.01f; // 수평 반동
        }

        GetComponent<CameraController>().CameraRecoil(recoilXRot, recoilYRot);
    }
    #endregion
}