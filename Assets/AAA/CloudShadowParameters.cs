using UnityEngine;

namespace AAA
{
    [CreateAssetMenu(fileName = "CloudShadowParameters", menuName = "AAA/CloudShadowParameters", order = 0)]
    public class CloudShadowParameters : ScriptableObject
    {
        [SerializeField] public Texture2D m_CloudShadowTexture;
        [SerializeField] public Vector2 m_CloudShadowTiling = Vector2.one;
        [SerializeField] public Vector2 m_CloudShadowOffset = Vector2.zero;

        [SerializeField] public Vector2 m_cloudShadowSpeed = Vector2.zero;
        [SerializeField] public Vector4 m_cloudShadowParams = Vector4.zero;
    }
}