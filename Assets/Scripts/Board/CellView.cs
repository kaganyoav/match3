using UnityEngine;

namespace Board
{
    public class CellView : MonoBehaviour
    {
        public int Row { get; private set; }
        public int Col { get; private set; }

        public void Init(int r, int c) { Row = r; Col = c; }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.9f);
        }
#endif
    }

}
