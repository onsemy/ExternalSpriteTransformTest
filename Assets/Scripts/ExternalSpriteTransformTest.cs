using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ExternalSpriteTransformTest : MonoBehaviour
{
    [SerializeField] private Texture2D _texture;
    [SerializeField] private string _remoteAddress;

    private bool _isInitialized;
    private int _sequence;

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        if (string.IsNullOrEmpty(_remoteAddress))
        {
            Debug.LogError($"RemoteAddress is null or empty!");
            yield break;
        }

        using (var www = UnityWebRequestTexture.GetTexture(_remoteAddress))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to download from {_remoteAddress} - errorCode({www.responseCode}): {www.result} / {www.error}");
                yield break;
            }

            var texture = DownloadHandlerTexture.GetContent(www);
            if (texture == null)
            {
                Debug.LogError($"Failed to load from {_remoteAddress}");
                yield break;
            }

            Debug.Log($"Texture Info - width: {texture.width}, height: {texture.height}, texelSize: {texture.texelSize}");

            texture = _ScaleTexture(texture, 256, 256);
            texture.name = $"DownloadedTexture({_remoteAddress})";

            _texture = texture;
        }

        _isInitialized = true;
    }

    private void OnGUI()
    {
        if (!_isInitialized)
        {
            return;
        }

        if (GUILayout.Button("Generate Sprite"))
        {
            var obj = _GenerateSprite(_texture, _sequence % 2 == 0 ? 0.5f : -0.5f);
            obj.transform.position = new Vector3(_sequence++, 0f, 0f);
        }
    }

    private Texture2D _ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
        Color[] rpixels = result.GetPixels(0);
        float incX = (1.0f / (float)targetWidth);
        float incY = (1.0f / (float)targetHeight);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }

    private GameObject _GenerateSprite(Texture2D texture, float skewValue)
    {
        var obj = new GameObject();
        var render = obj.AddComponent<SpriteRenderer>();

        var shader = Shader.Find("Custom/SpriteSkewShader");
        var material = new Material(shader);
        material.SetTexture("_MainTex", texture);
        material.SetFloat("_VerticalSkew", skewValue);

        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 256f);
        sprite.name = texture.name;
        render.sprite = sprite;
        render.material = material;

        return obj;
    }
}
