using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class ActivityManager : MonoBehaviour
{
    public GameObject marbleSpawner;
    public GameObject marblePlayer;
    public GameObject marbleShootArea;
    public GameObject marbleGameManager;
    public GameObject game1Spawner;
    public GameObject game1BattleController;

    public GameObject Game1GameObject;
    public GameObject Game2GameObject;

    public GameObject gameOverScreen;
    public GameObject winScreen;

    public GameObject DebugButton1;
    public GameObject DebugButton2;
    public GameObject DebugButton3;
    public GameObject DebugButton4;

    public Animator disableGame1Animator;
    public Animator disableGame2Animator;
    public Animator disableSpecialAnimator;
    public GameObject gameStartButton;

    public Animator HeartContainer1;
    public Animator HeartContainer2;
    public Animator HeartContainer3;

    private Game1BattleController battleControllerComponent;
    private Spawner msComponent;
    private GameManager mgmComponent;

    public GameObject[] disabledOverlays;
    public GameObject[] WaitingForStartObjects;
    public GameObject WaitingForStartGameObject;

    public bool inDeactivationMarble;

    void Start() {
        //Enable Debug Buttons
        /* DebugButton1.SetActive(true); */
        
        msComponent = marbleSpawner.GetComponent<Spawner>();
        mgmComponent = marbleGameManager.GetComponent<GameManager>();
        battleControllerComponent = game1BattleController.GetComponent<Game1BattleController>();

        StartCoroutine(initializeStartSquence());
    }

    private IEnumerator initializeStartSquence() {
        disableGame1Animator.SetTrigger("FadeIn");
        disableGame1Animator.SetBool("FadedOut", true);

        disableGame2Animator.SetTrigger("FadeOut");
        disableGame2Animator.SetBool("FadedIn", true);

        disableSpecialAnimator.SetTrigger("FadeOut");
        disableSpecialAnimator.SetBool("FadedIn", true);

        yield return new WaitForSeconds(1.2f);

        game1Spawner.GetComponent<Game1Spawner>().initialize();

        yield return new WaitForSeconds(1.2f);

        HeartContainer1.SetTrigger("FadeIn");
        yield return new WaitForSeconds(0.3f);
        HeartContainer2.SetTrigger("FadeIn");
        yield return new WaitForSeconds(0.3f);
        HeartContainer3.SetTrigger("FadeIn");
        yield return new WaitForSeconds(0.7f);

        StartCoroutine(game1Spawner.GetComponent<Game1Spawner>().enableWarnings());

        yield return new WaitForSeconds(1.5f);
        disableGame2Animator.SetTrigger("FadeIn");
        disableGame2Animator.SetBool("FadedOut", true);

        disableSpecialAnimator.SetTrigger("FadeIn");
        disableSpecialAnimator.SetBool("FadedOut", true);
        initializeWaitingForStart();
    }

    private void initializeWaitingForStart() {
        // Fade in Disable Overlays
        foreach(GameObject o in disabledOverlays) {
            StartCoroutine(fadeIn(o, 0.8f));
        }
        // Fade in Start Indicator
        gameStartButton.SetActive(true);
        StartCoroutine(fadeIn(WaitingForStartObjects[0], 1f));
    }

    public IEnumerator initializeStartGameloop() {
        // Fade out Start Indicator + Marble Shooter
        StartCoroutine(fadeOut(WaitingForStartObjects[0], 1f));
        StartCoroutine(fadeOut(WaitingForStartObjects[1], 1f));

        StartCoroutine(startGame());

        // disable not needed Game Objects
        yield return new WaitForSeconds(1f);
        WaitingForStartGameObject.SetActive(false);
    }

    private IEnumerator startGame() {
        // wait so first click doesn't immediatly shoot marble
        yield return new WaitForSeconds(0.1f);

        // disable Game1/SpecialAbilities
        disableGame1Animator.SetTrigger("FadeOut");
        disableGame1Animator.SetBool("FadedIn", true);

        disableSpecialAnimator.SetTrigger("FadeOut");
        disableSpecialAnimator.SetBool("FadedIn", true);

        marblePlayer.SetActive(true);
        marblePlayer.GetComponent<Player>().initialize();

        marbleSpawner.SetActive(true);
        marbleSpawner.GetComponent<Spawner>().initialize();

        marbleShootArea.SetActive(true);
        marbleShootArea.GetComponent<MarbleShootArea>().initialize();

        foreach(GameObject o in disabledOverlays) {
            o.SetActive(false);
        }
    }

    public void DebugButton1Perform() {
        initializeWaitingForStart();
    }

    public void DebugButton2Perform() {
        /* StartCoroutine(fadeOut(disabledOverlays, 0.8f)); */
    }

    /* private void StartRotationCoroutine(GameObject obj) {
        if(whirlRotate) StartCoroutine(rotate(obj));
    }

    private IEnumerator rotate(GameObject obj) {
        obj.transform. Rotate(0, 0, 6f * -30f * Time.deltaTime);
        yield return new WaitForEndOfFrame();
        StartRotationCoroutine(obj);
    } */

    private IEnumerator fadeIn(GameObject o, float duration) {
        float time = 0;
        float opacity = 0f;
        while (time < duration)
        {
            opacity = Mathf.Lerp(0f, 1f, time / duration);
            time += Time.deltaTime;

            Color tempCol = o.GetComponent<SpriteRenderer>().color;
            tempCol.a = opacity;
            o.GetComponent<SpriteRenderer>().color = tempCol;

            /* foreach(GameObject o in objs) {
                Color tempCol = o.GetComponent<SpriteRenderer>().color;
                tempCol.a = opacity;
                o.GetComponent<SpriteRenderer>().color = tempCol;
            } */
            yield return null;
        }
    }

    private IEnumerator fadeOut(GameObject o, float duration) {
        float time = 0;
        float opacity = 0f;
        while (time < duration)
        {
            opacity = Mathf.Lerp(0f, 1f, time / duration);
            time += Time.deltaTime;

            Color tempCol = o.GetComponent<SpriteRenderer>().color;
            tempCol.a = 1 - opacity;
            o.GetComponent<SpriteRenderer>().color = tempCol;

            /* foreach(GameObject o in objs) {
                Color tempCol = o.GetComponent<SpriteRenderer>().color;
                tempCol.a = 1 - opacity;
                o.GetComponent<SpriteRenderer>().color = tempCol;
            } */
            yield return null;
        }
    }

    public void closeGame() {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Map"));
        SceneManager.UnloadSceneAsync("Main");
    }

    public void lostGame() {
        SceneManager.LoadScene("Map", LoadSceneMode.Single);
    }

    public void gameOver() {
        disableMarbleShoot();
        gameOverScreen.SetActive(true);
    }

    public void Win() {
        disableMarbleShoot();
        disableGame1();
        winScreen.SetActive(true);
    }

    public void disableGame1() {
        battleControllerComponent.hero.GetComponent<Collider2D>().enabled = false;

        foreach(GameObject enemy in battleControllerComponent.enemies) {
            if(enemy != null) {
                enemy.GetComponent<Collider2D>().enabled = false;
            }
        }
    }

    public void disableMarbleShoot() {
        StartCoroutine(disableMarbleShooter());
    }

    public void enableMarbleShoot() {
        AnimatorClipInfo[] animatorinfo = disableSpecialAnimator.GetCurrentAnimatorClipInfo(0);
        string  current_animation = animatorinfo[0].clip.name;

        if( !(current_animation == "Faded_In") ) {
            disableSpecialAnimator.SetTrigger("FadeOut");
            disableSpecialAnimator.SetBool("FadedIn", true); 
        }

        marbleSpawner.GetComponent<BoxCollider2D>().enabled = true;
        marblePlayer.GetComponent<Player>().enabled = true;

        // Disable Game 1 / Disable Special of not allready
        disableGame2Animator.SetTrigger("FadeIn");
        disableGame2Animator.SetBool("FadedOut", true);

        StartCoroutine(disableGame1Delay());
    }

    private IEnumerator disableGame1Delay() {
        yield return new WaitForSeconds(0.7f);
        disableGame1Animator.SetTrigger("FadeOut");
        disableGame1Animator.SetBool("FadedIn", true);
    }

    IEnumerator disableMarbleShooter() {
        inDeactivationMarble = true;
        yield return new WaitUntil(() => mgmComponent.inMovement == false);

        bool spawnTicker = msComponent.spawnTicker;

        yield return new WaitUntil(() => spawnTicker != msComponent.spawnTicker);

        /* Debug.Log(mgmComponent.shootAmount); */

        if(mgmComponent.shootAmount >= mgmComponent.shootCount) {
            if(mgmComponent.lastMarbleMatched) {
                // disable Game2 / Enable Game 1
                disableGame1Animator.SetTrigger("FadeIn");
                disableGame1Animator.SetBool("FadedOut", true);

                disableGame2Animator.SetTrigger("FadeOut");
                disableGame2Animator.SetBool("FadedIn", true);

                StartCoroutine(battleControllerComponent.abilityProcedure(true, mgmComponent.matchColors));

                /* disableSpecialAnimator.SetTrigger("FadeIn");
                disableSpecialAnimator.SetBool("FadedOut", true); */
            }
            else {
                // disable Game 1 /Game2 / Enable Special
                disableGame1Animator.SetTrigger("FadeIn");
                disableGame1Animator.SetBool("FadedOut", true);

                disableGame2Animator.SetTrigger("FadeOut");
                disableGame2Animator.SetBool("FadedIn", true);

                disableSpecialAnimator.SetTrigger("FadeIn");
                disableSpecialAnimator.SetBool("FadedOut", true);

                battleControllerComponent.startSpecialAbilityProcedure();
            }
        }
        else if(mgmComponent.lastMarbleMatched) {
            // disable Game2 / Enable Game 1
            disableGame1Animator.SetTrigger("FadeIn");
            disableGame1Animator.SetBool("FadedOut", true);

            disableGame2Animator.SetTrigger("FadeOut");
            disableGame2Animator.SetBool("FadedIn", true);
            
            StartCoroutine(battleControllerComponent.abilityProcedure(false, mgmComponent.matchColors));
        }

        mgmComponent.lastMarbleMatched = false;
        marbleSpawner.GetComponent<BoxCollider2D>().enabled = false;
        marblePlayer.GetComponent<Player>().enabled = false;
        mgmComponent.stopMarbles();
        inDeactivationMarble = false;
    }

    public void disableSpecialDelayed() {
        disableSpecialAnimator.SetTrigger("FadeIn");
        disableSpecialAnimator.SetBool("FadedOut", true);
    }
}
