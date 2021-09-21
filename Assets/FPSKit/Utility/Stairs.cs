using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Stairs : MonoBehaviour
{
    [Header("Generation")]
    [SerializeField]
    float width = 3f;
    [SerializeField]
    float height = 3f;
    [SerializeField]
    float depth = 3f;
    [SerializeField]
    float stepHeight = 0.2f;

    private void Awake()
    {
        Generate();
    }

    private void OnValidate()
    {
        Generate();
    }

    bool initialized = false;

    void Generate(bool awake = false)
    {
        var mf = GetComponent<MeshFilter>();
        if (mf == null)
            mf = gameObject.AddComponent<MeshFilter>();

        Mesh mesh;
        if(!initialized)
        {
            mesh = new Mesh();
            mf.sharedMesh = mesh;
            initialized = true;
        }
        else
        {
            mesh = mf.sharedMesh;
        }



        int numstairs = Mathf.FloorToInt(height / stepHeight);
        float sh = height / numstairs; // Actual Step Height
        float sd = depth / numstairs; // Actual Step Height

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> uvs = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> indices = new List<int>();

        Vector3 V3(float x, float y, float z)
        {
            return new Vector3(x, y, z);
        }

        Vector3 V2(float u, float v)
        {
            return new Vector2(u,v);
        }

        void AddVertex(Vector3 pos, Vector2 uv, Vector3 normal)
        {
            vertices.Add(pos);
            uvs.Add(uv);
            normals.Add(normal);
        }

        // Left Face
        Vector3 nrm = Vector3.left;
        AddVertex(V3(0, 0, depth),  V2(0,0), nrm);
        AddVertex(V3(0, 0, 0),      V2(1,0), nrm);

        for(int i = 0; i < numstairs; i++)
        {
            float s = 1f / numstairs;
            AddVertex(V3(0, (i+1) * sh, i * sd), V2(1f- i*s, (i+1)*s), nrm);
            AddVertex(V3(0, (i+1) * sh, (i+1) * sd), V2(1f- (i+1)*s, (i+1)*s), nrm);

            indices.Add(0);
            indices.Add(i * 2 + 2);
            indices.Add(i * 2 + 1);

            indices.Add(0);
            indices.Add(i * 2 + 3);
            indices.Add(i * 2 + 2);
        }

        // Right Face
        nrm = Vector3.right;
        AddVertex(V3(width, 0, depth), V2(0, 0), nrm);
        AddVertex(V3(width, 0, 0), V2(1, 0), nrm);

        int stride = (numstairs + 1) * 2;

        for (int i = 0; i < numstairs; i++)
        {
            float s = 1f / numstairs;
            AddVertex(V3(width, (i + 1) * sh, i * sd), V2(1f - i * s, (i + 1) * s), nrm);
            AddVertex(V3(width, (i + 1) * sh, (i + 1) * sd), V2(1f - (i + 1) * s, (i + 1) * s), nrm);

            indices.Add(stride);
            indices.Add(stride + i * 2 + 1);
            indices.Add(stride + i * 2 + 2);

            indices.Add(stride);
            indices.Add(stride + i * 2 + 2);
            indices.Add(stride + i * 2 + 3);
        }

        stride = vertices.Count;


        // Stair Steps
        for (int i = 0; i < numstairs; i++)
        {
            float s = 1f / numstairs;

            nrm = Vector3.back;
            AddVertex(V3(0, i * sh, i * sd),                V2(0, i * s)           , nrm);
            AddVertex(V3(0, (i + 1) * sh, i * sd),          V2(0, (i + 0.5f) * s)     , nrm);
            AddVertex(V3(width, i * sh, i * sd),            V2(1f, i * s)           , nrm);
            AddVertex(V3(width, (i + 1) * sh, i * sd),      V2(1f, (i + 0.5f) * s)     , nrm);

            nrm = Vector3.up;
            AddVertex(V3(0, (i + 1) * sh, i * sd),           V2(0, (i + 0.5f) * s)         , nrm);
            AddVertex(V3(0, (i + 1) * sh, (i + 1) * sd),     V2(0, (i + 1) * s)   , nrm);
            AddVertex(V3(width, (i + 1) * sh, i * sd),       V2(1, (i + 0.5f) * s)         , nrm);
            AddVertex(V3(width, (i + 1) * sh, (i + 1) * sd), V2(1, (i + 1) * s)   , nrm);


            indices.Add(stride + (i * 8) + 0);
            indices.Add(stride + (i * 8) + 3);
            indices.Add(stride + (i * 8) + 2);

            indices.Add(stride + (i * 8) + 0);
            indices.Add(stride + (i * 8) + 1);
            indices.Add(stride + (i * 8) + 3);

            indices.Add(stride + (i * 8) + 4);
            indices.Add(stride + (i * 8) + 7);
            indices.Add(stride + (i * 8) + 6);

            indices.Add(stride + (i * 8) + 4);
            indices.Add(stride + (i * 8) + 5);
            indices.Add(stride + (i * 8) + 7);
        }


        // Bottom 
        stride = vertices.Count;
        stride = vertices.Count;
        nrm = Vector3.down;
        AddVertex(new Vector3(0, 0, 0), new Vector2(0, 0), nrm);
        AddVertex(new Vector3(0, 0, depth), new Vector2(0, 1), nrm);
        AddVertex(new Vector3(width, 0, 0), new Vector2(1, 0), nrm);
        AddVertex(new Vector3(width, 0, depth), new Vector2(1, 1), nrm);

        indices.Add(stride + 0);
        indices.Add(stride + 2);
        indices.Add(stride + 1);

        indices.Add(stride + 1);
        indices.Add(stride + 2);
        indices.Add(stride + 3);

        // Back
        stride = vertices.Count;
        nrm = Vector3.forward;

        AddVertex(new Vector3(0, 0, depth), new Vector2(0, 0), nrm);
        AddVertex(new Vector3(0, height, depth), new Vector2(0, 1), nrm);
        AddVertex(new Vector3(width, 0, depth), new Vector2(1, 0), nrm);
        AddVertex(new Vector3(width, height, depth), new Vector2(1, 1), nrm);

        indices.Add(stride + 0);
        indices.Add(stride + 2);
        indices.Add(stride + 1);

        indices.Add(stride + 1);
        indices.Add(stride + 2);
        indices.Add(stride + 3);

        mesh.Clear();
        mesh.name = "Stairs (Generated)";
        mesh.SetVertices(vertices);
        mesh.SetUVs(0,uvs);
        mesh.SetNormals(normals);
        mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);


        if(TryGetComponent(out MeshCollider collider))
        {
            collider.sharedMesh = mesh;
            // Hack : Force Regenerate of the collider by switching convex flag twice
            collider.convex = !collider.convex;
            collider.convex = !collider.convex;
        }
    }
}
