using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public Game1BattleController battleController;
    public GameObject activityManager;
    public GameObject indicator;
    public Animator animator;

    public float maxHP;
    public float currentHP;
    public float shield;
    public float redAttackDamage;

    void Start()
    {
        maxHP = 3;
        currentHP = maxHP;
        shield = 0;
        redAttackDamage = 1;
    }

    void Update()
    {
        if ((currentHP <= 0)) heroDie();
    }

    private void heroDie()
    {
        activityManager.GetComponent<ActivityManager>().gameOver();
        Destroy(gameObject);
    }

    void OnMouseUp()
    {
        battleController.inSelection = false;
        battleController.disableSelection();
    }

    public void playBlueAbilityAnimation() {
        animator.SetTrigger("PlayerBlueCast");
    }

    public void playGreenAbilityAnimation() {
        animator.SetTrigger("PlayerGreenCast");
    }

    public IEnumerator waitAndFinishAnimation(string name, float duration) {
        yield return new WaitForSeconds(duration);
        animator.SetBool(name, false);
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
        // Debug.Log("Damaging Player: Health" + currentHP + ", Shield: " + shield);
    }

    public void addShield(float shieldAmmount) {
        if(shield + shieldAmmount <= 3) {
            shield = shield + shieldAmmount;
        }
        else {
            shield = 3;
        }
        // Debug.Log("Hero: My Shield now is: " + shield);
    }

    public void heal(float healthToGain) {
        if((currentHP+healthToGain) >= maxHP) currentHP = maxHP;
        else currentHP = currentHP + healthToGain;

        // also buffes
        redAttackDamage += 1;
        // Debug.Log("Healing Player: " + currentHP);
    }

    public void stun(int duration) {
        // Debug.Log("Stunning Player");
    }
}
