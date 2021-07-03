using UnityEditor;

static class ImportPackage
{
    const string kPackageExportPath = "Packages/net.peeweek.fpskit/Packages/FPSKit-SampleAdditions.unitypackage";

    [MenuItem("Tools/FPS Kit/Import Package")]
    static void Import()
    {
        AssetDatabase.ImportPackage(kPackageExportPath, false);
    }
}
