using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : Player
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CoSendPacket());
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator CoSendPacket()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            float xpos = UnityEngine.Random.Range(5, -5);
            float ypos = UnityEngine.Random.Range(5, -5);
            transform.position = new Vector2(xpos, ypos);

            C_Move movePacket = new C_Move();
            movePacket.xPos = xpos;
            movePacket.yPos = ypos;
            movePacket.zPos = 0;

            NetworkManager.Instance.Send(movePacket.Write());
        }
    }
}
