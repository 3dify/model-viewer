using UnityEngine;
using System.Collections;
using com.youvisio;

public class ModelSpawner : MonoBehaviour {

	private ObjImporter importer;
	public Material material;
	public float distance;
	public GameObject currentModel;
	
	private BackgroundWorker worker;

	public void SpawnModel(string modelFiles){
		if(modelFiles.Length==0) return;
		string[] files = modelFiles.Split('\n');
		if( files.Length > 1 ){
			foreach(var mfile in files){
				SpawnModel(mfile);
			}
			return;
		}
		Debug.Log("Spawning Model");
        
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
		Mesh mesh = new Mesh();
		Texture2D tex = new Texture2D(1,1);
		Material mat = new Material( material );
		byte[] texBytes = new byte[0];
            
        worker = new BackgroundWorker();
		importer = new ObjImporter();
		importer.ImportFile(meshFile,mesh);
		
		importer.OnComplete += ()=>{
			
			if( mesh.normals[0]==Vector3.zero){
				mesh.RecalculateNormals();
			}
			GameObject model = new GameObject();
			GameObject modelContainer = new GameObject();
			modelContainer.name = meshFile;
			model.name = "model";
			
			MeshFilter meshFilter = model.AddComponent<MeshFilter>();
			MeshRenderer meshRenderer = model.AddComponent<MeshRenderer>();
			meshRenderer.sharedMaterial = mat;
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
            
			
        };
        
		
        worker.DoWork += (o,a)=>
        {
			GetTexture(textureFile, ref texBytes);
		};
        worker.RunWorkerCompleted += (o,a)=>
		{
			tex.LoadImage(texBytes);
			mat.mainTexture = tex;
        };
		
		worker.RunWorkerAsync();
        
        
		
	}
	
	public void Update(){
		if( worker != null ) worker.Update();
		if( importer != null ) importer.Update();
	}
	
	static protected bool FileExists(string filePath){
		return System.IO.File.Exists(filePath);
	}
	
	protected void DisplayError(string message){
		
	}
	
	static protected void GetTexture(string filePath, ref byte[] bytes){
		bytes = System.IO.File.ReadAllBytes(filePath);
	}
}
