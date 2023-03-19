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
            // Quad�̉����Əc�������ꂼ��size�̒����ɂȂ�悤�ɔ����̒��������߂�
            var hsize = size * 0.5f;

            // Quad�̒��_�f�[�^
            var vertices = new Vector3[] {
                new Vector3(-hsize,  hsize, 0f), // 0�ڂ̒��_ Quad�̍���̈ʒu
				new Vector3( hsize,  hsize, 0f), // 1�ڂ̒��_ Quad�̉E��̈ʒu
				new Vector3( hsize, -hsize, 0f), // 2�ڂ̒��_ Quad�̉E���̈ʒu
				new Vector3(-hsize, -hsize, 0f)  // 3�ڂ̒��_ Quad�̍����̈ʒu
			};

            // Quad��uv���W�f�[�^
            var uv = new Vector2[] {
                new Vector2(0f, 1f), // 0�ڂ̒��_��uv���W
				new Vector2(1f, 1f), // 1�ڂ̒��_��uv���W
				new Vector2(1f, 0f), // 2�ڂ̒��_��uv���W
				new Vector2(0f, 0f)  // 3�ڂ̒��_��uv���W
			};

            // Quad�̖@���f�[�^
            var normals = new Vector3[] {
                new Vector3(0f, 0f, -1f), // 0�ڂ̒��_�̖@��
				new Vector3(0f, 0f, -1f), // 1�ڂ̒��_�̖@��
				new Vector3(0f, 0f, -1f), // 2�ڂ̒��_�̖@��
				new Vector3(0f, 0f, -1f)  // 3�ڂ̒��_�̖@��
			};

            // Quad�̖ʃf�[�^ ���_��index��3���ׂ�1�̖�(�O�p�`)�Ƃ��ĔF������
            var triangles = new int[] {
                0, 1, 2, // 1�ڂ̎O�p�`
				2, 3, 0  // 2�ڂ̎O�p�`
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