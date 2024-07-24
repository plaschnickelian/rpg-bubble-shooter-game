using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PathCreation;

public class Follower : MonoBehaviour
{
    public PathCreator pathCreator;
    public GameObject myPrefab;
    public float speed;
    public float distanceTravelled;
    private bool distanceTravelledSet;
    public float MarbleDistance; // set by GameManager, Distanz zwischen jetziger Kugel und vorderster Kugel in dem bewegenden Segment
    private Vector3 nextPoint;
    private float angle;
    private Quaternion rotation;
    public bool spawned = false;
    public GameManager gameManager; // set by Spawner, ShootMarble
    private List<GameObject> marbles = null;
    public bool isMatched = false; // set by GameManager
    public ActivityManager activityManager;
    public Animator animator;

    public enum MarbleStates {
        MOVING,
        MOVING_BACK,
        IDLE,
        NEXT_POSITION,
        IN_POSITION,
        KNOCKBACK
    }

    public MarbleStates marbleState;
    public MarbleStates nextMarbleState; // set by ShootMarble, gibt den MarbleState an, der nach dem Movement angenommen werden soll

    void Start() {
        speed = 0.45f;
    }

    // Update is called once per frame
    void Update()
    {
        if(isMatched) {
            StartCoroutine(destory());
        }

        switch(marbleState) {
            case MarbleStates.MOVING:
                if(gameObject != gameManager.marbles.First() && isMatched == false) {
                    int index = gameManager.marbles.FindIndex(m => m == gameObject);
                    Follower marble = gameManager.marbles[index - 1].GetComponent<Follower>();

                    if(marble.distanceTravelled < distanceTravelled + gameManager.marbleDistance && gameManager.marbles.First().GetComponent<Follower>().marbleState == MarbleStates.KNOCKBACK) {
                        distanceTravelled = marble.distanceTravelled - gameManager.marbleDistance;
                        transform.position = Vector3.MoveTowards(transform.position, pathCreator.path.GetPointAtDistance(distanceTravelled), 8f * Time.deltaTime);
                        break;
                    }
                }
                Move(speed);
                break;

            case MarbleStates.NEXT_POSITION:
                if(marbles == null) {
                    marbles = gameManager.marbles;
                }

                if(distanceTravelledSet == false) {
                    distanceTravelled = distanceTravelled + gameManager.marbleDistance;
                    distanceTravelledSet = true;
                }

                if(transform.position != pathCreator.path.GetPointAtDistance(distanceTravelled)) {

                    if(nextMarbleState == MarbleStates.MOVING) {
                        distanceTravelled += speed * Time.deltaTime;
                    }
                    
                    transform.position = Vector3.MoveTowards(transform.position, pathCreator.path.GetPointAtDistance(distanceTravelled), 9f * Time.deltaTime);
                    break;
                }
                else {
                    distanceTravelled += speed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, pathCreator.path.GetPointAtDistance(distanceTravelled), distanceTravelled);
                    marbleState = nextMarbleState;
                    distanceTravelledSet = false;
                    marbles = null;
                    break;
                    }

            case MarbleStates.MOVING_BACK:
                if(distanceTravelled > MarbleDistance) {
                    MarbleDistance += speed * Time.deltaTime;
                    Move(-2.5f);
                    break;
                }
                else {
                    Move(speed);
                    marbleState = MarbleStates.MOVING;
                    MarbleDistance = 0;
                    break;
                }

            case MarbleStates.KNOCKBACK:
                if(distanceTravelled > MarbleDistance) {
                    Move(-2.5f);
                    break;
                }
                else {
                    Move(speed);
                    marbleState = MarbleStates.MOVING;
                    MarbleDistance = 0;
                    break;
                }
            
            case MarbleStates.IN_POSITION:
                if(transform.position != pathCreator.path.GetPointAtDistance(distanceTravelled)) {

                    if(nextMarbleState == MarbleStates.MOVING) {
                        distanceTravelled += speed * Time.deltaTime;
                    }

                    transform.position = Vector3.MoveTowards(transform.position, pathCreator.path.GetPointAtDistance(distanceTravelled), 9f * Time.deltaTime);
                    break;
                }
                else {
                    Move(speed);
                    marbleState = nextMarbleState;
                    gameManager.checkMatch();
                    break;
                }

            case MarbleStates.IDLE:
                if(gameObject != gameManager.marbles.Last() && isMatched == false) {
                    int index = gameManager.marbles.FindIndex(m => m == gameObject);
                    Follower marbleBehind = gameManager.marbles[index + 1].GetComponent<Follower>();

                    if(gameObject != gameManager.marbles.First()) {
                    Follower marbleFront = gameManager.marbles[index - 1].GetComponent<Follower>();

                        if(marbleFront.distanceTravelled < distanceTravelled + gameManager.marbleDistance && gameManager.marbles.First().GetComponent<Follower>().marbleState == MarbleStates.KNOCKBACK) {
                            distanceTravelled = marbleFront.distanceTravelled - gameManager.marbleDistance;
                            transform.position = Vector3.MoveTowards(transform.position, pathCreator.path.GetPointAtDistance(distanceTravelled), 8f * Time.deltaTime);
                            break;
                        }
                    }

                    if(marbleBehind.distanceTravelled >= distanceTravelled - gameManager.marbleDistance) {

                        distanceTravelled = marbleBehind.distanceTravelled + gameManager.marbleDistance;
                        transform.position = Vector3.MoveTowards(transform.position, pathCreator.path.GetPointAtDistance(distanceTravelled), 8f * Time.deltaTime);
                        
                        if(marbleBehind.marbleState == MarbleStates.MOVING) {
                            Move(speed);
                            marbleState = MarbleStates.MOVING;
                        }
                    }
                }
                break;
            }
    }

    private IEnumerator destory() {
        animator.SetTrigger("Destroy");
        yield return new WaitForSeconds(0.5f);
        gameManager.marbles.Remove(gameObject);
        Destroy(gameObject);
    }

    void Move(float moveSpeed) {
        if(moveSpeed > 0) {
            nextPoint = pathCreator.path.GetPointAtDistance(distanceTravelled + 0.07f);
            nextPoint.z = 0;
            nextPoint.x = nextPoint.x - transform.position.x;
            nextPoint.y = nextPoint.y - transform.position.y;

            angle = Mathf.Atan2(nextPoint.y, nextPoint.x) * Mathf.Rad2Deg;
            rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 5f * Time.deltaTime);
        }

        distanceTravelled += moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, pathCreator.path.GetPointAtDistance(distanceTravelled), distanceTravelled);
    }
}
