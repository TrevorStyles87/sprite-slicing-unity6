using System.Collections.Generic;
using UnityEngine;

namespace Slicing.Runtime
{
    public class PoolManager : MonoBehaviour
    {
        [SerializeField] private GameObject piecePrefab;
        [SerializeField] private int initialPoolSize = 32;

        private readonly Queue<GameObject> piecePool = new Queue<GameObject>();

        private void Awake()
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                var pooled = Instantiate(piecePrefab, transform);
                pooled.SetActive(false);
                piecePool.Enqueue(pooled);
            }
        }

        public GameObject GetPiece()
        {
            if (piecePool.Count > 0)
            {
                var item = piecePool.Dequeue();
                item.SetActive(true);
                return item;
            }

            var created = Instantiate(piecePrefab, transform);
            created.SetActive(true);
            return created;
        }

        public void ReleasePiece(GameObject piece)
        {
            if (piece == null)
                return;

            piece.SetActive(false);
            piecePool.Enqueue(piece);
        }
    }
}
