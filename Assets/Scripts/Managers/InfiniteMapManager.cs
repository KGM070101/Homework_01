using System;
using UnityEngine;

public class InfiniteMapManager : MonoBehaviour
{
    [SerializeField]
    private Transform player;

    [SerializeField]
    private Transform[] tiles;

    private float tileSize = 48f;

    private Vector2Int currentTile;

    private void Start()
    {
        currentTile = GetPlayerPosition();
    }

    private Vector2Int GetPlayerPosition()
    {
        int x = Mathf.FloorToInt(player.position.x / tileSize);
        int y = Mathf.FloorToInt(player.position.y / tileSize);

        return new Vector2Int(x, y);
    }

    private void Update()
    {
        Vector2Int playerPos = GetPlayerPosition();

        if (playerPos == currentTile)
            return;


        Vector2Int moveDir = playerPos - currentTile;

        MoveTile(moveDir);

        currentTile = playerPos;
    }

    private void MoveTile(Vector2Int moveDir)
    {
        if(moveDir.x>0)
        {
            MoveLeftTileToRight();
        }
        else if(moveDir.x<0)
        {
            MoveRightTileToLeft();
        }
        
        if(moveDir.y>0)
        {
            MoveBottomTileToTop();
        }
        else if(moveDir.y<0)
        {
            MoveTopTileToBottom();
        }
    }    

    private void MoveLeftTileToRight()
    {
        float leftX = GetMinX();
        float moveDistance = tileSize * 3f;

        foreach(Transform tile in tiles)
        {
            if(Mathf.Abs(tile.position.x-leftX)<0.01f)
            {
                tile.position += Vector3.right * moveDistance;
            }
        }

    }

    private void MoveRightTileToLeft()
    {
        float rightX = GetMaxX();
        float moveDistance = tileSize * 3f;

        foreach(Transform tile in tiles)
        {
            if(Mathf.Abs(tile.position.x-rightX)<0.01f)
            {
                tile.position += Vector3.left * moveDistance;
            }
        }
    }

    private void MoveBottomTileToTop()
    {
        float bottomY = GetMinY();
        float moveDistance = tileSize * 3f;

        foreach(Transform tile in tiles)
        {
            if (Mathf.Abs(tile.position.y - bottomY) < 0.01f) 
            {
                tile.position += Vector3.up * moveDistance;
            }
        }
    }

    private void MoveTopTileToBottom()
    {
        float topY = GetMaxY();
        float moveDistance = tileSize * 3f;

        foreach(Transform tile in tiles)
        {
            if (Mathf.Abs(tile.position.y - topY) < 0.01f)
            {
                tile.position += Vector3.down * moveDistance;
            }
        }
    }

    private float GetMinX()
    {
        float minX = tiles[0].position.x;

        foreach(Transform tile in tiles)
        {
            if(tile.position.x<minX)
            {
                minX = tile.position.x;
            }
        }

        return minX;
    }

    private float GetMaxX()
    {
        float maxX = tiles[0].position.x;

        foreach(Transform tile in tiles)
        {
            if(tile.position.x>maxX)
            {
                maxX = tile.position.x;
            }
        }

        return maxX;
    }

    private float GetMinY()
    {
        float minY = tiles[0].position.y;

        foreach(Transform tile in tiles)
        {
            if(tile.position.y<minY)
            {
                minY = tile.position.y;
            }
        }

        return minY;
    }

    private float GetMaxY()
    {
        float maxY = tiles[0].position.y;

        foreach(Transform tile in tiles)
        {
            if(tile.position.y>maxY)
            {
                maxY = tile.position.y;
            }
        }

        return maxY;
    }
}
