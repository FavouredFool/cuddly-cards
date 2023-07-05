using UnityEngine;
using UnityEditor;


namespace TMPro.EditorUtilities
{
    public class AdvancedDissolve_TMP_Default : ShaderGUI
    {
        public override void OnGUI(UnityEditor.MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            if (AmazingAssets.AdvancedDissolveEditor.AdvancedDissolveMaterialProperties.DrawDefaultOptionsHeader("TextMeshPro Default Options", (Material)materialEditor.target))
            {
                base.OnGUI(materialEditor, properties);
            }

            //AmazingAssets
            AmazingAssets.AdvancedDissolveEditor.AdvancedDissolveMaterialProperties.Init(properties);

            //AmazingAssets
            AmazingAssets.AdvancedDissolveEditor.AdvancedDissolveMaterialProperties.DrawCurvedWorldHeader(true, UnityEngine.GUIStyle.none, materialEditor, (Material)materialEditor.target);

            //AmazingAssets
            AmazingAssets.AdvancedDissolveEditor.AdvancedDissolveMaterialProperties.DrawDissolveOptions(true, materialEditor, false, false, false, false, false);
        }

        public override void ValidateMaterial(Material material)
        {
            base.ValidateMaterial(material);

            AmazingAssets.AdvancedDissolve.AdvancedDissolveKeywords.Reload(material);
        }
    }
}