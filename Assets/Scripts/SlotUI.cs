using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BalanceManager balanceManager;
    [SerializeField] private GameManager gameManager;

    [Header("UI Text")]
    [SerializeField] private TMP_Text balanceText;
    [SerializeField] private TMP_Text resultText;

    [Header("Bet Buttons")]
    [SerializeField] private Button bet10Button;
    [SerializeField] private Button bet50Button;
    [SerializeField] private Button bet100Button;

    private void OnEnable()
    {
        if (balanceManager != null)
        {
            balanceManager.OnBalanceChanged += UpdateBalance;
            balanceManager.OnBetChanged += HandleBetChanged;
        }

        if (gameManager != null)
        {
            gameManager.OnSpinStarted += HandleSpinStarted;
            gameManager.OnSpinWon += HandleSpinWon;
            gameManager.OnSpinLost += HandleSpinLost;
            gameManager.OnInsufficientBalance += HandleInsufficientBalance;
        }
    }

    private void OnDisable()
    {
        if (balanceManager != null)
        {
            balanceManager.OnBalanceChanged -= UpdateBalance;
            balanceManager.OnBetChanged -= HandleBetChanged;
        }

        if (gameManager != null)
        {
            gameManager.OnSpinStarted -= HandleSpinStarted;
            gameManager.OnSpinWon -= HandleSpinWon;
            gameManager.OnSpinLost -= HandleSpinLost;
            gameManager.OnInsufficientBalance -= HandleInsufficientBalance;
        }
    }

    private void Start()
    {
        if (balanceManager != null)
        {
            UpdateBalance(balanceManager.Balance);
            HandleBetChanged(balanceManager.CurrentBet);
        }

        if (resultText != null)
        {
            resultText.text = "READY";
        }
    }

    private void UpdateBalance(int balance)
    {
        if (balanceText == null)
            return;

        balanceText.text = $"{balance}G";

        UpdateBetButtons(balance);
    }

    private void HandleBetChanged(int currentBet)
    {
        if (balanceManager == null)
            return;

        UpdateBetButtons(balanceManager.Balance);
    }

    private void UpdateBetButtons(int balance)
    {
        if (bet10Button != null)
            bet10Button.interactable = balance >= 10;

        if (bet50Button != null)
            bet50Button.interactable = balance >= 50;

        if (bet100Button != null)
            bet100Button.interactable = balance >= 100;
    }

    private void HandleSpinStarted()
    {
        if (resultText == null)
            return;

        resultText.text = "SPINNING...";
    }

    private void HandleSpinWon(int payoutMultiplier)
    {
        if (resultText == null || balanceManager == null)
            return;

        int winnings = balanceManager.CurrentBet * payoutMultiplier;

        resultText.text = $"WIN! +{winnings}G";
    }

    private void HandleSpinLost()
    {
        if (resultText == null)
            return;

        resultText.text = "NO WIN";
    }

    private void HandleInsufficientBalance()
    {
        if (resultText == null)
            return;

        resultText.text = "NOT ENOUGH BALANCE!";
    }

    public void SetBet10()
    {
        SetBet(10);
    }

    public void SetBet50()
    {
        SetBet(50);
    }

    public void SetBet100()
    {
        SetBet(100);
    }

    private void SetBet(int targetBet)
    {
        if (balanceManager == null)
            return;

        if (balanceManager.Balance < targetBet)
            return;

        while (balanceManager.CurrentBet < targetBet)
        {
            balanceManager.IncreaseBet();
        }

        while (balanceManager.CurrentBet > targetBet)
        {
            balanceManager.DecreaseBet();
        }
    }
}