using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

	[Header("Jugador")]
	public Transform player;

	[Header("UI")]
	public Text plantedText;
	public Text inventoryText;
	public GameObject controlsPanel;

	[Header("Debug")]
	public bool showDebug = true;

	private List<Crystal> plantedCrystals = new List<Crystal>();
	private List<CrystalData> inventory = new List<CrystalData>();

	private static Mesh octahedronMesh;
	#endregion

	#region Unity Callbacks
	// Start is called before the first frame update
	void Start()
	{
		LoadData();
		UpdateUI();
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

		if (Input.GetKeyDown(KeyCode.H))
		{
			ToggleControlsPanel();
		}

		if (Input.GetKeyDown(KeyCode.X))
		{
			ResetInventory();
		}
	}

	void OnDestroy()
	{
		SaveFile();
	}
	#endregion

	#region Public Methods
	public void UpdateUI()
	{
		if (plantedText != null)
		{
			plantedText.text = $"Cristales en el jardín: {plantedCrystals.Count}";
		}

		if (inventoryText != null)
		{
			inventoryText.text = BuildInventoryText();
		}
	}
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
		UpdateUI();
	}

	private void CollectNearestCrystal()
	{
		if (player == null)
		{
			Debug.Log("[Jardín] No hay jugador asignado en el GardenManager.");
			return;
		}

		Crystal nearest = FindNearestCrystalInRadius(player.position);

		if (nearest == null)
		{
			Debug.Log($"[Jardín] No hay cristales a menos de {collectRadius} m del jugador.");
			return;
		}

		CrystalData data = nearest.ToData(true);
		inventory.Add(data);

		plantedCrystals.Remove(nearest);
		Destroy(nearest.gameObject);

		Debug.Log($"[Inventario] Recogido cristal: {Describe(data)}. Total: {inventory.Count}.");
		PrintStatus();
		UpdateUI();
	}

	private void ResetInventory()
	{
		if (inventory.Count == 0)
		{
			Debug.Log("[Inventario] El inventario ya está vacío.");
			return;
		}

		inventory.Clear();
		Debug.Log("[Inventario] Inventario vaciado.");
		PrintStatus();
		UpdateUI();
	}

	private Crystal FindNearestCrystalInRadius(Vector3 center)
	{
		Crystal nearest = null;
		float nearestDistance = float.MaxValue;

		foreach (Crystal crystal in plantedCrystals)
		{
			float distance = Vector3.Distance(crystal.transform.position, center);
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

		crystal.basePosition = position;
		crystal.floatSpeed = Random.Range(1f, 2f);
		crystal.rotateSpeed = Random.Range(30f, 90f);

		MeshFilter meshFilter = instance.GetComponent<MeshFilter>();
		if (meshFilter != null)
		{
			meshFilter.sharedMesh = GetOctahedronMesh();
		}

		plantedCrystals.Add(crystal);
	}

	private Mesh GetOctahedronMesh()
	{
		if (octahedronMesh == null)
		{
			float radius = 0.5f;

			Vector3 top = new Vector3(0f, radius, 0f);
			Vector3 bottom = new Vector3(0f, -radius, 0f);
			Vector3 right = new Vector3(radius, 0f, 0f);
			Vector3 left = new Vector3(-radius, 0f, 0f);
			Vector3 front = new Vector3(0f, 0f, radius);
			Vector3 back = new Vector3(0f, 0f, -radius);

			Vector3[] corners = { top, bottom, right, left, front, back };
			int[] faces = { 0, 4, 2, 0, 3, 4, 0, 5, 3, 0, 2, 5, 1, 2, 4, 1, 4, 3, 1, 3, 5, 1, 5, 2 };

			Vector3[] vertices = new Vector3[24];
			int[] triangles = new int[24];

			for (int i = 0; i < 24; i++)
			{
				vertices[i] = corners[faces[i]];
				triangles[i] = i;
			}

			Mesh mesh = new Mesh();
			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			octahedronMesh = mesh;
		}

		return octahedronMesh;
	}

	private Vector3 RandomGardenPosition(float height)
	{
		float half = gardenSize / 2f - 0.5f;
		return new Vector3(Random.Range(-half, half), height / 2f, Random.Range(-half, half));
	}

	private SaveData Save()
	{
		SaveData data = new SaveData();

		plantedCrystals.RemoveAll(crystal => crystal == null);

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

	private void ToggleControlsPanel()
	{
		if (controlsPanel == null)
		{
			return;
		}

		controlsPanel.SetActive(!controlsPanel.activeSelf);
	}

	private string BuildInventoryText()
	{
		int[] rarityCounts = new int[rarityNames.Length];

		foreach (CrystalData data in inventory)
		{
			if (data.rarityIndex >= 0 && data.rarityIndex < rarityCounts.Length)
			{
				rarityCounts[data.rarityIndex]++;
			}
		}

		string result = $"Inventario: {inventory.Count} cristales\n";

		for (int i = 0; i < rarityNames.Length; i++)
		{
			result += $"{rarityNames[i]}: {rarityCounts[i]}\n";
		}

		return result.TrimEnd('\n');
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
