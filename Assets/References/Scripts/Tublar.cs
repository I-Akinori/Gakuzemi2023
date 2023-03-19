using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace ProceduralModeling_AI
{
    public class Tublar : MonoBehaviour
    {
        [SerializeField, Range(0.01f, 1f)]
        protected float radius = 0.1f;
        [SerializeField, Range(3, 32)]
        protected int radialSegments = 8;
        [SerializeField, Range(4, 1024)]
        protected int tublarSegments = 128;
        const float PI2 = Mathf.PI * 2f;

        [SerializeField]
        Material mat;
        MeshFilter filt;
        MeshRenderer rend;

        List<float> rands;

        public struct SamplingPoint
        {
            public Vector3 position;
            public Vector3 normal;
            public Vector3 tangent;
            public Vector3 binormal;
        }

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

            rands = new List<float>();
            for (int i = 0; i < 6; i++)
            {
                rands.Add(Random.Range(-3f, 3f));// X
                rands.Add(Random.Range(-3f, 3f));// Y
                rands.Add(Random.Range(-3f, 3f));// Z

                rands.Add(Random.Range(0, 5f));// X_A
                rands.Add(Random.Range(0, 5f));// Y_A
                rands.Add(Random.Range(0, 5f));// Z_A

                rands.Add(Random.Range(-5f, 5f));// X_f
                rands.Add(Random.Range(-5f, 5f));// Y_f
                rands.Add(Random.Range(-5f, 5f));// Z_f

                rands.Add(Random.Range(-Mathf.PI, Mathf.PI));// X_d
                rands.Add(Random.Range(-Mathf.PI, Mathf.PI));// Y_d
                rands.Add(Random.Range(-Mathf.PI, Mathf.PI));// Z_d
            }
        }
        private Mesh Build(List<SamplingPoint> points)
        {
            var mesh = new Mesh();
            var vertices = new List<Vector3>();
            var uv = new List<Vector2>();
            var normals = new List<Vector3>();
            var triangles = new List<int>();

            for (int i = 0; i < tublarSegments + 1; i++)
            {
                for (int j = 0; j < radialSegments; j++) 
                {
                    float ratio = (float)j / radialSegments;
                    float rad = ratio * PI2;
                    float cos = Mathf.Cos(rad), sin = Mathf.Sin(rad);
                    vertices.Add(points[i].position + cos * radius * points[i].normal + sin * radius * points[i].binormal);
                    uv.Add(new Vector2(ratio, (float) i / tublarSegments));
                    normals.Add(points[i].normal);
                }
            }

            for (int i = 0; i < tublarSegments; i++)
            {
                for (int j = 0; j < radialSegments; j++)
                {
                    int a = i * radialSegments + j;
                    int b = i * radialSegments + (j + 1) % (radialSegments);
                    int c = (i + 1) * radialSegments + j;
                    int d = (i + 1) * radialSegments + (j + 1) % (radialSegments);

                    triangles.Add(a);
                    triangles.Add(b);
                    triangles.Add(c);

                    triangles.Add(c);
                    triangles.Add(b);
                    triangles.Add(d);
                }
            }

            mesh.vertices = vertices.ToArray();
            mesh.uv = uv.ToArray();
            mesh.normals = normals.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateBounds();
            //mesh.RecalculateNormals();

            return mesh;
        }

        Vector3 GetPointOnCatmullRomCurve(Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3, float t)
        {
            if (P0 == P1)
                return (P1 - 2 * P2 + P3) / 2 * t * t
                    + (-3 * P1 + 4 * P2 - P3) / 2 * t + P1;
            if (P2 == P3)
                return (P0 - 2 * P1 + P2) / 2 * t * t
                    + (-P0 + P2) / 2 * t + P1;

            return (-P0 + 3 * P1 - 3 * P2 + P3) / 2 * t * t * t
                    + (2 * P0 - 5 * P1 + 4 * P2 - P3) / 2 * t * t
                    + (-P0 + P2) / 2 * t + P1;
        }
        List<Vector3> CatmullRomCurve(List<Vector3> ctrlPs, int segments)
        {
            var points = new List<Vector3>();
            var delta = (float)(ctrlPs.Count - 1) / segments;
            for (int i = 0; i < segments; i++)
            {
                var s = i * delta;
                var j = Mathf.FloorToInt(s);
                points.Add(GetPointOnCatmullRomCurve(ctrlPs[Mathf.Max(0, j - 1)], ctrlPs[j], ctrlPs[j + 1], ctrlPs[Mathf.Min(ctrlPs.Count - 1, j + 2)], s - j));
            }
            points.Add(ctrlPs[ctrlPs.Count - 1]);
            return points;
        }
        List<SamplingPoint> CatmullRomCurveWithFrenetFrame(List<Vector3> ctrlPs, int segments)
        {
            var points = new List<SamplingPoint>();
            var positions = CatmullRomCurve(ctrlPs, segments);

            SamplingPoint p;
            for (int i = 0; i < segments + 1; i++)
            {
                p = new SamplingPoint();
                p.position = positions[i];
                points.Add(p);
            }

            // 位置の中心差分 (次 - 前) をとって接線方向 (tangent) を求める
            for (int i = 1; i < segments; i++)
            {
                p = points[i];
                p.tangent = (points[i + 1].position - points[i - 1].position).normalized;                
                points[i] = p;
            }
            // 最初と最後サンプリング点ではそれぞれ前進差分 (次 - 自身)、後退差分(自身 - 後)をとる
            p = points[0];
            p.tangent = (points[1].position - points[0].position).normalized;
            points[0] = p;
            p = points[segments];
            p.tangent = (points[segments].position - points[segments - 1].position).normalized;
            points[segments] = p;

            // さらに接線方向の差分を主法線方向 (normal) とする
            for (int i = 1; i < segments; i++)
            {
                p = points[i];
                p.normal = (points[i + 1].tangent - points[i - 1].tangent).normalized;
                points[i] = p;
            }
            // 最初と最後サンプリング点での処理も同様
            p = points[0];
            p.normal = (points[1].tangent - points[0].tangent).normalized;
            points[0] = p;
            p = points[segments];
            p.normal = (points[segments].tangent - points[segments - 1].tangent).normalized;
            points[segments] = p;

            // 従法線方向 (binormal) は接線方向と法線方向の外積で求める
            for (int i = 0; i < segments + 1; i++)
            {
                p = points[i];
                p.binormal = Vector3.Cross(p.tangent, p.normal);
                points[i] = p;
            }

            return points;
        }

        void Update()
        {
            var Vs = new List<Vector3>();
            Vs.Add(new Vector3(0, 0, 0));
            Vs.Add(new Vector3(1, 1, 1));
            Vs.Add(new Vector3(2, 3, 0));
            Vs.Add(new Vector3(0, 2, 4));
            Vs.Add(new Vector3(-2, 3, 2));
            Vs.Add(new Vector3(1, 2, 2));
            /*
            for (int i = 0; i < 6; i++)
            {
                int i12 = i * 12;
                Vs.Add(
                    new Vector3(rands[i12], rands[i12 + 1], rands[i12 + 2])
                    + new Vector3(rands[i12 + 3] * Mathf.Sin(rands[i12 + 6] * Time.time + rands[i12 + 9]),
                                    rands[i12 + 4] * Mathf.Sin(rands[i12 + 7] * Time.time + rands[i12 + 10]),
                                        rands[i12 + 5] * Mathf.Sin(rands[i12 + 8] * Time.time + rands[i12 + 11]))
                    );

            }
            */

            //var Ps = CatmullRomCurve(Vs, tublarSegments);
            var Ps = CatmullRomCurveWithFrenetFrame(Vs, tublarSegments);
            for (int i = 0; i < Ps.Count - 1; i++)
            {
                //Debug.DrawLine(Ps[i], Ps[i + 1], Color.HSVToRGB((float)i / Ps.Count, 0.5f, 1f));
                Debug.DrawLine(Ps[i].position, Ps[i + 1].position, Color.HSVToRGB((float)i / Ps.Count, 0.5f, 1f));
            }
            for (int i = 0; i < Ps.Count; i++)
            {
                Debug.DrawLine(Ps[i].position, Ps[i].position + Ps[i].tangent * 0.1f, Color.red);
                Debug.DrawLine(Ps[i].position, Ps[i].position + Ps[i].normal * 0.1f, Color.green);
                Debug.DrawLine(Ps[i].position, Ps[i].position + Ps[i].binormal * 0.1f, Color.blue);
            }

            rend.material = mat;
            filt.mesh = Build(Ps);
        }
    } // class
} // napespace