//****************************************************
//* TEXTURE RESIZER EDITOR WINDOW
//****************************************************
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

//****************************************************
//*	NAMESPACE
//****************************************************
namespace Byton {

    //************************************************
	//*	CLASS
    //************************************************
    public class TextureResizerInspector : EditorWindow {

		//********************************************
		//*	ENUM
		//********************************************
        public enum ImageFilterMode : int {
            Nearest = 0,
            Biliner = 1,
            Average = 2,
            Super = 3
        }
		public enum FolderConvention : int {
			TK2D = 0,
			FoldersPerSize = 1
		}

		//********************************************
		//*	VARIABLES
		//********************************************
		private Vector2 scrollPostion = new Vector2();
		private FolderConvention folderConvention = FolderConvention.TK2D;
		private bool stripExtraFileExtentions = false;
		
		//********************************************
		//*	LAUNCH METHOD
		//********************************************
        [MenuItem(BytonEditorMenu.MENU_TITLE + "/Texture Resizer")]
	    public static void Init () {
			
	        //*** Get existing open window or if none, make a new one
            EditorWindow.GetWindow (typeof (TextureResizerInspector));
	    }

		//********************************************
		//*	UNITY METHODS
		//********************************************
        private void Awake() {

            Selection.selectionChanged += delegate {

                Debug.Log("Selection changed");
            };
        }
		private void Update() {
			Repaint();
		}
	    private void OnGUI () {
            
			//*** Render
			Render();
	    }

		//********************************************
		//* MAIN METHODS
		//********************************************
		private void Render(){

            //*** Render Tab
            BytonEditorAssetManager.Render_TabHeader("Resizer", this);

            //*** Render Header
            BytonEditorAssetManager.Render_WindowHeader("Texture Resizer", this, ref scrollPostion);
	    
			//*** Get texture array
			Object[] aTextures = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets);

            GUILayout.Label ("Selected Textures".ToUpper(), BytonEditorAssetManager.skin.label);
			
			//*** Loop through selected textures
	        foreach (Object oTexture in aTextures) {
	            
				//*** Render Additional Options
				RenderInspector(oTexture as Texture2D);
	        }
			
			//*** Space
			GUILayout.Space(10);
			
			EditorGUILayout.Separator();
			
			//*** Space
			GUILayout.Space(10);

            GUILayout.Label ("Resize Textures".ToUpper(), BytonEditorAssetManager.skin.label);
			
			//*** Resize Textures
			EditorGUILayout.BeginHorizontal();
			
			//*** Folder convention pop up
			folderConvention = (FolderConvention)EditorGUILayout.EnumPopup("Naming Convention", folderConvention);
			
			//*** If tk2d
			if(folderConvention == FolderConvention.TK2D){
				stripExtraFileExtentions = EditorGUILayout.Toggle("Strip Extra File Extentions:", stripExtraFileExtentions);
			}
			else{
				stripExtraFileExtentions = false;
			}
			
			if (GUILayout.Button("Resize")) {
				Resize (aTextures, folderConvention);
			}
			
			EditorGUILayout.EndHorizontal();		
			
			//*** Space
			GUILayout.Space(10);

            GUILayout.Label ("Texture Format Presets".ToUpper(), BytonEditorAssetManager.skin.label);
			
			//*** Resize Textures
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("32 Bit, Readable")) {
				ApplySpriteCollectionSettings(aTextures);
			}
			EditorGUILayout.EndHorizontal();
			
			//*** Space
			GUILayout.Space(100);
			
