using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class LineOfSight : MonoBehaviour
{
    [SerializeField] private float viewAngle;
    [SerializeField] private float viewDistance;
    [SerializeField] private int rayCount;
    [SerializeField] private LayerMask ignoreLayer;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material detectedMaterial;


    public string targetTag = "Player";
    private Mesh mesh;
    private MeshRenderer meshRenderer;


    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void FixedUpdate()
    {
        CheckHit();
    }

    private void CheckHit()
    {
        var origin = Vector3.zero;
        var angle = viewAngle / 2;
        var angleIncrease = viewAngle / rayCount;

        var vertices = new Vector3[rayCount + 1 + 1];
        var triangles = new int[rayCount * 3];

        vertices[0] = origin;
        var vertexIndex = 1;
        var triangleIndex = 0;
        Transform target = null;

        for (var i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;
            var tr = transform;
            var isHit = Physics.Raycast(tr.position, GetVectorFromAngle(angle - tr.rotation.eulerAngles.y),
                out var hit, viewDistance, ~ignoreLayer);

            if (isHit)
            {
                var hitTransform = hit.transform;
                vertex = transform.InverseTransformPoint(hit.point);
                if (hitTransform.CompareTag(targetTag))
                {
                    target = hitTransform;
                }
            }
            else
            {
                vertex = origin + GetVectorFromAngle(angle) * viewDistance;
            }

            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;
            angle -= angleIncrease;
        }

        meshRenderer.material = target ? detectedMaterial : defaultMaterial;
        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }

    private Vector3 GetVectorFromAngle(float angle)
    {
        var angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));
    }
}