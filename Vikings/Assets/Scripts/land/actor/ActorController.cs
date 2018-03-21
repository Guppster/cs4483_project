﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class ActorController : MonoBehaviour {

    public const string PLATFORM_TAG = "platform";
    public const string ENEMY_TAG = "enemy";

    protected Rigidbody2D rb;
    protected SpriteRenderer sprenderer;
    protected Animator animator;

    public int hp = 1;

    protected bool canShoot = true;
    protected bool canMelee = true;

    // TODO set this properly
    protected bool facingRight = true;

    protected float projectileCooldown = 1f;
    protected float meleeCooldown = 1f;

    // Use this for initialization
    protected void Start() {
        rb = GetComponent<Rigidbody2D>();
        sprenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    protected void Update() {
        //Debug.Log("Updating controller for " + name);
        //Debug.Log(transform.position);
        if(transform.position.y < -15) {
            Debug.Log(name + " fell off the map");
            Destroy(gameObject);
        }
    }

    protected void FixedUpdate() {
        
    }

    protected void RangedAttack() {
        if(!canShoot) {
            return;
        }

        ProjectileManager.instance().NewProjectile(transform, facingRight);

        //start cooldown timer
        StartCoroutine(ToggleCanShoot());
    }

    //Toggles the canShoot variable to false for [cooldown] amount of seconds
    protected IEnumerator ToggleCanShoot() {
        canShoot = false;
        yield return new WaitForSeconds(projectileCooldown);
        canShoot = true;
    }

    protected void MeleeAttack() {
        if(!canMelee) {
            return;
        }

        //Debug.Log(name + " attack!");
        animator.SetBool("attacking", true);

        //Debug.Break();
        StartCoroutine(UnsetAttacking());
        StartCoroutine(ToggleCanMelee());
    }

    // We want to only attack "once", so we unset attacking variable as soon as animation begins.
    protected IEnumerator UnsetAttacking() {
        // wait 1 frame
        yield return 0;
        //Debug.Log("Unset attacking");
        animator.SetBool("attacking", false);
    }

    //Toggles the canShoot variable to false for [cooldown] amount of seconds
    protected IEnumerator ToggleCanMelee() {
        canMelee = false;
        yield return new WaitForSeconds(meleeCooldown);
        canMelee = true;
    }

    public void GetShot(int damage) {
        hp -= damage;
        if(hp <= 0) {
            Die();
        }
    }

    public void Die() {
        Debug.Log(name + " is dead");
        Destroy(gameObject);
    }
}
