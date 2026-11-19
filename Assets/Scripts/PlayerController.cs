using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
	#region Properties
	#endregion

	#region Fields
	public float moveSpeed = 5f;
	#endregion

	#region Unity Callbacks
	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		Move();
	}
	#endregion

	#region Public Methods
	#endregion

	#region Private Methods
	private void Move()
	{
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");

		Vector3 direction = new Vector3(horizontal, 0f, vertical);

		if (direction.sqrMagnitude > 1f)
		{
			direction.Normalize();
		}

		CharacterController controller = GetComponent<CharacterController>();
		controller.Move(direction * moveSpeed * Time.deltaTime);
	}
	#endregion
}
