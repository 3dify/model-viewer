using UnityEngine;
using System.Collections;

public class ModelSpawner : MonoBehaviour {

	private ObjImporter importer = new ObjImporter();
	public Material material;
	public float distance;
	public GameObject currentModel;

	public void SpawnModel(string modelFiles){
		if(modelFiles.Length==0) return;
		string[] files = modelFiles.Split('\n');
		if( files.Length > 1 ){
			foreach(var mfile in files){
				SpawnModel(mfile);
			}
			return;
		}
		
		string[] info = files[0].Split(',');
		if(info.Length<2){
			return;
		}
		string meshFile = info[0].Trim();
		string textureFile = info[1].Trim();
				
		if( !FileExists(meshFile) ){
			DisplayError(string.Format("Mesh file {0} does not exist",meshFile));
			return;
		}
		if( !FileExists(textureFile) ){
			DisplayError(string.Format("Texture file {0} does not exist",textureFile));
		}
		
		Mesh mesh = importer.ImportFile(meshFile);
		mesh.Optimize();
		Debug.Log(mesh.normals[0]);
		if( mesh.normals[0]==Vector3.zero){
			mesh.RecalculateNormals();
		}
		mesh.RecalculateBounds();
		GameObject model = new GameObject();
		GameObject modelContainer = new GameObject();
		modelContainer.name = meshFile;
		model.name = "model";
		
		MeshFilter meshFilter = model.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = model.AddComponent<MeshRenderer>();
		meshRenderer.material = material;
		meshRenderer.material.mainTexture = GetTexture(textureFile);
		meshFilter.mesh = mesh;
		
		Vector3 size = Vector3.Max( meshRenderer.bounds.size, Vector3.one );
		float max = Mathf.Max(new float[]{ size.x, size.y, size.z });
		model.transform.localScale = 1f/max * Vector3.one;
		modelContainer.transform.position = Camera.main.transform.position + distance * Camera.main.transform.forward;
		model.transform.parent = modelContainer.transform;
		modelContainer.AddComponent<Spin>();
		modelContainer.AddComponent<SlideInAndOut>();
		model.transform.localPosition = -1f * meshRenderer.bounds.center;
		
		if( currentModel != null ){
			currentModel.SendMessage("TransitionOut");
		}
		currentModel = modelContainer;
		
	}
	
	protected bool FileExists(string filePath){
		return System.IO.File.Exists(filePath);
	}
	
	protected void DisplayError(string message){
		
	}
	
	protected Texture GetTexture(string filePath){
		var bytes = System.IO.File.ReadAllBytes(filePath);
		var tex = new Texture2D(1, 1);
		tex.LoadImage(bytes);
		return tex;
	}
}
