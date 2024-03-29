using UnityEngine;

public class SG_Title : MonoBehaviour
{
    public void OnGameStartButtonClick()
    {
        SG_Loading.LoadScene(Scenes.SG_Game);
    }
}
