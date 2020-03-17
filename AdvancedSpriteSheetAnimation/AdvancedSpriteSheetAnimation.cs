﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This is my code for 2D animations (AdvancedAnimation's counterpart). This one allows you
/// to assign multiple animations and change which one is currently playing through different
/// scripts. You can also use SpriteSheetAnimator for single animations.
/// Version 1.1, 17/03/2020
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
    private void Reset()
    {
        renderer = GetComponent<SpriteRenderer>();
    }
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
    /// <summary>
    /// Switches to the specified animation.
    /// </summary>
    /// <param name="animation">Animation ID to change to.</param>
    /// <param name="forceRestart">Restart the animation if the new ID is equal to the current one.</param>
    public void Activate(int animation, bool forceRestart = false)
    {
        if (!forceRestart && currentAnimation == animation)
        {
            return;
        }
        currentAnimation = animation;
        speed = Animations[currentAnimation].Speed > 0 ? Animations[currentAnimation].Speed : BaseSpeed;
        loop = Animations[currentAnimation].Loop;
        currentFrame = 0;
        count = 0;
        Active = true;
        renderer.sprite = Animations[currentAnimation].Frames[currentFrame];
    }
    /// <summary>
    /// Switches to the specified animation.
    /// </summary>
    /// <param name="animationName">Name of the animation to switch to.</param>
    /// <param name="forceRestart">Restart the animation if the new animation is equal to the current one.</param>
    /// <exception cref="System.Exception">No matching animation</exception>
    public void Activate(string animationName, bool forceRestart = false)
    {
        int newID = Animations.FindIndex(a => a.Name == animationName);
        if (newID < 0)
        {
            throw new System.Exception("No matching animation!");
        }
        if (!forceRestart && currentAnimation == newID)
        {
            return;
        }
        currentAnimation = newID;
        speed = Animations[currentAnimation].Speed > 0 ? Animations[currentAnimation].Speed : BaseSpeed;
        loop = Animations[currentAnimation].Loop;
        currentFrame = 0;
        count = 0;
        Active = true;
        renderer.sprite = Animations[currentAnimation].Frames[currentFrame];
    }
    /// <summary>
    /// Restarts the current animation.
    /// </summary>
    public void Restart()
    {
        currentFrame = 0;
        count = 0;
        Active = true;
        renderer.sprite = Animations[currentAnimation].Frames[currentFrame];
    }
    [ContextMenu("Assign first frame to renderer")]
    public void EditorPreview()
    {
        Animations[0].Split();
        renderer.sprite = Animations[0].Frames[0];
    }
}
