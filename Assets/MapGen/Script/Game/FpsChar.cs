using UnityEngine;

namespace MapGen
{
    public class FpsChar : MonoBehaviour
    {
        [SerializeField] private Transform Cam1;
        [SerializeField] private Transform Cam2;
        [SerializeField] private Transform Char;
        [SerializeField] private float yOffsetOfCams = 1.4f;
        
        public void SetPos(Vector3 pos)
        {
            Cam1.position = pos + Vector3.up * yOffsetOfCams;
            Cam2.position = pos + Vector3.up * yOffsetOfCams;
            Char.position = pos;
        }
    }
}