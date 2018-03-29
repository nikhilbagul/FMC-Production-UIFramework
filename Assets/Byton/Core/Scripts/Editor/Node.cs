/* 
 *Author : Nikhil Bagul
 *Date : 02/12/2018
 *
 * Node class
 * This class is responsible for drawing and adding widget nodes
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Rect nodeRect;
    public string title;    
    public bool isDragged, isSelected;
    public static bool nodeClicked;

    public GUIStyle style;
    public GUIStyle defaultNodeStyle;
    public GUIStyle selectedNodeStyle;

    public Node(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedStyle)
    {
        nodeRect = new Rect(position.x, position.y, width, height);
        style = nodeStyle;
        defaultNodeStyle = style;
        selectedNodeStyle = selectedStyle;
    }

    public void Drag(Vector2 delta)
    {
        nodeRect.position += delta;
    }

    public void Draw()
    {
        //style.alignment = TextAnchor.UpperCenter;
        //style.padding = nodePadding;

        GUI.Box(nodeRect, "", style);            
    }

    public bool ProcessEvents(Event e, Rect resizerRect)
    {        
        switch (e.type)
        {
            case EventType.MouseDown:

                if (nodeRect.Contains(e.mousePosition))
                {
                    //Debug.Log("Node clicked!");
                    nodeClicked = true;
                }
                
                if (e.button == 0)
                {
                    if (nodeRect.Contains(e.mousePosition))
                    {
                        //Debug.Log("Node clicked!");
                        isDragged = true;
                        GUI.changed = true;
                        isSelected = true;
                        style = selectedNodeStyle;
                    }
                    else
                    {
                        GUI.changed = true;
                        isSelected = false;
                        style = defaultNodeStyle;
                    }                    
                }                
                break;               

            case EventType.MouseUp:
                //nodeClicked = false;
                isDragged = false;
                break;

            case EventType.MouseDrag:
                if (e.button == 0 && isDragged)
                {
                    //Debug.Log(e.delta);
                    if (!nodeRect.Overlaps(resizerRect))    //nodeRect.xMax < resizerRect.xMin - 20)
                        Drag(e.delta);
                    else if (e.delta.x < 0)
                        Drag(e.delta);

                    e.Use();
                    return true;
                }
                break;
        }

        return false;
    }
}


