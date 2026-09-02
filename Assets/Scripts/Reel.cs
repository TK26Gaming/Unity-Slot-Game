using System.Collections;
using UnityEngine;

public class Reel : MonoBehaviour
{
    [Header("Reel References")]
    [SerializeField] private RectTransform symbolContainer;

    [Header("Reel Settings")]
    [SerializeField] private float symbolSpacing = 130f;
    [SerializeField] private float spinSpeed = 1800f;
    [SerializeField] private int minimumCycles = 3;

    private RectTransform[] symbols;
    private bool isSpinning;

    public bool IsSpinning => isSpinning;

    private void Awake()
    {
        CacheAndInitializeSymbols();
    }

    /// <summary>
    /// Centers symbols around Y = 0 on boot so both positive (top) 
    /// and negative (bottom) positions are populated cleanly.
    /// </summary>
    private void CacheAndInitializeSymbols()
    {
        int count = symbolContainer.childCount;
        if (count == 0) return;

        symbols = new RectTransform[count];
        Canvas.ForceUpdateCanvases();

        int halfCount = count / 2;

        for (int i = 0; i < count; i++)
        {
            symbols[i] = symbolContainer.GetChild(i).GetComponent<RectTransform>();

            // Ensure consistent anchors and pivots
            symbols[i].anchorMin = new Vector2(0.5f, 0.5f);
            symbols[i].anchorMax = new Vector2(0.5f, 0.5f);
            symbols[i].pivot = new Vector2(0.5f, 0.5f);

            // Calculate wrapped offset relative to index 0
            int offset = i;
            if (offset > halfCount) offset -= count;

            // Align position (e.g., -130, 0, +130, +260...)
            symbols[i].anchoredPosition = new Vector2(0f, offset * symbolSpacing);
        }
    }

    /// <summary>
    /// Starts the reel spinning toward targetIndex. 
    /// Allows overriding total full spin cycles dynamically.
    /// </summary>
    public void Spin(int targetIndex, int cycles = -1)
    {
        if (isSpinning) return;

        if (symbols == null || symbols.Length == 0)
        {
            Debug.LogWarning($"{gameObject.name} has no symbols.");
            return;
        }

        if (targetIndex < 0 || targetIndex >= symbols.Length)
        {
            Debug.LogWarning($"Invalid target index: {targetIndex}");
            return;
        }

        int spinCycles = (cycles > 0) ? cycles : minimumCycles;
        StartCoroutine(SpinRoutine(targetIndex, spinCycles));
    }

    private IEnumerator SpinRoutine(int targetIndex, int cyclesToRun)
    {
        isSpinning = true;

        RectTransform target = symbols[targetIndex];

        float targetY = target.anchoredPosition.y;
        float cycleDistance = symbolSpacing * symbols.Length;

        float distanceToCenter = targetY;
        while (distanceToCenter <= 0f)
        {
            distanceToCenter += cycleDistance;
        }

        float totalDistance = distanceToCenter + (cycleDistance * cyclesToRun);

        float estimatedDuration = (2f * totalDistance) / (spinSpeed + (spinSpeed * 0.08f));
        float elapsedTime = 0f;
        float distanceTravelled = 0f;

        while (elapsedTime < estimatedDuration)
        {
            elapsedTime += Time.deltaTime;
            float timeProgress = Mathf.Clamp01(elapsedTime / estimatedDuration);

            float easeMultiplier = Mathf.SmoothStep(1f, 0.08f, timeProgress);
            float currentSpeed = spinSpeed * easeMultiplier;

            float movement = currentSpeed * Time.deltaTime;

            if (distanceTravelled + movement > totalDistance)
            {
                movement = totalDistance - distanceTravelled;
            }

            MoveSymbols(movement);
            distanceTravelled += movement;

            if (Mathf.Approximately(distanceTravelled, totalDistance))
                break;

            yield return null;
        }

        SnapToGrid(targetIndex);

        isSpinning = false;
    }

    private void MoveSymbols(float distance)
    {
        for (int i = 0; i < symbols.Length; i++)
        {
            Vector2 position = symbols[i].anchoredPosition;
            position.y -= distance;
            symbols[i].anchoredPosition = position;
        }

        RecycleSymbols();
    }

    private void RecycleSymbols()
    {
        float threshold = -symbolSpacing * 2.5f;

        for (int i = 0; i < symbols.Length; i++)
        {
            if (symbols[i].anchoredPosition.y < threshold)
            {
                float highestY = float.MinValue;
                for (int j = 0; j < symbols.Length; j++)
                {
                    if (symbols[j].anchoredPosition.y > highestY)
                    {
                        highestY = symbols[j].anchoredPosition.y;
                    }
                }

                Vector2 pos = symbols[i].anchoredPosition;
                pos.y = highestY + symbolSpacing;
                symbols[i].anchoredPosition = pos;
            }
        }
    }

    private void SnapToGrid(int targetIndex)
    {
        int count = symbols.Length;
        int halfCount = count / 2;

        for (int i = 0; i < count; i++)
        {
            int offset = i - targetIndex;

            if (offset > halfCount) offset -= count;
            if (offset < -halfCount) offset += count;

            symbols[i].anchoredPosition = new Vector2(0f, offset * symbolSpacing);
        }
    }
}