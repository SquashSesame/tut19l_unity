using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class BundleSprite : MonoBehaviour
{
    [SerializeField] AssetReference AssetRefImage;

    UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<Sprite> operation;

    // Start is called before the first frame update
    void Start()
    {
        var sprite = GetComponent<SpriteRenderer> ();
        if (sprite != null) {

            operation = Addressables.LoadAssetAsync<Sprite> (AssetRefImage);
            operation.Completed += op => {

                // 読み込み終了時
                sprite.sprite = op.Result;
            };
        }
    }

    void Update ()
    {

    }
}
