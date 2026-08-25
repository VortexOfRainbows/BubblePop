using UnityEngine;
using UnityEngine.Rendering.Universal;

public static class CameraManager
{
    private readonly static Camera[] Buffer = new Camera[] { Camera.main, null, null, null, null, null, null, null, null };
    public static Camera MainCamera => Buffer[0] == null ? Buffer[0] = Camera.main : Buffer[0];
    public static Camera LightingCamera => Buffer[1] == null ? Buffer[1]  = MainCamera.transform.GetChild(0).GetComponent<Camera>() : Buffer[1];
    public static Camera TileBorderCamera => Buffer[2] == null ? Buffer[2] = MainCamera.transform.GetChild(1).GetComponent<Camera>() : Buffer[2];
    public static Camera BorderMaskCamera => Buffer[3] == null ? Buffer[3] = MainCamera.transform.GetChild(2).GetComponent<Camera>() : Buffer[3];
    public static Camera SolidTileCamera => Buffer[4] == null ? Buffer[4] = MainCamera.transform.GetChild(3).GetComponent<Camera>() : Buffer[4];
    public static Camera OcclusionTileCamera => Buffer[5] == null ? Buffer[5] = MainCamera.transform.GetChild(4).GetComponent<Camera>() : Buffer[5];
    public static Camera UICamera => Buffer[6] == null ? Buffer[6] = MainCamera.transform.parent.GetChild(1).GetComponent<Camera>() : Buffer[6];
    public static Camera OilCamera => Buffer[7] == null ? Buffer[7] = MainCamera.transform.GetChild(5).GetComponent<Camera>() : Buffer[7];
    public static Camera CompendiumScreenshotCamera => Buffer[8] == null ? Buffer[8] = MainCamera.transform.parent.GetChild(2).GetComponent<Camera>() : Buffer[8];
    public static void SetCameraOrthographicSize(float value)
    {
        MainCamera.orthographicSize = value;
        LightingCamera.orthographicSize = value;
        TileBorderCamera.orthographicSize = value;
        BorderMaskCamera.orthographicSize = value;
        SolidTileCamera.orthographicSize = value + 2; //2 is the size of tiles we need on the edge of these cameras to help with rendering
        OcclusionTileCamera.orthographicSize = value + 2;
        OilCamera.orthographicSize = value;
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
    public static void SetSolidTileLightingOffset(Vector2 sunlightVector)
    {
        float tileScaleFactor = 2.0f;
        Vector2 scaledPosition = -sunlightVector * tileScaleFactor;
        SolidTileCamera.transform.localPosition = scaledPosition + new Vector2(0, 0.7f) * tileScaleFactor; //0.2f is the offset for tile tops, -0.5f is the offset for tile bottoms
        OcclusionTileCamera.transform.localPosition = scaledPosition; // - new Vector2(Utils.SignNoZero(sunlightVector.x), Utils.SignNoZero(sunlightVector.y));
    }

    public static RenderTexture ExportTexture => Resources.Load<RenderTexture>("UI/Compendium/ExportTexture");
    private static UniversalAdditionalCameraData MainCameraData;
    private static UniversalAdditionalCameraData CompendiumScreenshotCameraData;
    public static void ResizeExportTexture()
    {
        ExportTexture.Release();
        ExportTexture.width = 3840;
        TierListCompendiumPage page = Compendium.Instance.Pages[Compendium.Instance.PageNumber] as TierListCompendiumPage;
        ExportTexture.height = Mathf.RoundToInt(page.TierList.VerticalSize + 100) * 2;
        ExportTexture.Create();
    }
    public static void ReloadCameraData()
    {
        MainCameraData = MainCamera.GetUniversalAdditionalCameraData();
        CompendiumScreenshotCameraData = CompendiumScreenshotCamera.GetUniversalAdditionalCameraData();
    }
    public static void SwitchToScreenshotCamera()
    {
        ResizeExportTexture();
        ReloadCameraData();

        if (MainCameraData.cameraStack.Contains(UICamera))
            MainCameraData.cameraStack.Remove(UICamera);

        if (!CompendiumScreenshotCameraData.cameraStack.Contains(UICamera))
            CompendiumScreenshotCameraData.cameraStack.Add(UICamera);

        FixOverlayProperties(CompendiumScreenshotCamera);
        MainCamera.enabled = false;
    }
    public static void SwitchToMainCamera()
    {
        ReloadCameraData();

        if (CompendiumScreenshotCameraData.cameraStack.Contains(UICamera))
            CompendiumScreenshotCameraData.cameraStack.Remove(UICamera);
        
        if (!MainCameraData.cameraStack.Contains(UICamera))
            MainCameraData.cameraStack.Add(UICamera);

        FixOverlayProperties(MainCamera);
        MainCamera.enabled = true;
    }
    private static void FixOverlayProperties(Camera currentBaseCamera)
    {
        // Keeps the overlay camera physically aligned and scaled with the active base camera
        UICamera.targetTexture = currentBaseCamera.targetTexture;
        UICamera.orthographic = currentBaseCamera.orthographic;
        if (currentBaseCamera.orthographic)
            UICamera.orthographicSize = currentBaseCamera.orthographicSize;
        else
            UICamera.fieldOfView = currentBaseCamera.fieldOfView;
        UICamera.nearClipPlane = currentBaseCamera.nearClipPlane;
        UICamera.farClipPlane = currentBaseCamera.farClipPlane;
    }
}
