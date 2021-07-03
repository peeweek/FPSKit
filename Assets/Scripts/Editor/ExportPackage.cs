using UnityEditor;

static class ExportPackage
{
    const string kPackageExportPath = "Packages/net.peeweek.fpskit/Packages/FPSKit-SampleAdditions.unitypackage";

    [MenuItem("Tools/FPS Kit/Export Package")]
    static void Export()
    {
        try
        {
            EditorUtility.DisplayProgressBar("FPSKit Export", "Exporting Sample Package...", 0f);
            AssetDatabase.ExportPackage("Assets/FPSKIt", kPackageExportPath, ExportPackageOptions.Recurse);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }
}
