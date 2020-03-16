using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SpriteSheetData
{
    public string Name;
    public Sprite SpriteSheet;
    public int NumberOfFrames;
    public float Speed = -1;
    public bool Loop = false;
    [HideInInspector]
    public List<Sprite> Frames;
    public void Split()
    {
        Frames = new List<Sprite>();
        float Width = SpriteSheet.rect.width / NumberOfFrames;
        for (int i = 0; i < NumberOfFrames; i++)
        {
            Frames.Add(Sprite.Create(SpriteSheet.texture, new Rect(Width * i, 0, Width, SpriteSheet.rect.height), new Vector2(0.5f, 0.5f)));
        }
    }
}
