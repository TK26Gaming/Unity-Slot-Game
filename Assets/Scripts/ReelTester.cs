using UnityEngine;

public class ReelTester : MonoBehaviour
{
    [SerializeField] private Reel reel;

    [SerializeField] private int targetSymbol = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            reel.Spin(targetSymbol);
        }
    }
}