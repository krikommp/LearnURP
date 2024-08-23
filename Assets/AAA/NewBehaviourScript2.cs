using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class NewBehaviourScript2 : MonoBehaviour
{
    public MeshRenderer maskRenderer;
    public SpriteAtlas spriteAtlas;
    public string spriteName;
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        var texture = spriteAtlas.GetSprite(spriteName).texture;
        maskRenderer.sharedMaterial.SetTexture("_MainTex", texture);
    }
}
