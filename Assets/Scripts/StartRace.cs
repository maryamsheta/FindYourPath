using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class StartRace : MonoBehaviour
{
    public Tilemap tilemap;
    public Tilemap tilemapDFS;

    public TileBase start;
    private bool startClicked;
    public Button startButton;

    public TileBase startDFS;
    private bool startClickedDFS;
    public Button startButtonDFS;

    public TileBase end;
    private bool endClicked;
    public Button endButton;

    public TileBase endDFS;
    private bool endClickedDFS;
    public Button endButtonDFS;

    public TileBase obstacle;
    private bool obstacleClicked;
    public Button obstacleButton;

    public TileBase obstacleDFS;
    private bool obstacleClickedDFS;
    public Button obstacleButtonDFS;

    public TileBase normal;  

    public TileBase win;
    public TileBase winDFS;

    public Tilemap bottomTilemap;
    public Tilemap bottomTilemapDFS;

    public TileBase startTileColored;
    public TileBase endTileColored;

    public Button startAiButton;
    private bool setStartAiButtonOnce;

    public Button resetTilesButton;

    public Sprite startAiButtonSprite;
    public Sprite stopAiButtonSprite;

    public Vector3Int startPosition;
    public Vector3Int endPosition;

    public Vector3Int startPositionDFS;
    public Vector3Int endPositionDFS;

    public Text BFSWon;
    public Text DFSWon;
    public Text tie;


    int pathCount;
    int pathCountDFS;
    Vector3Int beforeLastDFS;

    private bool isStopped;

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

    public void StartButtonClickedDFS()
    {
        startClickedDFS = true;
    }

    public void EndButtonClickedDFS()
    {
        endClickedDFS = true;
    }

    public void ObstacleButtonClickedDFS()
    {
        obstacleClickedDFS = true;
    }

    private void Start()
    {
        setStartAiButtonOnce = false;
        startAiButton.interactable = false;
        DFSWon.gameObject.SetActive(false);
        BFSWon.gameObject.SetActive(false);
        tie.gameObject.SetActive(false);
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

            if (startClickedDFS)
            {
                startClickedDFS = false;
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cellPos = tilemapDFS.WorldToCell(mouseWorldPos);

                if (tilemapDFS.HasTile(cellPos))
                {
                    if (tilemapDFS.GetTile(cellPos) != endDFS && tilemapDFS.GetTile(cellPos) != obstacleDFS)
                    {
                        tilemapDFS.SetTile(cellPos, startDFS);
                        startButtonDFS.interactable = false;
                    }
                }
            }

            if (endClickedDFS)
            {
                endClickedDFS = false;
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cellPos = tilemapDFS.WorldToCell(mouseWorldPos);

                if (tilemapDFS.HasTile(cellPos))
                {
                    if (tilemapDFS.GetTile(cellPos) != startDFS && tilemapDFS.GetTile(cellPos) != obstacleDFS)
                    {
                        tilemapDFS.SetTile(cellPos, endDFS);
                        endButtonDFS.interactable = false;
                    }
                }
            }

            if (obstacleClickedDFS)
            {
                obstacleClickedDFS = false;
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cellPos = tilemapDFS.WorldToCell(mouseWorldPos);

                if (tilemapDFS.HasTile(cellPos))
                {
                    if (tilemapDFS.GetTile(cellPos) != endDFS && tilemapDFS.GetTile(cellPos) != startDFS)
                    {
                        tilemapDFS.SetTile(cellPos, obstacleDFS);
                    }
                }
            }
        }

        if (startButton.interactable == false && endButton.interactable == false && !setStartAiButtonOnce && startButtonDFS.interactable == false && endButtonDFS.interactable == false)
        {
            startAiButton.interactable = true;
            setStartAiButtonOnce = true;
        }

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

            foreach (var position in tilemapDFS.cellBounds.allPositionsWithin)
            {
                if (tilemapDFS.HasTile(position))
                {
                    if (tilemapDFS.GetTile(position) == startDFS)
                    {
                        startPositionDFS = position;
                    }
                    if (tilemapDFS.GetTile(position) == endDFS)
                    {
                        endPositionDFS = position;
                    }
                }
            }
            isStopped = false;
            bottomTilemap.SetTile(startPosition, startTileColored);
            bottomTilemap.SetTile(endPosition, endTileColored);
            bottomTilemapDFS.SetTile(startPositionDFS, startTileColored);
            bottomTilemapDFS.SetTile(endPositionDFS, endTileColored);
            startAiButton.image.sprite = stopAiButtonSprite;
            resetTilesButton.interactable = false;
            obstacleButton.interactable = false;
            obstacleButtonDFS.interactable = false;

            BFS(startPosition, endPosition);
            DFS(startPositionDFS, endPositionDFS);  
        }
        else
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
                pathCount = path.Count;
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
        startAiButton.interactable = false;
        resetTilesButton.interactable = true;
    }

    private void DFS(Vector3Int startPosition, Vector3Int endPosition)
    {
        Dictionary<Vector3Int, Vector3Int> visited = new Dictionary<Vector3Int, Vector3Int>();
        Stack<Vector3Int> stack = new Stack<Vector3Int>();
        List<Vector3Int> path = new List<Vector3Int>();
        Vector3Int parent = endPosition;
        stack.Push(startPosition);
        visited[startPosition] = startPosition; //cannot be null
        while (stack.Count > 0)
        {
            Vector3Int current = stack.Pop();
            if (current == endPosition)
            {
                while (parent != startPosition)
                {
                    parent = visited[parent];
                    path.Add(parent);
                }
                path.Reverse();
                pathCountDFS = path.Count;
                beforeLastDFS = visited[endPosition];
                StartCoroutine(animationDFS(path, visited));
                return;
            }

            foreach (var item in GetNeighborsDFS(current))
            {
                if (!visited.ContainsKey(item))  //not visited before
                {
                    visited[item] = current;     //set as visited & parent is current
                    stack.Push(item);
                }
            }
        }
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

    private List<Vector3Int> GetNeighborsDFS(Vector3Int current)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();
        neighbors.Add(current + Vector3Int.up);
        neighbors.Add(current + Vector3Int.down);
        neighbors.Add(current + Vector3Int.left);
        neighbors.Add(current + Vector3Int.right);

        List<Vector3Int> returnedNeighbors = new List<Vector3Int>();
        foreach (var neighbor in neighbors)
        {
            if (tilemapDFS.HasTile(neighbor))
            {
                if (tilemapDFS.GetTile(neighbor) != obstacleDFS)
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
            BFSWon.gameObject.SetActive(true);
            if (pathCount == pathCountDFS)
            {
                BFSWon.gameObject.SetActive(false); 
                tie.gameObject.SetActive(true);
                tilemapDFS.SetTile(endPositionDFS, winDFS);
                tilemapDFS.SetTile(beforeLastDFS, startTileColored);
                bottomTilemapDFS.SetTile(endPositionDFS, startTileColored);
            }
            StopAllCoroutines();
        }
    }

    IEnumerator animationDFS(List<Vector3Int> path, Dictionary<Vector3Int, Vector3Int> visited)
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
                tilemapDFS.SetTile(path[i - 1], startTileColored);
                tilemapDFS.SetTile(path[i], startDFS);
                bottomTilemapDFS.SetTile(path[i], startTileColored);
            }
        }
        if (isStopped)
        {
            yield return null;
        }
        else
        {
            yield return new WaitForSeconds(.5f);
            tilemapDFS.SetTile(visited[endPositionDFS], startTileColored);
            tilemapDFS.SetTile(endPositionDFS, winDFS);
            bottomTilemapDFS.SetTile(endPositionDFS, startTileColored);
            startAiButton.interactable = false;
            resetTilesButton.interactable = true;
            DFSWon.gameObject.SetActive(true);
            StopAllCoroutines();
        }
    }

    public void ResetTiles()
    {
        tilemap.SwapTile(obstacle, normal);
        tilemap.SwapTile(end, normal);
        tilemap.SwapTile(start, normal);
        tilemap.SwapTile(win, normal);
        tilemap.SwapTile(startTileColored, normal);
        tilemapDFS.SwapTile(obstacleDFS, normal);
        tilemapDFS.SwapTile(endDFS, normal);
        tilemapDFS.SwapTile(startDFS, normal);
        tilemapDFS.SwapTile(winDFS, normal);
        tilemapDFS.SwapTile(startTileColored, normal);

        startButton.interactable = true;
        endButton.interactable = true;
        obstacleButton.interactable = true;
        startClicked = false;
        endClicked = false;
        startButtonDFS.interactable = true;
        endButtonDFS.interactable = true;
        obstacleButtonDFS.interactable = true;
        startClickedDFS = false;
        endClickedDFS = false;

        bottomTilemap.SwapTile(startTileColored, normal);
        bottomTilemap.SwapTile(endTileColored, normal);
        bottomTilemapDFS.SwapTile(startTileColored, normal);
        bottomTilemapDFS.SwapTile(endTileColored, normal);

        startPosition = Vector3Int.zero;
        endPosition = Vector3Int.zero;
        startPositionDFS = Vector3Int.zero;
        endPositionDFS = Vector3Int.zero;

        startAiButton.image.sprite = startAiButtonSprite;
        startAiButton.interactable = false;
        setStartAiButtonOnce = false;

        DFSWon.gameObject.SetActive(false);
        BFSWon.gameObject.SetActive(false);
        tie.gameObject.SetActive(false);
    }
}