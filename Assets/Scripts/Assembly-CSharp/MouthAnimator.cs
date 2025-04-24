using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using BezierSolution;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEngine.Networking;


public class MouthAnimator : MonoBehaviour
{
	private const long PHONEME_P_CLOSED = 37L;

	private const long PHONEME_P_OPEN = 50L;

	private const long PHONEME_B_CLOSED = 17L;

	private const long PHONEME_B_OPEN = 51L;

	private const long PHONEME_CH_CLOSED = 18L;

	private const long PHONEME_CH_OPEN = 52L;

	private const long PHONEME_JH_CLOSED = 29L;

	private const long PHONEME_JH_OPEN = 53L;

	private const long PHONEME_D_CLOSED = 19L;

	private const long PHONEME_D_OPEN = 54L;

	private const long PHONEME_T_CLOSED = 41L;

	private const long PHONEME_T_OPEN = 55L;

	private const long PHONEME_G_CLOSED = 25L;

	private const long PHONEME_G_OPEN = 56L;

	private const long PHONEME_K_CLOSED = 30L;

	private const long PHONEME_K_OPEN = 57L;

	public GameObject lowerJawBone;

	public GameObject tongue;

	public GameObject palate;

	public GameObject uvula;

	public GameObject lips;

	public BezierSpline spline;

	private List<List<int>> duplicateVerts = new List<List<int>>();

	private List<List<GameObject>> tongeTargets = new List<List<GameObject>>();

	private List<List<GameObject>> lipTargets = new List<List<GameObject>>();

	private List<GameObject> jawTargets = new List<GameObject>();

	private List<GameObject> palateTargets = new List<GameObject>();

	private List<GameObject> uvulaTargets = new List<GameObject>();

	private List<List<GameObject>> airflowTargets = new List<List<GameObject>>();

	private SkinnedMeshRenderer meshFilter;

	private Mesh sourceMesh;

	private Mesh mesh;

	private long prevPhoneme;

	private bool animating;

	public Vector3 scaleFactor = new Vector3(1.05f, 1.05f, 1.05f);

	public int smoothFactor = 4;

	public bool continualUpdate;

	private static readonly int tongueVectors = 63;

	private static readonly int lipVectors = 22;

	private static readonly int airflowVectors = 6;

	private int i;

	private int n;

	private int j;

	private int k;
	
	private void Awake()
	{
		CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
		meshFilter = tongue.GetComponentInChildren<SkinnedMeshRenderer>();
		if (meshFilter == null)
			Debug.Log("Tongue SkinnedMeshRenderer not found");

		tongue.transform.localScale = Vector3.Scale(transform.localScale, scaleFactor);
		StartCoroutine(LoadAllPhonemeTargets());
	}

