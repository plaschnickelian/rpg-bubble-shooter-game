using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootMarble : MonoBehaviour
{
    private bool collided;
    public GameObject gameManager; // set by Player
    private GameManager gameManagerComponent;
    public GameObject pathPrefab;
    public Spawner spawner; // set by Player
    public ActivityManager activityManager;
    public bool isDestroyed;

    public AudioSource mySounds;
    public AudioClip collideSound;

    // Start is called before the first frame update
    void Start()
    {
        collided = false;
        gameManagerComponent = gameManager.GetComponent<GameManager>();
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        GameObject colliderObject = coll.gameObject;
        if (colliderObject.tag.Contains("Half"))
        {
            mySounds.PlayOneShot(collideSound);
            if (collided == true)
            {
                return;
            }

            Follower colliderComponent = colliderObject.transform.parent.GetComponent<Follower>();
            Follower.MarbleStates nextState = colliderComponent.marbleState;
            List<GameObject> marbles = gameManagerComponent.marbles;
            int newMarbleIndex = -1;
            int index = marbles.FindIndex(a => a == colliderObject.transform.parent.gameObject);

            if (colliderObject.tag == "BottomHalf")
            {
                for (int i = 0; i <= index; i++)
                {
                    Follower marble = marbles[i].GetComponent<Follower>();

                    if(marble.marbleState == colliderComponent.marbleState) {
                        marble.nextMarbleState = marble.marbleState;
                        marble.marbleState = Follower.MarbleStates.NEXT_POSITION;
                    }
                }
                collided = true;
                newMarbleIndex = index + 1;
            }

            if (colliderObject.tag == "TopHalf")
            {
                for (int i = 0; i < index; i++)
                {
                    Follower marble = marbles[i].GetComponent<Follower>();

                    if(marble.marbleState == colliderComponent.marbleState) {
                        marble.nextMarbleState = marble.marbleState;
                        marble.marbleState = Follower.MarbleStates.NEXT_POSITION;
                    }
                }
                collided = true;
                newMarbleIndex = index;
            }

            if (newMarbleIndex != -1)
            {
                GameObject newMarble = Instantiate(pathPrefab, transform.position, Quaternion.identity);
                Follower newMarbleComponent = newMarble.GetComponent<Follower>();
                newMarbleComponent.pathCreator = spawner.pathCreator;
                newMarbleComponent.gameManager = gameManagerComponent;
                newMarbleComponent.activityManager = spawner.activityManagerComponent;
                newMarbleComponent.nextMarbleState = nextState;
                newMarbleComponent.marbleState = Follower.MarbleStates.IN_POSITION;
                if (newMarbleIndex == 0)
                {;
                    newMarble.GetComponent<Follower>().distanceTravelled = marbles[0].GetComponent<Follower>().distanceTravelled + gameManagerComponent.marbleDistance;
                }
                else
                {
                    if(colliderObject.tag == "BottomHalf") {
                        if(newMarbleIndex == 1) {
                            newMarbleComponent.distanceTravelled = marbles[0].GetComponent<Follower>().distanceTravelled;
                        }
                        else {
                            newMarbleComponent.distanceTravelled = marbles[newMarbleIndex - 2].GetComponent<Follower>().distanceTravelled - gameManagerComponent.marbleDistance;
                        }
                    }
                    if(colliderObject.tag == "TopHalf") {
                        newMarbleComponent.distanceTravelled = marbles[newMarbleIndex].GetComponent<Follower>().distanceTravelled + gameManagerComponent.marbleDistance;
                    }
                }

                marbles.Insert(newMarbleIndex, newMarble);
            }
            isDestroyed = true;
            Destroy(gameObject);
        }
    }
}
