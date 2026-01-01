using System.Collections.Generic;
using UnityEngine;

public class GridPuzzleManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int n = 5;
    public int pairCount = 2;
    public float spacing = 1.2f;

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
            Vector3 localPos = new Vector3(
                x * spacing,
                0f,
                y * spacing
            );

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
        // Track which cells are occupied by solution paths
        bool[,] occupied = new bool[n, n];

        for (int i = 0; i < pairCount; i++)
        {
            // Find a valid path for this pair
            List<Vector2Int> path = GenerateRandomPath(occupied);
            
            if (path == null || path.Count < 2)
            {
                Debug.LogError($"Failed to generate path for pair {i}");
                continue;
            }

            // Mark path cells as occupied
            foreach (var cell in path)
                occupied[cell.x, cell.y] = true;

            // Get start and end plates
            Vector2Int startPos = path[0];
            Vector2Int endPos = path[path.Count - 1];

            var start = grid[startPos.x, startPos.y];
            var end = grid[endPos.x, endPos.y];

            // Set up the pair
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

            Debug.Log($"Generated pair {i}: {startPos} -> {endPos} with path length {path.Count}");
        }
    }

    List<Vector2Int> GenerateRandomPath(bool[,] occupied)
    {
        int maxAttempts = 100;
        
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // Pick random start position that's not occupied
            Vector2Int start = GetRandomUnoccupiedCell(occupied);
            if (start.x == -1) return null;

            // Generate path using random walk
            List<Vector2Int> path = new List<Vector2Int> { start };
            bool[,] localOccupied = (bool[,])occupied.Clone();
            localOccupied[start.x, start.y] = true;

            // Random walk for 3-8 steps (or until we get stuck)
            int targetLength = Random.Range(3, Mathf.Min(9, n * n / pairCount));
            
            for (int step = 0; step < targetLength; step++)
            {
                Vector2Int current = path[path.Count - 1];
                List<Vector2Int> neighbors = GetUnoccupiedNeighbors(current, localOccupied);
                
                if (neighbors.Count == 0)
                    break; // Dead end
                
                // Pick random neighbor
                Vector2Int next = neighbors[Random.Range(0, neighbors.Count)];
                path.Add(next);
                localOccupied[next.x, next.y] = true;
            }

            // Valid path if it's at least 2 cells long
            if (path.Count >= 2)
                return path;
        }

        return null;
    }

    Vector2Int GetRandomUnoccupiedCell(bool[,] occupied)
    {
        List<Vector2Int> available = new List<Vector2Int>();
        
        for (int x = 0; x < n; x++)
        for (int y = 0; y < n; y++)
        {
            if (!occupied[x, y])
                available.Add(new Vector2Int(x, y));
        }

        if (available.Count == 0)
            return new Vector2Int(-1, -1);

        return available[Random.Range(0, available.Count)];
    }

    List<Vector2Int> GetUnoccupiedNeighbors(Vector2Int pos, bool[,] occupied)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        Vector2Int[] directions = {
            new Vector2Int(0, 1),   // up
            new Vector2Int(0, -1),  // down
            new Vector2Int(1, 0),   // right
            new Vector2Int(-1, 0)   // left
        };

        foreach (var dir in directions)
        {
            Vector2Int neighbor = pos + dir;
            
            if (neighbor.x >= 0 && neighbor.x < n &&
                neighbor.y >= 0 && neighbor.y < n &&
                !occupied[neighbor.x, neighbor.y])
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    // ---------------- INPUT ----------------

    public void OnPlateStepped(LinePressurePlate plate)
    {
        // Don't allow any input if puzzle is completed
        if (puzzleCompleted)
            return;

        if (currentPath == null)
        {
            if (!plate.isDot)
                return;
            
            // Don't allow starting a path if this pair is already completed
            if (completedPaths.ContainsKey(plate.pairId))
            {
                Debug.Log($"Pair {plate.pairId} is already completed");
                return;
            }
            
            Debug.Log($"Started path for pair {plate.pairId}");
            StartPath(plate);
            return;
        }

        LinePressurePlate last = currentPath.plates[^1];

        if (plate == last)
            return;

        if (!IsAdjacent(last, plate))
        {
            Debug.Log("Not adjacent - canceling path");
            CancelCurrentPath();
            return;
        }

        if (plate.occupied && plate.occupiedByPair != currentPath.pairId)
        {
            Debug.Log($"Plate occupied by different pair - canceling path");
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
            startPlate = start  // Remember which dot we started from
        };

        AddPlateToPath(start);
    }

    void AddPlateToPath(LinePressurePlate plate)
    {
        plate.SetOccupied(currentPath.pairId);
        currentPath.plates.Add(plate);

        Debug.Log($"Added plate {plate.gridPos} to path for pair {currentPath.pairId}");

        // Check if we've reached a dot
        if (plate.isDot)
        {
            // Make sure it's the correct pair's endpoint, not a different pair's dot
            if (plate.pairId != currentPath.pairId)
            {
                Debug.Log("Reached wrong pair's dot - canceling path");
                CancelCurrentPath();
                return;
            }
            
            // Make sure it's not the dot we started from
            if (plate != currentPath.startPlate)
            {
                Debug.Log("Path completed!");
                CompletePath();
            }
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
        if (currentPath != null)
        {
            currentPath.Reset();
            currentPath = null;
        }
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
            Debug.Log("🎉 PUZZLE SOLVED! 🎉");
        }
    }

    public bool IsPuzzleCompleted()
    {
        return puzzleCompleted;
    }

    // ---------------- RESET ----------------

    public void ResetPuzzle()
    {
        // Don't allow reset if puzzle is completed
        if (puzzleCompleted)
        {
            Debug.Log("Cannot reset - puzzle is completed!");
            return;
        }

        Debug.Log("Resetting puzzle...");
        
        // Cancel any current path
        if (currentPath != null)
        {
            currentPath.Reset();
            currentPath = null;
        }

        // Reset all completed paths
        foreach (var pathState in completedPaths.Values)
        {
            pathState.Reset();
        }
        completedPaths.Clear();

        // Clear all non-dot plates
        for (int x = 0; x < n; x++)
        for (int y = 0; y < n; y++)
        {
            var plate = grid[x, y];
            if (plate != null && !plate.isDot)
            {
                plate.Clear();
            }
        }

        Debug.Log("Puzzle reset complete!");
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
        float s = 0.75f;
        float v = 0.9f;
        return Color.HSVToRGB(h, s, v);
    }
}