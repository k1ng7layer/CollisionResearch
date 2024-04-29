using System;
using UnityEngine;

namespace OBB
{
    public class ObbRunner : MonoBehaviour
    {
        [SerializeField] private MeshFilter _mesh1;
        [SerializeField] private MeshFilter _mesh2;

        [SerializeField] private Cube _cube1;
        [SerializeField] private Cube _cube2;
        
        private void Awake()
        {
            _cube1 = new Cube(Vector3.one);
            _cube1.Position = _mesh1.transform.position;
            _cube1.Rotation = _mesh1.transform.rotation;

            _cube2 = new Cube(Vector3.one);
            _cube2.Position = _mesh2.transform.position;
            _cube2.Rotation = _mesh2.transform.rotation;
        }

        private void Update()
        {
            var x = Input.GetAxisRaw("Horizontal");
            var z = Input.GetAxisRaw("Vertical");

            var dir = new Vector3(x, 0f, z);

            _cube1.Position += dir.normalized * 3f * Time.deltaTime;
        
            var mtv = Vector3.zero;

            if (OrientedBB.IsColliding(_cube1, _cube2, ref mtv)){}
            _cube1.Position += new Vector3(mtv.x, mtv.y, mtv.z);
            
            _mesh1.transform.position = _cube1.Position;
            _mesh1.transform.rotation = _cube1.Rotation;
        }
    }
}