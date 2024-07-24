using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System;
using System.Linq;

public class Spawner : MonoBehaviour
{
    public PathCreator pathCreator;
    public GameObject gameManager;  
    private GameManager gameManagerComponent;
    public float spawnPoint = 0.38f;
    public GameObject[] marblePrefabs;
    private int spawnCounter = 0;
    private int counter;
    public GameObject activityManager;
    public ActivityManager activityManagerComponent;
    public bool spawnTicker;

    private Collider2D spawnCollider;

    private float timeFromLastSpawn = 0;

    public void initialize() {
        gameManagerComponent = gameManager.GetComponent<GameManager>();
        activityManagerComponent = activityManager.GetComponent<ActivityManager>();
        spawnCollider = gameObject.GetComponent<Collider2D>();
        createNewMarble(spawnPoint);
    }

    void Update() {
        if(spawnCollider.enabled == true) {
            timeFromLastSpawn += Time.deltaTime;
        }

        if(timeFromLastSpawn >= 3) {
            createNewMarble(spawnPoint);
        }
    }
    
    void OnTriggerExit2D(Collider2D coll) {
        if(gameObject.GetComponent<BoxCollider2D>().enabled == false) {
            return;
        }
        
        GameObject colliderObject = coll.gameObject;

        //Spawnt neue Kugel (Wird nur 1 mal per Kugel ausgelöst)
        if(colliderObject.tag.Contains("PathMarble")) {
            Follower colliderComponent = colliderObject.GetComponent<Follower>();
            if(colliderComponent.spawned == false && colliderComponent.distanceTravelled >= spawnPoint) {
                createNewMarble(spawnPoint);

                coll.gameObject.GetComponent<Follower>().spawned = true;

                spawnCounter++;

                // Berechnet die Distanz der Bewegung, wenn Platz gemacht werden muss
                if(spawnCounter == 2) {
                    List<GameObject> marbles2 = gameManager.GetComponent<GameManager>().marbles;
                    float tmp = marbles2[0].GetComponent<Follower>().distanceTravelled - marbles2[1].GetComponent<Follower>().distanceTravelled;
                    gameManager.GetComponent<GameManager>().marbleDistance = tmp;
                }
            }
        }
    }

    void createNewMarble(float spawnDistance) {
        GameObject prefab = marblePrefabs[UnityEngine.Random.Range(0, marblePrefabs.Length)];
        System.Random randomizer = new System.Random();
        int odds = randomizer.Next(0, 100);
        List<GameObject> marbles = gameManager.GetComponent<GameManager>().marbles;

        if(marbles.Count == 0) {
            counter = 1;
        }
        else {
            GameObject previousPrefab = marbles.Last().GetComponent<Follower>().myPrefab;

            switch(counter) {
                case 1:
                    if(odds < 75) {
                        prefab = previousPrefab;
                        counter = 2;
                        break;
                    }
                    else {
                        prefab = getDifferentPrefab(prefab, previousPrefab);
                        break;
                    }
                
                case 2:
                    if(odds < 50) {
                        prefab = previousPrefab;
                        counter = 3;
                        break;
                    }
                    else {
                        prefab = getDifferentPrefab(prefab, previousPrefab);
                        counter = 1;
                        break;
                    }
                    
                case 3:
                    prefab = getDifferentPrefab(prefab, previousPrefab);
                    counter = 1;
                    break;
            }
        }

        GameObject newCircle = Instantiate(prefab, pathCreator.path.GetPointAtDistance(spawnDistance), Quaternion.identity);

        newCircle.GetComponent<Follower>().enabled = true;
        newCircle.GetComponent<CircleCollider2D>().enabled = true;

        newCircle.GetComponent<Follower>().pathCreator = pathCreator;
        newCircle.GetComponent<Follower>().distanceTravelled = spawnDistance;
        newCircle.GetComponent<Follower>().gameManager = gameManagerComponent;
        newCircle.GetComponent<Follower>().activityManager = activityManagerComponent;

        gameManager.GetComponent<GameManager>().marbles.Add(newCircle);

        spawnTicker = !spawnTicker;
        timeFromLastSpawn = 0;
    }

    GameObject getDifferentPrefab(GameObject current, GameObject previous) {
        GameObject prefab = current;
        while(prefab.tag == previous.tag) {
            prefab = marblePrefabs[UnityEngine.Random.Range(0, marblePrefabs.Length)];
        }
        return prefab;
    }
}
