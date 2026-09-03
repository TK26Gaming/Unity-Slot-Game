using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;


public class GameManager : MonoBehaviour
{
    [Header("Reel References")]
    [SerializeField] private Reel reelLeft;
    [SerializeField] private Reel reelCenter;
    [SerializeField] private Reel reelRight;

    [Header("Balance Reference")]
    [SerializeField] private BalanceManager balanceManager;

    [Header("Spin Settings")]
    [SerializeField] private float reelStopDelay = 0.35f;

    [Header("Payout Multipliers")]
    [SerializeField] private int cherryPayout = 5;
    [SerializeField] private int bellPayout = 10;
    [SerializeField] private int barPayout = 20;
    [SerializeField] private int sevenPayout = 50;

    // Events for UI / Audio triggers
    public event Action OnSpinStarted;
    public event Action<int> OnSpinWon;
    public event Action OnSpinLost;
    public event Action OnInsufficientBalance;

    private readonly SlotSymbol[] reelLayout =
    {
        SlotSymbol.Seven,  // Index 0
        SlotSymbol.Cherry, // Index 1
        SlotSymbol.Bell,   // Index 2
        SlotSymbol.Bar,    // Index 3
        SlotSymbol.Seven,  // Index 4
        SlotSymbol.Bell,   // Index 5
        SlotSymbol.Bar     // Index 6
    };

    private bool isSpinning;
    public bool IsSpinning => isSpinning;

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space) && !isSpinning)
    //    {
    //        StartSpin();
    //    }
    //}

    public void StartSpin()
    {
        if (isSpinning)
            return;

        if (balanceManager == null)
        {
            Debug.LogWarning("BalanceManager is not assigned.");
            return;
        }

        if (balanceManager.Balance < balanceManager.CurrentBet)
        {
            Debug.LogWarning("Not enough balance to spin.");
            OnInsufficientBalance?.Invoke();
            return;
        }

        StartCoroutine(SpinAllReels());
    }

    private IEnumerator SpinAllReels()
    {
        isSpinning = true;
        OnSpinStarted?.Invoke();

        // 1. Generate independent target results
        int leftIndex = GetRandomReelIndex();
        int centerIndex = GetRandomReelIndex();
        int rightIndex = GetRandomReelIndex();

        SlotSymbol leftResult = reelLayout[leftIndex];
        SlotSymbol centerResult = reelLayout[centerIndex];
        SlotSymbol rightResult = reelLayout[rightIndex];

        Debug.Log(
            $"Spin Targets | LEFT: {leftResult} [{leftIndex}] | " +
            $"CENTER: {centerResult} [{centerIndex}] | " +
            $"RIGHT: {rightResult} [{rightIndex}]"
        );

        // 2. START ALL REELS AT ONCE WITH STAGGERED CYCLES
        reelLeft.Spin(leftIndex, 5);
        reelCenter.Spin(centerIndex, 7);
        reelRight.Spin(rightIndex, 9);

        // 3. LANDING SEQUENCE: Left -> Center -> Right
        yield return new WaitUntil(() => !reelLeft.IsSpinning);
        yield return new WaitForSeconds(reelStopDelay);

        yield return new WaitUntil(() => !reelCenter.IsSpinning);
        yield return new WaitForSeconds(reelStopDelay);

        yield return new WaitUntil(() => !reelRight.IsSpinning);

        // 4. Check results
        CheckResult(leftResult, centerResult, rightResult);

        isSpinning = false;
    }

    private int GetRandomReelIndex()
    {
        return Random.Range(0, reelLayout.Length);
    }

    private void CheckResult(
        SlotSymbol left,
        SlotSymbol center,
        SlotSymbol right)
    {
        bool playerWon = (left == center) && (center == right);

        if (!playerWon)
        {
            Debug.Log("No win this spin.");
            OnSpinLost?.Invoke();
            return;
        }

        int payout = GetPayout(left);

        Debug.Log(
            $"<b>WIN!</b> Matched 3x {left} | " +
            $"Payout: {payout}x"
        );

        OnSpinWon?.Invoke(payout);
    }

    private int GetPayout(SlotSymbol symbol)
    {
        return symbol switch
        {
            SlotSymbol.Cherry => cherryPayout,
            SlotSymbol.Bell => bellPayout,
            SlotSymbol.Bar => barPayout,
            SlotSymbol.Seven => sevenPayout,
            _ => 0
        };
    }
}