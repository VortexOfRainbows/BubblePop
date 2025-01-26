using UnityEngine;

public class Interactablewater : MonoBehaviour
{
    [Header("Springs")]
    [SerializeField] private float springConstant = 1.4f;
    [SerializeField, Range(1, 10)] private int wavePropogationIterations = 8;
    [SerializeField, ]

    [Header("Mesh Generation")]
    [Range(2, 500)] public int NumOfXVertices = 70;
    public float Width = 10f;
    public float Height = 4f;
    public Material WaterMaterial;
    private const int num_of_y_vertices = 2;

    [Header("Gizmo")]
    public Color GizmoColor = Color.white;

    private Mesh mesh;
    private MeshRenderer meshRenderer;
    private MeshFilter meshfilter;
    private Vector3[] vertices;
    private int[] topverticesindex;

    private void Reset()
    {

    }
    //public void GenerateMesh()
    //{
    //    Mesh mesh = new Mesh();
    //    GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Quad);

    //    mesh.vertices = new Vector3[NumOfXVertices * num_of_y_vertices];
    //    topverticesindex = new int[NumOfXVertices];
    //    for (int y = 0; y < num_of_y_vertices; y++) 
    //    {
    //        for (int x = 0; x < NumOfXVertices; x++)
    //        {
    //            float xPos = (x / (float)(NumOfXVertices - 1)) * Width - Width/2;
    //            float yPos = (y / (float)(num_of_y_vertices - 1)) * Height - Height / 2; 
    //            vertices[y * NumOfXVertices + x] = new Vector3 (xPos, yPos, 0f);

    //            if (y == num_of_y_vertices -1)
    //            {
    //                topverticesindex[x] = y * NumOfXVertices + x;
    //            }

    //        }
    //    }



    //    //UVs
    //    Vector2[] uvs = new Vector2[vertices.Length];
    //    for(int i = 0; i < vertices.Length; i++)
    //    {
    //        uvs[i] = new Vector2((vertices[i].x + Width /2) / Width, (vertices[i].y + Height / 2) / Height);
    //    }

    //    if (meshRenderer == null)
    //        meshRenderer = GetComponent <MeshRenderer>();

    //    if (meshfilter == null)
    //        meshfilter = GetComponent <MeshFilter>();

    //    meshRenderer.material = WaterMaterial;

    //    mesh.vertices = vertices;
    //    mesh.uv = uvs;
    //    //will need mesh of capsule here

    //    meshfilter.mesh = mesh;
    //}
}
