using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSheetAnimation : MonoBehaviour
{
    public Sprite SpriteSheet;
    public int NumberOfFrames;
    public bool Loop;
    public float Speed;
    public bool Active;
    [SerializeField]
    private SpriteRenderer renderer;
    private List<Sprite> frames;
    private float count = 0;
    private int currentFrame = 0;
    private void Start()
    {
        if (renderer == null)
        {
            renderer = GetComponent<SpriteRenderer>();
        }
        frames = new List<Sprite>();
        float Width = SpriteSheet.rect.width / NumberOfFrames;
        for (int i = 0; i < NumberOfFrames; i++)
        {
            frames.Add(Sprite.Create(SpriteSheet.texture, new Rect(Width * i, 0, Width, SpriteSheet.rect.height), new Vector2(0.5f, 0.5f)));
        }
        if (Active)
        {
            renderer.sprite = frames[currentFrame];
        }
    }
    private void Update()
    {
        if (Active)
        {
            count += Time.deltaTime * Speed;
            if (count >= 1)
            {
                count--;
                currentFrame++;
                if (currentFrame >= frames.Count)
                {
                    if (Loop)
                    {
                        currentFrame = 0;
                    }
                    else
                    {
                        Active = false;
                        return;
                    }
                }
                renderer.sprite = frames[currentFrame];
            }
        }
    }
    public void Activate()
    {
        currentFrame = 0;
        count = 0;
        Active = true;
        renderer.sprite = frames[currentFrame];
    }
}
