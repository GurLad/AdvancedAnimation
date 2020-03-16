using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Version 2.3.0, 10/08/2019
 */
public enum Style { None, SinCurve, SlowFastCurve, FastSlowCurve }
/// <summary>
/// AdvancedAnimation allows you to easily add and interact with animations. Basically, you give it
/// every major frame (in the form of a copy of the object, but only with transform), the speed of
/// the transition between them and the style (ex. if it's accelrating, use SlowFastCurve). The
/// AdvancedAnimation code does all the rest. In addition, you can easily interact with every
/// variable of it through code, so if you want to change its speed/frames midway through it, you can
/// do that (Also, I don't know how to use any other animation tool, so bear with me).
/// </summary>
public class AdvancedAnimation : MonoBehaviour
{
    [Header("Base")]
    public GameObject Main;
    public List<GameObject> AnimationSteps;
    public float[] Speed;
    public Style[] Styles;
    [Header("Modifiers")]
    public int LoopTo = -1;
    public bool ImmidiateReturn = false;
    public bool AffectAll = false;
    public bool AffectPosition = false;
    public bool AffectMain = false;
    public bool AffectActive = false;
    public bool OneWay;
    public bool ActivateOnStart = false;
    public string FindMainByName;
    //You probably don't want to touch these
    [HideInInspector]
    public int PreviousGoal = 0;
    [HideInInspector]
    public int NextStep = 1;
    [HideInInspector]
    public int PreviousStep = 0;
    [HideInInspector]
    public int GoalStep = 0;
    [HideInInspector]
    public bool Active = false;
    [HideInInspector]
    public float Count;
    [HideInInspector]
    public bool FinishedHalf = false;
    [HideInInspector]
    public List<IAdvancedAnimationListener> AdvancedAnimationListeners { get; private set; }
    [HideInInspector]
    public bool Ordered = false;
    private List<Transform> Parts;
    private List<List<Transform>> AnimationParts; //AnimationParts[StepID][PartPointer]
    private List<List<int>> Pointers; //Pointers[StepID][MatchingPartID] = PartPointer ^
    private void Awake()
    {
        AdvancedAnimationListeners = new List<IAdvancedAnimationListener>();
    }
    // Gets all parts and sets pointers
    void Start()
    {
        if (Main == null)
        {
            if (FindMainByName == "")
            {
                throw new System.Exception("You need something to animate (assign the Main variable or use FindMainByName).");
            }
            else
            {
                Main = GameObject.Find(FindMainByName);
                if (Main == null)
                {
                    throw new System.Exception("Main not found (did you enter its name correctly?).");
                }
            }
        }
        Parts = new List<Transform>();
        AnimationParts = new List<List<Transform>>();
        Pointers = new List<List<int>>();
        Parts.AddRange(Main.GetComponentsInChildren<Transform>(true));
        if (!AffectMain) Parts.Remove(Main.transform);
        for (int i = 0; i < AnimationSteps.Count; i++)
        {
            AnimationParts.Add(new List<Transform>());
            AnimationParts[i].AddRange(AnimationSteps[i].GetComponentsInChildren<Transform>(true));
            if (!AffectMain) AnimationParts[i].Remove(AnimationSteps[i].transform);
        }
        for (int j = 0; j < Parts.Count; j++)
        {
            for (int i = 0; i < AnimationSteps.Count; i++)
            {
                Pointers.Add(new List<int>());
                for (int z = 0; z < 99; z++)
                {
                    Pointers[i].Add(-1);
                }
                for (int w = 0; w < AnimationParts[i].Count; w++)
                {
                    if (AnimationParts[i][w].name == Parts[j].name) Pointers[i][j] = w;
                }
            }
        }
        //Debug
        string ToPrint = "---Parts---\n";
        foreach (var item in Parts)
        {
            ToPrint += (item.name) + ", ";
        }
        Debug.Log(ToPrint + "\n---End---\n");
        ToPrint = "---AnimationParts---\n";
        int ii = 0;
        foreach (var item in AnimationParts)
        {
            ToPrint += "In " + ii + ": ";
            foreach (var item2 in item)
            {
                ToPrint += item2.name + ", ";
            }
            ToPrint += "\n";
            ii++;
        }
        Debug.Log(ToPrint + "\n---End---\n");
        //End debug
        if (ActivateOnStart)
        {
            StartAnimations();
        }
    }
    void Animate()
    {
        for (int i = 0; i < Parts.Count; i++)
        {
            float BackupCount = Count;
            try
            {
                if (Styles[PreviousStep] == Style.SinCurve) Count = -(Mathf.Sin(Count * Mathf.PI + Mathf.PI / 2)) / 2 + 0.5f;
                else if (Styles[PreviousStep] == Style.SlowFastCurve) Count = Mathf.Pow(Count, 2);
                else if (Styles[PreviousStep] == Style.FastSlowCurve) Count = Mathf.Pow(Count, 0.5f);
                Quaternion BaseValue = AnimationParts[PreviousStep][Pointers[PreviousStep][i]].localRotation;
                Quaternion FinalValue = AnimationParts[NextStep][Pointers[NextStep][i]].localRotation;
                if (BaseValue != FinalValue || AffectAll)
                {
                    Parts[i].localRotation = Quaternion.Slerp(BaseValue, FinalValue, Count);//new Quaternion(BaseValue.x * (1 - Count) + FinalValue.x * Count, BaseValue.y * (1 - Count) + FinalValue.y * Count, BaseValue.z * (1 - Count) + FinalValue.z * Count, BaseValue.w * (1 - Count) + FinalValue.w * Count);
                }
                if (AffectPosition)
                {
                    Vector3 BaseValue2 = AnimationParts[PreviousStep][Pointers[PreviousStep][i]].localPosition;
                    Vector3 FinalValue2 = AnimationParts[NextStep][Pointers[NextStep][i]].localPosition;
                    if (BaseValue2 != FinalValue2 || AffectAll)
                    {
                        Parts[i].localPosition = new Vector3(BaseValue2.x * (1 - Count) + FinalValue2.x * Count, BaseValue2.y * (1 - Count) + FinalValue2.y * Count, BaseValue2.z * (1 - Count) + FinalValue2.z * Count);
                    }
                }
            }
            catch { }
            Count = BackupCount;
        }
    }
    void Update()
    {
        if (!Ordered)
        {
            DoOneFrame();
        }
    }
    public void DoOneFrame()
    {
        if (!Active) return;
        Count += Time.deltaTime * Speed[PreviousStep];
        if (Count >= 1)
        {
            Count = 1;
            Animate();
            Count = 0;
            if (NextStep == 0 && LoopTo == -1)
            {
                Active = false;
                //Count = 0;
                return;
            }
            PreviousStep = NextStep;
            if (NextStep < GoalStep) NextStep++;
            else if (NextStep > GoalStep)
            {
                if (OneWay)
                {
                    FinishedHalf = true;
                    Active = false;
                }
                NextStep--;
                if (ImmidiateReturn)
                {
                    if (LoopTo != -1) NextStep = LoopTo;
                    else NextStep = 0;
                }
            }
            if (NextStep == LoopTo && LoopTo != -1)
            {
                GoalStep = PreviousGoal;
            }
            if (NextStep == GoalStep)
            {
                PreviousGoal = GoalStep;
                GoalStep = LoopTo;
            }
            for (int i = 0; i < Parts.Count; i++)
            {
                try
                {
                    bool BaseValue = AnimationParts[PreviousStep][Pointers[PreviousStep][i]].gameObject.activeSelf;
                    bool FinalValue = AnimationParts[NextStep][Pointers[NextStep][i]].gameObject.activeSelf;
                    if (BaseValue != FinalValue || AffectAll)
                    {
                        Parts[i].gameObject.SetActive(FinalValue);
                    }
                }
                catch { }
            }
            AdvancedAnimationListeners.ForEach(AAL => AAL.OnStepChange(NextStep));
        }
        Animate();
    }
    public void StartAnimations(bool BaseOnCurrent = false)
    {
        if (Active) return;
        GoalStep = AnimationSteps.Count - 1;
        PreviousGoal = GoalStep;
        NextStep = 1;
        PreviousStep = 0;
        if (NextStep == GoalStep)
        {
            PreviousGoal = GoalStep;
            GoalStep = LoopTo;
        }
        if (BaseOnCurrent)
        {
            for (int i = 0; i < Parts.Count; i++)
            {
                try
                {
                    AnimationParts[0][Pointers[0][i]].localRotation = Parts[i].localRotation;
                    AnimationParts[0][Pointers[0][i]].localPosition = Parts[i].localPosition;
                }
                catch { }
            }
        }
        Active = true;
    }
}