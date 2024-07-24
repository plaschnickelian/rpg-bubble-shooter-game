using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleShootArea : MonoBehaviour
{
    public GameObject gameManager;
    private GameManager gameManagerComponent;
    public GameObject activityManager;

    public void initialize() {
        gameManagerComponent = gameManager.GetComponent<GameManager>();
    }

    void OnTriggerExit2D(Collider2D coll) {
        if(coll.gameObject.tag =="ShootMarble") {
            if(coll.gameObject.GetComponent<ShootMarble>().isDestroyed) {
                return;
            }

            Destroy(coll.gameObject);
            gameManagerComponent.inMovement = false;
        }
    }
}
