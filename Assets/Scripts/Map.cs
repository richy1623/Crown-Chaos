using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public Terrain terrain;

    public Transform playertrans;
    public Transform target;

    public float radius;
    private int layer = 0;

    public Vector3[,] points;
    public Vector3[,] waypoints;
    public Node[,] grid;
    public bool[,] valid;

    public float width, height;

    public int w, h;

    private Vector3 player;

    // Start is called before the first frame update
    void Awake()
    {
        w = (int)(width / radius / 2);
        h = (int)(height / radius / 2);


        valid = new bool[w, h];
        waypoints = new Vector3[w, h];
        grid = new Node[w, h];

        int layermask = 1 << layer;
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                Vector3 point = new Vector3(-83.5f + x * radius * 2 + radius, radius + 0.5f, -height / 2 + y * radius * 2 + radius);
                Collider[] hitColliders = Physics.OverlapSphere(point, radius, layermask);
                point.y = 0;
                grid[x,y] = new Node(!((hitColliders.Length > 0) || !CheckTexture(point)), point, x, y);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<Node> path;
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(width, 1, height));


        if (grid != null)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = (n.valid) ? Color.white : Color.red;
                if (nodeFromPos(playertrans.position).Equals(n)) Gizmos.color = Color.black;
                if (path != null && path.Contains(n)) Gizmos.color = Color.blue;
                Gizmos.DrawCube(n.pos-new Vector3(0, radius-0.15f,0), Vector3.one * (radius*2 - .1f));
            }
        }
    }

    public Node nodeFromPos(Vector3 pos)
    {
        return grid[(int)((pos.x + 83.5f) / radius / 2), (int)((pos.z + height / 2) / radius / 2)];
    }

    bool CheckTexture(Vector3 pos)
    {
        Vector3 terrainPosition = pos - terrain.transform.position;
        Vector3 mapPosition = new Vector3
        (terrainPosition.x / terrain.terrainData.size.x, 0,
        terrainPosition.z / terrain.terrainData.size.z);
        float xCoord = mapPosition.x * terrain.terrainData.alphamapWidth;
        float zCoord = mapPosition.z * terrain.terrainData.alphamapHeight;
        int posX = (int)xCoord;
        int posZ = (int)zCoord;
        //print(pos.x);
        float[,,] aMap = terrain.terrainData.GetAlphamaps(posX, posZ, 1, 1);

        return aMap[0, 0, 4] == 1 || aMap[0, 0, 5] == 1 || aMap[0, 0, 6] == 1;
    }

    private int[] posToIndex(Vector3 pos)
    {
        return new int[] { (int)((pos.x + 83.5f) / radius / 2), (int)((pos.z + height / 2) / radius / 2) };
    }

    public Vector3 getTarget()
    {
        int x = Random.Range(0, w - 1);
        int y = Random.Range(0, h - 1);
        //print(x + " " + y);
        Node n = grid[x, y];
        if (n.valid) return n.pos;
        for (; x < w; x++)
        {
            for (y=0; y < h; y++)
            {
               n = grid[x, y];
               if (n.valid) return n.pos;
            }
            for (y = h-1; y > 0; y--)
            {
                n = grid[x, y];
                if (n.valid) return n.pos;
            }
        }
        return grid[0,0].pos;
    }

    public List<Node> getNeighbours(Node node)
    {
        List<Node> neibours = new List<Node>();
        /*for (int x = Mathf.Max(node.gridX-1, 0); x <= Mathf.Min(node.gridX+1, w); x++)
        {
            for (int y = Mathf.Max(node.gridY-1, 0); y <= Mathf.Min(node.gridY+1, w); y++)
            {
                if (node.gridX == x && node.gridY == y) continue;
                neibours.Add(grid[x, y]);
            }
        }*/
        neibours.Add(grid[node.gridX+1, node.gridY]);
        neibours.Add(grid[node.gridX-1, node.gridY]);
        neibours.Add(grid[node.gridX, node.gridY+1]);
        neibours.Add(grid[node.gridX, node.gridY-1]);
        return neibours;
    }
}
