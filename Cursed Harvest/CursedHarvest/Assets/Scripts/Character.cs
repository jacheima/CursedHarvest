using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField] protected float speed;

    [SerializeField] protected Rigidbody2D rigidbody;

    public Interactable focus;


    protected Vector2 direction;

    protected Animator animator;
    
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        Move();
    }

    protected virtual void Move()
    {
        rigidbody.velocity = direction * speed;

        if(rigidbody.velocity.x != 0 || rigidbody.velocity.y != 0)
        {
            AnimateMovement();
            RemoveFocus();
            
        }
        else
        {
            animator.SetLayerWeight(1, 0);
        }

        
    }

    protected void AnimateMovement()
    {
        animator.SetLayerWeight(1, 1);
        animator.SetFloat("x", direction.x);
        animator.SetFloat("y", direction.y);
    }

    protected virtual void SetFocus(Interactable newFocus)
    {
        if(newFocus != focus)
        {
            if(focus != null)
            {
                focus.OnDefocused();
            }

            focus = newFocus;
        }

        newFocus.OnFocused(transform);
    }

    protected virtual void RemoveFocus()
    {
        if (focus != null)
        {
            focus.OnDefocused();
        }

        focus = null;
    }
}
