using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://www.youtube.com/watch?v=CSeUMTaNFYk

public class FieldOfView : MonoBehaviour
{

    [SerializeField] LayerMask layerMask;
    float fov = 360f;
    int rayCount = 360;
    float viewDistance = 5f;

    Vector3[] vectorAngles;

    MeshFilter meshFilter = null;

    Mesh mesh;

    float angleIncrease;

    private void Start()
    {
        mesh = new Mesh();
        angleIncrease = fov / rayCount;
        meshFilter = GetComponent<MeshFilter>();

        CreateAngleTable();
    }

    void CreateAngleTable()
    {
        vectorAngles = new Vector3[rayCount + 1];

        float angle = 0f;

        for (int i = 0; i <= rayCount; i++)
        {
            float angleRad = angle * Mathf.Deg2Rad;
            vectorAngles[i] = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
            angle -= angleIncrease;
        }
    }


    // Start is called before the first frame update
    void Update()
    {
        float angle = 0f;

        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = Vector3.zero;


        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, vectorAngles[i], viewDistance, layerMask);

            Vector3 vertex;


            if (raycastHit.collider == null)
            {
                vertex = Vector3.zero + (vectorAngles[i] * viewDistance);
            }
            else
            {
                vertex =  (Vector3)raycastHit.point - transform.position;
            }

            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;

            angle -= angleIncrease;
        }


        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        meshFilter.mesh = mesh;

    }


}
