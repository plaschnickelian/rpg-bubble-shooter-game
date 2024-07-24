using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyType : Enemy
{
    // Start is called before the first frame update
    void Awake()
    {
        base.Start();
        newTurnsToAction();
    }

    public override void newTurnsToAction() {
        turnsToAction = Random.Range(1, 3);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
