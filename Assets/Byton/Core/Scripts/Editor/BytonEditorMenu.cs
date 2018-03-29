//****************************************************
//*	BYTON FRAMEWORK MENU
//****************************************************
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

//****************************************************
//* NAMESPACE
//****************************************************
namespace Byton {

    //************************************************
    //*	CLASS
    //************************************************
    public static class BytonEditorMenu {

        //********************************************
        //*	STRUCTS
        //********************************************
        private struct FileSize {

            //*** Properties
            public string path;
            public long length;

            //*** Constructor
            public FileSize(string pPath, long pLength) {

                //*** Set values
                path = pPath;
                length = pLength;
            }
        }

        //********************************************
        //*	CONSTANTS
        //********************************************
        public const string MENU_TITLE = "Byton";
        private const string ASSETS_FOLDER = "Assets";
        private const string RESOURCES_FOLDER = "Resources";

        //********************************************
        //*	VARIABLES
        //********************************************


        //********************************************
        //*	GENERAL METHODS
        //********************************************

        /*
        [MenuItem(MENU_TITLE + "/Data/Clear Player Prefs")]
        public static void ClearPlayerPrefs() {

            //*** Delete Player Prefs
            PlayerPrefs.DeleteAll();
        }

        [MenuItem("Assets/" + MENU_TITLE + "/Ext/Strip Last")]
        public static void Extentions_StripLast() {

            //*** Variables
            int i;

            //*** Save Undo
            Undo.RecordObjects(Selection.objects, "Change Extentions");

            //*** Loop through selection
            for (i = 0; i < Selection.objects.Length; i++) {

                //*** Get Asset Path
                string xPath = AssetDatabase.GetAssetPath(Selection.objects[i]);

                //*** Remove Last Extention
                List<string> aPath = new List<string>(xPath.Split(new char[] { '.' }, System.StringSplitOptions.None));

                //*** IF Can Strip
                if (aPath.Count > 1) {
                    aPath.RemoveAt(aPath.Count - 1);
                }

                //** Make New Path
                string xNewPath = string.Join(".", aPath.ToArray());

                //*** Rename
                File.Move(xPath, xNewPath);
            }

            //*** Refresh
            AssetDatabase.Refresh();
        }
        */
    }
}
