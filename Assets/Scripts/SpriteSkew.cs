using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSkew : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        if (_renderer == null)
        {
            Debug.LogError("SpriteRenderer is not found!");
        }

        var material = _renderer.material;
    }

    public void SetSpriteSkew(Texture2D texture, float skewValue)
    {
        if (_renderer == null)
        {
            Debug.LogError("SpriteRenderer is not found!");
            return;
        }

        var shader = Shader.Find("Custom/SpriteSkewShader");
        _renderer.material.shader = shader;
        _renderer.material.SetFloat("_VerticalSkew", skewValue);
        _renderer.material.SetTexture("_MainTex", texture);

        var sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.one * 0.5f, 256f);
        _renderer.sprite = sprite;
    }
}
