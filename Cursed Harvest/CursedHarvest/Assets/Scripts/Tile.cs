using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : Interactable
{
    [SerializeField] private Sprite grassSprite;
    [SerializeField] private Sprite plowedSprite;

    [HideInInspector] public int row;
    [HideInInspector] public int col;

    public bool isPlowed = false;
    public bool hasPlant = false;

    Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Player>())
        {
            collision.GetComponent<Player>().currentTile = this;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (player)
        {
            player.currentTile = this;
        }
    }

    public override void Interact()
    {
        if(!isPlowed)
        {
            PlowTile();
        }

        if(isPlowed && !hasPlant)
        {
            PlantTile();
        }

        //base.Interact();
    }

    public void PlowTile()
    {
            player.currentState = Player.PlayerStates.plowing;
            player.timer = player.plowTime;
    }

    public void PlantTile()
    {
            player.currentState = Player.PlayerStates.planting;
            player.timer = player.plantTime;
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
