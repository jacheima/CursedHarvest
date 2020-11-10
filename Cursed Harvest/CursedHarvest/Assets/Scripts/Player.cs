using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public Tile currentTile;
    public Tile selectedTile;

    protected override void Update()
    {
        direction = new Vector2(0f, 0f);
        GetInput();
        SelectTile();
        base.Update();
    }

    void SelectTile()
    {
        //if we are facing right
        if(direction.x > 0)
        {
            selectedTile = GameManager.instance.tiles[currentTile.col + 1, currentTile.row];
        }

        //if we are facing left
        if (direction.x < 0)
        {
            selectedTile = GameManager.instance.tiles[currentTile.col - 1, currentTile.row];
        }

        //if we are faving up
        if (direction.y < 0)
        {
            selectedTile = GameManager.instance.tiles[currentTile.col, currentTile.row + 1];
        }

        //if we are facing down
        if (direction.y > 0)
        {
            selectedTile = GameManager.instance.tiles[currentTile.col, currentTile.row - 1];
        }
    }

    void GetInput()
    {
        if(Input.GetKey(KeyCode.W))
        {
            direction += Vector2.up.normalized;
        }

        if (Input.GetKey(KeyCode.A))
        {
            direction += Vector2.left.normalized;
        }

        if (Input.GetKey(KeyCode.S))
        {
            direction += Vector2.down.normalized;
        }

        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector2.right.normalized;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(selectedTile != null)
            {
                selectedTile.PlowTile();
            }
        }
    }
}
