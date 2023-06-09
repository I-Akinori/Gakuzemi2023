using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralModeling_AI
{
    public class Quad : MonoBehaviour
    {
        [SerializeField, Range(0.1f, 10f)]
        protected float size = 1f;
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
            // Quadの横幅と縦幅がそれぞれsizeの長さになるように半分の長さを求める
            var hsize = size * 0.5f;

            // Quadの頂点データ
            var vertices = new Vector3[] {
                new Vector3(-hsize,  hsize, 0f), // 0つ目の頂点 Quadの左上の位置
				new Vector3( hsize,  hsize, 0f), // 1つ目の頂点 Quadの右上の位置
				new Vector3( hsize, -hsize, 0f), // 2つ目の頂点 Quadの右下の位置
				new Vector3(-hsize, -hsize, 0f)  // 3つ目の頂点 Quadの左下の位置
			};

            // Quadのuv座標データ
            var uv = new Vector2[] {
                new Vector2(0f, 1f), // 0つ目の頂点のuv座標
				new Vector2(1f, 1f), // 1つ目の頂点のuv座標
				new Vector2(1f, 0f), // 2つ目の頂点のuv座標
				new Vector2(0f, 0f)  // 3つ目の頂点のuv座標
			};

            // Quadの法線データ
            var normals = new Vector3[] {
                new Vector3(0f, 0f, -1f), // 0つ目の頂点の法線
				new Vector3(0f, 0f, -1f), // 1つ目の頂点の法線
				new Vector3(0f, 0f, -1f), // 2つ目の頂点の法線
				new Vector3(0f, 0f, -1f)  // 3つ目の頂点の法線
			};

            // Quadの面データ 頂点のindexを3つ並べて1つの面(三角形)として認識する
            var triangles = new int[] {
                0, 1, 2, // 1つ目の三角形
				2, 3, 0  // 2つ目の三角形
			};

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.normals = normals;
            mesh.triangles = triangles;

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