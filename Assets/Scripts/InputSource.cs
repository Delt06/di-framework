using UnityEngine;

public sealed class InputSource : IInputSource
{
	public float HorizontalAxis => Input.GetAxis("Horizontal");
	public float VerticalAxis => Input.GetAxis("Vertical");
}