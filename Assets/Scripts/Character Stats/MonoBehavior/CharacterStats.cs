using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    public CharacterData_SO templateData;
    
    public CharacterData_SO characterData;
    public AttackData_SO attackData;

    
    [HideInInspector]
    public bool isCritical;
    
    #region Read from Data_SO

    private void Awake()
    {
        if (templateData != null)
            characterData = Instantiate(templateData);
    }

    public int MaxHealth
    {
        get
        {
            if (characterData != null)
            {
                return characterData.maxHealth;
            }
            else
            {
                return 0;
            }
        }
        set { characterData.maxHealth = value; }
    }

    public int CurrentHealth
    {
        get
        {
            if (characterData != null)
            {
                return characterData.currentHealth;
            }
            else
            {
                return 0;
            }
        }
        set { characterData.currentHealth = value; }
    }

    public int BaseDefence
    {
        get
        {
            if (characterData != null)
            {
                return characterData.baseDefence;
            }
            else
            {
                return 0;
            }
        }
        set { characterData.baseDefence = value; }
    }

    public int CurrentDefence
    {
        get
        {
            if (characterData != null)
            {
                return characterData.currentDefence;
            }
            else
            {
                return 0;
            }
        }
        set { characterData.currentDefence = value; }
    }

    #endregion

    #region Character Combat

    public void TakeDamage(CharacterStatus attacker,CharacterStatus defener)
    {
        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence,1);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        if (attacker.isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("Hit");
        }
        //更新UI
        //死亡
        //经验值
    }

    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamaga, attackData.maxDamage);
        if (isCritical)
        {
            coreDamage = coreDamage * attackData.critialMultiplier;
        }
        return (int)coreDamage;
    }
    #endregion
}
