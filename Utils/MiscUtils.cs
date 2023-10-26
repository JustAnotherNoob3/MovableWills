using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
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
    public static int CalcLevenshteinDistance(string a, string b)
    {
    if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b)) {
        return 0;
    }
    if (string.IsNullOrEmpty(a)) {
        return b.Length;
    }
    if (string.IsNullOrEmpty(b)) {
        return a.Length;
    }
    int  lengthA   = a.Length;
    int  lengthB   = b.Length;
    var  distances = new int[lengthA + 1, lengthB + 1];
    for (int i = 0;  i <= lengthA;  distances[i, 0] = i++);
    for (int j = 0;  j <= lengthB;  distances[0, j] = j++);

    for (int i = 1;  i <= lengthA;  i++)
        for (int j = 1;  j <= lengthB;  j++)
            {
            int  cost = b[j - 1] == a[i - 1] ? 0 : 1;
            distances[i, j] = Math.Min 
                (
                Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                distances[i - 1, j - 1] + cost
                );
            }
    return distances[lengthA, lengthB];
    }
    }
    public class DropOutStack<T>
    {
    private T[] items;
    private int top = 0;
    public DropOutStack(int capacity)
    { 
        items = new T[capacity];
    }
    public void Clear(){
        for(int i = 0; i< items.Length; i++){
            items[i] = default(T);
        }
        top = 0;
    }
    public int Count(){
        int countNonDefaultItems = items.Count(item => !EqualityComparer<T>.Default.Equals(item, default(T)));
        return countNonDefaultItems;
    }
    public void Push(T item)
    {
        items[top] = item;
        top = (top + 1) % items.Length;
    }
    public T Pop()
    {
        if(Count() == 0) return default(T);
        top = (items.Length + top - 1) % items.Length;
        var toReturn = items[top];
        items[top] = default(T);
        return toReturn;
    }

   
    public T Peek()
    {
        return items[(items.Length + top - 1) % items.Length];
    }
    }
}