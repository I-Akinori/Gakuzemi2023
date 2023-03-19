using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace ProceduralModeling_AI
{
    public class Tree2D : MonoBehaviour
    {
        [SerializeField, Range(1, 15)]
        protected int generations = 8;
        [SerializeField, Range(0f, 1.0f)]
        protected float length_ratio = 0.7f;
        [SerializeField, Range(0, 180)]
        protected float angle = 30;

        // Update is called once per frame
        void Update()
        {
            var position = Vector3.zero;
            var length = 10f;
            var direction = 90 * Mathf.Deg2Rad;
            position = DrawLine(position, new Vector3(Mathf.Cos(direction), Mathf.Sin(direction), 0) * length);
            Branch(generations, position, direction, length);
        }
        void Branch(int gen, Vector3 pos, float dir, float len)
        {
            if (gen > 0)
            {
                len *= length_ratio;
                var dir1 = dir + angle * Mathf.Deg2Rad;
                var pos1 = DrawLine(pos, new Vector3(Mathf.Cos(dir1), Mathf.Sin(dir1), 0) * len);
                Branch(gen - 1, pos1, dir1, len);

                dir = dir - angle * Mathf.Deg2Rad;
                pos = DrawLine(pos, new Vector3(Mathf.Cos(dir), Mathf.Sin(dir), 0) * len);
                Branch(gen - 1, pos, dir, len);
            }
        }
        Vector3 DrawLine(Vector3 pos, Vector3 delta)
        {
            var end = pos + delta;
            Debug.DrawLine(pos, end);
            return end;
        }
    } // class
} //namespace