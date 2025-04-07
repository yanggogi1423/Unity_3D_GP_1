using TMPro;
using UnityEngine;

public class PlayerUIControl : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text playerHp;

    [Header("Player")]
    public PlayerCtrl player;
    
    private void Start()
    {
        if(player == null) Debug.LogError("Player is null");
        
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>();
        
        playerHp.SetText("Hp : " + player.curHp + "/" + player.maxHp);
    }

    public void UpdatePlayerUI()
    {
        playerHp.SetText("Hp : " + player.curHp + "/" + player.maxHp);
    }
}
