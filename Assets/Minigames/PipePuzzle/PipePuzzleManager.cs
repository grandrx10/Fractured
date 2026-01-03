using System.Collections.Generic;
using UnityEngine;
using Characters.Dialogue;

public class PipePuzzleManager : MonoBehaviour
{
    public static PipePuzzleManager Instance;

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
    private Vector3 gridOrigin;

    [Header("Puzzle Completion")]
    [Tooltip("Trigger dialogue when puzzle is solved")]
    public TriggerDialogueEvent completionDialogue;


    private void Awake()
    {
        Instance = this;
        gridOrigin = transform.position;
        GeneratePuzzle();
        RecalculateFlow();
    }

    // ======================================================
    // PUZZLE GENERATION
    // ======================================================

    private void GeneratePuzzle()
    {
        ClearGrid();
        grid.Clear();

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
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int cell = new(x, y);
                if (grid.ContainsKey(cell)) continue;

                PipePiece randomPipe = SpawnRandomPipe(cell);
                grid[cell] = randomPipe;
            }
        }

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

        int maxIterations = width * height * 2; // Safety limit
        int iterations = 0;

        while (current != end && iterations < maxIterations)
        {
            iterations++;
            
            List<Vector2Int> options = new();

            // Prioritize moving toward the end
            if (current.x < end.x && IsValidCell(current + Vector2Int.right) && !visited.Contains(current + Vector2Int.right))
                options.Add(Vector2Int.right);
            
            if (current.x > end.x && IsValidCell(current + Vector2Int.left) && !visited.Contains(current + Vector2Int.left))
                options.Add(Vector2Int.left);
            
            if (current.y < end.y && IsValidCell(current + Vector2Int.up) && !visited.Contains(current + Vector2Int.up))
                options.Add(Vector2Int.up);
            
            if (current.y > end.y && IsValidCell(current + Vector2Int.down) && !visited.Contains(current + Vector2Int.down))
                options.Add(Vector2Int.down);

            // Add other valid directions as backup
            if (IsValidCell(current + Vector2Int.up) && !visited.Contains(current + Vector2Int.up) && !options.Contains(Vector2Int.up))
                options.Add(Vector2Int.up);
            
            if (IsValidCell(current + Vector2Int.down) && !visited.Contains(current + Vector2Int.down) && !options.Contains(Vector2Int.down))
                options.Add(Vector2Int.down);
            
            if (IsValidCell(current + Vector2Int.right) && !visited.Contains(current + Vector2Int.right) && !options.Contains(Vector2Int.right))
                options.Add(Vector2Int.right);
            
            if (IsValidCell(current + Vector2Int.left) && !visited.Contains(current + Vector2Int.left) && !options.Contains(Vector2Int.left))
                options.Add(Vector2Int.left);

            // If no options, we're stuck - restart path generation
            if (options.Count == 0)
            {
                Debug.LogWarning("Path generation stuck, restarting...");
                return GeneratePath(start, end); // Recursive restart
            }

            options.Shuffle();
            Vector2Int next = current + options[0];

            current = next;
            path.Add(current);
            visited.Add(current);
        }

        if (iterations >= maxIterations)
        {
            Debug.LogError("Path generation exceeded max iterations!");
            // Return a simple direct path as fallback
            return CreateDirectPath(start, end);
        }

        return path;
    }

    private List<Vector2Int> CreateDirectPath(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> path = new() { start };
        Vector2Int current = start;

        while (current.x != end.x)
        {
            current.x += (end.x > current.x) ? 1 : -1;
            path.Add(current);
        }

        while (current.y != end.y)
        {
            current.y += (end.y > current.y) ? 1 : -1;
            path.Add(current);
        }

        return path;
    }

    private bool IsValidCell(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < width && cell.y >= 0 && cell.y < height;
    }

    private PipePiece SpawnPipeForPath(
        Vector2Int cell,
        Vector2Int? prev,
        Vector2Int? next
    )
    {
        HashSet<PipeDirection> needed = new();

        if (prev.HasValue)
            needed.Add(DirectionFromTo(cell, prev.Value));

        if (next.HasValue)
            needed.Add(DirectionFromTo(cell, next.Value));

        PipePiece prefab =
            needed.Count == 1 ? straightPrefab :
            needed.Count == 2 && IsStraight(needed) ? straightPrefab :
            needed.Count == 2 ? cornerPrefab :
            needed.Count == 3 ? tPrefab :
            plusPrefab;

        PipePiece pipe = Instantiate(prefab, GridToWorld(cell), Quaternion.identity, transform);
        pipe.shape = prefab.shape;
        pipe.isSource = false;
        pipe.isSink = false;

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

        PipePiece pipe = Instantiate(prefab, GridToWorld(cell), Quaternion.identity, transform);
        pipe.shape = prefab.shape;
        return pipe;
    }

    private void ClearGrid()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }

    // ======================================================
    // FLOW LOGIC
    // ======================================================

    public void RecalculateFlow()
    {
        // First apply special materials to source and sink
        foreach (var pipe in grid.Values)
        {
            if (pipe.isSource)
                ApplyMaterial(pipe, sourceMaterial);
            else if (pipe.isSink)
                ApplyMaterial(pipe, sinkMaterial);
            else
                ApplyMaterial(pipe, noFlowMaterial);
        }

        bool sinkReached = false;

        // Then flood from sources (this will override non-special pipes)
        foreach (var pipe in grid.Values)
        {
            if (pipe.isSource)
            {
                if (FloodFrom(pipe))
                {
                    sinkReached = true;
                }
            }
        }

        // Trigger dialogue if puzzle solved
        if (sinkReached && completionDialogue != null)
        {
            completionDialogue.TryTrigger();
        }
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
            Vector2Int nextCell = WorldToGrid(current.transform.position) + DirToOffset(outDir);

            if (!grid.TryGetValue(nextCell, out PipePiece next)) continue;
            if (visited.Contains(next)) continue;

            PipeDirection opposite = Opposite(outDir);
            if (!next.GetOpenDirections().Contains(opposite)) continue;

            if (next.isSink)
            {
                sinkReached = true;
            }

            if (!next.isSource && !next.isSink)
                ApplyMaterial(next, flowMaterial);

            visited.Add(next);

            foreach (var dir in next.GetOpenDirections())
                if (dir != opposite)
                    queue.Enqueue((next, dir));
        }

        return sinkReached;
    }


    // ======================================================
    // VISUALS
    // ======================================================

    private void ApplyMaterial(PipePiece pipe, Material material)
    {
        if (!pipe.visualRoot) return;

        MeshRenderer[] renderers =
            pipe.visualRoot.GetComponentsInChildren<MeshRenderer>(true);

        foreach (var r in renderers)
            r.material = material;
    }

    // ======================================================
    // HELPERS
    // ======================================================

    private Vector3 GridToWorld(Vector2Int cell)
    {
        return gridOrigin + new Vector3(
            cell.x * cellSize,
            0f,
            cell.y * cellSize
        );
    }

    private Vector2Int WorldToGrid(Vector3 pos)
    {
        Vector3 local = pos - gridOrigin;

        return new(
            Mathf.RoundToInt(local.x / cellSize),
            Mathf.RoundToInt(local.z / cellSize)
        );
    }

    private Vector2Int DirToOffset(PipeDirection dir) => dir switch
    {
        PipeDirection.Up => Vector2Int.up,
        PipeDirection.Right => Vector2Int.right,
        PipeDirection.Down => Vector2Int.down,
        PipeDirection.Left => Vector2Int.left,
        _ => Vector2Int.zero
    };

    private PipeDirection Opposite(PipeDirection dir)
        => (PipeDirection)(((int)dir + 2) % 4);

    private PipeDirection DirectionFromTo(Vector2Int from, Vector2Int to)
    {
        Vector2Int d = to - from;

        if (d == Vector2Int.up) return PipeDirection.Up;
        if (d == Vector2Int.right) return PipeDirection.Right;
        if (d == Vector2Int.down) return PipeDirection.Down;
        return PipeDirection.Left;
    }

    private bool IsStraight(HashSet<PipeDirection> dirs)
    {
        return (dirs.Contains(PipeDirection.Up) && dirs.Contains(PipeDirection.Down)) ||
               (dirs.Contains(PipeDirection.Left) && dirs.Contains(PipeDirection.Right));
    }
}

public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}