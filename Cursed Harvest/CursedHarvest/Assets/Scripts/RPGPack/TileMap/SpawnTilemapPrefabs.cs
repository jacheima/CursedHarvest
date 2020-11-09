using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnTilemapPrefabs : MonoBehaviour, ISaveable
{
    [SerializeField]
    private Tilemap targetTileMap = null;

    [System.Serializable]
    public class SpawnPrefab
    {
        public SaveablePrefab prefab;
        public int amount;
    }

    [SerializeField]
    private SpawnPrefab[] prefabs;

    [SerializeField]
    private Vector2 placementOffset;

    [SerializeField]
    private Tilemap[] dontAllowOverlapTileMaps;

    private bool hasPlacedPrefabs = false;

    public string OnSave()
    {
        hasPlacedPrefabs = true;

        return hasPlacedPrefabs.ToString();
    }

    public void OnLoad(string data)
    {
        bool getResult = false;

        bool.TryParse(data, out getResult);

        hasPlacedPrefabs = getResult;
    }

    public bool OnSaveCondition()
    {
        if (hasPlacedPrefabs)
        {
            return false;
        }

        List<Vector3> tileWorldLocations = new List<Vector3>();

        foreach (var pos in targetTileMap.cellBounds.allPositionsWithin)
        {
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            Vector3 place = targetTileMap.CellToWorld(localPlace) + (Vector3)placementOffset;

            if (targetTileMap.HasTile(localPlace))
            {
                tileWorldLocations.Add(place);
            }
        }

        foreach (SpawnPrefab spawnConfig in prefabs)
        {
            HashSet<Vector3> selectLocations = new HashSet<Vector3>();

            for (int i = 0; i < spawnConfig.amount; i++)
            {
                if (tileWorldLocations.Count <= 0)
                {
                    Debug.Log("No more locations to spawn object.");
                    break;
                }

                int randomLocationIndex = Random.Range(0, tileWorldLocations.Count);
                Vector3Int location = targetTileMap.WorldToCell(tileWorldLocations[randomLocationIndex]);

                for (int i2 = 0; i2 < dontAllowOverlapTileMaps.Length; i2++)
                {
                    if (dontAllowOverlapTileMaps[i2].HasTile(location))
                    {
                        tileWorldLocations.RemoveAt(randomLocationIndex);
                        i--;
                        goto escapeloop;
                    }
                }

                if (selectLocations.Add(tileWorldLocations[randomLocationIndex]))
                {
                    tileWorldLocations.RemoveAt(randomLocationIndex);
                }

                escapeloop:
                continue;
            }

            foreach (Vector3 location in selectLocations)
            {
                GameObject spawnedGameObject = spawnConfig.prefab.Retrieve<GameObject>();
                spawnedGameObject.transform.position = location;

#if UNITY_EDITOR
                spawnedGameObject.transform.parent = this.transform.root;
#endif
            }
        }

        return true;
    }
}
