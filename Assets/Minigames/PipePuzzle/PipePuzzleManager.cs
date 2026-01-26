using System.Collections.Generic;
using UnityEngine;
using Characters.Dialogue;

public class PipePuzzleManager : PuzzleManager
{
    [Header("Grid")]
    public int width = 6;
    public int height = 6;
    public float cellSize = 1f;

    [Header("Prefabs")]
    public PipePiece straightPrefab; // I
    public PipePiece cornerPrefab;   // L
    public PipePiece tPrefab;        // T
    public PipePiece plusPrefab;     // +

    [Header("Flow Materials")]
    public Material noFlowMaterial;
    public Material flowMaterial;
    public Material sourceMaterial;
    public Material sinkMaterial;

    private Dictionary<Vector2Int, PipePiece> grid = new();
    private Dictionary<PipePiece, int> initialRotations = new(); // store rotations for reset
    private Vector3 gridOrigin;

    [Header("Puzzle Completion")]
    public TriggerDialogueEvent completionDialogue;

    protected void Awake()
    {
        gridOrigin = Vector3.zero; // LOCAL origin
        gridOrigin = transform.position;
        GeneratePuzzle();
        RecalculateFlow();
    }

    public override void OnPuzzleSolved()
    {
        base.OnPuzzleSolved();
        Debug.Log($"{gameObject.name} pipe puzzle solved!");

        if (completionDialogue != null)
            completionDialogue.TryTrigger();
    }

    // ===================== PUZZLE GENERATION =====================

    private void GeneratePuzzle()
    {
        ClearGrid();
        grid.Clear();
        initialRotations.Clear();

        Vector2Int start = new(0, Random.Range(0, height));
        Vector2Int end = new(width - 1, Random.Range(0, height));
        List<Vector2Int> path = GeneratePath(start, end);

        for (int i = 0; i < path.Count; i++)
        {
            Vector2Int cell = path[i];
            Vector2Int? prev = i > 0 ? path[i - 1] : null;
            Vector2Int? next = i < path.Count - 1 ? path[i + 1] : null;

            PipePiece pipe = SpawnPipeForPath(cell, prev, next);
            grid[cell] = pipe;

            if (i == 0) pipe.isSource = true;
            if (i == path.Count - 1) pipe.isSink = true;

            initialRotations[pipe] = pipe.CurrentRotation;
        }

        // Fill remaining cells with random pipes
        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        {
            Vector2Int cell = new(x, y);
            if (grid.ContainsKey(cell)) continue;

            PipePiece randomPipe = SpawnRandomPipe(cell);
            grid[cell] = randomPipe;
            initialRotations[randomPipe] = randomPipe.CurrentRotation;
        }

        // Randomize rotation for all pipes
        foreach (var pipe in grid.Values)
        {
            int r = Random.Range(0, 4);
            pipe.SetInitialRotation(r);
        }
    }

    private List<Vector2Int> GeneratePath(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> path = new();
        HashSet<Vector2Int> visited = new();

        Vector2Int current = start;
        path.Add(current);
        visited.Add(current);

        int maxIterations = width * height * 2;
        int iterations = 0;

        while (current != end && iterations < maxIterations)
        {
            iterations++;
            List<Vector2Int> options = new();

            if (current.x < end.x && IsValidCell(current + Vector2Int.right) && !visited.Contains(current + Vector2Int.right))
                options.Add(Vector2Int.right);
            if (current.x > end.x && IsValidCell(current + Vector2Int.left) && !visited.Contains(current + Vector2Int.left))
                options.Add(Vector2Int.left);
            if (current.y < end.y && IsValidCell(current + Vector2Int.up) && !visited.Contains(current + Vector2Int.up))
                options.Add(Vector2Int.up);
            if (current.y > end.y && IsValidCell(current + Vector2Int.down) && !visited.Contains(current + Vector2Int.down))
                options.Add(Vector2Int.down);

            if (options.Count == 0) return GeneratePath(start, end); // stuck -> restart

            options.Shuffle();
            Vector2Int next = current + options[0];
            current = next;
            path.Add(current);
            visited.Add(current);
        }

        return path;
    }

    private bool IsValidCell(Vector2Int cell) => cell.x >= 0 && cell.x < width && cell.y >= 0 && cell.y < height;

    private PipePiece SpawnPipeForPath(Vector2Int cell, Vector2Int? prev, Vector2Int? next)
    {
        HashSet<PipeDirection> needed = new();

        if (prev.HasValue) needed.Add(DirectionFromTo(cell, prev.Value));
        if (next.HasValue) needed.Add(DirectionFromTo(cell, next.Value));

        PipePiece prefab =
            needed.Count == 1 ? straightPrefab :
            needed.Count == 2 && IsStraight(needed) ? straightPrefab :
            needed.Count == 2 ? cornerPrefab :
            needed.Count == 3 ? tPrefab :
            plusPrefab;

        PipePiece pipe = Instantiate(prefab, transform);
        pipe.transform.localPosition = GridToLocal(cell);
        pipe.transform.localRotation = Quaternion.identity;
        pipe.shape = prefab.shape;
        pipe.isSource = false;
        pipe.isSink = false;
        pipe.puzzleManager = this;
        return pipe;
    }

