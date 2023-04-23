using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class ChefWatchScript : MonoBehaviour
{
    public Tilemap tilemap;

    public TileBase start;
    private bool startClicked;
    public Button startButton;

    public TileBase end;
    private bool endClicked;
    public Button endButton;

    public TileBase obstacle;
    private bool obstacleClicked;
    public Button obstacleButton;

    public TileBase normal;
    public TileBase win;

    public Tilemap bottomTilemap;
    public TileBase startTileColored;
    public TileBase endTileColored;

    public Button startAiButton;
    private bool setStartAiButtonOnce;

    public Button resetTilesButton;

    public Sprite startAiButtonSprite;
    public Sprite stopAiButtonSprite;

    public Vector3Int startPosition;
    public Vector3Int endPosition;

    private bool isStopped;

    public Text winText;
    public Text lostText;

    public void StartButtonClicked()
    {
        startClicked = true;
    }

    public void EndButtonClicked()
    {
        endClicked = true;
    }

    public void ObstacleButtonClicked()
    {
        obstacleClicked = true;
    }

    private void Start()
    {
        setStartAiButtonOnce = false;
        startAiButton.interactable = false;
        winText.gameObject.SetActive(false);
        lostText.gameObject.SetActive(false);
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if (startClicked)
            {
                startClicked = false;
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cellPos = tilemap.WorldToCell(mouseWorldPos);

                if (tilemap.HasTile(cellPos))
                {
                    if (tilemap.GetTile(cellPos) != end && tilemap.GetTile(cellPos) != obstacle)
                    {
                        tilemap.SetTile(cellPos, start);
                        startButton.interactable = false;
                    }
                }
            }

            if (endClicked)
            {
                endClicked = false;
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cellPos = tilemap.WorldToCell(mouseWorldPos);

                if (tilemap.HasTile(cellPos))
                {
                    if (tilemap.GetTile(cellPos) != start && tilemap.GetTile(cellPos) != obstacle)
                    {
                        tilemap.SetTile(cellPos, end);
                        endButton.interactable = false;
                    }
                }
            }

            if (obstacleClicked)
            {
                obstacleClicked = false;
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cellPos = tilemap.WorldToCell(mouseWorldPos);

                if (tilemap.HasTile(cellPos))
                {
                    if (tilemap.GetTile(cellPos) != end && tilemap.GetTile(cellPos) != start)
                    {
                        tilemap.SetTile(cellPos, obstacle);
                    }
                }
            }
        }

        if (startButton.interactable == false && endButton.interactable == false && !setStartAiButtonOnce)
        {
            startAiButton.interactable = true;
            setStartAiButtonOnce = true;
        }

    }

    public void ResetTiles()
    {
        tilemap.SwapTile(obstacle, normal);
        tilemap.SwapTile(end, normal);
        tilemap.SwapTile(start, normal);
        tilemap.SwapTile(win, normal);
        tilemap.SwapTile(startTileColored, normal);
        startButton.interactable = true;
        endButton.interactable = true;
        obstacleButton.interactable = true;
        startClicked = false;
        endClicked = false;
        startAiButton.interactable = false;
        setStartAiButtonOnce = false;
        bottomTilemap.SwapTile(startTileColored, normal);
        bottomTilemap.SwapTile(endTileColored, normal);
        startPosition = Vector3Int.zero;
        endPosition = Vector3Int.zero;
        winText.gameObject.SetActive(false);
        lostText.gameObject.SetActive(false);
        startAiButton.image.sprite = startAiButtonSprite;
    }

    public void StartAi()
    {
        if (startAiButton.image.sprite == startAiButtonSprite)
        {
            foreach (var position in tilemap.cellBounds.allPositionsWithin)
            {
                if (tilemap.HasTile(position))
                {
                    if (tilemap.GetTile(position) == start)
                    {
                        startPosition = position;
                    }
                    if (tilemap.GetTile(position) == end)
                    {
                        endPosition = position;
                    }
                }
            }
            isStopped = false;
            bottomTilemap.SetTile(startPosition, startTileColored);
            bottomTilemap.SetTile(endPosition, endTileColored);
            startAiButton.image.sprite = stopAiButtonSprite;
            resetTilesButton.interactable = false;
            obstacleButton.interactable = false;
            BFS(startPosition, endPosition);

        } else
        {
            isStopped = true;
            startAiButton.image.sprite = startAiButtonSprite;
            resetTilesButton.interactable = true;
        }

    }

    private void BFS(Vector3Int startPosition, Vector3Int endPosition)
    {
        Dictionary<Vector3Int, Vector3Int> visited = new Dictionary<Vector3Int, Vector3Int>();
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        List<Vector3Int> path = new List<Vector3Int>();
        Vector3Int parent = endPosition;
        queue.Enqueue(startPosition);
        visited[startPosition] = startPosition; //cannot be null
        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();
            if (current == endPosition)
            {
                while (parent != startPosition)
                {
                    parent = visited[parent];
                    path.Add(parent);
                }
                path.Reverse(); 
                StartCoroutine(animation(path, visited));
                return;
            }

            foreach (var item in GetNeighbors(current))
            {
                if (!visited.ContainsKey(item))  //not visited before
                {
                    visited[item] = current;     //set as visited & parent is current
                    queue.Enqueue(item);
                }
            }
        }
        lostText.gameObject.SetActive(true);
        startAiButton.interactable = false;
        resetTilesButton.interactable = true;
    }

    private List<Vector3Int> GetNeighbors(Vector3Int current)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();
        neighbors.Add(current + Vector3Int.up);
        neighbors.Add(current + Vector3Int.down);
        neighbors.Add(current + Vector3Int.left);
        neighbors.Add(current + Vector3Int.right);

        List<Vector3Int> returnedNeighbors = new List<Vector3Int>();
        foreach (var neighbor in neighbors)
        {
            if (tilemap.HasTile(neighbor))
            {
                if (tilemap.GetTile(neighbor) != obstacle)
                {
                    returnedNeighbors.Add(neighbor);
                }
            }
        }
        return returnedNeighbors;
    }

    IEnumerator animation(List<Vector3Int> path, Dictionary<Vector3Int, Vector3Int> visited)
    {
        for (int i = 1; i < path.Count; i++)
        {
            if (isStopped)
            {
                yield return null;
            } 
            else
            {
                yield return new WaitForSeconds(.5f);
                tilemap.SetTile(path[i - 1], startTileColored);
                tilemap.SetTile(path[i], start);
                bottomTilemap.SetTile(path[i], startTileColored);
            }
        }
        if (isStopped)
        {
            yield return null;
        } 
        else
        {
            yield return new WaitForSeconds(.5f);
            tilemap.SetTile(visited[endPosition], startTileColored);
            tilemap.SetTile(endPosition, win);
            bottomTilemap.SetTile(endPosition, startTileColored);
            startAiButton.interactable = false;
            resetTilesButton.interactable = true;
            winText.gameObject.SetActive(true);
        }
    }
}