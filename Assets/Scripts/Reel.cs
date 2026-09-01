using System.Collections;
using UnityEngine;

public class Reel : MonoBehaviour
{
    [Header("Reel References")]
    [SerializeField] private RectTransform symbolContainer;

    [Header("Reel Settings")]
    [SerializeField] private float symbolSpacing = 130f;
    [SerializeField] private float spinSpeed = 1800f;
    [SerializeField] private float spinDuration = 1.5f;
    [SerializeField] private int minimumCycles = 3;

    private RectTransform[] symbols;
    private bool isSpinning;

    public bool IsSpinning => isSpinning;

    private void Awake()
    {
        CacheSymbols();
    }

    private void CacheSymbols()
    {
        int symbolCount = symbolContainer.childCount;

        symbols = new RectTransform[symbolCount];

        for (int i = 0; i < symbolCount; i++)
        {
            symbols[i] = symbolContainer.GetChild(i).GetComponent<RectTransform>();
        }
    }

    /// <summary>
    /// Starts the reel spinning.
    /// targetIndex determines which symbol should stop in the center.
    /// </summary>
    public void Spin(int targetIndex)
    {
        if (isSpinning)
            return;

        if (symbols == null || symbols.Length == 0)
        {
            Debug.LogWarning("Reel has no symbols assigned.");
            return;
        }

        if (targetIndex < 0 || targetIndex >= symbols.Length)
        {
            Debug.LogWarning("Invalid target index: " + targetIndex);
            return;
        }

        StartCoroutine(SpinRoutine(targetIndex));
    }

    private IEnumerator SpinRoutine(int targetIndex)
    {
        isSpinning = true;

        // Store the target symbol's current position.
        float targetStartY = symbols[targetIndex].anchoredPosition.y;

        // Move enough distance for several complete rotations.
        float cycleDistance = symbolSpacing * symbols.Length;
        float totalDistance = cycleDistance * minimumCycles + targetStartY;

        // Make sure we always have a positive amount of movement.
        totalDistance = Mathf.Abs(totalDistance);

        float elapsed = 0f;
        float distanceMoved = 0f;

        while (elapsed < spinDuration)
        {
            float progress = elapsed / spinDuration;

            // Smooth acceleration/deceleration curve.
            float speedMultiplier = Mathf.SmoothStep(0.4f, 1f, progress);

            float currentSpeed = spinSpeed * speedMultiplier;

            float distanceThisFrame = currentSpeed * Time.deltaTime;

            MoveSymbols(distanceThisFrame);

            distanceMoved += distanceThisFrame;
            elapsed += Time.deltaTime;

            yield return null;
        }

        // Continue until we have completed the required distance.
        while (distanceMoved < totalDistance)
        {
            float remainingDistance = totalDistance - distanceMoved;

            // Gradually slow down near the end.
            float slowdown = Mathf.Clamp01(remainingDistance / 300f);

            float currentSpeed = Mathf.Lerp(
                150f,
                spinSpeed,
                slowdown
            );

            float distanceThisFrame = currentSpeed * Time.deltaTime;

            if (distanceThisFrame > remainingDistance)
            {
                distanceThisFrame = remainingDistance;
            }

            MoveSymbols(distanceThisFrame);

            distanceMoved += distanceThisFrame;

            yield return null;
        }

        // Align every symbol perfectly to the 130-unit grid.
        SnapSymbolsToGrid();

        isSpinning = false;
    }

    private void MoveSymbols(float distance)
    {
        for (int i = 0; i < symbols.Length; i++)
        {
            Vector2 position = symbols[i].anchoredPosition;

            // Move symbols downward.
            position.y -= distance;

            symbols[i].anchoredPosition = position;
        }

        RecycleSymbols();
    }

    private void RecycleSymbols()
    {
        float highestY = float.MinValue;
        float lowestY = float.MaxValue;

        // Find the highest and lowest symbols.
        for (int i = 0; i < symbols.Length; i++)
        {
            float y = symbols[i].anchoredPosition.y;

            if (y > highestY)
                highestY = y;

            if (y < lowestY)
                lowestY = y;
        }

        // If a symbol moves below the strip,
        // move it back to the top.
        for (int i = 0; i < symbols.Length; i++)
        {
            RectTransform symbol = symbols[i];

            if (symbol.anchoredPosition.y < -symbolSpacing * 4f)
            {
                Vector2 position = symbol.anchoredPosition;

                position.y = highestY + symbolSpacing;

                symbol.anchoredPosition = position;

                highestY = position.y;
            }
        }
    }

    private void SnapSymbolsToGrid()
    {
        for (int i = 0; i < symbols.Length; i++)
        {
            Vector2 position = symbols[i].anchoredPosition;

            position.y =
                Mathf.Round(position.y / symbolSpacing) * symbolSpacing;

            symbols[i].anchoredPosition = position;
        }
    }
}