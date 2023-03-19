using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProceduralModeling_AI
{
    public class Cylinder : MonoBehaviour
    {
        [SerializeField, Range(0.1f, 10f)]
        protected float height = 3f, radius = 1f;
        [SerializeField, Range(3, 32)]
        protected int segments = 16;
        const float PI2 = Mathf.PI * 2f;


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

            float top = height * 0.5f, bottom = -height * 0.5f;
            for (int i = 0; i < segments; i++)
            {
                float ratio = (float)i / segments;
                float rad = ratio * PI2;
                float cos = Mathf.Cos(rad), sin = Mathf.Sin(rad);
                float x = cos * radius, z = sin * radius;
                Vector3 tp = new Vector3(x, top, z);
                Vector3 bp = new Vector3(x, bottom, z);
                // 上端
                vertices.Add(tp);
                uv.Add(new Vector2(ratio, 1f));
                normals.Add(new Vector3(0f, 1f, 0f)); // 蓋の上を向く法線
                                                      // 下端
                vertices.Add(bp);
                uv.Add(new Vector2(ratio, 0f));
                normals.Add(new Vector3(0f, -1f, 0f)); // 蓋の下を向く法線
            }
            // 各面の中心
            vertices.Add(new Vector3(0f, top, 0f));
            uv.Add(new Vector2(0f, 1f));
            normals.Add(new Vector3(0f, 1f, 0f));
            vertices.Add(new Vector3(0f, bottom, 0f));
            uv.Add(new Vector2(0f, 0f));
            normals.Add(new Vector3(0f, -1f, 0f));
            // 側面用の頂点
            for (int i = 0; i < segments; i++)
            {
                float ratio = (float)i / segments;
                float rad = ratio * PI2;
                float cos = Mathf.Cos(rad), sin = Mathf.Sin(rad);
                float x = cos * radius, z = sin * radius;
                Vector3 tp = new Vector3(x, top, z);
                Vector3 bp = new Vector3(x, bottom, z);
                // 上端
                vertices.Add(tp);
                uv.Add(new Vector2(ratio, 1f));
                normals.Add(new Vector3(cos, 0f, sin)); // 外側を向く法線
                                                        // 下端
                vertices.Add(bp);
                uv.Add(new Vector2(ratio, 0f));
                normals.Add(new Vector3(cos, 0f, sin)); // 外側を向く法線
            }

            var it = segments * 2;
            var ib = segments * 2 + 1;

            // 上端の蓋の面
            for (int i = 0; i < segments; i++)
            {
                triangles.Add(it);
                triangles.Add(((i + 1) % segments) * 2);
                triangles.Add(i * 2);
            }

            // 下端の蓋の面
            for (int i = 0; i < segments; i++)
            {
                triangles.Add(ib);
                triangles.Add(i * 2 + 1);
                triangles.Add(((i + 1) % segments) * 2 + 1);
            }

            for (int i = 0; i < segments; i++)
            {
                int a = i * 2;
                int b = i * 2 + 1;
                int c = (i + 1) % segments * 2;
                int d = (i + 1) % segments * 2 + 1;

                int offset = (segments + 1) * 2;
                a += offset; b += offset;
                c += offset; d += offset;

                triangles.Add(a);
                triangles.Add(c);
                triangles.Add(b);

                triangles.Add(c);
                triangles.Add(d);
                triangles.Add(b);
            }

            mesh.vertices = vertices.ToArray();
            mesh.uv = uv.ToArray();
            mesh.normals = normals.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateBounds();
            //mesh.RecalculateNormals();

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