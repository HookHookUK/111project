using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Button_PointUpDown : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Player player;
    private bool isDown = false;
    private int amount; // ¡°«¡ ∞‘¿Ã¡ˆ »πµÊ∑Æ
    public void OnPointerDown(PointerEventData eventData)
    {
        player.jumpForce = 200;
        isDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDown = false;
        amount = Mathf.FloorToInt(player.jumpForce);
        player.OnClick_Jump(amount / 2);

        GameMGR.Inst.ResetSliderPos(0);

    }

    private void Update()
    {
        if (!isDown) return;
        if (player.jumpForce < player.jumpMaxForce) player.jumpForce += player.jumpGageSpeed;
        else player.jumpForce = player.jumpMaxForce;
    }
}
