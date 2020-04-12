using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAdvancedSpriteSheetAnimationListener
{
    void FinishedAnimation(int id, string name);
    void ChangedFrame(int id, string name, int newFrame);
}
