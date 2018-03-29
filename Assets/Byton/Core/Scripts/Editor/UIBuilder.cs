/* 
 *Author : Nikhil Bagul
 *Date : 02/12/2018
 *
 * UI Builder editor class
 * Kickoff in editor Byton UI framework tools
 * This class is responsible for drawing the editor window, adding nodes, drawing Panel within the editor window
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class UIBuilder : EditorWindow
{
    private List<Node> nodes;
    private GUIStyle nodeStyle, labelStyle, resizerStyle, widgetPanel_HeaderStyle, widgetPanelStyle, selectedNodeStyle;
    private const string STYLE_WIDGET_NODE = "WIDGET NODE";
    private const string STYLE_WIDGET_PANEL_HEADER = "WIDGET PANEL HEADER";
    private const string STYLE_WIDGET_PANEL = "WIDGET PANEL";
    private Rect widgetSpacePanelHeaderRect, labelRect, resizerRect, widgetSpacePanelRect;    

    [MenuItem("Byton/UI Builder")]
    static void Init()
    {
        UIBuilder window = GetWindow<UIBuilder>();
        window.minSize = new Vector2(700.0f, 400.0f);
    }

    private void OnEnable()
    {        
        labelStyle = new GUIStyle();
        resizerStyle = new GUIStyle();

        labelStyle = Byton.BytonEditorAssetManager.skin.GetStyle("label");
        //nodeStyle = Byton.BytonEditorAssetManager.skin.GetStyle(STYLE_WIDGET_NODE);
        resizerStyle = Byton.BytonEditorAssetManager.skin.GetStyle("verticalscrollbar");
        widgetPanel_HeaderStyle = Byton.BytonEditorAssetManager.skin.GetStyle(STYLE_WIDGET_PANEL_HEADER);
        widgetPanelStyle = Byton.BytonEditorAssetManager.skin.GetStyle(STYLE_WIDGET_PANEL);

        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);

        selectedNodeStyle = new GUIStyle();
        selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);

        //resizerStyle.normal.background = EditorGUIUtility.Load("icons/d_AvatarBlendBackground.png") as Texture2D;
        
    }

    private void OnGUI()
    {                      
        //Event contextClickEvent = Event.current;
        //if (contextClickEvent.type == EventType.ContextClick)
        //{
        //    GenericMenu menu = new GenericMenu();
        //    menu.AddItem(new GUIContent("Widget1"), false, GenerateWidgetNode);
        //    menu.AddItem(new GUIContent("Widget2"), false, GenerateWidgetNode);

        //    menu.ShowAsContext();
        //    contextClickEvent.Use();
        //}        

        //Draw nodes every frame
        DrawNodes();

        //Process current events - mouse clicks
        ProcessNodeEvents(Event.current);

        //Draw widget space panel and resizer
        DrawResizer();

        //Draw Labels
        DrawNodeSpaceLabel();       

        if(!Node.nodeClicked)
            ProcessEvents(Event.current);
        //Repaint if GUI changes
        if (GUI.changed)
        {
            Repaint();            
        }
        Node.nodeClicked = false;
    }
    private void DrawResizer()
    {
        resizerRect = new Rect(Screen.width - 400, -5, 10, Screen.height);        
        GUILayout.BeginArea(resizerRect, resizerStyle);        
        GUILayout.EndArea();
        EditorGUIUtility.AddCursorRect(resizerRect, MouseCursor.ResizeHorizontal);
        WidgetSpacePanel(resizerRect);
    }

    private void WidgetSpacePanel(Rect resizerRect)
    {
        widgetSpacePanelHeaderRect = new Rect(resizerRect.xMax, 0, position.width - resizerRect.xMax , position.height * 0.10f);
        GUILayout.BeginArea(widgetSpacePanelHeaderRect, widgetPanel_HeaderStyle);
        GUILayout.Label("Widget Space", widgetPanel_HeaderStyle);
        GUILayout.EndArea();

        widgetSpacePanelRect = new Rect(resizerRect.xMax, widgetSpacePanelHeaderRect.yMax, position.width - resizerRect.xMax, position.height * 0.9f);
        GUILayout.BeginArea(widgetSpacePanelRect, widgetPanelStyle);              //GUI.skin.GetStyle("Box"));
        GUILayout.EndArea();
    }

    private void DrawNodeSpaceLabel()
    {
        //Debug.Log(Screen.width);  
        //OR
        //Debug.Log(this.position.x);  
        labelRect = new Rect(10, Screen.height - 70, 250, 40);
        GUI.Label(labelRect, "1.  Right click to add/delete widgets. \n2. Click on any node to edit the widget.", labelStyle);                  
    }    

    private void DrawNodes()
    {        
        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Draw();
            }
        }
    }

    private void ProcessEvents(Event e)
    {
        //Debug.Log(e);
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.mousePosition.x > resizerRect.xMin - 210)
                {
                    Node.nodeClicked = true;
                }

                else if (e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);
                    e.Use();
                }
                                   
                break;            
        }        
    }    

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add Widget"), false, () => OnClickAddWidget(mousePosition));
        genericMenu.ShowAsContext();
    }

    private void OnClickAddWidget(Vector2 mousePos)
    {
       if (nodes == null)
        {
            nodes = new List<Node>();
        }

        nodes.Add(new Node(mousePos, 200, 50, nodeStyle, selectedNodeStyle));

    }

    private void ProcessNodeEvents(Event e)
    {
        if (nodes != null)
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = nodes[i].ProcessEvents(e, resizerRect);

                if (guiChanged)
                {
                    GUI.changed = true;                    
                }                                
            }
        }
    }

}
