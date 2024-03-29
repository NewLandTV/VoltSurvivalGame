using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SG_WaterWave : MonoBehaviour
{
    [SerializeField]
    private int columnCount = 10;
    [SerializeField]
    private float width = 2f;
    [SerializeField]
    private float height = 1f;
    [SerializeField]
    private float k = 0.025f;
    [SerializeField]
    private float m = 1f;
    [SerializeField]
    private float drag = 0.025f;
    [SerializeField]
    private float spread = 0.025f;
    [SerializeField]
    private float power = -1f;

    [SerializeField]
    private MeshFilter meshFilter;

    private Camera mainCamera;

    private WaitForFixedUpdate waitTimeFixedUpdate = new WaitForFixedUpdate();

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private IEnumerator Start()
    {
        Setup();
        StartCoroutine(Loop());

        while (true)
        {
            for (int i = 0; i < columns.Count; i++)
            {
                columns[i].UpdateColumn();
            }

            float[] leftDeltas = new float[columns.Count];
            float[] rightDeltas = new float[columns.Count];

            for (int i = 0; i < columns.Count; i++)
            {
                if (i > 0)
                {
                    leftDeltas[i] = (columns[i].height - columns[i - 1].height) * spread;

                    columns[i - 1].velocity += leftDeltas[i];
                }
                if (i < columns.Count - 1)
                {
                    rightDeltas[i] = (columns[i].height - columns[i + 1].height) * spread;

                    columns[i + 1].velocity += rightDeltas[i];
                }
            }

            for (int i = 0; i < columns.Count; i++)
            {
                if (i > 0)
                {
                    columns[i - 1].height += leftDeltas[i];
                }
                if (i < columns.Count - 1)
                {
                    columns[i + 1].height += rightDeltas[i];
                }
            }

            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[columns.Count * 2];

            int v = 0;

            for (int i = 0; i < columns.Count; i++)
            {
                vertices[v] = new Vector3(columns[i].xPosition, columns[i].height, 0f);
                vertices[v + 1] = new Vector3(columns[i].xPosition, 0f, 0f);

                v += 2;
            }

            int[] triangles = new int[(columns.Count - 1) * 6];

            int t = 0;

            v = 0;

            for (int i = 0; i < columns.Count - 1; i++)
            {
                triangles[t] = v;
                triangles[t + 1] = v + 2;
                triangles[t + 2] = v + 1;
                triangles[t + 3] = v + 1;
                triangles[t + 4] = v + 2;
                triangles[t + 5] = v + 3;

                v += 2;
                t += 6;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.Optimize();

            meshFilter.mesh = mesh;

            yield return waitTimeFixedUpdate;
        }
    }

    private void Setup()
    {
        columns.Clear();

        float space = width / columnCount;

        for (int i = 0; i <= columnCount; i++)
        {
            columns.Add(new WaterColumn(i * space - width * 0.5f, height, k, m, drag));
        }
    }

    internal int? WorldToColumn(Vector2 position)
    {
        float space = width / columnCount;

        int result = Mathf.RoundToInt((position.x + width * 0.5f) / space);

        if (result >= columns.Count || result < 0)
        {
            return null;
        }

        return result;
    }

    private IEnumerator Loop()
    {
        while (true)
        {
            int? column = WorldToColumn(mainCamera.ScreenToWorldPoint(Input.mousePosition));

            if (Input.GetMouseButtonDown(0) && column.HasValue)
            {
                columns[column.Value].velocity = power;
            }

            yield return null;
        }
    }

    private List<WaterColumn> columns = new List<WaterColumn>();

    public class WaterColumn
    {
        public float xPosition;
        public float height;
        public float targetHeight;
        public float k;
        public float m;
        public float velocity;
        public float drag;

        public WaterColumn(float xPosition, float targetHeight, float k, float m, float drag)
        {
            this.xPosition = xPosition;
            height = targetHeight;
            this.targetHeight = targetHeight;
            this.k = k;
            this.m = m;
            this.drag = drag;
        }

        public void UpdateColumn()
        {
            float a = -k / m * (height - targetHeight);

            velocity += a;
            velocity -= drag * velocity;
            height += velocity;
        }
    }
}
