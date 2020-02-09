using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    public GameObject chunk;
    public GameObject viewer;

    public int num_chunk_x = 3;
    public int num_chunk_z = 3;

    private Dictionary<Vector2, GameObject> chunks = new Dictionary<Vector2, GameObject>();
    private Queue<GameObject> active_chunks = new Queue<GameObject>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 pos = viewer.transform.position;
        int x_pos = (int)pos.x / 32;
        int z_pos = (int)pos.z / 32;

        for (int i = 0; i < active_chunks.Count; i ++)
        {
            active_chunks.Dequeue().SetActive(false);            
        }

        for (int x = x_pos - Configuration.RENDER_DIST; x < x_pos + Configuration.RENDER_DIST; x ++)
        {
            for (int z = z_pos - Configuration.RENDER_DIST; z < z_pos + Configuration.RENDER_DIST; z ++)
            {
                Vector2 coords = new Vector2(x, z);
                if (chunks.ContainsKey(new Vector2(x, z)))
                {
                    chunks[coords].SetActive(true);
                    active_chunks.Enqueue(chunks[coords]);
                }
                else
                {
                    chunks.Add(new Vector2(x, z), Instantiate(chunk, new Vector3(x * Configuration.CHUNK_WIDTH, 0, z * Configuration.CHUNK_WIDTH), Quaternion.identity));
                    active_chunks.Enqueue(chunks[coords]);
                }
            }
        }
    }
}
