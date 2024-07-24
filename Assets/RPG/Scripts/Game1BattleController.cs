using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game1BattleController : MonoBehaviour
{
    public GameObject spawner;
    public GameObject[] hearts;
    public GameObject[] shields;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public Sprite halfHeart;
    public Sprite fullShield;
    public Sprite halfShield;

    public GameObject activityManager;

    public GameObject textYellow;
    public GameObject textGreen;
    public GameObject textRed;
    public GameObject textBlue;

    public GameObject specialAbilityButton1;
    public GameObject specialAbilityButton2;
    public GameObject specialAbilityButton3;
    public GameObject specialAbilityButton4;
    public GameObject skipButton;

    public GameObject heroObject;
    public Hero hero;
    public GameObject enemy1Object;
    public Enemy enemy1;
    public GameObject enemy2Object;
    public Enemy enemy2;
    public GameObject enemy3Object;
    public Enemy enemy3;
    public GameObject[] enemies;
    public Enemy selectionEnemy;

    public GameObject AllSpecialGameObj;
    public Animator SpecialBrushStroke;
    public Animator DisableAllSpecial;
    public Animator HeroSpecialYellow;
    public Animator HeroSpecialBlue;
    public Animator HeroSpecialGreen;
    public Animator HeroSpecialRed;
    public Animator SpecialEnergyYellow;
    public Animator SpecialEnergyBlue;
    public Animator SpecialEnergyGreen;
    public Animator SpecialEnergyRed;
    public Animator[] EnemySpecial;

    public GameObject energyRed;
    public GameObject energyGreen;
    public GameObject energyBlue;
    public GameObject energyYellow;

    private int resourceGreen;
    private int resourceBlue;
    private int resourceRed;
    private int resourceYellow;

    private bool enableSpecial;
    public bool inSelection;

    public AudioSource mySounds;
    public AudioClip damageSound;
    public AudioClip healSound;
    public AudioClip shieldSound;
    public AudioClip stunSound;
    public AudioClip playerDamageSound;
    public AudioClip specialAbilitySound;
    public AudioClip warningAppearing;

    void Start()
    {
        resourceGreen = 0;
        resourceBlue = 0;
        resourceRed = 0;
        resourceYellow = 0;

        enableSpecial = false;

        enemies = new GameObject[3];

        // disable Special Abilities on Startup
        specialAbilityButton1.SetActive(false);
        specialAbilityButton2.SetActive(false);
        specialAbilityButton3.SetActive(false);
        specialAbilityButton4.SetActive(false);
        skipButton.SetActive(false);

        AllSpecialGameObj.SetActive(true);
    }

    void Update()
    {
        //Special Ability Disablers
        // Ability1: 4 Blue, 4 Green
        if (enableSpecial && resourceBlue >= 4 && resourceGreen >= 4) {     specialAbilityButton1.SetActive(true); }
        else { specialAbilityButton1.SetActive(false); }

        //Ability2: 4 Red, 4 Yellow
        if (enableSpecial && resourceRed >= 4 && resourceYellow >= 4) {      specialAbilityButton2.SetActive(true); }
        else { specialAbilityButton2.SetActive(false); }

        //Ability3: 4 Blue, 4 Green
        if (enableSpecial && resourceBlue >= 4 && resourceGreen >= 4) {       specialAbilityButton3.SetActive(true); }
        else { specialAbilityButton3.SetActive(false); }

        //Ability4: 4 Red
        if (enableSpecial && resourceRed >= 4) {       specialAbilityButton4.SetActive(true); }
        else { specialAbilityButton4.SetActive(false); }

        //Skip Ability: Free
        if (enableSpecial) {    skipButton.SetActive(true); }
        else {      skipButton.SetActive(false); }

        // Resources
        textYellow.GetComponent<TextMesh>().text = resourceYellow.ToString();
        textGreen.GetComponent<TextMesh>().text = resourceGreen.ToString();
        textRed.GetComponent<TextMesh>().text = resourceRed.ToString();
        textBlue.GetComponent<TextMesh>().text = resourceBlue.ToString();

        //Heart Containers
        if (hero)
        {
            for (int i = 0; i < hearts.Length; i++)
            {
                SpriteRenderer heart = hearts[i].GetComponent<SpriteRenderer>();

                if (i + 1 > hero.currentHP)
                {
                    if (i + 0.5f == hero.currentHP)
                    {
                        heart.sprite = halfHeart;
                    }
                    else
                    {
                        heart.sprite = emptyHeart;
                    }
                }
                else
                {
                    heart.sprite = fullHeart;
                }
            }

            for (int i = 0; i < shields.Length; i++)
            {
                SpriteRenderer shield = shields[i].GetComponent<SpriteRenderer>();

                if (i + 1 > hero.shield)
                {
                    if (i + 0.5f == hero.shield)
                    {
                        shield.sprite = halfShield;
                    }
                    else
                    {
                        shield.sprite = null;
                    }
                }
                else
                {
                    shield.sprite = fullShield;
                }
            }
        }
    }

    public void gainResourceGreen(int ammount)
    {
        resourceGreen = resourceGreen + ammount;
    }

    public void gainResourceBlue(int ammount)
    {
        resourceBlue = resourceBlue + ammount;
    }

    public void gainResourceRed(int ammount)
    {
        resourceRed = resourceRed + ammount;
    }

    public void gainResourceYellow(int ammount)
    {
        resourceYellow = resourceYellow + ammount;
    }

    public void performAbility1(Enemy enemy)
    {
        /* Debug.Log("Ability 1 beeing cast!"); */
        enemy.takeDamage(hero.redAttackDamage);
        mySounds.PlayOneShot(damageSound);
        enemy.playRedAbilityAnimation();
    }

    public void performAbility2()
    {
        /* Debug.Log("Ability 2 beeing cast!"); */
        hero.playGreenAbilityAnimation();
        hero.addShield(1.5f);
        mySounds.PlayOneShot(shieldSound);
    }

    public void performAbility3(Enemy enemy)
    {
        /* Debug.Log("Ability 3 beeing cast!"); */
        if(enemy == null) {
            hero.playBlueAbilityAnimation();
            hero.heal(1f);
            mySounds.PlayOneShot(healSound);
        }
        else {
            enemy.debuffAttack(0.5f);
            mySounds.PlayOneShot(healSound);
            enemy.playBlueAbilityAnimation();
        }
    }

    public void performAbility4(Enemy enemy)
    {
        /* Debug.Log("Ability 4 beeing cast!"); */
        enemy.stun(1);
        mySounds.PlayOneShot(stunSound);
        enemy.playYellowAbilityAnimation();
    }

    public void performSpecialAbility1()
    {
        // Debug.Log("Dealing 1 to all enemies!");
        mySounds.PlayOneShot(specialAbilitySound);
        StartCoroutine(specialCoroutine1());
    }

    private IEnumerator specialCoroutine1() {
        resourceBlue = resourceBlue - 4;
        resourceGreen = resourceGreen -4;

        DisableAllSpecial.SetTrigger("FadeIn");
        SpecialBrushStroke.SetTrigger("FadeIn");
        HeroSpecialRed.SetTrigger("FadeIn");
        SpecialEnergyRed.SetTrigger("FadeIn");
        int i = 0;
        foreach (GameObject enemy in enemies) {
            if (enemy != null) {
                EnemySpecial[i].SetTrigger("FadeIn");
                i += 1;
            }
        }

        yield return new WaitForSeconds(1.9f);

        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.GetComponent<Enemy>().takeDamage(1f);
            }
        }

        HeroSpecialRed.SetTrigger("FadeOut");
        SpecialEnergyRed.SetTrigger("FadeOut");
        foreach (Animator a in EnemySpecial) {
            a.SetTrigger("FadeOut");
        }

        yield return new WaitForSeconds(0.6f);
        DisableAllSpecial.SetTrigger("FadeOut");
        SpecialBrushStroke.SetTrigger("FadeOut");

        yield return new WaitForSeconds(1.5f);
        enableSpecial = false;
        enemyAttack();
        returnToMarbleShooter();
    }

    public void performSpecialAbility2()
    {
        // Debug.Log("Stunning all enemies!");
        mySounds.PlayOneShot(specialAbilitySound);
        StartCoroutine(specialCoroutine2());
    }

    private IEnumerator specialCoroutine2() {
        resourceRed = resourceRed - 4;
        resourceYellow = resourceYellow - 4;

        DisableAllSpecial.SetTrigger("FadeIn");
        SpecialBrushStroke.SetTrigger("FadeIn");
        HeroSpecialYellow.SetTrigger("FadeIn");
        SpecialEnergyYellow.SetTrigger("FadeIn");
        int i = 0;
        foreach (GameObject enemy in enemies) {
            if (enemy != null) {
                EnemySpecial[i].SetTrigger("FadeIn");
                i += 1;
            }
        }

        yield return new WaitForSeconds(1.9f);

        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.GetComponent<Enemy>().stun(1);
            }
        }

        HeroSpecialYellow.SetTrigger("FadeOut");
        SpecialEnergyYellow.SetTrigger("FadeOut");
        yield return new WaitForSeconds(0.6f);
        DisableAllSpecial.SetTrigger("FadeOut");
        SpecialBrushStroke.SetTrigger("FadeOut");
        foreach (Animator a in EnemySpecial) {
            a.SetTrigger("FadeOut");
        }

        yield return new WaitForSeconds(1.5f);
        enableSpecial = false;
        enemyAttack();
        returnToMarbleShooter();
    }

    public void performSpecialAbility3()
    {
        // Debug.Log("Adding massive shield to hero!");
        mySounds.PlayOneShot(specialAbilitySound);
        StartCoroutine(specialCoroutine3());
    }

    private IEnumerator specialCoroutine3() {
        resourceBlue = resourceBlue - 4;
        resourceGreen = resourceGreen - 4;

        DisableAllSpecial.SetTrigger("FadeIn");
        SpecialBrushStroke.SetTrigger("FadeIn");
        HeroSpecialGreen.SetTrigger("FadeIn");
        SpecialEnergyGreen.SetTrigger("FadeIn");
        int i = 0;
        foreach (GameObject enemy in enemies) {
            if (enemy != null) {
                EnemySpecial[i].SetTrigger("FadeIn");
                i += 1;
            }
        }

        yield return new WaitForSeconds(1.9f);

        hero.addShield(3f);

        HeroSpecialGreen.SetTrigger("FadeOut");
        SpecialEnergyGreen.SetTrigger("FadeOut");
        yield return new WaitForSeconds(0.6f);
        DisableAllSpecial.SetTrigger("FadeOut");
        SpecialBrushStroke.SetTrigger("FadeOut");
        foreach (Animator a in EnemySpecial) {
            a.SetTrigger("FadeOut");
        }

        yield return new WaitForSeconds(1.5f);
        enableSpecial = false;
        enemyAttack();
        returnToMarbleShooter();
    }

    public void performSpecialAbility4()
    {
        // Debug.Log("Healing hero to full HP!");
        mySounds.PlayOneShot(specialAbilitySound);
        StartCoroutine(specialCoroutine4());
    }

    private IEnumerator specialCoroutine4() {
        resourceRed = resourceRed - 4;

        DisableAllSpecial.SetTrigger("FadeIn");
        SpecialBrushStroke.SetTrigger("FadeIn");
        HeroSpecialBlue.SetTrigger("FadeIn");
        SpecialEnergyBlue.SetTrigger("FadeIn");
        int i = 0;
        foreach (GameObject enemy in enemies) {
            if (enemy != null) {
                EnemySpecial[i].SetTrigger("FadeIn");
                i += 1;
            }
        }

        yield return new WaitForSeconds(1.9f);

        hero.heal(3f);

        HeroSpecialBlue.SetTrigger("FadeOut");
        SpecialEnergyBlue.SetTrigger("FadeOut");
        yield return new WaitForSeconds(0.6f);
        DisableAllSpecial.SetTrigger("FadeOut");
        SpecialBrushStroke.SetTrigger("FadeOut");
        foreach (Animator a in EnemySpecial) {
            a.SetTrigger("FadeOut");
        }

        yield return new WaitForSeconds(1.5f);
        enableSpecial = false;
        enemyAttack();
        returnToMarbleShooter();
    }

    public void skipSpecialAbility()
    {
        enableSpecial = false;
        enemyAttack();
        returnToMarbleShooter();
    }

    public void checkWin()
    {
        int counter = 0;
        foreach (GameObject enemy in enemies)
        {
            if (enemy == null)
            {
                counter++;
            }
        }

        if (counter == enemies.Length)
        {
            if(spawner.GetComponent<Game1Spawner>().wave == spawner.GetComponent<Game1Spawner>().ofWaves) {
                activityManager.GetComponent<ActivityManager>().Win();
            }
            else {
                StartCoroutine(spawner.GetComponent<Game1Spawner>().createNewEnemySet(GlobalVariables.enemyNumber));
            }
        }
    }

    private void enemyAttack()
    {
        foreach (GameObject enemyObject in enemies)
        {
            if (enemyObject)
            {
                Enemy enemy = enemyObject.GetComponent<Enemy>();

                enemy.turnsToAction -= 1;

                if (enemy.turnsToAction == 0)
                {
/*                     enemy.attackWarning.SetActive(true);
                    enemy.attackWarning.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingLayerName = "SubBack";
                    enemy.attackWarning.transform.GetChild(1).GetComponent<SpriteRenderer>().sortingLayerName = "SubBack";
                    enemy.attackWarning.transform.GetChild(2).GetComponent<SpriteRenderer>().sortingLayerName = "SubBack";
                     */
                    hero.transform.GetChild (0).gameObject.GetComponent<Animator>().SetTrigger("PlayerTakeDamage");
                    mySounds.PlayOneShot(playerDamageSound, 0.2f);
                    hero.takeDamage(enemy.attackDamage);
                    enemy.newTurnsToAction();
                }

                if (enemy.turnsToAction == 1)
                {
                    mySounds.PlayOneShot(warningAppearing);
                    enemy.attackWarning.SetActive(true);
                    enemy.attackWarning.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingLayerName = "SubBack";
                    enemy.attackWarning.transform.GetChild(1).GetComponent<SpriteRenderer>().sortingLayerName = "SubBack";
                    enemy.attackWarning.transform.GetChild(2).GetComponent<SpriteRenderer>().sortingLayerName = "SubBack";
                }
            }
        }
    }

    private void returnToMarbleShooter()
    {
        // Debug.Log("in RETURN");
        activityManager.GetComponent<ActivityManager>().enableMarbleShoot();
    }

    public void startSpecialAbilityProcedure()
    {
        //Debug.Log("Starting Special Ability Procedure");
        enableSpecial = true;
    }

    public IEnumerator abilityProcedure(bool specialAfter, Queue<string> matchedColors)
    {
        string color;
        while (matchedColors.Count >= 1)
        {
            color = matchedColors.Dequeue();

            switch (color)
            {
                case "red":
                    startEnemySelection();
                    energyRed.GetComponent<BackgroundScroller>().activate();
                    yield return new WaitUntil(() => inSelection == false);
                    energyRed.GetComponent<BackgroundScroller>().deactivate();
                    performAbility1(selectionEnemy);
                    selectionEnemy = null;
                    break;

                case "green":
                    startHeroSelection();
                    energyGreen.GetComponent<BackgroundScroller>().activate();
                    yield return new WaitUntil(() => inSelection == false);
                    energyGreen.GetComponent<BackgroundScroller>().deactivate();
                    performAbility2();
                    break;

                case "blue":
                    startHeroSelection();
                    startEnemySelection();
                    energyBlue.GetComponent<BackgroundScroller>().activate();
                    yield return new WaitUntil(() => inSelection == false);
                    energyBlue.GetComponent<BackgroundScroller>().deactivate();
                    if(selectionEnemy == null) {
                        performAbility3(null);
                    }
                    else { performAbility3(selectionEnemy);
                    }
                    selectionEnemy = null;
                    break;

                case "orange":
                    startEnemySelection();
                    energyYellow.GetComponent<BackgroundScroller>().activate();
                    yield return new WaitUntil(() => inSelection == false);
                    energyYellow.GetComponent<BackgroundScroller>().deactivate();
                    performAbility4(selectionEnemy);
                    selectionEnemy = null;
                    break;
            }
        }

        if (specialAfter)
        {
            activityManager.GetComponent<ActivityManager>().disableSpecialDelayed();
            startSpecialAbilityProcedure();
        }
        else
        {
            returnToMarbleShooter();
        }
    }

    private void startEnemySelection()
    {
        inSelection = true;
        foreach (GameObject enemy in enemies)
        {
            if (enemy)
            {
                enemy.GetComponent<BoxCollider2D>().enabled = true;
                enemy.GetComponent<Enemy>().indicator.SetActive(true);
            }
        }
    }

    public void disableSelection()
    {
        //enemy
        foreach (GameObject enemy in enemies)
        {
            if (enemy)
            {
                enemy.GetComponent<BoxCollider2D>().enabled = false;
                enemy.GetComponent<Enemy>().indicator.SetActive(false);
            }
        }

        //hero
        hero.GetComponent<BoxCollider2D>().enabled = false;
        hero.indicator.SetActive(false);
    }

    private void startHeroSelection()
    {
        inSelection = true;
        heroObject.GetComponent<BoxCollider2D>().enabled = true;
        hero.indicator.SetActive(true);
    }
}
