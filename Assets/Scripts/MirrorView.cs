using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class MirrorView : MonoBehaviour
{
    public bool horizontal = true;
    public bool vertical = false;

    GameManager gameManager;
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if(gameManager != null){
            horizontal = gameManager.mirrorView;
        }

        float scaleX = horizontal ? -1f : 1f;
        float scaleY = vertical ? -1f : 1f;
        Vector2 scale = new Vector2(scaleX, scaleY);
        Vector2 offset = new Vector2(scaleX < 0 ? 1 : 0, scaleY < 0 ? 1 : 0);

        Graphics.Blit(src, dest, scale, offset);
    }
}
