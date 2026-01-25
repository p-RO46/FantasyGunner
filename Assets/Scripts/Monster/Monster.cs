using System.Collections;
using UnityEngine;
using TMPro;

public class Monster : MonoBehaviour
{
    public CharacterData monsterData;
    public TextMeshProUGUI hpText;

    private MonsterMove monsterMove; // 몬스터의 NavMeshAgent 이동과 관련한 코드
    private bool isAlive = true;
    private float maxHP;
    private float curHP;

    private void Start()
    {
        monsterMove = GetComponent<MonsterMove>();
        maxHP = monsterData.MaxHP;
        curHP = maxHP;
    }

    private void Update()
    {
        //몬스터 사망
        if (!isAlive)
        {
            monsterMove.ToDead();
        }
    }

    public void Attacked(int damage, bool hasStun, float stunTime) // 플레이어의 무기 타입 관련 매개변수
    {
        if (!isAlive)
        {
            // 몬스터가 이미 죽은 상태라면 실행하지 않는다.
            return;
        }

        curHP -= damage;
        UpdateHP(); // 몬스터 관련 UI 업데이트

        if (curHP <= 0)
        {
            curHP = 0;
            isAlive = false;
        }
        else if (hasStun) // 무기가 stun을 걸 수 있는 종류인 경우. 일반적으로는 false
        {
            monsterMove.ToStun(stunTime);
        }
        else
        {
            monsterMove.ToFollow();
        }
    }

    private void UpdateHP()
    {
        if (curHP >= 0)
        {
            hpText.text = curHP.ToString();
        }
        else
        {
            hpText.text = "0";
        }
    }

    public void MonsterDestroy()
    {
        Destroy(gameObject);
    }
}
