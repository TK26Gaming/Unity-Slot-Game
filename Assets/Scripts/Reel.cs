using System.Collections;
using UnityEngine;

public class Reel : MonoBehaviour
{
    [Header("Reel References")]
    [SerializeField] private RectTransform symbolContainer;

    [Header("Reel Settings")]
    [SerializeField] private float symbolSpacing = 130f;
    [SerializeField] private float spinSpeed = 1800f;
   // [SerializeField] private float spinDuration = 2f;
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
        int count = symbolContainer.childCount;

        symbols = new RectTransform[count];

        for (int i = 0; i < count; i++)
        {
            symbols[i] = symbolContainer
                .GetChild(i)
                .GetComponent<RectTransform>();
        }
    }

    /// <summary>
    /// Starts the reel spinning.
    /// The selected symbol will naturally land in the center
    /// when the reel finishes.
    /// </summary>
    public void Spin(int targetIndex)
    {
        if (isSpinning)
            return;

        if (symbols == null || symbols.Length == 0)
        {
            Debug.LogWarning(
                gameObject.name +
                " has no symbols."
            );

            return;
        }

        if (targetIndex < 0 ||
            targetIndex >= symbols.Length)
        {
            Debug.LogWarning(
                "Invalid target index: " +
                targetIndex
            );

            return;
        }

        StartCoroutine(
            SpinRoutine(targetIndex)
        );
    }

    private IEnumerator SpinRoutine(int targetIndex)
    {
        isSpinning = true;

        RectTransform target =
            symbols[targetIndex];

        // ---------------------------------------------------------
        // Determine where the target symbol currently is.
        // ---------------------------------------------------------

        float targetY =
            target.anchoredPosition.y;

        float cycleDistance =
            symbolSpacing * symbols.Length;

        /*
         * Symbols move DOWN.
         *
         * We want:
         *
         * targetY - distance = 0
         *
         * So the distance must be targetY.
         *
         * If targetY is negative, add complete reel cycles
         * until the target can reach the center while continuing
         * to move in the same direction.
         */

        float targetDistance = targetY;

        while (targetDistance <= 0f)
        {
            targetDistance += cycleDistance;
        }

        // Add extra complete cycles so the reel gets
        // a satisfying long spin.
        targetDistance +=
            cycleDistance * minimumCycles;

        // ---------------------------------------------------------
        // Spin and naturally slow down over the calculated distance.
        // ---------------------------------------------------------

        float distanceTravelled = 0f;

        while (distanceTravelled < targetDistance)
        {
            float progress =
                distanceTravelled /
                targetDistance;

            /*
             * Speed starts fast and gradually becomes slower.
             * This is one continuous movement.
             */
            float speedMultiplier =
                Mathf.SmoothStep(
                    1f,
                    0.08f,
                    progress
                );

            float currentSpeed =
                spinSpeed * speedMultiplier;

            float remainingDistance =
                targetDistance -
                distanceTravelled;

            float movement =
                currentSpeed *
                Time.deltaTime;

            // Never move farther than the calculated
            // stopping distance.
            if (movement > remainingDistance)
            {
                movement = remainingDistance;
            }

            MoveSymbols(movement);

            distanceTravelled += movement;

            yield return null;
        }

        // ---------------------------------------------------------
        // The target should now naturally be at the center.
        //
        // There is NO repositioning here.
        // ---------------------------------------------------------

        isSpinning = false;
    }

    /// <summary>
    /// Moves all symbols together.
    /// </summary>
    private void MoveSymbols(float distance)
    {
        for (int i = 0; i < symbols.Length; i++)
        {
            Vector2 position =
                symbols[i].anchoredPosition;

            position.y -= distance;

            symbols[i].anchoredPosition =
                position;
        }

        RecycleSymbols();
    }

    /// <summary>
    /// Loops symbols from bottom to top while maintaining
    /// the exact spacing between them.
    /// </summary>
    private void RecycleSymbols()
    {
        float highestY =
            float.MinValue;

        // Find the highest symbol.
        for (int i = 0; i < symbols.Length; i++)
        {
            float y =
                symbols[i].anchoredPosition.y;

            if (y > highestY)
            {
                highestY = y;
            }
        }

        // Recycle symbols that have moved below
        // the visible reel area.
        for (int i = 0; i < symbols.Length; i++)
        {
            RectTransform symbol =
                symbols[i];

            if (symbol.anchoredPosition.y <
                -symbolSpacing * 4f)
            {
                Vector2 position =
                    symbol.anchoredPosition;

                position.y =
                    highestY + symbolSpacing;

                symbol.anchoredPosition =
                    position;

                highestY =
                    position.y;
            }
        }
    }
}