	private IEnumerator LoadAllPhonemeTargets()
	{
		i = 0;
		while (true)
		{
			tongeTargets.Add(new List<GameObject>());
			lipTargets.Add(new List<GameObject>());
			airflowTargets.Add(new List<GameObject>());

			string path = Path.Combine(Application.streamingAssetsPath, $"langs/en_us/transforms/phoneme_{i}.txt");

	#if UNITY_ANDROID && !UNITY_EDITOR
			UnityWebRequest request = UnityWebRequest.Get(path);
			yield return request.SendWebRequest();

			if (request.result != UnityWebRequest.Result.Success)
			{
				Debug.Log("File not found or failed: " + path + " - " + request.error);
				break;
			}

			string[] array = request.downloadHandler.text.Split('\n');
	#else
			if (!File.Exists(path))
			{
				Debug.Log("File not found: " + path);
				break;
			}

			string[] array = File.ReadAllLines(path);
	#endif

			Debug.Log("Loaded file: " + path);

			n = 0;
			for (n = 0; n < tongueVectors; n++)
			{
				float[] array2 = array[n].Split(',').Select(Convert.ToSingle).ToArray();
				tongeTargets[i].Add(new GameObject("tongueTarget." + i + "(" + n + ")"));
				tongeTargets[i][n].transform.parent = transform;
				tongeTargets[i][n].transform.position = new Vector3(array2[0], array2[1], array2[2]);
			}
			for (j = 0; j < lipVectors; j++)
			{
				float[] array3 = array[n++].Split(',').Select(Convert.ToSingle).ToArray();
				lipTargets[i].Add(new GameObject("lipTarget." + i + "(" + j + ")"));
				lipTargets[i][j].transform.parent = transform;
				lipTargets[i][j].transform.position = new Vector3(array3[0], array3[1], array3[2]);
			}
			float[] array4 = array[n++].Split(',').Select(Convert.ToSingle).ToArray();
			jawTargets.Add(new GameObject("jawTarget." + i));
			jawTargets[i].transform.parent = transform;
			jawTargets[i].transform.eulerAngles = new Vector3(array4[0], array4[1], array4[2]);

			float[] array5 = array[n++].Split(',').Select(Convert.ToSingle).ToArray();
			palateTargets.Add(new GameObject("palateTarget." + i));
			palateTargets[i].transform.parent = transform;
			palateTargets[i].transform.eulerAngles = new Vector3(array5[0], array5[1], array5[2]);

			float[] array6 = array[n++].Split(',').Select(Convert.ToSingle).ToArray();
			uvulaTargets.Add(new GameObject("uvulaTarget." + i));
			uvulaTargets[i].transform.parent = transform;
			uvulaTargets[i].transform.eulerAngles = new Vector3(array6[0], array6[1], array6[2]);

			for (k = 0; k < airflowVectors; k++)
			{
				float[] array7 = array[n++].Split(',').Select(Convert.ToSingle).ToArray();
				airflowTargets[i].Add(new GameObject("airflowTarget." + i + "(" + k + ")"));
				airflowTargets[i][k].transform.parent = transform;
				airflowTargets[i][k].transform.position = new Vector3(array7[0], array7[1], array7[2]);
			}

			i++;
		}

		sourceMesh = utils.CloneMesh(meshFilter.sharedMesh);
		mesh = new Mesh();
		meshFilter.BakeMesh(mesh);
		GetDuplicateVerts(ref mesh);
		mesh = SmoothingFilter.LaplacianFilter(mesh, smoothFactor);
		SetDuplicateVerts(ref mesh);
		meshFilter.sharedMesh = mesh;
		global.phoneme = 7L;
		global.duration = 100L;
		yield break; 
	}

	private void Update()
	{
		if (!animating)
		{
			StartCoroutine(animateMouth());
		}
	}

	public PhonemesMap phonemesMap;

	public void TestingFunc()
	{
		// Debug.Log("TestingFunc "+EventSystem.current.currentSelectedGameObject.name);
		prevPhoneme = 0;
		int phon = phonemesMap.GetPhonemeNumber(StaticData.selectedPhoneme);
		Debug.Log("TestingFunc phoneme " + phon);
		global.phoneme = phon;
		global.duration = 1000;
		StartCoroutine(animateMouth());
	}

	// public long testPhoneme, globalDuration;

