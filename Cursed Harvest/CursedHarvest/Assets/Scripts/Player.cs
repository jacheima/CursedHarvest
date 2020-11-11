using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
    public Tile currentTile;
    public Tile selectedTile;

    public float plowTime;

    public float timer;

    Vector2 facing;

    [Header("Player UI Elements")]
    public GameObject actionBar;
    public Image actionFill;

    [Header("Item Interactions")]
    [SerializeField] private LayerMask layer;

    public enum PlayerStates
    {
        plowing, watering, planting, harvesting, none
    }

    public PlayerStates currentState;

    protected override void Update()
    {
        direction = new Vector2(0f, 0f);
        GetInput();
        SelectTile();

        switch (currentState)
        {
            case PlayerStates.plowing:
                Plowing();
                break;
            case PlayerStates.watering:
                break;
            case PlayerStates.planting:
                break;
            case PlayerStates.harvesting:
                break;
            case PlayerStates.none:
                break;
        }



        base.Update();
    }

    void Plowing()
    {
        actionBar.SetActive(true);

        timer -= Time.deltaTime;

        actionFill.fillAmount = timer / plowTime;

        if (timer < 0)
        {
            actionBar.SetActive(false);
            actionFill.fillAmount = 1;
            selectedTile.ChangeTileToPlowTile();
            currentState = PlayerStates.none;
        }

    }

    void SelectTile()
    {
        //if we are facing right
        if (facing.x > 0)
        {
            selectedTile = GameManager.instance.tiles[currentTile.col + 1, currentTile.row];
        }

        //if we are facing left
        if (facing.x < 0)
        {
            selectedTile = GameManager.instance.tiles[currentTile.col - 1, currentTile.row];
        }

        //if we are faving up
        if (facing.y < 0)
        {
            selectedTile = GameManager.instance.tiles[currentTile.col, currentTile.row + 1];
        }

        //if we are facing down
        if (facing.y > 0)
        {
            selectedTile = GameManager.instance.tiles[currentTile.col, currentTile.row - 1];
        }
    }

    void GetInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector2.up.normalized;
            facing = direction;
        }

        if (Input.GetKey(KeyCode.A))
        {
            direction += Vector2.left.normalized;
            facing = direction;
        }

        if (Input.GetKey(KeyCode.S))
        {
            direction += Vector2.down.normalized;
            facing = direction;
        }

        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector2.right.normalized;
            facing = direction;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (selectedTile != null)
            {
                selectedTile.PlowTile(this);
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, facing, 100f, layer);

            Debug.Log(hit.collider.gameObject.name);

            if(hit)
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();

                SetFocus(interactable);
            }
        }

        
    }
}
