using System.Collections.Generic;
using UnityEngine;

public class GridPuzzleManager : PuzzleManager
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

    protected void Awake()
    {

        LoadPuzzleFromFile();
        GenerateGrid();
        PlacePairsFromFile();
    }

    public override void OnPuzzleSolved()
    {
        base.OnPuzzleSolved();

        Debug.Log($"{gameObject.name} grid puzzle completed!");

        // Optional: Release plates visually
        ReleaseAllPlates();
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

    void LoadPuzzleFromFile()
    {
        if (puzzleFile == null)
        {
            Debug.LogError("No puzzle file assigned!");
            return;
        }

        string[] lines = puzzleFile.text.Split('\n');
        List<string> validLines = new();
        foreach (string line in lines)
        {
            string trimmed = line.Trim();
            if (!string.IsNullOrEmpty(trimmed))
                validLines.Add(trimmed);
        }

        n = validLines.Count;
        HashSet<int> uniquePairs = new();
        for (int y = 0; y < n; y++)
        {
            string[] cells = validLines[y].Split(' ');
            for (int x = 0; x < n; x++)
            {
                string cell = cells[x].Trim();
                if (cell != "." && int.TryParse(cell, out int pairId))
                    uniquePairs.Add(pairId);
            }
        }

        pairCount = uniquePairs.Count;
        Debug.Log($"Loaded {n}x{n} grid puzzle with {pairCount} pairs");
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
                    var plate = grid[x, n - 1 - y]; // flip y to match Unity coordinates
                    plate.isDot = true;
                    plate.pairId = pairId;
                    plate.SetDotColor(GetColorForPair(pairId));

                    if (!pairPlates.ContainsKey(pairId))
                        pairPlates[pairId] = new();
                    
                    pairPlates[pairId].Add(plate);
                }
            }
        }

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
        if (isSolved) return;

        if (currentPath == null)
        {
            if (!plate.isDot || completedPaths.ContainsKey(plate.pairId)) return;
            StartPath(plate);
            return;
        }

        LinePressurePlate last = currentPath.plates[^1];

        if (plate == last) return; // same plate
        if (!IsAdjacent(last, plate) || (plate.occupied && plate.occupiedByPair != currentPath.pairId) || (plate.isDot && plate.pairId != currentPath.pairId))
        {
            CancelCurrentPath();
            return;
        }

        AddPlateToPath(plate);
    }

    void StartPath(LinePressurePlate start)
    {
        currentPath = new PathState { pairId = start.pairId, startPlate = start };
        AddPlateToPath(start);
    }

    void AddPlateToPath(LinePressurePlate plate)
    {
        if (currentPath.plates.Contains(plate))
        {
            CancelCurrentPath();
            return;
        }

        plate.SetOccupied(currentPath.pairId);
        currentPath.plates.Add(plate);

        if (plate.isDot && plate.pairId == currentPath.pairId && plate != currentPath.startPlate)
            CompletePath();
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
        if (completedPaths.Count != pairCount) return;

        for (int x = 0; x < n; x++)
        for (int y = 0; y < n; y++)
            if (grid[x, y] != null && !grid[x, y].occupied)
                return;

        // Puzzle fully completed
        OnPuzzleSolved();
    }

    void ReleaseAllPlates()
    {
        float destroyDelay = 3f;

        for (int x = 0; x < n; x++)
        for (int y = 0; y < n; y++)
            grid[x, y]?.ReleaseAndDestroy(destroyDelay);
    }

    // ---------------- RESET ----------------

    public override void ResetPuzzle()
    {
        base.ResetPuzzle();

        currentPath?.Reset();
        currentPath = null;

        foreach (var p in completedPaths.Values)
            p.Reset();
        completedPaths.Clear();

        for (int x = 0; x < n; x++)
        for (int y = 0; y < n; y++)
            if (grid[x, y] != null)
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
