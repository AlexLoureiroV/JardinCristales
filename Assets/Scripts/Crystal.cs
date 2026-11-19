using UnityEngine;

public class Crystal : MonoBehaviour
{
	#region Properties
	#endregion

	#region Fields
	public int prefabIndex;
	public int colorIndex;
	public float height;
	public int rarityIndex;

	public float floatAmplitude = 0.15f;
	public float floatSpeed = 1.5f;
	public float rotateSpeed = 60f;

	public Vector3 basePosition;

	private float phase;
	#endregion

	#region Unity Callbacks
	// Start is called before the first frame update
	void Start()
	{
		if (basePosition == Vector3.zero)
		{
			basePosition = transform.position;
		}

		phase = Random.value * Mathf.PI * 2f;
	}

	// Update is called once per frame
	void Update()
	{
		FloatAndSpin();
	}
	#endregion

	#region Public Methods
	public CrystalData ToData(bool inInventory)
	{
		CrystalData data = new CrystalData
		{
			prefabIndex = prefabIndex,
			position = basePosition,
			colorIndex = colorIndex,
			height = height,
			rarityIndex = rarityIndex,
			inInventory = inInventory
		};
		return data;
	}
	#endregion

	#region Private Methods
	private void FloatAndSpin()
	{
		float y = basePosition.y + Mathf.Sin(Time.time * floatSpeed + phase) * floatAmplitude;
		transform.position = new Vector3(basePosition.x, y, basePosition.z);
		transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
	}
	#endregion
}