    private PipePiece SpawnRandomPipe(Vector2Int cell)
    {
        PipePiece prefab = Random.value switch
        {
            < 0.4f => straightPrefab,
            < 0.7f => cornerPrefab,
            < 0.9f => tPrefab,
            _ => plusPrefab
        };

        PipePiece pipe = Instantiate(prefab, transform);
        pipe.transform.localPosition = GridToLocal(cell);
        pipe.transform.localRotation = Quaternion.identity;
        pipe.shape = prefab.shape;
        pipe.puzzleManager = this;
        
        return pipe;
    }

    private void ClearGrid()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }

    private PipeDirection DirectionFromTo(Vector2Int from, Vector2Int to)
    {
        Vector2Int d = to - from;
        if (d == Vector2Int.up) return PipeDirection.Up;
        if (d == Vector2Int.right) return PipeDirection.Right;
        if (d == Vector2Int.down) return PipeDirection.Down;
        return PipeDirection.Left;
    }

    private bool IsStraight(HashSet<PipeDirection> dirs)
        => (dirs.Contains(PipeDirection.Up) && dirs.Contains(PipeDirection.Down)) ||
           (dirs.Contains(PipeDirection.Left) && dirs.Contains(PipeDirection.Right));

    // FIXED: Use local space instead of world space
    private Vector3 GridToLocal(Vector2Int cell) => gridOrigin + new Vector3(cell.x * cellSize, 0f, cell.y * cellSize);

    private Vector2Int DirToOffset(PipeDirection dir) => dir switch
    {
        PipeDirection.Up => Vector2Int.up,
        PipeDirection.Right => Vector2Int.right,
        PipeDirection.Down => Vector2Int.down,
        PipeDirection.Left => Vector2Int.left,
        _ => Vector2Int.zero
    };
    
    private PipeDirection Opposite(PipeDirection dir) => (PipeDirection)(((int)dir + 2) % 4);

    // ===================== FLOW =====================
    public void RecalculateFlow()
    {
        foreach (var pipe in grid.Values)
        {
            if (pipe.isSource) ApplyMaterial(pipe, sourceMaterial);
            else if (pipe.isSink) ApplyMaterial(pipe, sinkMaterial);
            else ApplyMaterial(pipe, noFlowMaterial);
        }

        bool sinkReached = false;

        foreach (var pipe in grid.Values)
        {
            if (pipe.isSource && FloodFrom(pipe))
                sinkReached = true;
        }

        if (sinkReached) OnPuzzleSolved();
    }

    private bool FloodFrom(PipePiece source)
    {
        Queue<(PipePiece, PipeDirection)> queue = new();
        HashSet<PipePiece> visited = new();

        bool sinkReached = false;
        visited.Add(source);

        foreach (var dir in source.GetOpenDirections())
            queue.Enqueue((source, dir));

        while (queue.Count > 0)
        {
            var (current, outDir) = queue.Dequeue();
            Vector2Int nextCell = LocalToGrid(current.transform.localPosition) + DirToOffset(outDir);

            if (!grid.TryGetValue(nextCell, out PipePiece next)) continue;
            if (visited.Contains(next)) continue;

            PipeDirection opposite = Opposite(outDir);
            if (!next.GetOpenDirections().Contains(opposite)) continue;

            if (next.isSink) sinkReached = true;
            if (!next.isSource && !next.isSink) ApplyMaterial(next, flowMaterial);

            visited.Add(next);

            foreach (var dir in next.GetOpenDirections())
                if (dir != opposite)
                    queue.Enqueue((next, dir));
        }

        return sinkReached;
    }

    private void ApplyMaterial(PipePiece pipe, Material mat)
    {
        if (!pipe.visualRoot) return;
        foreach (var r in pipe.visualRoot.GetComponentsInChildren<MeshRenderer>(true))
            r.material = mat;
    }

    // FIXED: Convert local position to grid coordinates
    private Vector2Int LocalToGrid(Vector3 localPos)
    {
        Vector3 relative = localPos - gridOrigin;
        return new(Mathf.RoundToInt(relative.x / cellSize), Mathf.RoundToInt(relative.z / cellSize));
    }

    // ===================== RESET =====================
    public override void ResetPuzzle()
    {
        base.ResetPuzzle();
        Debug.Log($"{gameObject.name} pipe puzzle reset.");

        foreach (var kvp in initialRotations)
            kvp.Key.SetInitialRotation(kvp.Value);

        foreach (var pipe in grid.Values)
        {
            if (pipe.isSource) ApplyMaterial(pipe, sourceMaterial);
            else if (pipe.isSink) ApplyMaterial(pipe, sinkMaterial);
            else ApplyMaterial(pipe, noFlowMaterial);
        }

        isSolved = false;
    }
}