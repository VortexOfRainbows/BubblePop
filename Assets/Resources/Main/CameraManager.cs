using UnityEngine;

public static class CameraManager
{
    private readonly static Camera[] Buffer = new Camera[] { Camera.main, null, null, null };
    public static Camera MainCamera => Buffer[0];
    public static Camera LightingCamera => Buffer[1] == null ? Buffer[1]  = MainCamera.transform.GetChild(0).GetComponent<Camera>() : Buffer[1];
    public static Camera TileBorderCamera => Buffer[2] == null ? Buffer[2] = MainCamera.transform.GetChild(1).GetComponent<Camera>() : Buffer[2];
    public static Camera BorderMaskCamera => Buffer[3] == null ? Buffer[3] = MainCamera.transform.GetChild(2).GetComponent<Camera>() : Buffer[3];
    public static void SetCameraOrthographicSize(float value)
    {
        MainCamera.orthographicSize = value;
        LightingCamera.orthographicSize = value;
        TileBorderCamera.orthographicSize = value;
        BorderMaskCamera.orthographicSize = value;
    }
    public static void LerpCameraOrthographicSize(float target, float t)
    {
        SetCameraOrthographicSize(Mathf.Lerp(MainCamera.orthographicSize, target, t));
    }
    public static void SetCameraPosition(Vector2 pos)
    {
        MainCamera.transform.position = new Vector3(pos.x, pos.y, MainCamera.transform.position.z);
    }
    public static void LerpCameraPosition(Vector2 vector3, float t)
    {
        SetCameraPosition(Vector3.Lerp(MainCamera.transform.position, vector3, t));
    }
}
