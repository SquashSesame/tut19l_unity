using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TestAddressable : MonoBehaviour
{
    [SerializeField]　AssetReference assetRefImage;

    // Start is called before the first frame update
    void Start()
    {
        var sprite = GetComponent<SpriteRenderer> ();
        if (sprite != null) {

            // 読み込み
            var operation = Addressables.LoadAssetAsync<Sprite> (assetRefImage);
            operation.Completed += op => {
                // 読み込みが終了したら、Sprite の画像を設定する
                sprite.sprite = op.Result;
            };
        }
    }
}
