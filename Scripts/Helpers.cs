
using UnityEngine;

public class Helpers 
{
   public static void setpostionX(Transform t, float x)
    {
        t.position = new Vector3(x, t.position.y, t.position.z);
    }

    public static void setpostionY(Transform t, float y)
    {
        t.position = new Vector3(t.position.x, y, t.position.z);
    }

    public static void setpostionZ(Transform t, float z)
    {
        t.position = new Vector3(t.position.x, t.position.y, z);
    }

    public static int Mod(int x, int m)
    {
        //returns the result of x % m, adds m to that remainder to keep it positive
        //then finds the remainder on the positive value
        return (x % m + m) % m;
    }
}
