//****************************************************
//*	BYTON EDITOR ASSET MANAGER
//****************************************************
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

//****************************************************
//*	NAMESPACE
//****************************************************
namespace Byton {

    //************************************************
	//*	CLASS
    //************************************************
    public static class BytonEditorAssetManager {

		//********************************************
		//*	CONSTANTS
		//********************************************
        private const string STYLE_HEADER_WITH_LOGO = "HEADER WITH LOGO";
        private const string STYLE_LOGO_SMALL = "LOGO SMALL";        

        //********************************************
        //*	VARIABLES
        //********************************************
        private static Dictionary<string, GUISkin> skins = new Dictionary<string, GUISkin>();

		//********************************************
		//*	GETTER / SETTERS
		//********************************************
		private static GUISkin _skin;
		public static GUISkin skin{

			//*** Creation on Demand
			get{
				//*** If Not Loaded
				if (_skin == null) {
                    _skin = GetSkin ("BytonGUISkin");
				}

				//*** Return
				return _skin;
			}
		}

		//********************************************
		//*	METHODS
		//********************************************
		public static GUISkin GetSkin(string pName){

			//*** Variables
			GUISkin oReturn = null;

			//*** If skin in collection, return
			if (skins.ContainsKey (pName)) {
				oReturn = skins [pName];
			}

			//*** If is not already in dictionary
			else {

				//*** Find Asset from Asset DB
				string[] aAssets = AssetDatabase.FindAssets (pName);

				//*** Use First Asset (could add more logic here to make sure that teh asset is in the correct folder)
				if(aAssets.Length > 0) {

					//*** Get Path
					string xPath = AssetDatabase.GUIDToAssetPath (aAssets [0]);

					//*** Load Asset
					oReturn = AssetDatabase.LoadAssetAtPath<GUISkin> (xPath);
				}
			}

			//*** Process Failed attempts

			//*** Return
			return oReturn;
		}

		//********************************************
		//* RENDER METHODS
		//********************************************
        public static void Render_TabHeader(string pTitle, EditorWindow pWindow) {

            //*** Variables
            GUIContent xTitleContent = new GUIContent {
                text = pTitle,
                image = skin.GetStyle(STYLE_LOGO_SMALL).normal.background
            };

            //*** Set Window content
            pWindow.titleContent = xTitleContent;
        }
		public static void Render_WindowHeader(string pTitle, EditorWindow pWindow, ref Vector2 pScrollPosition){

			//*** Start Scrolling
			pScrollPosition = EditorGUILayout.BeginScrollView(pScrollPosition, skin.window);

			//*** Render heading
            GUILayout.Label ("             " + pTitle.ToUpper(), skin.GetStyle(STYLE_HEADER_WITH_LOGO));
		}
	}
}