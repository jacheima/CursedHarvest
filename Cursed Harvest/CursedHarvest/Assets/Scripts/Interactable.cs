using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float radius = 1.5f;

    bool isFocused = false;
    bool hasInteracted = false;

    Transform player;

    public virtual void Interact()
    {
        //This method is meant to be overwritten by the child classes

        Debug.Log("Interacting with " + transform.name);
    }
    
    private void Update()
    {
        if(isFocused && !hasInteracted)
        {
            float distance = Vector2.Distance(transform.position, player.position);

            if(distance <= radius)
            {
                Interact();
                hasInteracted = true;
            }
        }
    }

    public void OnFocused(Transform playerTransform)
    {
        player = playerTransform;
        isFocused = true;
        hasInteracted = false;
    }

    public void OnDefocused()
    {
        isFocused = false;
        hasInteracted = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, radius);
    }
}