			//*** End Scroll view
			EditorGUILayout.EndScrollView();
		}
		private void RenderInspector(Texture2D pTexture) {
			
			//*** Space
			GUILayout.Space(10);
			
			EditorGUILayout.BeginHorizontal();
	            
			//*** Render Selected Object
			//EditorGUILayout.InspectorTitlebar(true, oTexture);
			EditorGUILayout.ObjectField(pTexture, typeof(Texture), false, GUILayout.Height(100), GUILayout.Width(100));
			
			//*** TEXTURE OPTIONS
			EditorGUILayout.BeginVertical();
			
			//*** MIP MAP BIAS
			EditorGUILayout.BeginHorizontal();
			pTexture.mipMapBias = EditorGUILayout.FloatField("Mip Map Bias", pTexture.mipMapBias);
			EditorGUILayout.EndHorizontal();
			
			TextureImporter oTI = TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(pTexture)) as TextureImporter;
			
			//*** DATA
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Max Size: " + oTI.maxTextureSize.ToString());
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Size: " + pTexture.width + "x" + pTexture.height);
			EditorGUILayout.EndHorizontal();
			
			
			EditorGUILayout.EndVertical();		
			EditorGUILayout.EndHorizontal();
			
			//*** Space
			GUILayout.Space(10);
		}
		private void Resize(Object[] aTextures, FolderConvention pFolderConvention){

            //*** Variables
            Texture2D oTexture;
			string xAssetFolder = "Assets";
			string xSpriteFolder = "Sprites";
			string xFinalPath = "";
			int i;
			
			string[] aTK2D = new string[]{"@4x", "@2x", "@1x"};
			string[] aFolders = new string[]{"Full", "Half", "Quater"};
			float[] aSizes = new float[]{1, 0.5f, 0.25f};
			ImageFilterMode oMode = ImageFilterMode.Average;
			
			
			//*** Get Base Path
			string xBasePath = Application.dataPath;
			xBasePath = xBasePath.Substring(0, xBasePath.Length - xAssetFolder.Length);
			
			
			//*** Loop through selected textures
	        foreach (Object oObject in aTextures) {
				
				//*** Cast
				oTexture = oObject as Texture2D;
				
				//*** IF it is a valid texture
				if(oTexture != null){
					
					//*** Get Asset Path
					string xAssetPath = AssetDatabase.GetAssetPath(oTexture);
					string xPath = Path.GetDirectoryName(xAssetPath);
					string xNewPath = xPath + Path.DirectorySeparatorChar + xSpriteFolder + Path.DirectorySeparatorChar;
					
					//*** If TK2d
					if(pFolderConvention == FolderConvention.TK2D){
						
						//*** If Folder Does not exist
						if(!System.IO.Directory.Exists(xBasePath + xNewPath)){
							
							//*** Make it
							AssetDatabase.CreateFolder(xPath, xSpriteFolder);					
						
							//*** Refresh
							AssetDatabase.Refresh();		
						}
					}
					
					//*** If Set Folders
					else if(pFolderConvention == FolderConvention.FoldersPerSize){
						
						//*** Loop through sizes
						for(i=0; i<aFolders.Length; i++){
							
							//*** If Folder Does not exist
							if(!System.IO.Directory.Exists(xBasePath + xPath + Path.DirectorySeparatorChar + aFolders[i] + Path.DirectorySeparatorChar)){
								
								//*** Make it
								AssetDatabase.CreateFolder(xPath, aFolders[i]);					
							
								//*** Refresh
								AssetDatabase.Refresh();
							}
						}
					}
						
					//*** Get Importer data
					TextureImporter oTI = TextureImporter.GetAtPath(xAssetPath) as TextureImporter;
					
					//*** If Not Readable, set
					if(!oTI.isReadable){
						oTI.isReadable = true;
						
						//*** Apply Settings & Reload
						AssetDatabase.ImportAsset(xAssetPath, ImportAssetOptions.ForceUpdate);
					}
					
					//*** Get All the source pixels
					Color[] aSourceColor = oTexture.GetPixels(0);
					Vector2 vSourceSize = new Vector2(oTexture.width, oTexture.height);
				
					//*** Loop through sizes
					for(i=0; i<aFolders.Length; i++){
						
						//*** Make New Paths
						string xNewAssetPath = xNewPath + Path.GetFileName(xAssetPath);
						
						//*** Strip Extra File Extention Data
						if(stripExtraFileExtentions){
							
							List<string> aNewAssetPath = new List<string>(xNewAssetPath.Split(new char[]{'.'}, System.StringSplitOptions.RemoveEmptyEntries));
							
							//*** If Has an extention, remove last
							if(aNewAssetPath.Count > 1){
								aNewAssetPath.RemoveAt(aNewAssetPath.Count - 1);
							}
							
							//*** Make join back together
							//xNewAssetPath = xNewPath + string.Join(".", aNewAssetPath.ToArray());
							xNewAssetPath = string.Join(".", aNewAssetPath.ToArray());
						}
						
						//*** Calculate New Size
						float xWidth = (int)Mathf.Round((float)oTexture.width * aSizes[i]);						
						float xHeight = (int)Mathf.Round((float)oTexture.height * aSizes[i]);
						
						//*** Make New
						Texture2D oNewTex = new Texture2D((int)xWidth, (int)xHeight, TextureFormat.RGBA32, false);
						
						//*** Make destination array
						int xLength = (int)xWidth * (int)xHeight;
						Color[] aColor = new Color[xLength];
						
						Vector2 vPixelSize = new Vector2(vSourceSize.x / xWidth, vSourceSize.y / xHeight);
						
						//*** If Not a resize but a copy
						if(aSizes[i] == 1){
							
							//*** Just get pixels
							aColor = aSourceColor;
						}
						
						//*** If Needs resizing
						else{
							
							//*** Loop through destination pixels and process
							int ii;
							Vector2 vCenter = new Vector2();
							for(ii=0; ii<xLength; ii++){
								
								//*** Figure out x&y
								float xX = (float)ii % xWidth;
								float xY = Mathf.Floor((float)ii / xWidth);
								
								//*** Calculate Center
								vCenter.x = (xX / xWidth) * vSourceSize.x;
								vCenter.y = (xY / xHeight) * vSourceSize.y;
								
								
								//*** Do Based on mode
								//*** Nearest neighbour (testing)
								if(oMode == ImageFilterMode.Nearest){
									
									//*** Nearest neighbour (testing)
									vCenter.x = Mathf.Round(vCenter.x);
									vCenter.y = Mathf.Round(vCenter.y);
									
									//*** Calculate source index
									int xSourceIndex = (int)((vCenter.y * vSourceSize.x) + vCenter.x);
									
									//*** Copy Pixel
									aColor[ii] = aSourceColor[xSourceIndex];
								}
								
								//*** Bilinear
								else if(oMode == ImageFilterMode.Biliner){
									
									//*** Get Ratios
									float xRatioX = vCenter.x - Mathf.Floor(vCenter.x);
									float xRatioY = vCenter.y - Mathf.Floor(vCenter.y);
									
									//*** Get Pixel index's
									int xIndexTL = (int)((Mathf.Floor(vCenter.y) * vSourceSize.x) + Mathf.Floor(vCenter.x));
									int xIndexTR = (int)((Mathf.Floor(vCenter.y) * vSourceSize.x) + Mathf.Ceil(vCenter.x));
									int xIndexBL = (int)((Mathf.Ceil(vCenter.y) * vSourceSize.x) + Mathf.Floor(vCenter.x));
									int xIndexBR = (int)((Mathf.Ceil(vCenter.y) * vSourceSize.x) + Mathf.Ceil(vCenter.x));
									
									//*** Calculate Color
									aColor[ii] = Color.Lerp(
										Color.Lerp(aSourceColor[xIndexTL], aSourceColor[xIndexTR], xRatioX),
										Color.Lerp(aSourceColor[xIndexBL], aSourceColor[xIndexBR], xRatioX),
										xRatioY
									);
								}
								
								//*** Average
								else if(oMode == ImageFilterMode.Average){
									
									//*** Calculate grid
									int xXFrom = (int)Mathf.Max(Mathf.Floor(vCenter.x - (vPixelSize.x * 0.5f)), 0);
									int xXTo = (int)Mathf.Min(Mathf.Ceil(vCenter.x + (vPixelSize.x * 0.5f)), vSourceSize.x);
									int xYFrom = (int)Mathf.Max(Mathf.Floor(vCenter.y - (vPixelSize.y * 0.5f)), 0);
									int xYTo = (int)Mathf.Min(Mathf.Ceil(vCenter.y + (vPixelSize.y * 0.5f)), vSourceSize.y);
									
									//*** Loop and accumulate
									Vector4 oColorTotal = new Vector4();
									Color oColorTemp = new Color();
									float xGridCount = 0;
									for(int iy = xYFrom; iy < xYTo; iy++){
										for(int ix = xXFrom; ix < xXTo; ix++){
											
											//*** Get Color
											oColorTemp += aSourceColor[(int)(((float)iy * vSourceSize.x) + ix)];
											
											//*** Sum
											xGridCount++;
										}
									}
									
									//*** Average Color
									aColor[ii] = oColorTemp / (float)xGridCount;
								}
							}
						}
						
						//*** Set Pixels
						oNewTex.SetPixels(aColor);
						oNewTex.Apply();
						
						//*** if TK2D
						if(pFolderConvention == FolderConvention.TK2D){
							if(stripExtraFileExtentions){
								xFinalPath = xBasePath + xNewAssetPath + aTK2D[i] + ".png";
							}
							else{
								xFinalPath = xBasePath + xNewAssetPath + "." + aTK2D[i] + ".png";
							}
						}
						
						//*** If Sepearte Folders
						else if(pFolderConvention == FolderConvention.FoldersPerSize){
							xFinalPath = xBasePath + xPath + Path.DirectorySeparatorChar + aFolders[i] + Path.DirectorySeparatorChar + oTexture.name + ".png";
						}
						
						//*** Encode and save
						byte[] aData = oNewTex.EncodeToPNG();
						File.WriteAllBytes(xFinalPath, aData);
						
						//*** Open New Asset
						
						//*** Set Texture
						
					}
				}
	        }
			
			//*** Refresh
			AssetDatabase.Refresh();		
		}
		private void ApplySpriteCollectionSettings(Object[] aTextures){
			
			//*** Variables
			int i;
			Texture2D oTexture;
			
			//*** Loop through textures
			for(i=0; i<aTextures.Length; i++){
			
				//*** Get Texture
				oTexture = aTextures[i] as Texture2D;
				
				//*** If Texture is good
				if(oTexture != null){
					
					//*** Get Asset Path
					string xPath = AssetDatabase.GetAssetPath(oTexture);
					
					//*** Get Importer data
					TextureImporter oTI = TextureImporter.GetAtPath(xPath) as TextureImporter;
					
					//*** Set Settings
					oTI.textureType = TextureImporterType.Default;
					oTI.npotScale = TextureImporterNPOTScale.None;
					oTI.alphaIsTransparency = true;
					oTI.isReadable = true;
					oTI.mipmapEnabled = false;
					oTI.wrapMode = TextureWrapMode.Clamp;
					oTI.filterMode = FilterMode.Trilinear;
					oTI.maxTextureSize = 4096;
					oTI.textureFormat = TextureImporterFormat.AutomaticTruecolor;
					
					//*** Apply Settings & Reload
					AssetDatabase.ImportAsset(xPath, ImportAssetOptions.ForceUpdate);
				}
			}
		}
	}
}