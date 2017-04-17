using UnityEngine;

public static class Extensions
{
   public static float X(this Transform t)
    {
        return t.position.x;
    }
    public static float Y(this Transform t)
    {
        return t.position.y;
    }

    public static T AssureSingletonAndDestroyExtras<T>(ref T mgrInstance, T obj) where T : MonoBehaviour
    {
        if (mgrInstance == null)
        {
            mgrInstance = obj;
        }
        else if (!mgrInstance.Equals(obj))
        {
            Object.Destroy(obj.gameObject);
            Debug.LogWarning("Destroyed nth instance of game Object.");
        }
        return mgrInstance;
    }
}
