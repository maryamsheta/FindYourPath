using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.IO;

public class ProgrammerWatch : MonoBehaviour
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
    public Vector3Int intersection;

    private bool isStopped;
    bool doneOne;
    bool doneTwo;   

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

        if(doneOne&&doneTwo)
        {
            doneOne = false;
            doneTwo = false;
            tilemap.SwapTile(start, startTileColored);
            tilemap.SwapTile(end, endTileColored);
            tilemap.SetTile(intersection, win);
            bottomTilemap.SetTile(intersection, normal);
            startAiButton.interactable = false;
            resetTilesButton.interactable = true;
            winText.gameObject.SetActive(true);
        }

    }

    public void ResetTiles()
    {
        tilemap.SwapTile(obstacle, normal);
        tilemap.SwapTile(end, normal);
        tilemap.SwapTile(start, normal);
        tilemap.SwapTile(win, normal);
        tilemap.SwapTile(startTileColored, normal);
        tilemap.SwapTile(endTileColored, normal);
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
        intersection = Vector3Int.zero;
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
            BS(startPosition, endPosition);

        } else
        {
            isStopped = true;
            startAiButton.image.sprite = startAiButtonSprite;
            resetTilesButton.interactable = true;
        }

    }

    private void BS(Vector3Int startPosition, Vector3Int endPosition)
    {
        Dictionary<Vector3Int, Vector3Int> visitedStart = new Dictionary<Vector3Int, Vector3Int>();
        Dictionary<Vector3Int, Vector3Int> visitedEnd = new Dictionary<Vector3Int, Vector3Int>();
        Queue<Vector3Int> queueStart = new Queue<Vector3Int>();
        Queue<Vector3Int> queueEnd = new Queue<Vector3Int>();

        List<Vector3Int> pathStart = new List<Vector3Int>();
        List<Vector3Int> pathEnd = new List<Vector3Int>();

        queueStart.Enqueue(startPosition);
        queueEnd.Enqueue(endPosition);

        visitedStart[startPosition] = startPosition;
        visitedEnd[endPosition] = endPosition;

        while (queueStart.Count > 0 && queueEnd.Count > 0)
        {
            Vector3Int currentStart = queueStart.Dequeue();
            Vector3Int currentEnd = queueEnd.Dequeue();

            if (visitedEnd.ContainsKey(currentStart))
            {
                Vector3Int currentS = currentStart;
                Vector3Int currentE = currentStart;

                while (currentS != startPosition)
                {
                    pathStart.Add(currentS);
                    currentS = visitedStart[currentS];
                }
                pathStart.Reverse();

                while (currentE != endPosition)
                {
                    pathEnd.Add(currentE);
                    currentE = visitedEnd[currentE];
                }
                pathEnd.Reverse();

                StartCoroutine(animationStart(pathStart));
                StartCoroutine(animationEnd(pathEnd));
                intersection = currentStart;
                return;
            }

            if (visitedStart.ContainsKey(currentEnd))
            {
                Vector3Int currentS = currentEnd;
                Vector3Int currentE = currentEnd;

                while (currentS != startPosition)
                {
                    pathStart.Add(currentS);
                    currentS = visitedStart[currentS];
                }
                pathStart.Reverse();

                while (currentE != endPosition)
                {
                    pathEnd.Add(currentE);
                    currentE = visitedEnd[currentE];
                }
                pathEnd.Reverse();

                StartCoroutine(animationStart(pathStart));
                StartCoroutine(animationEnd(pathEnd));
                intersection = currentEnd;
                return;
            }


            foreach (var item in GetNeighbors(currentStart))
            {
                if (!visitedStart.ContainsKey(item))
                {

                    visitedStart[item] = currentStart;
                    queueStart.Enqueue(item);
                }
            }

            foreach (var item in GetNeighbors(currentEnd))
            {
                if (!visitedEnd.ContainsKey(item))
                {
                    visitedEnd[item] = currentEnd;
                    queueEnd.Enqueue(item);
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

    IEnumerator animationStart(List<Vector3Int> path)
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
                tilemap.SetTile(startPosition, startTileColored);
                tilemap.SetTile(path[i - 1], startTileColored);
                tilemap.SetTile(path[i], start);
                bottomTilemap.SetTile(path[i], startTileColored);
            }
        }
        doneTwo = true;
    }

    IEnumerator animationEnd(List<Vector3Int> path)
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
                tilemap.SetTile(endPosition, endTileColored);
                tilemap.SetTile(path[i - 1], endTileColored);
                tilemap.SetTile(path[i], end);
                bottomTilemap.SetTile(path[i], endTileColored);
            }
        }
        doneOne = true;

    }
}
