using System;
using UnityEngine;

public class BalanceManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;

    [Header("Balance Settings")]
    [SerializeField] private int startingBalance = 1000;

    [Header("Bet Settings")]
    [SerializeField] private int startingBet = 10;
    [SerializeField] private int minimumBet = 1;
    [SerializeField] private int maximumBet = 100;

    private int balance;
    private int currentBet;

    public int Balance => balance;
    public int CurrentBet => currentBet;

    public event Action<int> OnBalanceChanged;
    public event Action<int> OnBetChanged;

    private void Awake()
    {
        balance = startingBalance;
        currentBet = startingBet;
    }

    private void OnEnable()
    {
        if (gameManager == null)
            return;

        gameManager.OnSpinStarted += HandleSpinStarted;
        gameManager.OnSpinWon += HandleSpinWon;
    }

    private void OnDisable()
    {
        if (gameManager == null)
            return;

        gameManager.OnSpinStarted -= HandleSpinStarted;
        gameManager.OnSpinWon -= HandleSpinWon;
    }

    private void Start()
    {
        OnBalanceChanged?.Invoke(balance);
        OnBetChanged?.Invoke(currentBet);
    }

    private void HandleSpinStarted()
    {
        if (balance < currentBet)
        {
            Debug.LogWarning(
                "Not enough balance for this bet."
            );

            return;
        }

        balance -= currentBet;

        Debug.Log(
            $"Bet placed: {currentBet} | " +
            $"Balance: {balance}"
        );

        OnBalanceChanged?.Invoke(balance);
    }

    private void HandleSpinWon(int payoutMultiplier)
    {
        int winnings =
            currentBet * payoutMultiplier;

        balance += winnings;

        Debug.Log(
            $"Winnings: {winnings} | " +
            $"Balance: {balance}"
        );

        OnBalanceChanged?.Invoke(balance);
    }

    public void IncreaseBet()
    {
        if (currentBet >= maximumBet)
            return;

        currentBet++;

        OnBetChanged?.Invoke(currentBet);

        Debug.Log(
            $"Current Bet: {currentBet}"
        );
    }

    public void DecreaseBet()
    {
        if (currentBet <= minimumBet)
            return;

        currentBet--;

        OnBetChanged?.Invoke(currentBet);

        Debug.Log(
            $"Current Bet: {currentBet}"
        );
    }

    public void ResetBalance()
    {
        balance = startingBalance;
        currentBet = startingBet;

        OnBalanceChanged?.Invoke(balance);
        OnBetChanged?.Invoke(currentBet);

        Debug.Log(
            $"Balance reset: {balance} | " +
            $"Bet: {currentBet}"
        );
    }
}