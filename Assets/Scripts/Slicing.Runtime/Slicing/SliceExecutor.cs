using System.Collections.Generic;
using Slicing.Core;
using UnityEngine;

namespace Slicing.Runtime
{
    public class SliceExecutor : MonoBehaviour
    {
        [SerializeField] private SliceSettingsSO sliceSettings;
        [SerializeField] private LayerMask sliceableMask;
        [SerializeField] private PoolManager poolManager;

        private readonly List<Sliceable> sliceables = new List<Sliceable>();

        public void ExecuteSwipe(Vector2 startWorld, Vector2 endWorld)
        {
            if (sliceSettings == null || sliceSettings.Validate() == false)
                return;

            Vector2 delta = endWorld - startWorld;
            float speed = delta.magnitude / Mathf.Max(Time.deltaTime, 0.01f);
            if (speed < sliceSettings.MinimumSliceSpeed)
                return;

            var hit = Physics2D.OverlapPoint(startWorld, sliceableMask);
            if (hit == null)
                hit = Physics2D.OverlapPoint(endWorld, sliceableMask);
            if (hit == null)
                return;

            var sliceable = hit.GetComponent<Sliceable>();
            if (sliceable == null)
                return;

            TrySlice(sliceable, startWorld, endWorld);
        }

        private void TrySlice(Sliceable sliceable, Vector2 startWorld, Vector2 endWorld)
        {
            if (sliceable == null || sliceable.IsDestroyed)
                return;

            var spriteRenderer = sliceable.GetComponent<SpriteRenderer>();
            Sprite sprite = spriteRenderer != null ? spriteRenderer.sprite : null;
            if (sprite == null)
                return;

            Vector2 localStart = sliceable.transform.InverseTransformPoint(startWorld);
            Vector2 localEnd = sliceable.transform.InverseTransformPoint(endWorld);
            Vector2 localDirection = localEnd - localStart;

            var polygon = new List<Vector2>
            {
                new Vector2(-sprite.bounds.extents.x, -sprite.bounds.extents.y),
                new Vector2(sprite.bounds.extents.x, -sprite.bounds.extents.y),
                new Vector2(sprite.bounds.extents.x, sprite.bounds.extents.y),
                new Vector2(-sprite.bounds.extents.x, sprite.bounds.extents.y)
            };

            var result = GeometryMath.SplitPolygonAlongLine(polygon, localStart, localEnd);
            if (result.LeftPolygon.Count < 3 || result.RightPolygon.Count < 3)
                return;

            sliceable.TrySlice(startWorld, endWorld);

            var leftPiece = poolManager != null ? poolManager.GetPiece() : new GameObject("SlicePieceLeft");
            var rightPiece = poolManager != null ? poolManager.GetPiece() : new GameObject("SlicePieceRight");

            SetupPiece(leftPiece, result.LeftPolygon, sliceable, sliceable.transform.position, Vector2.zero);
            SetupPiece(rightPiece, result.RightPolygon, sliceable, sliceable.transform.position, Vector2.zero);

            if (sliceable.transform.parent != null)
            {
                leftPiece.transform.SetParent(sliceable.transform.parent, true);
                rightPiece.transform.SetParent(sliceable.transform.parent, true);
            }

            Destroy(sliceable.gameObject);

            var eventBus = FindObjectOfType<RuntimeEventBus>();
            eventBus?.PublishSlice(new SliceEvent { Parent = sliceable.gameObject, SliceCount = sliceable.SliceCount, Area = GeometryMath.ComputeArea(result.LeftPolygon) + GeometryMath.ComputeArea(result.RightPolygon) });
        }

        private void SetupPiece(GameObject pieceObject, List<Vector2> polygon, Sliceable source, Vector3 position, Vector2 velocity)
        {
            var spriteRenderer = pieceObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
                spriteRenderer = pieceObject.AddComponent<SpriteRenderer>();

            var meshFilter = pieceObject.GetComponent<MeshFilter>();
            if (meshFilter == null)
                meshFilter = pieceObject.AddComponent<MeshFilter>();

            var meshRenderer = pieceObject.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
                meshRenderer = pieceObject.AddComponent<MeshRenderer>();

            var rigidbody = pieceObject.GetComponent<Rigidbody2D>();
            if (rigidbody == null)
                rigidbody = pieceObject.AddComponent<Rigidbody2D>();

            var collider = pieceObject.GetComponent<PolygonCollider2D>();
            if (collider == null)
                collider = pieceObject.AddComponent<PolygonCollider2D>();

            var mesh = new Mesh();
            mesh.name = "SlicePieceMesh";

            Vector3[] vertices = new Vector3[polygon.Count];
            Vector2[] uvs = new Vector2[polygon.Count];
            int[] triangles = new int[(polygon.Count - 2) * 3];

            for (int i = 0; i < polygon.Count; i++)
            {
                vertices[i] = polygon[i];
                uvs[i] = polygon[i];
            }

            int index = 0;
            for (int i = 1; i < polygon.Count - 1; i++)
            {
                triangles[index++] = 0;
                triangles[index++] = i;
                triangles[index++] = i + 1;
            }

            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            meshFilter.sharedMesh = mesh;
            collider.SetPath(0, polygon.ToArray());

            if (source != null && source.Definition != null && source.Definition.Sprite != null)
            {
                spriteRenderer.sprite = source.Definition.Sprite;
                spriteRenderer.sortingOrder = source.GetComponent<SpriteRenderer>().sortingOrder;
            }

            pieceObject.transform.position = position;
            rigidbody.velocity = velocity;
            rigidbody.mass = Mathf.Max(0.1f, GeometryMath.ComputeArea(polygon) * 0.05f);
            pieceObject.layer = source.gameObject.layer;
            pieceObject.SetActive(true);
        }
    }
}
