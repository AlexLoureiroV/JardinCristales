using System.Collections.Generic;
using UnityEngine;

public class GardenManager : MonoBehaviour
{
	#region Properties
	private string SaveFilePath => Application.dataPath + "/dataFinal.json";
	#endregion

	#region Fields
	[System.Serializable]
	public class SaveData
	{
		public List<CrystalData> crystals = new List<CrystalData>();
	}

	[Header("Configuración del jardín")]
	public List<GameObject> crystalPrefabs = new List<GameObject>();
	public int maxCrystals = 15;
	public float gardenSize = 10f;
	public float collectRadius = 2f;

	[Header("Configuración de cristales (arrays)")]
	public Color[] possibleColors;
	public float[] possibleHeights;
	public string[] rarityNames;

	[Header("Debug")]
	public bool showDebug = true;

	private List<Crystal> plantedCrystals = new List<Crystal>();
	private List<CrystalData> inventory = new List<CrystalData>();
	#endregion

	#region Unity Callbacks
	// Start is called before the first frame update
	void Start()
	{
		LoadData();
		PrintStatus();
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			PlantRandomCrystal();
		}

		if (Input.GetKeyDown(KeyCode.E))
		{
			CollectNearestCrystal();
		}
	}

	void OnDestroy()
	{
		SaveFile();
	}
	#endregion

	#region Public Methods
	#endregion

	#region Private Methods
	private void PlantRandomCrystal()
	{
		if (plantedCrystals.Count >= maxCrystals)
		{
			Debug.Log($"[Jardín] El jardín está lleno ({maxCrystals} cristales).");
			return;
		}

		int prefabIndex = Random.Range(0, crystalPrefabs.Count);
		int colorIndex = Random.Range(0, possibleColors.Length);
		float height = possibleHeights[Random.Range(0, possibleHeights.Length)];
		int rarityIndex = Random.Range(0, rarityNames.Length);

		Vector3 position = RandomGardenPosition(height);
		CreateObject(position, prefabIndex, colorIndex, height, rarityIndex);

		Debug.Log($"[Jardín] Plantado cristal {rarityNames[rarityIndex]} en {position}.");
		PrintStatus();
	}

	private void CollectNearestCrystal()
	{
		Crystal nearest = FindNearestCrystalInRadius();

		if (nearest == null)
		{
			Debug.Log($"[Jardín] No hay cristales a menos de {collectRadius} m del centro.");
			return;
		}

		CrystalData data = nearest.ToData(true);
		inventory.Add(data);

		plantedCrystals.Remove(nearest);
		Destroy(nearest.gameObject);

		Debug.Log($"[Inventario] Recogido cristal: {Describe(data)}. Total: {inventory.Count}.");
		PrintStatus();
	}

	private Crystal FindNearestCrystalInRadius()
	{
		Crystal nearest = null;
		float nearestDistance = float.MaxValue;

		foreach (Crystal crystal in plantedCrystals)
		{
			float distance = Vector3.Distance(crystal.transform.position, Vector3.zero);
			if (distance <= collectRadius && distance < nearestDistance)
			{
				nearest = crystal;
				nearestDistance = distance;
			}
		}

		return nearest;
	}

	private void CreateObject(Vector3 position, int prefabIndex, int colorIndex, float height, int rarityIndex)
	{
		GameObject instance = Instantiate(crystalPrefabs[prefabIndex], position, Quaternion.identity);

		Crystal crystal = instance.GetComponent<Crystal>();
		if (crystal == null)
		{
			crystal = instance.AddComponent<Crystal>();
		}

		crystal.prefabIndex = prefabIndex;
		crystal.colorIndex = colorIndex;
		crystal.height = height;
		crystal.rarityIndex = rarityIndex;

		instance.transform.localScale = new Vector3(1f, height, 1f);
		Renderer renderer = instance.GetComponent<Renderer>();
		if (renderer != null)
		{
			renderer.material.color = possibleColors[colorIndex];
		}

		plantedCrystals.Add(crystal);
	}

	private Vector3 RandomGardenPosition(float height)
	{
		float half = gardenSize / 2f - 0.5f;
		return new Vector3(Random.Range(-half, half), height / 2f, Random.Range(-half, half));
	}

	private SaveData Save()
	{
		SaveData data = new SaveData();

		foreach (Crystal crystal in plantedCrystals)
		{
			data.crystals.Add(crystal.ToData(false));
		}

		data.crystals.AddRange(inventory);
		return data;
	}

	private void SaveFile()
	{
		string json = JsonUtility.ToJson(Save(), true);
		System.IO.File.WriteAllText(SaveFilePath, json);
		Debug.Log($"[Jardín] Guardado en {SaveFilePath}");
	}

	private void LoadData()
	{
		if (!System.IO.File.Exists(SaveFilePath))
		{
			Debug.Log("[Jardín] No hay archivo de guardado. Jardín vacío.");
			return;
		}

		string json = System.IO.File.ReadAllText(SaveFilePath);
		SaveData data = JsonUtility.FromJson<SaveData>(json);

		plantedCrystals.Clear();
		inventory.Clear();

		foreach (CrystalData crystalData in data.crystals)
		{
			if (crystalData.inInventory)
			{
				inventory.Add(crystalData);
			}
			else
			{
				CreateObject(crystalData.position, crystalData.prefabIndex, crystalData.colorIndex, crystalData.height, crystalData.rarityIndex);
			}
		}

		Debug.Log($"[Jardín] Cargados {data.crystals.Count} cristales desde el archivo.");
	}

	private string Describe(CrystalData data)
	{
		return $"{rarityNames[data.rarityIndex]}, color {possibleColors[data.colorIndex]}, altura {data.height:F1}";
	}

	private void PrintStatus()
	{
		if (!showDebug) return;
		Debug.Log($"[Estado] Plantados: {plantedCrystals.Count} | Inventario: {inventory.Count}");
	}
	#endregion
}
