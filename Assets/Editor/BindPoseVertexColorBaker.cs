using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BindPoseVertexColorBaker : AssetPostprocessor
{
    const string BakeFlag = "BakeBindPoseToVertexColor";

    void OnPostprocessModel(GameObject root)
    {
        var importer = assetImporter as ModelImporter;
        if (importer == null)
            return;

        // Only bake if explicitly enabled
        if (!importer.userData.Contains(BakeFlag))
            return;

        foreach (var mf in root.GetComponentsInChildren<MeshFilter>())
            Bake(mf.sharedMesh);

        foreach (var smr in root.GetComponentsInChildren<SkinnedMeshRenderer>())
            Bake(smr.sharedMesh);
    }
    
    public static void Bake(Mesh mesh)
    {
        if (mesh == null) throw new System.ArgumentNullException(nameof(mesh));
        if (!mesh.isReadable) 
            throw new System.InvalidOperationException("Mesh must be readable. Enable Read/Write in import settings.");

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        // 1. Accumulate area-weighted face normals per vertex index
        // This matches Blender's default smooth shading algorithm
        Vector3[] perIndexNormals = new Vector3[vertices.Length];
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int i0 = triangles[i];
            int i1 = triangles[i + 1];
            int i2 = triangles[i + 2];

            Vector3 v0 = vertices[i0];
            Vector3 v1 = vertices[i1];
            Vector3 v2 = vertices[i2];

            // Face normal (not normalized) - magnitude = 2 * area
            Vector3 faceNormal = Vector3.Cross(v1 - v0, v2 - v0).normalized;
            
            // Skip degenerate triangles
            float area = faceNormal.magnitude * 0.5f;
            if (area < 0.00001f) continue;
            
            // Add area-weighted contribution to each vertex
            perIndexNormals[i0] += faceNormal;
            perIndexNormals[i1] += faceNormal;
            perIndexNormals[i2] += faceNormal;
        }

        // 2. Group by position and average to smooth sharp edges
        const float tolerance = 0.0001f;
        var positionToIndices = new Dictionary<Vector3, List<int>>(new Vector3EqualityComparer(tolerance));
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 pos = vertices[i];
            if (!positionToIndices.TryGetValue(pos, out var indices))
            {
                indices = new List<int>();
                positionToIndices[pos] = indices;
            }
            indices.Add(i);
        }

        Vector3[] finalNormals = new Vector3[vertices.Length];
        foreach (var indices in positionToIndices.Values)
        {
            Vector3 groupNormal = Vector3.zero;
            foreach (int idx in indices)
                groupNormal += perIndexNormals[idx];
            
            groupNormal.Normalize();
            foreach (int idx in indices)
                finalNormals[idx] = groupNormal;
        }

        // 3. Write to UV1 (Vector4)
        Vector4[] uv1 = new Vector4[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 n = finalNormals[i];
            uv1[i] = new Vector4(
                n.x,  // [-1,1] → [0,1]
                n.y,
                n.z,
                0.0f
            );
        }
        mesh.SetUVs(1, uv1);
        
        Color[] colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 normalizedPos = vertices[i];
            colors[i] = new Color(normalizedPos.x, normalizedPos.y, normalizedPos.z, 1.0f);
        }
        mesh.colors = colors;
    }

    private static float CalculateAngleWeight(Vector3 vertex, Vector3 neighbor1, Vector3 neighbor2)
    {
        Vector3 edge1 = neighbor1 - vertex;
        Vector3 edge2 = neighbor2 - vertex;
        
        float length1 = edge1.magnitude;
        float length2 = edge2.magnitude;
        
        if (length1 < 0.00001f || length2 < 0.00001f)
            return 0f;
        
        // Calculate angle between edges using atan2 for numerical stability
        float dot = Vector3.Dot(edge1 / length1, edge2 / length2);
        dot = Mathf.Clamp(dot, -1f, 1f); // Prevent floating point errors
        
        float angle = Mathf.Acos(dot);
        return angle;
    }

    private class Vector3EqualityComparer : IEqualityComparer<Vector3>
    {
        private readonly float toleranceSquared;
        
        public Vector3EqualityComparer(float tolerance)
        {
            toleranceSquared = tolerance * tolerance;
        }
        
        public bool Equals(Vector3 a, Vector3 b)
        {
            return (a - b).sqrMagnitude < toleranceSquared;
        }
        
        public int GetHashCode(Vector3 v)
        {
            int x = Mathf.RoundToInt(v.x / 0.0001f);
            int y = Mathf.RoundToInt(v.y / 0.0001f);
            int z = Mathf.RoundToInt(v.z / 0.0001f);
            return (x * 73856093) ^ (y * 19349663) ^ (z * 83492791);
        }
    }
}