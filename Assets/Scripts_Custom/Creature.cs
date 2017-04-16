using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MovingObject {

    public int playerDamage;

    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;
    private Animator animator;
    private GameObject player;
    private Transform target;
    private bool skipMove;

    protected override void Start () {
        GameManager.Instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("PlayerTag");
        try { 
        target = player.transform;
        }
        catch
        {
            target = GameManager.Instance.transform;
        }
        base.Start();
	}

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if(skipMove)
        {
            skipMove = false;
            return;
        }
        base.AttemptMove<T>(xDir, yDir);

        skipMove = true;
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;
        if(RoughlyEquals(target, transform))
        {
            yDir = MoveTowardsPlayer(target, transform, false);
        }
        else
        {
            xDir = MoveTowardsPlayer(target, transform, true);
        }
        AttemptMove<Player>(xDir, yDir);
    }

    public int MoveTowardsPlayer(Transform player, Transform self, bool useX)
    {
        if(useX)
            return BoolToIntDirection(target.X() > transform.X());
        else
            return BoolToIntDirection(target.Y() > transform.Y());
    }
    public int BoolToIntDirection(bool isPositive)
    {
        return isPositive ? 1 : -1;
    }

    public bool RoughlyEquals(Transform first, Transform second)
    {
        return RoughlyEquals(first.X(), second.X());
    }
    public bool RoughlyEquals(float first, float second)
    {
        return (Mathf.Abs(first - second) < float.Epsilon);
    }

    protected override void OnCantMove<T>(T component)
    {
        animator.SetTrigger("enemyAttack");
        Player hitPlayer = component as Player;
        hitPlayer.LoseFood(playerDamage);

        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
    }
}
