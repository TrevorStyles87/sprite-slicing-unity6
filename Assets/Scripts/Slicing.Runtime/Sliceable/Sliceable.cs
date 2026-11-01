using System.Collections.Generic;
using Slicing.Core;
using Slicing.Data;
using UnityEngine;

namespace Slicing.Runtime
{
    public interface IHittable
    {
        void ApplyDamage(float damage, Vector2 hitPoint, Vector2 force);
    }

    public class Sliceable : MonoBehaviour, IHittable
    {
        public SliceableDefinitionSO Definition;
        public int CurrentHealth { get; private set; }
        public int SliceCount { get; private set; }
        public bool IsDestroyed { get; private set; }
        public int ParentId { get; private set; }

        private SpriteRenderer spriteRenderer;
        private Rigidbody2D body;
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            body = GetComponent<Rigidbody2D>();
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            CurrentHealth = Definition != null ? Definition.MaxHealth : 1;
            ParentId = GetInstanceID();
        }

        public void Initialize(SliceableDefinitionSO definition)
        {
            Definition = definition;
            CurrentHealth = Definition != null ? Definition.MaxHealth : 1;
            spriteRenderer ??= GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && Definition != null)
            {
                spriteRenderer.sprite = Definition.Sprite;
            }
        }

        public bool TrySlice(Vector2 startWorld, Vector2 endWorld)
        {
            if (IsDestroyed || Definition == null || !Definition.CanSlice)
                return false;

            SliceCount++;
            return true;
        }

        public void ApplyDamage(float damage, Vector2 hitPoint, Vector2 force)
        {
            if (Definition == null || !Definition.CanBeDamaged)
                return;

            CurrentHealth = Mathf.Max(0, CurrentHealth - Mathf.CeilToInt(damage));
            if (body != null)
            {
                body.velocity += force * 0.3f;
            }

            if (CurrentHealth <= 0)
            {
                Destroy(gameObject);
                IsDestroyed = true;
            }
        }

        public void SetParentId(int parentId)
        {
            ParentId = parentId;
        }

        public void ApplyMesh(List<Vector2> polygon)
        {
            if (polygon == null || polygon.Count < 3)
                return;

            var mesh = new Mesh();
            mesh.name = "SlicedPieceMesh";

            Vector3[] vertices = new Vector3[polygon.Count];
            Vector2[] uvs = new Vector2[polygon.Count];
            int[] triangles = new int[(polygon.Count - 2) * 3];

            for (int i = 0; i < polygon.Count; i++)
            {
                Vector2 point = polygon[i];
                vertices[i] = point;
                uvs[i] = new Vector2(point.x, point.y);
            }

            int triangleIndex = 0;
            for (int i = 1; i < polygon.Count - 1; i++)
            {
                triangles[triangleIndex++] = 0;
                triangles[triangleIndex++] = i;
                triangles[triangleIndex++] = i + 1;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            meshFilter ??= GetComponent<MeshFilter>();
            meshRenderer ??= GetComponent<MeshRenderer>();

            if (meshFilter != null)
                meshFilter.sharedMesh = mesh;

            if (meshRenderer != null)
                meshRenderer.sortingOrder = 1;

            var collider = GetComponent<PolygonCollider2D>();
            if (collider != null)
            {
                collider.SetPath(0, polygon.ToArray());
            }
        }
    }
}
