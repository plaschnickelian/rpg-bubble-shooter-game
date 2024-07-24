using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteTrigger : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject game1Spawner;

    void OnTriggerEnter2D(Collider2D coll) {
        GameObject colliderObject = coll.gameObject;
        if(colliderObject.tag.Contains("PathMarble")) {
            gameManager.GetComponent<GameManager>().deleteMarbles();
            game1Spawner.GetComponent<Game1Spawner>().getHero().GetComponent<Hero>().takeDamage(1f);
        }
    }
}
