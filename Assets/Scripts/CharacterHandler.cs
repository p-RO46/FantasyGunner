using UnityEngine;
using TMPro;

public class CharacterHandler : MonoBehaviour
{
    public CharacterData currentCharacterData;
    public TextMeshProUGUI hpText;
    public float curHP;
    public bool isAlive = true;
    private bool HPchanged = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        curHP = currentCharacterData.maxHP;
        hpText.text = curHP.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(HPchanged)
        {
            updateHP();
            if (curHP == 0)
            {
                isAlive = false; 
            }
        }
    }

    public void attacked(float value)
    {
        curHP -= value;
        HPchanged = true;
    }

    private void updateHP()
    {
        if(curHP  >= 0)
        {
            hpText.text = curHP.ToString();
            HPchanged = false;
        }
    }
}
