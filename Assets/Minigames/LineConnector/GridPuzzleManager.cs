using System.Collections.Generic;
using UnityEngine;

public class GridPuzzleManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int n = 5;
    public int pairCount = 2;
    public float spacing = 1.2f;
    public int minDistance = 2;

    [Header("Prefabs")]
    public LinePressurePlate platePrefab;

    private LinePressurePlate[,] grid;
    private Dictionary<int, PuzzlePair> pairs = new();
    private Dictionary<int, PathState> completedPaths = new();
    private Dictionary<int, Color> pairColors = new();

    private PathState currentPath;
    private bool puzzleCompleted = false;

    void Start()
    {
        GenerateGrid();
        GeneratePairsSolvable();
    }

    // ---------------- GRID ----------------

    void GenerateGrid()
    {
        grid = new LinePressurePlate[n, n];

        float halfSize = (n - 1) * spacing * 0.5f;
        Vector3 originOffset = new Vector3(-halfSize, 0f, -halfSize);

        for (int x = 0; x < n; x++)
        for (int y = 0; y < n; y++)
        {
            Vector3 localPos = new Vector3(x * spacing, 0f, y * spacing);

            var plate = Instantiate(
                platePrefab,
                transform.position + originOffset + localPos,
                Quaternion.identity,
                transform
            );

            plate.Init(this, new Vector2Int(x, y));
            grid[x, y] = plate;
        }
    }

    // ---------------- PAIRS ----------------

    void GeneratePairsSolvable()
    {
        bool[,] occupied = new bool[n, n];
        HashSet<Vector2Int> reservedDots = new();

        for (int i = 0; i < pairCount; i++)
        {
            List<Vector2Int> path = GenerateRandomPath(occupied, reservedDots);

            if (path == null || path.Count < 2)
            {
                Debug.LogError($"Failed to generate path for pair {i}");
                continue;
            }

            foreach (var cell in path)
                occupied[cell.x, cell.y] = true;

            Vector2Int startPos = path[0];
            Vector2Int endPos = path[^1];

            var start = grid[startPos.x, startPos.y];
            var end = grid[endPos.x, endPos.y];

            start.isDot = true;
            end.isDot = true;
            start.pairId = i;
            end.pairId = i;

            start.SetDotColor(GetColorForPair(i));
            end.SetDotColor(GetColorForPair(i));

            pairs[i] = new PuzzlePair
            {
                pairId = i,
                start = start,
                end = end
            };
        }
    }

    List<Vector2Int> GenerateRandomPath(bool[,] occupied, HashSet<Vector2Int> reservedDots)
    {
        int maxAttempts = 500;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector2Int start = GetRandomUnoccupiedCell(occupied);
            if (start.x == -1) return null;

            List<Vector2Int> path = new() { start };
            bool[,] localOccupied = (bool[,])occupied.Clone();
            localOccupied[start.x, start.y] = true;

            int targetLength = Random.Range(Mathf.Max(minDistance, 3), Mathf.Min(12, n * n / pairCount));

            for (int step = 0; step < targetLength; step++)
            {
                Vector2Int current = path[^1];
                var neighbors = GetUnoccupiedNeighbors(current, localOccupied);
                if (neighbors.Count == 0) break;

                Vector2Int next = neighbors[Random.Range(0, neighbors.Count)];
                path.Add(next);
                localOccupied[next.x, next.y] = true;
            }

            if (path.Count < 2) continue;

            Vector2Int end = path[^1];

            // Check if start and end meet minimum distance requirement
            int distance = Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y);
            if (distance < minDistance)
                continue;

            if (IsAdjacentToAnyDot(start, reservedDots)) continue;
            if (IsAdjacentToAnyDot(end, reservedDots)) continue;

            reservedDots.Add(start);
            reservedDots.Add(end);

            return path;
        }

        return null;
    }

    Vector2Int GetRandomUnoccupiedCell(bool[,] occupied)
    {
        List<Vector2Int> available = new();

        for (int x = 0; x < n; x++)
        for (int y = 0; y < n; y++)
            if (!occupied[x, y])
                available.Add(new Vector2Int(x, y));

        return available.Count == 0
            ? new Vector2Int(-1, -1)
            : available[Random.Range(0, available.Count)];
    }

    List<Vector2Int> GetUnoccupiedNeighbors(Vector2Int pos, bool[,] occupied)
    {
        List<Vector2Int> neighbors = new();
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left };

        foreach (var d in dirs)
        {
            Vector2Int npos = pos + d;
            if (npos.x >= 0 && npos.x < n &&
                npos.y >= 0 && npos.y < n &&
                !occupied[npos.x, npos.y])
            {
                neighbors.Add(npos);
            }
        }
        return neighbors;
    }

    bool IsAdjacentToAnyDot(Vector2Int pos, HashSet<Vector2Int> dots)
    {
        foreach (var d in dots)
            if (Mathf.Abs(pos.x - d.x) + Mathf.Abs(pos.y - d.y) == 1)
                return true;
        return false;
    }

    // ---------------- INPUT ----------------

    public void OnPlateStepped(LinePressurePlate plate)
    {
        if (puzzleCompleted) return;

        if (currentPath == null)
        {
            if (!plate.isDot) return;
            if (completedPaths.ContainsKey(plate.pairId)) return;

            StartPath(plate);
            return;
        }

        LinePressurePlate last = currentPath.plates[^1];

        if (plate == last) return;

        if (!IsAdjacent(last, plate))
        {
            CancelCurrentPath();
            return;
        }

        if (plate.occupied && plate.occupiedByPair != currentPath.pairId)
        {
            CancelCurrentPath();
            return;
        }

        AddPlateToPath(plate);
    }

    void StartPath(LinePressurePlate start)
    {
        currentPath = new PathState
        {
            pairId = start.pairId,
            startPlate = start
        };

        AddPlateToPath(start);
    }

    void AddPlateToPath(LinePressurePlate plate)
    {
        plate.SetOccupied(currentPath.pairId);
        currentPath.plates.Add(plate);

        if (plate.isDot &&
            plate.pairId == currentPath.pairId &&
            plate != currentPath.startPlate)
        {
            CompletePath();
        }
    }

    void CompletePath()
    {
        currentPath.completed = true;
        completedPaths[currentPath.pairId] = currentPath;
        currentPath = null;

        CheckPuzzleComplete();
    }

    void CancelCurrentPath()
    {
        if (currentPath == null) return;
        currentPath.Reset();
        currentPath = null;
    }

    // ---------------- VALIDATION ----------------

    bool IsAdjacent(LinePressurePlate a, LinePressurePlate b)
    {
        Vector2Int d = a.gridPos - b.gridPos;
        return Mathf.Abs(d.x) + Mathf.Abs(d.y) == 1;
    }

    void CheckPuzzleComplete()
    {
        if (completedPaths.Count == pairCount)
        {
            puzzleCompleted = true;
            ReleaseAllPlates();
        }
    }

    void ReleaseAllPlates()
    {
        float destroyDelay = 3f;

        for (int x = 0; x < n; x++)
        for (int y = 0; y < n; y++)
            if (grid[x, y] != null)
                grid[x, y].ReleaseAndDestroy(destroyDelay);
    }

    public bool IsPuzzleCompleted() => puzzleCompleted;

    // ---------------- RESET ----------------

    public void ResetPuzzle()
    {
        if (puzzleCompleted)
        {
            Debug.Log("Cannot reset - puzzle already completed");
            return;
        }

        if (currentPath != null)
        {
            currentPath.Reset();
            currentPath = null;
        }

        foreach (var p in completedPaths.Values)
            p.Reset();

        completedPaths.Clear();

        for (int x = 0; x < n; x++)
        for (int y = 0; y < n; y++)
            if (grid[x, y] != null && !grid[x, y].isDot)
                grid[x, y].Clear();
    }

    // ---------------- COLORS ----------------

    public Color GetColorForPair(int pairId)
    {
        if (!pairColors.TryGetValue(pairId, out Color c))
        {
            c = GenerateColor(pairId);
            pairColors[pairId] = c;
        }
        return c;
    }

    Color GenerateColor(int pairId)
    {
        float h = (pairId * 0.6180339887f) % 1f;
        return Color.HSVToRGB(h, 0.75f, 0.9f);
    }
}