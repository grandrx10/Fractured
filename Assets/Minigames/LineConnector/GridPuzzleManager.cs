using System.Collections.Generic;
using UnityEngine;

public class GridPuzzleManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public float spacing = 1.2f;
    public TextAsset puzzleFile;

    [Header("Prefabs")]
    public LinePressurePlate platePrefab;

    private int n;
    private int pairCount;

    private LinePressurePlate[,] grid;
    private Dictionary<int, PuzzlePair> pairs = new();
    private Dictionary<int, PathState> completedPaths = new();
    private Dictionary<int, Color> pairColors = new();

    private PathState currentPath;
    private bool puzzleCompleted = false;

    void Start()
    {
        LoadPuzzleFromFile();
        GenerateGrid();
        PlacePairsFromFile();
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

    void LoadPuzzleFromFile()
    {
        if (puzzleFile == null)
        {
            Debug.LogError("No puzzle file assigned!");
            return;
        }

        string[] lines = puzzleFile.text.Split('\n');
        
        // Remove empty lines and trim
        List<string> validLines = new();
        foreach (string line in lines)
        {
            string trimmed = line.Trim();
            if (!string.IsNullOrEmpty(trimmed))
                validLines.Add(trimmed);
        }

        if (validLines.Count == 0)
        {
            Debug.LogError("Puzzle file is empty!");
            return;
        }

        n = validLines.Count;
        
        // Parse the grid and count pairs
        HashSet<int> uniquePairs = new();
        
        for (int y = 0; y < n; y++)
        {
            string[] cells = validLines[y].Split(' ');
            
            if (cells.Length != n)
            {
                Debug.LogError($"Row {y} has {cells.Length} cells, expected {n}");
                return;
            }

            for (int x = 0; x < n; x++)
            {
                string cell = cells[x].Trim();
                if (cell != ".")
                {
                    if (int.TryParse(cell, out int pairId))
                    {
                        uniquePairs.Add(pairId);
                    }
                }
            }
        }

        pairCount = uniquePairs.Count;
        Debug.Log($"Loaded {n}x{n} puzzle with {pairCount} pairs");
    }

    void PlacePairsFromFile()
    {
        if (puzzleFile == null) return;

        string[] lines = puzzleFile.text.Split('\n');
        List<string> validLines = new();
        foreach (string line in lines)
        {
            string trimmed = line.Trim();
            if (!string.IsNullOrEmpty(trimmed))
                validLines.Add(trimmed);
        }

        Dictionary<int, List<LinePressurePlate>> pairPlates = new();

        for (int y = 0; y < n; y++)
        {
            string[] cells = validLines[y].Split(' ');

            for (int x = 0; x < n; x++)
            {
                string cell = cells[x].Trim();
                
                if (cell != "." && int.TryParse(cell, out int pairId))
                {
                    var plate = grid[x, n - 1 - y]; // Flip y to match Unity coordinates
                    plate.isDot = true;
                    plate.pairId = pairId;
                    plate.SetDotColor(GetColorForPair(pairId));

                    if (!pairPlates.ContainsKey(pairId))
                        pairPlates[pairId] = new();
                    
                    pairPlates[pairId].Add(plate);
                }
            }
        }

        // Create pairs
        foreach (var kvp in pairPlates)
        {
            int pairId = kvp.Key;
            var plates = kvp.Value;

            if (plates.Count != 2)
            {
                Debug.LogError($"Pair {pairId} has {plates.Count} dots, expected 2!");
                continue;
            }

            pairs[pairId] = new PuzzlePair
            {
                pairId = pairId,
                start = plates[0],
                end = plates[1]
            };
        }
    }

    // ---------------- INPUT ----------------

    public void OnPlateStepped(LinePressurePlate plate)
    {
        if (puzzleCompleted) return;

        // ---------------- START PATH ----------------
        if (currentPath == null)
        {
            if (!plate.isDot) return;
            if (completedPaths.ContainsKey(plate.pairId)) return;

            StartPath(plate);
            return;
        }

        LinePressurePlate last = currentPath.plates[^1];

        // Ignore staying on same plate
        if (plate == last) return;

        // Must be adjacent
        if (!IsAdjacent(last, plate))
        {
            CancelCurrentPath();
            return;
        }

        // ❌ Hit another pair's path
        if (plate.occupied && plate.occupiedByPair != currentPath.pairId)
        {
            CancelCurrentPath();
            return;
        }

        // ❌ Hit another pair's dot (even if not occupied)
        if (plate.isDot && plate.pairId != currentPath.pairId)
        {
            CancelCurrentPath();
            return;
        }

        // ✅ Valid move
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
        // Prevent retracing - can't step on a plate already in current path
        if (currentPath.plates.Contains(plate))
        {
            CancelCurrentPath();
            return;
        }

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
        // Check if all pairs are completed
        if (completedPaths.Count != pairCount)
            return;

        // Check if entire board is filled
        for (int x = 0; x < n; x++)
        for (int y = 0; y < n; y++)
        {
            if (grid[x, y] != null && !grid[x, y].occupied)
            {
                // Board not fully filled yet
                return;
            }
        }

        // All pairs complete AND board fully filled
        puzzleCompleted = true;
        ReleaseAllPlates();
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