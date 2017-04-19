using UnityEditor;
using UnityEngine;

public class CreateAssetBundles
{
	[MenuItem ("Assets/Build AssetBundles")]
	static void BuildAllAssetBundles ()
	{
		string path = EditorUtility.SaveFilePanel("Save Resource", "", "New Resource", "assetbundle");  

		if (path.Length != 0)  
		{  
			// 选择的要保存的对象  
			Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);  
			//打包  
			BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, BuildTarget.StandaloneWindows);  
		}  
	}
}
