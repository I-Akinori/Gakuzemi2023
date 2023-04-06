using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProceduralModeling_AI
{
    public class Plane : MonoBehaviour
    {
        [SerializeField, Range(1, 30)]
        protected int widthSegments = 8, heightSegments = 8;
        [SerializeField, Range(0.1f, 10f)]
        protected float width = 1f, height = 1f;
        [SerializeField, Range(0.0f, 1f)]
        protected float depth = 0.5f;
        [SerializeField, Range(0.1f, 10f)]
        protected float frequency = 2f;

        [SerializeField]
        Material mat;
        MeshFilter filt;
        MeshRenderer rend;

        // Start is called before the first frame update
        private void Start()
        {
            if (TryGetComponent<MeshFilter>(out MeshFilter mf))
            {
                filt = mf;
            }
            else
            {
                filt = this.gameObject.AddComponent<MeshFilter>();
            }
            if (TryGetComponent<MeshRenderer>(out MeshRenderer mr))
            {
                rend = mr;
            }
            else
            {
                rend = this.gameObject.AddComponent<MeshRenderer>();
            }
        }
        private Mesh Build()
        {
            var mesh = new Mesh();
            var vertices = new List<Vector3>();
            var uv = new List<Vector2>();
            var normals = new List<Vector3>();
            var triangles = new List<int>();
            
            for (int z = 0; z < heightSegments + 1; z++)
            {
                // s‚ÌˆÊ’u‚ÌŠ„‡(0.0 ~ 1.0)
                var rz = (float)z / heightSegments;
                for (int x = 0; x < widthSegments + 1; x++)
                {
                    // —ñ‚ÌˆÊ’u‚ÌŠ„‡(0.0 ~ 1.0)
                    var rx = (float)x / widthSegments;

                    vertices.Add(new Vector3(
                        (rx - 0.5f) * width,
                        Mathf.PerlinNoise(rx * frequency, rz * frequency) * depth,
                        (rz - 0.5f) * height
                    ));
                    uv.Add(new Vector2(rx, rz));
                    normals.Add(new Vector3(0f, 1f, 0f));
                }
            }

            for (int z = 0; z < heightSegments; z++)
            {
                for (int x = 0; x < widthSegments; x++)
                {
                    int index = z * (widthSegments + 1) + x;
                    var a = index;
                    var b = index + (widthSegments + 1);
                    var c = index + 1;
                    var d = index + (widthSegments + 1) + 1;

                    triangles.Add(a);
                    triangles.Add(b);
                    triangles.Add(c);

                    triangles.Add(b);
                    triangles.Add(d);
                    triangles.Add(c);
                }
            }
            mesh.vertices = vertices.ToArray();
            mesh.uv = uv.ToArray();
            mesh.normals = normals.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            return mesh;
        }

        // Update is called once per frame
        void Update()
        {
            rend.material = mat;
            filt.mesh = Build();
        }
    } // class
} // namespace