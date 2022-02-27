using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    [SerializeField]
    private float flySpeed;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool fly = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (fly)
        {
            float dirX = spriteRenderer.flipX ? -1 : 1;
            transform.position = new Vector3(transform.position.x - Time.deltaTime * dirX * flySpeed, transform.position.y + Time.deltaTime * flySpeed, transform.position.z);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            animator.Play("BirdFly");
            fly = true;
            Invoke("Die", 10);
        }
    }

    void Die()
    {
        fly = false;
        transform.position = Vector3.zero;
            animator.Play("BirdIdle");
    }
}
