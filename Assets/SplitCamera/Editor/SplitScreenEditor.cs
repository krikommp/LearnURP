using UnityEditor;
using UnityEngine;

public class SplitScreenEditor : ShaderGUI
{
    private MaterialProperty m_splitScreenMaskTexture1;
    private MaterialProperty m_splitScreenMaskTexture2;

    private MaterialProperty m_splitRotate;
    private MaterialProperty m_splitScreenRatio;

    private MaterialProperty m_maskScale;
    private MaterialProperty m_maskAttenuation;
    private MaterialProperty m_maskCoverage;
    private MaterialProperty m_maskStrength1;
    private MaterialProperty m_maskStrength2;

    private const string kSplitScreenMaskTexture1 = "_SplitScreenMaskTexture1";
    private const string kSplitScreenMaskTexture2 = "_SplitScreenMaskTexture2";
    private const string kSplitRotate = "_SplitRotate";
    private const string kSplitScreenRatio = "_SplitScreenRatio";
    private const string kMaskScale = "_MaskScale";
    private const string kMaskAttenuation = "_MaskAttenuation";
    private const string kMaskCoverage = "_MaskCoverage";
    private const string kMaskStrength1 = "_Mask1Strength";
    private const string kMaskStrength2 = "_Mask2Strength";

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        m_splitScreenMaskTexture1 = FindProperty(kSplitScreenMaskTexture1, properties);
        m_splitScreenMaskTexture2 = FindProperty(kSplitScreenMaskTexture2, properties);
        m_splitRotate = FindProperty(kSplitRotate, properties);
        m_splitScreenRatio = FindProperty(kSplitScreenRatio, properties);
        m_maskScale = FindProperty(kMaskScale, properties);
        m_maskAttenuation = FindProperty(kMaskAttenuation, properties);
        m_maskCoverage = FindProperty(kMaskCoverage, properties);
        m_maskStrength1 = FindProperty(kMaskStrength1, properties);
        m_maskStrength2 = FindProperty(kMaskStrength2, properties);

        EditorGUILayout.LabelField("分屏参数设置", EditorStyles.boldLabel);
        materialEditor.TexturePropertySingleLine(new GUIContent(kSplitScreenMaskTexture1), m_splitScreenMaskTexture1);
        // base.OnGUI(materialEditor, properties);
    }
}