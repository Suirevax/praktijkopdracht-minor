using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://www.youtube.com/watch?v=CSeUMTaNFYk

public class FieldOfView : MonoBehaviour
{

    [SerializeField] LayerMask layerMask;


    // Start is called before the first frame update
    void Update()
    {
        Mesh mesh = new Mesh();

        float fov = 360f;
        int rayCount = 360;

        float angle = 0f;
        float angleIncrease = fov / rayCount;
        float viewDistance = 5f;

        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = Vector3.zero;


        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            float angleRad = angle * Mathf.Deg2Rad;
            var vectorangle = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
            RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, vectorangle, viewDistance, layerMask);

            Vector3 vertex;


            if (raycastHit.collider == null)
            {
                vertex = Vector3.zero + (vectorangle * viewDistance);
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

        GetComponent<MeshFilter>().mesh = mesh;

    }

    //public void SetOrigin(Vector3 newOrigin)
    //{
    //    origin = newOrigin;
    //}
}
