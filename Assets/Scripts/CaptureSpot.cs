using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CaptureSpot : MonoBehaviour
{
    private PlayerInventory pi;

    private MonsterManager monsterManager;
    private GameObject Player;
    private GameObject GameUI;
    private GameObject ActivateUI;

    private AudioSource audioSource;
    public AudioClip loop;
    public AudioClip clip;

    private Transform _Player;
    public float range = 10f;

    public bool canInteract = false;
    public bool activated = false;

    private bool alreadyDetected = false;

    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        _Player = Player.transform;

        GameUI = GameObject.Find("GameUIParent").transform.GetChild(0).gameObject;
        Transform _ActivateUI = GameUI.transform.Find("Activate UI");
        ActivateUI = _ActivateUI.gameObject;

        //ActivateUI.SetActive(false);

        GameObject managerObject = GameObject.FindWithTag("MonsterManager");
        monsterManager = managerObject.GetComponent<MonsterManager>();

        audioSource = GetComponent<AudioSource>();

        pi = GameObject.Find("GameManager").transform.GetChild(0).gameObject.GetComponent<PlayerInventory>();
    }

    public void Interactable(bool active)
    {
        if (alreadyDetected == active) { return; }
        if (activated) { return; }

        ActivateUI.SetActive(active);
        alreadyDetected = active;

        if (active)
        {
            if (pi.ExistKey())
            {
                canInteract = true;
            }
            else
            {
                canInteract = false;
            }
        }

        //audioSource.PlayOneShot(loop);
        //Debug.Log(gameObject.name + " UI 상태: " + active);
    }

    public void ActivateSpot(InputAction.CallbackContext context)
    {
        if(!canInteract) 
        {
            Debug.Log("상호작용 불가");
            return;
        }

        if (context.started)
        {
            pi.UseKey();

            Debug.Log("상호작용 완료!");
            GetComponentInParent<Animator>().SetBool("Activate", true);
            activated = true;
            canInteract = false;
            ActivateUI.SetActive(false);

            audioSource.Stop();
            audioSource.spatialBlend = 0f;
            audioSource.PlayOneShot(clip);

            StartCoroutine(StopSound());

            monsterManager.AddActivation();

            // 활성화되는 코드
            // 지도 해금
            // 포인트 지급
        }
    }

    public void PlaySound()
    {
        audioSource.Play();
    }

    IEnumerator StopSound()
    {
        yield return Util.WaitForSecond(2f);

        audioSource.enabled = false;
    }
}
