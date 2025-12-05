using UnityEngine;
using System.Collections.Generic;

public class CameraCubemapRenderer : MonoBehaviour
{
    public Camera mainCamera;       // Your main camera
    public Camera captureCamera;    // Camera for cubemap capture
    public Material material;       // Material applied to fragments
    public Cubemap worldCubemap;    // Cubemap to store snapshot
    public GameObject[] fragments;  // Fragments to shatter
    public bool updating = true;    // Update cubemap each frame
    public GameObject world;
    public Material newSkybox;
    void LateUpdate()
    {
        if (updating)
        {
            UpdateProjection();
            material.SetFloat("_UseOrigPos", 0f); // Use current positions
        }
        else
        {
            material.SetFloat("_UseOrigPos", 1f); // Use stored original positions
        }
    }

    void UpdateProjection()
    {
        // Match capture camera to main camera
        captureCamera.transform.position = mainCamera.transform.position;
        captureCamera.transform.rotation = mainCamera.transform.rotation;
        captureCamera.fieldOfView = mainCamera.fieldOfView;
        captureCamera.nearClipPlane = mainCamera.nearClipPlane;
        captureCamera.farClipPlane = mainCamera.farClipPlane;

        // Render cubemap
        captureCamera.RenderToCubemap(worldCubemap);

        // Update global material
        material.SetTexture("_Cube", worldCubemap);
        material.SetVector("_CamPos", mainCamera.transform.position);
    }

    [ContextMenu("Snap")]
    void CaptureSnapshot()
    {
        // Capture cubemap snapshot
        captureCamera.RenderToCubemap(worldCubemap);

        // Store original vertex positions per fragment
        foreach (var frag in fragments)
        {
            MeshFilter mf = frag.GetComponent<MeshFilter>();
            if (mf == null) continue;

            Mesh mesh = mf.mesh;
            Vector3[] vertices = mesh.vertices;

            List<Vector2> uv2 = new List<Vector2>(vertices.Length); // store x, y
            List<Vector2> uv3 = new List<Vector2>(vertices.Length); // store z, 0

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 worldPos = frag.transform.TransformPoint(vertices[i]);
                uv2.Add(new Vector2(worldPos.x, worldPos.y));
                uv3.Add(new Vector2(worldPos.z, 0));
            }

            mesh.SetUVs(1, uv2); // uv2
            mesh.SetUVs(2, uv3); // uv3

            // Update material for fragment
            var mat = frag.GetComponent<MeshRenderer>().material;
            mat.SetTexture("_Cube", worldCubemap);
            mat.SetVector("_CamPos", captureCamera.transform.position);
            
            frag.GetComponent<Rigidbody>().isKinematic = false;
        }
        world.SetActive(false);
        RenderSettings.skybox = newSkybox;
        DynamicGI.UpdateEnvironment();
        updating = false; // Stop updating after snapshot
    }
}
