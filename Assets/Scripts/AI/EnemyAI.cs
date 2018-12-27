﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

    public float health = 100f;
    public float walkingSpeed = 1f;
    public float runSpeed = 1.8f;
    public float patrolMin = 0f;
    public float patrolMax = 5f;
    public float perceptionDistance = 10f;
    public float stoppingDistance = 0f;
    public float damage = 15f;
   
    [HideInInspector] public Vector2 startPosition;
    [HideInInspector] public bool facingRight = true;

    private Vector2 playerPosition;
    private int playerHealth;
    private Animator animator;
    private int nextUpdate = 1; // Next update in second
    private bool isAttacking = false;
    private bool isDying = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        startPosition = transform.position;
    }

    private void Update()
    {
        playerPosition = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().position;
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().playerStats.Health;

        DetectPlayer();

        CheckDie();

        //Debug.Log("Enemy Health: " + health);

        // If the next update is reached
        if (Time.time >= nextUpdate)
        {
            //Debug.Log(Time.time + ">=" + nextUpdate);

            // Change the next update (current second + 1)
            nextUpdate = Mathf.FloorToInt(Time.time) + 1;

            // Call your function
            if (isAttacking)
            {
                GiveDamage();
            }
        }
    }

    public void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }    

    public void DetectPlayer()
    {
        float distance = Vector2.Distance(transform.position, playerPosition);

        //Debug.Log("Distance: " + distance);

        if (distance < perceptionDistance && distance > stoppingDistance && playerHealth > 0 && playerHealth <= 100 && !isAttacking) //Follow
        {
            animator.SetBool("IsAttacking", false);
            animator.SetBool("IsPatrolling", false);

            animator.SetBool("IsFollowing", true);            
        }
        else if (isAttacking && playerHealth > 0 && playerHealth <= 100) //Attack
        {
            animator.SetBool("IsFollowing", false);
            animator.SetBool("IsPatrolling", false);

            animator.SetBool("IsAttacking", true);   
        }
        else //Idling
        {
            animator.SetBool("IsAttacking", false);
            //animator.SetBool("IsPatrolling", false);
            animator.SetBool("IsFollowing", false);          
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isAttacking = true;
        } 
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isAttacking = false;
        }
    }

    private void CheckDie()
    {
        //health = 0f;

        if (health <= 0f && !isDying)
        {
            isDying = true;
            animator.SetBool("IsDying", true);
        }
    }

    private void GiveDamage()
    {
        if (playerHealth > 0 && playerHealth <= 100)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().playerStats.Health -= (int)(UnityEngine.Random.Range(0.75f, 1f) * damage);
        }
    }

    public void DoPatrollingCoroutine(Animator animator, bool isPatrol)
    {
        StartCoroutine(ChangePatrolling(animator, isPatrol));
    }

    IEnumerator ChangePatrolling(Animator animator, bool isPatrol)
    {
        yield return new WaitForSeconds(4f);

        if (!isAttacking && !animator.GetBool("IsFollowing"))
        {
            animator.SetBool("IsPatrolling", isPatrol);
        }       
    }

    public void DoDestroyEnemy()
    {
        StartCoroutine(DestroyEnemy());
    }

    IEnumerator DestroyEnemy()
    {
        yield return new WaitForSeconds(3f);

        Destroy(gameObject);
    }
}