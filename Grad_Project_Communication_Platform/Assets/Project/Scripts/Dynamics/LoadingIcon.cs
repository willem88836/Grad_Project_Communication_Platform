using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingIcon : MonoBehaviour
{
	public Transform[] IconElements;


	public float Speed;
	public float Spread;
	public float Magnitude;
	public float Floor;

    // Update is called once per frame
    void Update()
    {
		for (int i = 0; i < IconElements.Length; i++)
		{
			Transform transform = IconElements[i];
			float size = Mathf.Clamp(Magnitude * Mathf.Sin(i * Spread / IconElements.Length * Mathf.PI + (Time.time * Speed)), Floor, int.MaxValue);
			transform.localScale = Vector3.one * size;
		}

    }
}
