using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Sprite grassSprite;
    [SerializeField] private Sprite plowedSprite;

    [HideInInspector] public int row;
    [HideInInspector] public int col;

    public bool isPlowed = false;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Player>())
        {
            Debug.Log("Hellow");
            collision.GetComponent<Player>().currentTile = this;
        }
    }

    public void PlowTile(Player player)
    {
        if (!isPlowed)
        {
            player.currentState = Player.PlayerStates.plowing;
            player.timer = player.plowTime;
            isPlowed = true;
        }
    }

    public void ChangeTileToPlowTile()
    {
        GetComponent<SpriteRenderer>().sprite = plowedSprite;
    }

    public void ChangeTileToGrassSprite()
    {
        GetComponent<SpriteRenderer>().sprite = grassSprite;
    }
}
