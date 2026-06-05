using UnityEngine;

public class TestBehaviour : MonoBehaviour
{
    public void Destroy()
    {
        Object.DestroyImmediate(this);
    }
}