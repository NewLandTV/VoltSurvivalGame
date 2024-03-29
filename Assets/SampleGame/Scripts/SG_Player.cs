using System.Collections;
using UnityEngine;

public class SG_Player : MonoBehaviour
{
    [SerializeField]
    private SG_GameManager gameManager;

    private void Awake()
    {
        gameManager.ConnectToServer();
    }

    private IEnumerator Start()
    {
        while (true)
        {
            Move();

            yield return null;
        }
    }

    private void Move()
    {
        string input = string.Empty;

        if (Input.GetKeyDown(KeyCode.W))
        {
            input = "W";
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            input = "A";
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            input = "S";
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            input = "D";
        }

        gameManager.SendData(input);
    }
}
