using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Sprite grassSprite;
    [SerializeField] private Sprite plowedSprite;

    [HideInInspector] public int row;
    [HideInInspector] public int col;

    private bool isPlowed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Player>())
        {
            Debug.Log("Hellow");
            collision.GetComponent<Player>().currentTile = this;
        }
    }

    public void PlowTile()
    {
        if (!isPlowed)
        {
            this.GetComponent<SpriteRenderer>().sprite = plowedSprite;
            isPlowed = true; 
        }
    }
}
