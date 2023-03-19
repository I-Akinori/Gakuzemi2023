using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProceduralModeling_AI
{
    public class Plane : MonoBehaviour
    {
        [SerializeField, Range(2, 30)]
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
            var winv = 1f / (widthSegments - 1);
            var hinv = 1f / (heightSegments - 1);

            for (int z = 0; z < heightSegments; z++)
            {
                // s‚ÌˆÊ’u‚ÌŠ„‡(0.0 ~ 1.0)
                var rz = z * hinv;
                for (int x = 0; x < widthSegments; x++)
                {
                    // —ñ‚ÌˆÊ’u‚ÌŠ„‡(0.0 ~ 1.0)
                    var rx = x * winv;

                    vertices.Add(new Vector3(
                        (rx - 0.5f) * width,
                        Mathf.PerlinNoise(rx * frequency, rz * frequency) * depth,
                        (0.5f - rz) * height
                    ));
                    uv.Add(new Vector2(rx, 1.0f - rz));
                    normals.Add(new Vector3(0f, 1f, 0f));
                }
            }

            for (int z = 0; z < heightSegments - 1; z++)
            {
                for (int x = 0; x < widthSegments - 1; x++)
                {
                    int index = z * widthSegments + x;
                    var a = index;
                    var b = index + 1;
                    var c = index + 1 + widthSegments;
                    var d = index + widthSegments;

                    triangles.Add(a);
                    triangles.Add(b);
                    triangles.Add(c);

                    triangles.Add(c);
                    triangles.Add(d);
                    triangles.Add(a);
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