using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LeverController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Image leverImage;

    [Header("Lever Sprites")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite pulledSprite;

    [Header("Pulled Position")]
    [SerializeField] private float pulledPositionY = -60f;

    [Header("Animation")]
    [SerializeField] private float pulledTime = 0.15f;
    [SerializeField] private float returnDelay = 0.1f;

    private RectTransform leverTransform;
    private Vector2 normalPosition;
    private bool isAnimating;

    private void Awake()
    {
        if (leverImage == null)
            leverImage = GetComponent<Image>();

        if (leverImage != null)
            leverTransform = leverImage.rectTransform;

        if (leverTransform != null)
            normalPosition = leverTransform.anchoredPosition;
    }

    public void PullLever()
    {
        if (isAnimating)
            return;

        if (gameManager == null)
        {
            Debug.LogWarning("GameManager is not assigned to LeverController.");
            return;
        }

        if (gameManager.IsSpinning)
            return;

        StartCoroutine(PullLeverRoutine());
    }

    private IEnumerator PullLeverRoutine()
    {
        isAnimating = true;

        // Change to pulled lever sprite
        if (leverImage != null && pulledSprite != null)
        {
            leverImage.sprite = pulledSprite;
        }

        // Move the lever downward
        if (leverTransform != null)
        {
            Vector2 pulledPosition = normalPosition;
            pulledPosition.y += pulledPositionY;

            leverTransform.anchoredPosition = pulledPosition;
        }

        // Start the slot machine
        gameManager.StartSpin();

        // Keep the lever pulled briefly
        yield return new WaitForSeconds(pulledTime);

        // Small delay before returning
        yield return new WaitForSeconds(returnDelay);

        // Return to normal sprite
        if (leverImage != null && normalSprite != null)
        {
            leverImage.sprite = normalSprite;
        }

        // Return to normal position
        if (leverTransform != null)
        {
            leverTransform.anchoredPosition = normalPosition;
        }

        isAnimating = false;
    }
}