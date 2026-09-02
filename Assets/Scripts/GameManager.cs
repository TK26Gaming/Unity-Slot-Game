using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Reels")]
    [SerializeField] private Reel reel1; // Center
    [SerializeField] private Reel reel2; // Left
    [SerializeField] private Reel reel3; // Right

    [Header("Spin Settings")]
    [SerializeField] private float reelStopDelay = 0.35f;

    [Header("Payout Settings")]
    [SerializeField] private int cherryPayout = 5;
    [SerializeField] private int bellPayout = 10;
    [SerializeField] private int barPayout = 20;
    [SerializeField] private int sevenPayout = 50;

    // Exact symbol order used by every reel.
    //
    // 0 = 7
    // 1 = Cherry
    // 2 = Bell
    // 3 = BAR
    // 4 = 7
    // 5 = Bell
    // 6 = BAR
    private readonly SlotSymbol[] reelLayout =
    {
        SlotSymbol.Seven,
        SlotSymbol.Cherry,
        SlotSymbol.Bell,
        SlotSymbol.Bar,
        SlotSymbol.Seven,
        SlotSymbol.Bell,
        SlotSymbol.Bar
    };

    private bool isSpinning;

    public bool IsSpinning => isSpinning;

    private void Update()
    {
        // Temporary testing input.
        // This will later be replaced by the lever.
        if (Input.GetKeyDown(KeyCode.Space) && !isSpinning)
        {
            StartSpin();
        }
    }

    /// <summary>
    /// Starts a complete slot machine spin.
    /// </summary>
    public void StartSpin()
    {
        if (isSpinning)
            return;

        StartCoroutine(SpinAllReels());
    }

    /// <summary>
    /// Spins all three reels with independent random results.
    /// </summary>
    private IEnumerator SpinAllReels()
    {
        isSpinning = true;

        // Generate independent random positions.
        int reel1ResultIndex = GetRandomReelIndex();
        int reel2ResultIndex = GetRandomReelIndex();
        int reel3ResultIndex = GetRandomReelIndex();

        // Convert positions into symbols.
        SlotSymbol reel1Result = reelLayout[reel1ResultIndex];
        SlotSymbol reel2Result = reelLayout[reel2ResultIndex];
        SlotSymbol reel3Result = reelLayout[reel3ResultIndex];

        Debug.Log(
            $"Spin Results | " +
            $"LEFT (Reel2): {reel2Result} [{reel2ResultIndex}] | " +
            $"CENTER (Reel1): {reel1Result} [{reel1ResultIndex}] | " +
            $"RIGHT (Reel3): {reel3Result} [{reel3ResultIndex}]"
        );

        // CENTER reel.
        reel1.Spin(reel1ResultIndex);

        yield return new WaitForSeconds(reelStopDelay);

        // LEFT reel.
        reel2.Spin(reel2ResultIndex);

        yield return new WaitForSeconds(reelStopDelay);

        // RIGHT reel.
        reel3.Spin(reel3ResultIndex);

        // Wait until every reel has stopped.
        yield return new WaitUntil(() =>
            !reel1.IsSpinning &&
            !reel2.IsSpinning &&
            !reel3.IsSpinning
        );

        CheckResult(
            reel1Result,
            reel2Result,
            reel3Result
        );

        isSpinning = false;
    }

    /// <summary>
    /// Returns a random position from the seven-position reel.
    /// </summary>
    private int GetRandomReelIndex()
    {
        return Random.Range(0, reelLayout.Length);
    }

    /// <summary>
    /// Checks whether all three reels show the same symbol.
    /// </summary>
    private void CheckResult(
        SlotSymbol reel1Result,
        SlotSymbol reel2Result,
        SlotSymbol reel3Result)
    {
        bool playerWon =
            reel1Result == reel2Result &&
            reel2Result == reel3Result;

        if (!playerWon)
        {
            Debug.Log("No win this time.");
            return;
        }

        int payout = GetPayout(reel1Result);

        Debug.Log(
            $"WIN! {reel1Result} x3 | Payout: {payout}x"
        );
    }

    /// <summary>
    /// Returns the payout multiplier for a matching symbol.
    /// </summary>
    private int GetPayout(SlotSymbol symbol)
    {
        switch (symbol)
        {
            case SlotSymbol.Cherry:
                return cherryPayout;

            case SlotSymbol.Bell:
                return bellPayout;

            case SlotSymbol.Bar:
                return barPayout;

            case SlotSymbol.Seven:
                return sevenPayout;

            default:
                return 0;
        }
    }
}