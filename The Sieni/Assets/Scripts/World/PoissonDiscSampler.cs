using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoissonDiscSampler
{
    private const int maxAttempts = 30; 

    private readonly Rect rect;
    private readonly float radius2;
    private readonly float cellSize;
    private Vector2[,] grid;
    private  List<Vector2> activeSamples = new List<Vector2>();

    public PoissonDiscSampler(float width, float height, float radius)
    {
        rect = new Rect(0, 0, width, height);
        radius2 = radius * radius;
        cellSize = radius / Mathf.Sqrt(2);
        grid = new Vector2[Mathf.CeilToInt(width / cellSize),
                           Mathf.CeilToInt(height / cellSize)];
    }

    public IEnumerable<Vector2> Samples()
    {
        yield return AddSample(new Vector2(Random.value * rect.width, Random.value * rect.height));

        while (activeSamples.Count > 0) {
            int randomSampleIndex = (int) Random.value * activeSamples.Count;
            Vector2 sample = activeSamples[randomSampleIndex];

            bool found = false;
            for (int index = 0; index < maxAttempts; index += 1) {

                float angle = 2 * Mathf.PI * Random.value;
                float sampleFactor = RandomNumberWithinAnnulus(radius2);
                Vector2 candidate = sample + sampleFactor * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                if (rect.Contains(candidate) && IsFarEnough(candidate)) {
                    found = true;
                    yield return AddSample(candidate);
                    break;
                }
            }

            if (!found) {
                activeSamples[randomSampleIndex] = activeSamples[activeSamples.Count - 1];
                activeSamples.RemoveAt(activeSamples.Count - 1);
            }
        }
    }

    private float RandomNumberWithinAnnulus(float radius) {
        // See: http://stackoverflow.com/questions/9048095/create-random-number-within-an-annulus/9048443#9048443
        return Mathf.Sqrt(Random.value * 3 * radius + radius);
    }

    private bool IsFarEnough(Vector2 sample)
    {
        GridPos pos = new GridPos(sample, cellSize);

        int xmin = Mathf.Max(pos.x - 2, 0);
        int ymin = Mathf.Max(pos.y - 2, 0);
        int xmax = Mathf.Min(pos.x + 2, grid.GetLength(0) - 1);
        int ymax = Mathf.Min(pos.y + 2, grid.GetLength(1) - 1);

        for (int y = ymin; y <= ymax; y++) {
            for (int x = xmin; x <= xmax; x++) {
                Vector2 s = grid[x, y];
                if (s != Vector2.zero) {
                    Vector2 d = s - sample;
                    if (d.x * d.x + d.y * d.y < radius2) return false;
                }
            }
        }

        return true;

        // Note: we use the zero vector to denote an unfilled cell in the grid. This means that if we were
        // to randomly pick (0, 0) as a sample, it would be ignored for the purposes of proximity-testing
        // and we might end up with another sample too close from (0, 0). This is a very minor issue.
    }

    private Vector2 AddSample(Vector2 sample)
    {
        activeSamples.Add(sample);
        GridPos pos = new GridPos(sample, cellSize);
        grid[pos.x, pos.y] = sample;
        return sample;
    }

    private struct GridPos
    {
        public int x;
        public int y;

        public GridPos(Vector2 sample, float cellSize)
        {
            x = (int)(sample.x / cellSize);
            y = (int)(sample.y / cellSize);
        }
    }
}