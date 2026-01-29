using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class WeaponHandler : MonoBehaviour
{
    private PlayerInventory pi;
    private InventoryManager im;

    //====지금까지 쏜 총알 수====//
    private int Shooted = 0;
    //=========================//

    public GameObject weapon;
    public Weapon currentWeapon;
    private WeaponData currentWeaponData;
    public Camera cam;
    public GameObject fireParticle; 
    public TextMeshProUGUI gunName;
    public TextMeshProUGUI remainAmmo;
    public TextMeshProUGUI curAmmo;

    private float MaxDistance; // WeaponData 참조하는 것으로 교체

    public bool isReloading = false;
    public bool isReloadingbySwaping = false;
    public Coroutine reloadCoroutine;

    public InputActionAsset ipa;
    public Coroutine shootCoroutine;
    private InputAction shootAction;
    public bool isShooting = false;

    public AudioClip shootSound; //임시, GunData에서 가져오는걸로 바꾸기
    public AudioClip reloadSound;
    public AudioClip equipSound;

    private AudioSource audioSource;

    void Awake()
    {
        pi = GameObject.Find("GameManager").transform.GetChild(0).gameObject.GetComponent<PlayerInventory>();
        im = GetComponent<InventoryManager>();
        shootAction = ipa.FindActionMap("GamePlay").FindAction("Shoot");
        audioSource = GetComponent<AudioSource>();
    }
    void OnEnable()
    {
        shootAction.started += StartFiring;
        shootAction.canceled += StopFiring;
        shootAction.Enable();
    }
    void OnDisable()
    {
        shootAction.started -= StartFiring;
        shootAction.canceled -= StopFiring;
        shootAction.Disable();
    }

    private void StartFiring(InputAction.CallbackContext context)
    {
        if (currentWeapon == null)
        {
            Debug.Log("장착된 무기가 없습니다.");
            return;
        }

        if (!isReloading && !im.isSwapping)
        {
            if (!currentWeapon.hasBullet() || currentWeapon.loadedBullet < 1)
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
        audioSource.PlayOneShot(shootSound);

        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        //Vector3 barrelRotation = cam.transform.forward + WeaponShake();
        //Ray ray = new Ray(cam.ScreenToWorldPoint(screenCenter), barrelRotation.normalized);
        Ray ray = cam.ScreenPointToRay(screenCenter);
        ray.direction += WeaponShake();
        RaycastHit hit;

        int layerMask = ~(1 << LayerMask.NameToLayer("Water")) & ~(1 << LayerMask.NameToLayer("CSdetect"));
        
        if (Physics.Raycast(ray, out hit, MaxDistance, layerMask))
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

        if (currentWeapon.remainBullet < toLoad)
        {
            currentWeapon.loadedBullet += currentWeapon.remainBullet;
            pi.UseAmmos(currentWeapon.weaponData.gunName, currentWeapon.remainBullet);
            currentWeapon.remainBullet = 0; // 다 썼음
        }
        else
        {
            currentWeapon.loadedBullet += toLoad;
            pi.UseAmmos(currentWeapon.weaponData.gunName, toLoad);
            currentWeapon.remainBullet -= toLoad;
        }
        UpdateRemainAmmos();
        UpdateCurrentAmmos();
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
                audioSource.PlayOneShot(reloadSound);
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
            //currentWeapon.hasBullet = false;
            Debug.Log(currentWeaponData.gunName + ":재장전불가.남은탄환없음");
            yield return null;
        }
        isReloadingbySwaping = false;
    }

    public void EquipGun(Weapon w)
    {
        if (w == null)
        {
            Debug.LogError("전달된 무기(w)가 null입니다!");
            return;
        }
        if (audioSource == null)
        {
            Debug.LogError("audioSource가 할당되지 않았습니다!");
            return;
        }

        audioSource.PlayOneShot(equipSound);

        currentWeapon = w;
        currentWeaponData = currentWeapon.weaponData;
        MaxDistance = currentWeaponData.shootRange;

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

    public void UpdateRemainAmmos()
    {
        if(remainAmmo.IsActive())
        {
            remainAmmo.text = currentWeapon.remainBullet.ToString();
        }
    }
    public void UpdateCurrentAmmos()
    {
        if(curAmmo.IsActive())
        {
            curAmmo.text = currentWeapon.loadedBullet.ToString();
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