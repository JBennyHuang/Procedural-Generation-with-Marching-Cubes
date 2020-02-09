using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class Chunk : MonoBehaviour
{
    public float threshold = 0.5f;

    private MarchingCubes marcher;
    private Dictionary<Vector3, int> vertex_table = new Dictionary<Vector3, int>();
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    private float[,,] density_map;
    private MeshFilter mesh_filter;
    private MeshCollider mesh_collider;

    // Start is called before the first frame update
    void Start()
    {
        mesh_filter = GetComponent<MeshFilter>();
        mesh_collider = GetComponent<MeshCollider>();
        density_map = new float[
            Configuration.CHUNK_WIDTH + 1,
            Configuration.CHUNK_DEPTH + 1,
            Configuration.CHUNK_WIDTH + 1];
        this.marcher = new MarchingCubes(threshold);
        generate_density_map();
        generate_mesh();
    }

    private void generate_density_map()
    {
        Vector3 pos = gameObject.transform.position;

        for (int x = 0; x < Configuration.CHUNK_WIDTH + 1; x++)
        {
            for (int y = 0; y < Configuration.CHUNK_DEPTH + 1; y++)
            {
                for (int z = 0; z < Configuration.CHUNK_WIDTH + 1; z++)
                {
                    if (y == 0 || y == Configuration.CHUNK_DEPTH)
                    {
                        density_map[x, y, z] = 1f;
                    }
                    else
                    {
                        density_map[x, y, z] = PerlinNoise3D((x + pos.x) / 8f, (y + pos.y) / 8f, (z + pos.z) / 8f);
                    }
                }
            }
        }
    }

    private void generate_mesh()
    {
        vertex_table.Clear();
        vertices.Clear();
        triangles.Clear();

        for (int x = 0; x < Configuration.CHUNK_WIDTH; x++)
        {
            for (int y = 0; y < Configuration.CHUNK_DEPTH; y++)
            {
                for (int z = 0; z < Configuration.CHUNK_WIDTH; z++)
                {
                    marcher.march(new Vector3Int(x, y, z), density_map, vertices, triangles, vertex_table);
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh_filter.mesh = mesh;
        mesh_collider.sharedMesh = mesh;
    }

    private float PerlinNoise3D(float x, float y, float z)
    {
        float xy = Mathf.PerlinNoise(x, y);
        float yz = Mathf.PerlinNoise(y, z);
        float xz = Mathf.PerlinNoise(x, z);

        float yx = Mathf.PerlinNoise(y, x);
        float zy = Mathf.PerlinNoise(z, y);
        float zx = Mathf.PerlinNoise(z, x);

        float xyz = xy + yz + xz + yx + zy + zx;

        return xyz / 6f;
    }
}
