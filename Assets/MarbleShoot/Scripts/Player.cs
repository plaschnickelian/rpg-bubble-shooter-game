using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#pragma warning disable 0168

public class Player : MonoBehaviour
{
    private bool initialized;
    public GameObject mainCamera;
    private bool disabled;
    public GameObject[] marblePrefabs;
    private GameObject nextMarble;
    public GameObject currentMarble;
    private bool shootCooldown = true;
    public GameObject spawner;
    public GameObject[] abilityButtons;
    public GameObject gameManager;
    public GameObject activityManager;
    private GameManager gameManagerComponent;
    private List<AbilityButton> abilityButtonComponents;

    public AudioSource mySounds;
    public AudioClip fireSound;
    public AudioClip collideSound;

    public Animator animator;

    private float timer = 0;

    public void initialize() {
        initialized = true;
        createNewNextMarble();
        createNewMarble();
        gameManagerComponent = gameManager.GetComponent<GameManager>();
        abilityButtonComponents = new List<AbilityButton>(abilityButtons.Length);
        foreach(GameObject button in abilityButtons) {
            abilityButtonComponents.Add(button.GetComponent<AbilityButton>());
        }

    }

    void Update()
    {
        // Rotieren des Player Objects zum Cursor
        Vector2 positionOnScreen = transform.position;
        Vector2 mouseOnScreen = (Vector2)mainCamera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
        float angle = angleBetweenTwoPoints(positionOnScreen, mouseOnScreen);
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));

        if(gameManagerComponent.inMovement) {
            timer += Time.deltaTime;
            if(timer >= 5) {
                gameManagerComponent.inMovement = false;
                timer = 0;
            }
        }

        // Schießen der Kugel mit Linksklick und starten des Cooldowns
        if(Input.GetMouseButtonDown(0)) {
            mySounds.PlayOneShot(fireSound);
            if(shootCooldown) {
                if(!disabled) {
                    StartCoroutine(shootMarble());
                    StartCoroutine(Cooldown(1.7f));
                }
            }
        }
    }

    void OnEnable() {
        if(initialized == true) {
            disabled = false;
            if(gameManagerComponent.shootAmount >= gameManagerComponent.shootCount) {
                gameManagerComponent.shootAmount = 0;
            }

            foreach(GameObject marble in gameManagerComponent.marbles) {
                Follower component = marble.GetComponent<Follower>();
                component.marbleState = component.nextMarbleState;
            }

            createNewMarble();
        }
    }

    void OnDisable() {
        if(currentMarble != null) {
            Destroy(currentMarble);
            currentMarble = null;
        }
    }

    // Funktion für die Rotation
    float angleBetweenTwoPoints(Vector3 a, Vector3 b) {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    void createNewMarble() {
        currentMarble = nextMarble;
        nextMarble.transform.parent = null;
        currentMarble.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        nextMarble.transform.parent = gameObject.transform;
        currentMarble.transform.localPosition = new Vector2(-4.4f, 0);

        createNewNextMarble();
    }

    void createNewNextMarble() {
        nextMarble = Instantiate(marblePrefabs[UnityEngine.Random.Range(0, 4)], transform.position, Quaternion.identity);
        nextMarble.GetComponent<ShootMarble>().spawner = spawner.GetComponent<Spawner>();
        nextMarble.GetComponent<ShootMarble>().gameManager = spawner.GetComponent<Spawner>().gameManager;
        nextMarble.GetComponent<ShootMarble>().activityManager = spawner.GetComponent<Spawner>().activityManagerComponent;
        nextMarble.GetComponent<ShootMarble>().mySounds = mySounds;
        nextMarble.GetComponent<ShootMarble>().collideSound = collideSound;
        nextMarble.transform.localScale = new Vector3(1f, 1f, 1f);
        nextMarble.transform.parent = gameObject.transform;
        /* nextMarble.transform.localPosition = new Vector2(0, 0); */
    }


    IEnumerator shootMarble() {
        try {
            animator.SetTrigger("Shoot");
            currentMarble.GetComponent<Rigidbody2D>().velocity = -transform.right * 18f;
            currentMarble.transform.parent = null;
            gameManagerComponent.inMovement = true;
            gameManagerComponent.shootAmount++;

            if(gameManagerComponent.shootAmount >= gameManagerComponent.shootCount) {
                activityManager.GetComponent<ActivityManager>().disableMarbleShoot();
            }
        }
        catch(NullReferenceException ex) {
            //nothing
        }

        currentMarble = null;

        yield return new WaitForSeconds(1.7f);

        if(activityManager.GetComponent<ActivityManager>().inDeactivationMarble) {
            disabled = true;
        }
        if(gameObject.GetComponent<Player>().enabled == true) {
            createNewMarble();
        }
    }

    IEnumerator Cooldown(float time) {
        shootCooldown = false;

        yield return new WaitForSeconds(time);

        shootCooldown = true;
    }
}
