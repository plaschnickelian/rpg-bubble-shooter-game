using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public Game1BattleController battleController; // Set by Spawner
    public GameObject attackWarning;
    public GameObject indicator;
    public Animator animator;

    public int index;

    public float maxHP;
    public float currentHP;
    public float shield;
    public float attackDamage;
    public int turnsToAction;
    public int speed;
    public bool stunned = false;

    protected virtual void Start()
    {
        maxHP = 2;
        currentHP = maxHP;
        shield = 0;
        attackDamage = 1;
    }

    // Neue Züge bis zum Angriff zuweisen
    public virtual void newTurnsToAction()
    {
        //Override
    }

    protected virtual void Update()
    {
        if ((currentHP <= 0)) StartCoroutine(enemyDie());
    }

    public void playRedAbilityAnimation() {
        animator.SetBool("redCast", true);
        StartCoroutine(waitAndFinishAnimation("redCast", 0.1f));
    }

    public void playYellowAbilityAnimation() {
        animator.SetBool("yellowCast", true);
        StartCoroutine(waitAndFinishAnimation("yellowCast", 0.1f));
    }

    public void playBlueAbilityAnimation() {
        animator.SetBool("blueCast", true);
        StartCoroutine(waitAndFinishAnimation("blueCast", 0.1f));
    }

    public IEnumerator waitAndFinishAnimation(string name, float duration) {
        yield return new WaitForSeconds(duration);
        animator.SetBool(name, false);
    }

    private IEnumerator enemyDie()
    {
        yield return new WaitForSeconds(0.4f);
        battleController.enemies[index] = null;
        Destroy(gameObject);

        battleController.checkWin();
    }

    public void takeDamage(float damage)
    {
        if (shield == 0)
        {
            currentHP = currentHP - damage;
        }
        else
        {
            if ((shield - damage) >= 0)
            {
                shield = shield - damage;
            }
            else
            {
                currentHP = currentHP + (shield - damage);
            }
        }
        /* Debug.Log("Damaging Enemy: Health" + currentHP + ", Shield: " + shield); */
    }

    public void debuffAttack(float ammount) {
        if((attackDamage - ammount) > 0) {
            attackDamage = attackDamage - ammount;
        }
    }

    public void addShield(float shieldAmmount)
    {
        shield = shield + shieldAmmount;
        /* Debug.Log("Enemy: My Shield now is: " + shield); */
    }

    public void heal(float healthToGain)
    {
        if ((currentHP + healthToGain) >= maxHP) currentHP = maxHP;
        else currentHP = currentHP + healthToGain;
        /* Debug.Log("Healing Enemy: " + currentHP); */
    }

    public void stun(int duration)
    {
        if (!stunned)
        {
            attackWarning.SetActive(false);
            turnsToAction += duration;
            /* Debug.Log("Stunning Enemy"); */
        }
        else
        {
            /* Debug.Log("Enemy already stunned"); */
        }
    }

    void OnMouseUp()
    {
        battleController.selectionEnemy = gameObject.GetComponent<Enemy>();

        battleController.inSelection = false;
        battleController.disableSelection();
    }
}
