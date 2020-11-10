using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Tile[,] tiles;

    public GameObject tilePrefab;
    public GameObject playerPrefab;

    public int columns;
    public int rows;

    private void Awake()
    {
        if(instance != this && instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        tiles = new Tile[columns, rows];

        SpawnTiles();

        Instantiate(playerPrefab, tiles[7, 4].transform.position, tiles[7, 4].transform.rotation);

        FindObjectOfType<Player>().currentTile = tiles[7, 4];
    }

    void SpawnTiles()
    {
        for(int i = 0; i < columns; i++)
        {
            for(int j = 0; j < rows; j++)
            {
                GameObject newTile = Instantiate(tilePrefab, transform.position, transform.rotation);
                newTile.name = "Tile_" + i + "_" + j;

                tiles[i, j] = newTile.GetComponent<Tile>();

                tiles[i, j].col = i;
                tiles[i, j].row = j;

                if(j < rows - 1)
                {
                    transform.position += new Vector3(0f, -1.33f, 0f);
                }
                else
                {
                    transform.position = new Vector3(transform.position.x, 5f, 0f);
                }
            }

            transform.position += new Vector3(1.33f, 0f, 0f);
        }
    }
}
