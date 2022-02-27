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
            transform.localPosition = new Vector3(transform.localPosition.x - Time.deltaTime * dirX * flySpeed, transform.localPosition.y + Time.deltaTime * flySpeed, transform.position.z);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            animator.Play("BirdFly");
            fly = true;
        }
    }

    public void Die()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (!animator.enabled)
        {
            fly = false;
            transform.localPosition = Vector3.zero;
            // reset animation state
            animator.Rebind();
            animator.Update(0f);
        }
    }
}
