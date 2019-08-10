using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderedAnimations : MonoBehaviour
{
    public List<AdvancedAnimation> animations;
    private void Start()
    {
        foreach (AdvancedAnimation animation in animations)
        {
            animation.Ordered = true;
        }
    }
    private void Update()
    {
        for (int i = 0; i < animations.Count; i++)
        {
            animations[i].DoOneFrame();
        }
    }
}
