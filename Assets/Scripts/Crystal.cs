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
	#endregion

	#region Unity Callbacks
	#endregion

	#region Public Methods
	public CrystalData ToData(bool inInventory)
	{
		CrystalData data = new CrystalData
		{
			prefabIndex = prefabIndex,
			position = transform.position,
			colorIndex = colorIndex,
			height = height,
			rarityIndex = rarityIndex,
			inInventory = inInventory
		};
		return data;
	}
	#endregion

	#region Private Methods
	#endregion
}