	private IEnumerator animateMouth()
	{
		float startTime = Time.time;
		float elapsedTime = 0f;
		animating = true;
		if (continualUpdate || prevPhoneme != global.phoneme)
		{
			// Debug.Log("animateMouth global phoneme" + global.phoneme);
			// long phoneme = global.phoneme;/
			long phoneme = global.phoneme;
			// Debug.Log("animateMouth duration" + global.duration);
			// Debug.Log("global animation time" + global.animationSpeed);
			float duration = Math.Min(0.5f, (float)global.duration / 1000f);
			// float duration = Math.Min(0.5f, (float)globalDuration / 1000f);
			while (elapsedTime < duration)
			{
				switch (phoneme)
				{
				case 17L:
				case 18L:
				case 19L:
				case 25L:
				case 29L:
				case 30L:
				case 37L:
				case 41L:
				case 50L:
				case 51L:
				case 52L:
				case 53L:
				case 54L:
				case 55L:
				case 56L:
				case 57L:
					elapsedTime = Time.time - startTime;
					break;
				default:
					elapsedTime = (Time.time - startTime) * global.animationSpeed;
					break;
				}
				Transform transform = jawTargets[(int)prevPhoneme].transform;
				Transform transform2 = jawTargets[(int)phoneme].transform;
				lowerJawBone.transform.rotation = Quaternion.Lerp(transform.rotation, transform2.rotation, elapsedTime * (1f / duration));
				transform = palateTargets[(int)prevPhoneme].transform;
				transform2 = palateTargets[(int)phoneme].transform;
				palate.transform.rotation = Quaternion.Lerp(transform.rotation, transform2.rotation, elapsedTime * (1f / duration));
				transform = uvulaTargets[(int)prevPhoneme].transform;
				transform2 = uvulaTargets[(int)phoneme].transform;
				uvula.transform.rotation = Quaternion.Lerp(transform.rotation, transform2.rotation, elapsedTime * (1f / duration));
				for (int i = 0; i < tongueVectors; i++)
				{
					Transform transform3 = lowerJawBone.transform.Find("tongue." + i.ToString("000"));
					if (transform3 != null)
					{
						transform = tongeTargets[(int)prevPhoneme][i].transform;
						transform2 = tongeTargets[(int)phoneme][i].transform;
						transform3.position = Vector3.Lerp(transform.position, transform2.position, elapsedTime * (1f / duration));
					}
				}
				for (int j = 0; j < lipVectors; j++)
				{
					Transform transform4 = GameObject.Find("lips." + j.ToString("000")).transform;
					if (transform4 != null)
					{
						transform = lipTargets[(int)prevPhoneme][j].transform;
						transform2 = lipTargets[(int)phoneme][j].transform;
						transform4.position = Vector3.Lerp(transform.position, transform2.position, elapsedTime * (1f / duration));
					}
				}
				for (int k = 0; k < airflowVectors; k++)
				{
					Transform transform5 = GameObject.Find("OralAirflow." + k.ToString("000")).transform;
					if (transform5 != null)
					{
						transform = airflowTargets[(int)prevPhoneme][k].transform;
						transform2 = airflowTargets[(int)phoneme][k].transform;
						transform5.position = Vector3.Lerp(transform.position, transform2.position, elapsedTime * (1f / duration));
					}
				}
				spline.AutoConstructSpline();
				meshFilter.sharedMesh = sourceMesh;
				meshFilter.BakeMesh(mesh);
				mesh = SmoothingFilter.LaplacianFilter(mesh, smoothFactor);
				SetDuplicateVerts(ref mesh);
				meshFilter.sharedMesh = mesh;
				yield return null;
			}
			prevPhoneme = phoneme;
			global.phoneme = 7L;
		}
		animating = false;
	}

	private void GetDuplicateVerts(ref Mesh mesh)
	{
		HashSet<Vector3> hashSet = new HashSet<Vector3>();
		HashSet<Vector3> hashSet2 = new HashSet<Vector3>();
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < mesh.vertices.Length; i++)
		{
			if (!hashSet.Add(mesh.vertices[i]))
			{
				if (!hashSet2.Add(mesh.vertices[i]))
				{
					int index = list.IndexOf(mesh.vertices[i]);
					duplicateVerts[index].Add(i);
					continue;
				}
				int item = Array.IndexOf(mesh.vertices, mesh.vertices[i]);
				list.Add(mesh.vertices[i]);
				duplicateVerts.Add(new List<int> { item, i });
			}
		}
	}

	private void SetDuplicateVerts(ref Mesh mesh)
	{
		Vector3[] vertices = mesh.vertices;
		foreach (List<int> duplicateVert in duplicateVerts)
		{
			Vector3 zero = Vector3.zero;
			foreach (int item in duplicateVert)
			{
				zero += vertices[item];
			}
			zero /= (float)duplicateVert.Count;
			foreach (int item2 in duplicateVert)
			{
				vertices[item2] = zero;
			}
		}
		mesh.vertices = vertices;
	}
}
