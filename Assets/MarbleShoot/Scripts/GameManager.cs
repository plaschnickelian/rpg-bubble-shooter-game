using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Verwaltet die Kugeln in einer Liste
public class GameManager : MonoBehaviour
{
    public List<GameObject> marbles;
    public float marbleDistance;

    public GameObject Spawner;
    private Spawner spawnerComponent;

    public bool inMovement;
    public int shootCount = 3; //Anzahl der insgesamt verfügbaren Schüsse
    public int shootAmount; //Counter der bisher geschossenen Schüsse
    private bool stopAfterMatch;
    public bool lastMarbleMatched;
    public Queue<string> matchColors;

    public Animator yellowAnimator;
    public Animator greenAnimator;
    public Animator redAnimator;
    public Animator blueAnimator;

    public GameObject game1BattleController;
    private Game1BattleController battleControllerComponent;
    public GameObject activityManager;

    public AudioSource mySounds;
    public AudioClip matchSound;

    // Start is called before the first frame update
    void Start()
    {
        battleControllerComponent = game1BattleController.GetComponent<Game1BattleController>();
        spawnerComponent = Spawner.GetComponent<Spawner>();
        marbles = new List<GameObject>();
        matchColors = new Queue<string>();
    }

    public void stopMarbles() {
        foreach(GameObject marble in marbles) {
            Follower component = marble.GetComponent<Follower>();
            component.nextMarbleState = component.marbleState;
            component.marbleState = Follower.MarbleStates.IDLE;
        }
    }

    public void deleteMarbles() {
        for(int i = marbles.Count - 1; i >= 0; i--) {
            Destroy(marbles[i]);
            marbles.RemoveAt(i);
        }
    }

    IEnumerator matchManager() {
        while(marbles.Find(m => m.GetComponent<Follower>().marbleState == Follower.MarbleStates.NEXT_POSITION) != null) {
            yield return new WaitForSeconds(0.1f);
        }
        bool match = true;
        int minIndex;
        HashSet<GameObject> matchedMarbles;
        GameObject checkIfDeleted = null;
        while(match) {
            bool lastMarbleMatchedFlag = false;

            (match, minIndex, matchedMarbles) = findMatch();

            if(matchedMarbles.Count != 0) {
                mySounds.PlayOneShot(matchSound);
                lastMarbleMatched = true;
                checkIfDeleted = matchedMarbles.First();
                Follower.MarbleStates marbleState = checkIfDeleted.GetComponent<Follower>().marbleState;
                addResources(matchedMarbles);
                matchColors.Enqueue(getMatchColor(matchedMarbles));

                foreach(GameObject marble in matchedMarbles) {
                    marble.GetComponent<Follower>().isMatched = true;

                    if(marble == marbles.Last()) {
                        lastMarbleMatchedFlag = true;
                    }
                }
            }
            if(match) {
                stopAfterMatch = true;
                yield return new WaitUntil(() => checkIfDeleted == null);

                (bool match, int minIndex, HashSet<GameObject> marbles) Match = findMatch();
                int counter = 1;
                float maxMarbleDistanceTravelled = 0;

                if(lastMarbleMatchedFlag == false) {
                    try {
                        maxMarbleDistanceTravelled = marbles[minIndex].GetComponent<Follower>().distanceTravelled;
                    }
                    catch(ArgumentOutOfRangeException e) {
                        //nothing
                    }
                }

                for(int i = minIndex - 1; i >= 0; i--) {
                    if(Match.match) {
                        Follower marble = marbles[i].GetComponent<Follower>();
                        marble.MarbleDistance = counter * marbleDistance + maxMarbleDistanceTravelled;
                        marble.marbleState = Follower.MarbleStates.MOVING_BACK;
                        counter++;
                    }
                    else {
                        try {
                            marbles[i].GetComponent<Follower>().marbleState = Follower.MarbleStates.IDLE;
                        }
                        catch(ArgumentOutOfRangeException e) {
                        //nothing
                        }
                    }
                }

                if(Match.match) {
                    yield return new WaitUntil(() => marbles.First().GetComponent<Follower>().marbleState == Follower.MarbleStates.MOVING);

                    if(marbles.Last().GetComponent<Follower>().distanceTravelled >= spawnerComponent.spawnPoint - marbleDistance) {
                        for(int i = minIndex - 1; i >= 0; i--) {
                            Follower marbleComponent = marbles[i].GetComponent<Follower>();
                            marbleComponent.MarbleDistance = marbleComponent.distanceTravelled - marbleDistance;
                            marbleComponent.marbleState = Follower.MarbleStates.KNOCKBACK;
                        }
                    }

                    yield return new WaitUntil(() => marbles.First().GetComponent<Follower>().marbleState == Follower.MarbleStates.MOVING);
                }
            }

            yield return null;
        }

        if(stopAfterMatch) {
            stopAfterMatch = false;
            if(shootAmount < shootCount) {
                activityManager.GetComponent<ActivityManager>().disableMarbleShoot();
            }
        }

        inMovement = false;
    }

    public void checkMatch() {
        StartCoroutine(matchManager());
    }

    private string getMatchColor(HashSet<GameObject> matchedMarbles) {
        return matchedMarbles.First().tag.Substring(0, matchedMarbles.First().tag.Length-10);
    }

    private void addResources(HashSet<GameObject> matchedMarbles) {
        string color = getMatchColor(matchedMarbles);
        
        switch(color) {
            case "blue":
                blueAnimator.SetTrigger("Flash");
                battleControllerComponent.gainResourceBlue(matchedMarbles.Count);
                break;

            case "green":
                greenAnimator.SetTrigger("Flash");
                battleControllerComponent.gainResourceGreen(matchedMarbles.Count);
                break;

            case "orange":
                yellowAnimator.SetTrigger("Flash");
                battleControllerComponent.gainResourceYellow(matchedMarbles.Count);
                break;

            case "red":
                redAnimator.SetTrigger("Flash");
                battleControllerComponent.gainResourceRed(matchedMarbles.Count);
                break;
        }
    }

    private (bool, int, HashSet<GameObject>) findMatch() {
        HashSet<GameObject> matchedMarbles = new HashSet<GameObject>();
        bool output = false;
        int minIndex = marbles.Count;
        if(marbles.Count >= 4) {
            for(int i = 3; i < marbles.Count; i++) {
                GameObject thisMarble = marbles[i];

                GameObject marble1 = marbles[i - 3];
                GameObject marble2 = marbles[i - 2];
                GameObject marble3 = marbles[i - 1];

                if(thisMarble.tag == marble1.tag && thisMarble.tag == marble2.tag && thisMarble.tag == marble3.tag) {
                    matchedMarbles.Add(thisMarble);
                    matchedMarbles.Add(marble1);
                    matchedMarbles.Add(marble2);
                    matchedMarbles.Add(marble3);
                    output = true;
                    minIndex = Mathf.Min(minIndex, i - 3);
                }
            }
        }
        return (output, minIndex, matchedMarbles);
    }
}
