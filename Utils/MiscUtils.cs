using UnityEngine;
using System.Collections;

namespace Utils{
    class MiscUtils{
        public static bool IsOffBounds(Vector2 uiObject, float xMargin, float yMargin){
        return uiObject.x < 0 - xMargin || uiObject.x > 100 + xMargin|| uiObject.y < 0 - yMargin || uiObject.y > 100 + yMargin;
    }
    public static string GetGameObjectPath(Transform obj)
    {
     string path = obj.name;
     while (obj.parent != null)
     {
         obj = obj.parent;
         path = obj.name + "/" + path;
     }
     return path;
    }
    
    }
}