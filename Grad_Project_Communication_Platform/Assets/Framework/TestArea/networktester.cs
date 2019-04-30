using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class networktester : MonoBehaviour
{
	public NetworkClient Client;

    // Update is called once per frame
    void Update()
    {
		if(Input.GetKeyDown(KeyCode.Space))
		{
			for(int i = 0; i < 100; i++)
			{
				Client.SendMessage(new NetworkMessage(NetworkMessageType.Null, i.ToString()));
			}
		}
    }
}
