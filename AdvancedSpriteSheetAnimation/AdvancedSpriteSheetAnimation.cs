using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This is my code for 2D animations (AdvancedAnimation's counterpart). This one allows you
/// to assign multiple animations and change which one is currently playing through different
/// scripts. You can also use SpriteSheetAnimator for single animations.
/// Version 1.0, 16/03/2020
/// </summary>
public class AdvancedSpriteSheetAnimation : MonoBehaviour
{
    public List<SpriteSheetData> Animations;
    public bool Active { get; private set; }
    public float BaseSpeed;
    public bool ActivateOnStart;
    [SerializeField]
    private SpriteRenderer renderer;
    private float speed;
    private bool loop;
    private float count = 0;
    private int currentAnimation = 0;
    private int currentFrame = 0;
    private void Start()
    {
        if (renderer == null)
        {
            renderer = GetComponent<SpriteRenderer>();
        }
        Animations.ForEach(a => a.Split());
        if (ActivateOnStart)
        {
            Activate(0);
        }
    }
    private void Update()
    {
        if (Active)
        {
            count += Time.deltaTime * speed;
            if (count >= 1)
            {
                count--;
                currentFrame++;
                if (currentFrame >= Animations[currentAnimation].Frames.Count)
                {
                    if (loop)
                    {
                        currentFrame = 0;
                    }
                    else
                    {
                        Active = false;
                        return;
                    }
                }
                renderer.sprite = Animations[currentAnimation].Frames[currentFrame];
            }
        }
    }
    public void Activate(int? animation = null)
    {
        currentAnimation = animation ?? currentAnimation;
        speed = Animations[currentAnimation].Speed > 0 ? Animations[currentAnimation].Speed : BaseSpeed;
        loop = Animations[currentAnimation].Loop;
        currentFrame = 0;
        count = 0;
        Active = true;
        renderer.sprite = Animations[currentAnimation].Frames[currentFrame];
    }
    public void Activate(string animationName)
    {
        int newID = Animations.FindIndex(a => a.Name == animationName);
        if (newID < 0)
        {
            throw new System.Exception("No matching animation!");
        }
        currentAnimation = newID;
        speed = Animations[currentAnimation].Speed > 0 ? Animations[currentAnimation].Speed : BaseSpeed;
        loop = Animations[currentAnimation].Loop;
        currentFrame = 0;
        count = 0;
        Active = true;
        renderer.sprite = Animations[currentAnimation].Frames[currentFrame];
    }
}
