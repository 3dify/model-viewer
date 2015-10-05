using UnityEngine;
using System.Collections;

public class ModelSpawner : MonoBehaviour {

	private ObjImporter importer = new ObjImporter();
	public Material material;
	public float distance;

	public void SpawnModel(string info){
		if(info.Length==0) return;
		string[] files = info.Split('\n');
		if( files.Length > 1 ){
			foreach(var mfile in files){
				SpawnModel(mfile);
			}
			return;
		}
		
		string file = files[0];
				
		Debug.Log (info);
		Mesh mesh = importer.ImportFile(file);
		GameObject model = new GameObject();
		GameObject modelContainer = new GameObject();
		modelContainer.name = file;
		model.name = "model";
		
		MeshFilter meshFilter = model.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = model.AddComponent<MeshRenderer>();
		meshRenderer.material = material;
		meshFilter.mesh = mesh;
		Vector3 size = Vector3.Max( meshRenderer.bounds.size, Vector3.one );
		float max = Mathf.Max(new float[]{ size.x, size.y, size.z });
		model.transform.localScale = 1f/max * Vector3.one;
		modelContainer.transform.position = Camera.main.transform.position + distance * Camera.main.transform.forward;
		model.transform.parent = modelContainer.transform;
		
		model.transform.localPosition = -1f * meshRenderer.bounds.center;
	}
}
