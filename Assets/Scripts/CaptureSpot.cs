using UnityEngine.InputSystem;
using UnityEngine;

public class CaptureSpot : MonoBehaviour
{
    public MonsterManager monsterManager;
    private GameObject Player;
    private Transform _Player;
    public float range = 10f;

    private bool canInteract = false;
    public bool activated = false;
    void Start()
    {
        Player = GameObject.FindWithTag("Player");

        GameObject managerObject = GameObject.FindWithTag("MonsterManager");
        monsterManager = managerObject.GetComponent<MonsterManager>();
    }

    void Update()
    {
        _Player= Player.GetComponent<Transform>();
        if (!activated)
        {
            //조건 : 카메라에 잡혔을 때 추가
            if((_Player.position - transform.position).sqrMagnitude < range * range)
            {
                canInteract = true;
            }
            else { canInteract = false; }
        }
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
            Debug.Log("상호작용 완료!");
            GetComponentInParent<Animator>().SetBool("Activate", true);
            activated = true;
            canInteract = false; 
            
            monsterManager.AddActivation();
            // 활성화되는 코드
            // 지도 해금
            // 포인트 지급
        }
    }
}
