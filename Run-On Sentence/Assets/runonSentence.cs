using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class runonSentence : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMAudio Audio;

    private int ModuleId;
    private static int ModuleIdCounter = 1;
    private bool ModuleSolved;

    public TextMesh text;
    public MeshRenderer TextRenderer;
    public MeshRenderer mask;
    public MaskShaderManager manage;
    private float textSpeed = 0.05f;

    public MeshRenderer ScreenColorBkgd;
    public MeshRenderer ScreenShape;
    public Material[] ScreenColorMats;
    public Texture[] ScreenTextures;

    public KMSelectable[] ColorButtons;
    public KMSelectable[] ShapeButtons;
    public KMSelectable[] PatternButtons;
    public KMSelectable ResetButton;

    private CellColor _currentColor;
    private CellPattern _currentPattern;
    private CellShape _currentShape;
    private List<string> conds = new List<string>();
    private string tempMesg;

    private string _condition = "";
    private CellInfo[] _possibleAnswers;

    public enum CellColor { White, LightGray, DarkGray, Black }
    public enum CellPattern { Empty, Checkered, Stripes, Filled }
    public enum CellShape { Circle, Square, Triangle, Cross }

    public class CellInfo : IEquatable<CellInfo>
    {
        public CellColor Color;
        public CellPattern Pattern;
        public CellShape Shape;

        public CellInfo(CellColor color, CellPattern pattern, CellShape shape)
        {
            Color = color;
            Pattern = pattern;
            Shape = shape;
        }

        public bool Equals(CellInfo other)
        {
            return other != null && other.Color == Color && other.Pattern == Pattern && other.Shape == Shape;
        }

        public override bool Equals(object obj)
        {
            return obj is CellInfo && Equals((CellInfo)obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    private static readonly CellInfo[] _cellinfos = new CellInfo[]
    {
        new CellInfo(CellColor.White, CellPattern.Filled, CellShape.Cross),
        new CellInfo(CellColor.Black, CellPattern.Stripes, CellShape.Cross),
        new CellInfo(CellColor.DarkGray, CellPattern.Empty, CellShape.Circle),
        new CellInfo(CellColor.Black, CellPattern.Checkered, CellShape.Circle),
        new CellInfo(CellColor.Black, CellPattern.Checkered, CellShape.Triangle),
        new CellInfo(CellColor.Black, CellPattern.Empty, CellShape.Square),
        new CellInfo(CellColor.DarkGray, CellPattern.Empty, CellShape.Cross),
        new CellInfo(CellColor.LightGray, CellPattern.Stripes, CellShape.Square),
        new CellInfo(CellColor.White, CellPattern.Stripes, CellShape.Circle),
        new CellInfo(CellColor.Black, CellPattern.Stripes, CellShape.Triangle),
        new CellInfo(CellColor.DarkGray, CellPattern.Empty, CellShape.Square),
        new CellInfo(CellColor.LightGray, CellPattern.Empty, CellShape.Circle),
        new CellInfo(CellColor.White, CellPattern.Filled, CellShape.Triangle),
        new CellInfo(CellColor.White, CellPattern.Checkered, CellShape.Square),
        new CellInfo(CellColor.Black, CellPattern.Checkered, CellShape.Square),
        new CellInfo(CellColor.DarkGray, CellPattern.Filled, CellShape.Cross),
        new CellInfo(CellColor.White, CellPattern.Empty, CellShape.Square),
        new CellInfo(CellColor.LightGray, CellPattern.Filled, CellShape.Square),
        new CellInfo(CellColor.LightGray, CellPattern.Empty, CellShape.Cross),
        new CellInfo(CellColor.LightGray, CellPattern.Stripes, CellShape.Triangle),
        new CellInfo(CellColor.White, CellPattern.Checkered, CellShape.Circle),
        new CellInfo(CellColor.LightGray, CellPattern.Filled, CellShape.Triangle),
        new CellInfo(CellColor.LightGray, CellPattern.Stripes, CellShape.Circle),
        new CellInfo(CellColor.LightGray, CellPattern.Checkered, CellShape.Square),
        new CellInfo(CellColor.White, CellPattern.Empty, CellShape.Cross),
        new CellInfo(CellColor.Black, CellPattern.Stripes, CellShape.Circle),
        new CellInfo(CellColor.DarkGray, CellPattern.Checkered, CellShape.Circle),
        new CellInfo(CellColor.LightGray, CellPattern.Filled, CellShape.Circle),
        new CellInfo(CellColor.White, CellPattern.Empty, CellShape.Triangle),
        new CellInfo(CellColor.Black, CellPattern.Filled, CellShape.Square),
        new CellInfo(CellColor.White, CellPattern.Stripes, CellShape.Square),
        new CellInfo(CellColor.LightGray, CellPattern.Filled, CellShape.Cross),
        new CellInfo(CellColor.Black, CellPattern.Empty, CellShape.Triangle),
        new CellInfo(CellColor.White, CellPattern.Empty, CellShape.Circle),
        new CellInfo(CellColor.White, CellPattern.Checkered, CellShape.Triangle),
        new CellInfo(CellColor.DarkGray, CellPattern.Stripes, CellShape.Triangle),
        new CellInfo(CellColor.White, CellPattern.Checkered, CellShape.Cross),
        new CellInfo(CellColor.LightGray, CellPattern.Stripes, CellShape.Cross),
        new CellInfo(CellColor.LightGray, CellPattern.Checkered, CellShape.Cross),
        new CellInfo(CellColor.White, CellPattern.Stripes, CellShape.Triangle),
        new CellInfo(CellColor.Black, CellPattern.Checkered, CellShape.Cross),
        new CellInfo(CellColor.DarkGray, CellPattern.Checkered, CellShape.Cross),
        new CellInfo(CellColor.LightGray, CellPattern.Empty, CellShape.Square),
        new CellInfo(CellColor.White, CellPattern.Filled, CellShape.Circle),
        new CellInfo(CellColor.DarkGray, CellPattern.Empty, CellShape.Triangle),
        new CellInfo(CellColor.Black, CellPattern.Filled, CellShape.Circle),
        new CellInfo(CellColor.LightGray, CellPattern.Checkered, CellShape.Circle),
        new CellInfo(CellColor.DarkGray, CellPattern.Filled, CellShape.Triangle),
        new CellInfo(CellColor.DarkGray, CellPattern.Filled, CellShape.Square),
        new CellInfo(CellColor.Black, CellPattern.Stripes, CellShape.Square),
        new CellInfo(CellColor.LightGray, CellPattern.Checkered, CellShape.Triangle),
        new CellInfo(CellColor.DarkGray, CellPattern.Stripes, CellShape.Circle),
        new CellInfo(CellColor.Black, CellPattern.Empty, CellShape.Circle),
        new CellInfo(CellColor.Black, CellPattern.Filled, CellShape.Triangle),
        new CellInfo(CellColor.White, CellPattern.Filled, CellShape.Square),
        new CellInfo(CellColor.DarkGray, CellPattern.Checkered, CellShape.Square),
        new CellInfo(CellColor.Black, CellPattern.Filled, CellShape.Cross),
        new CellInfo(CellColor.LightGray, CellPattern.Empty, CellShape.Triangle),
        new CellInfo(CellColor.White, CellPattern.Stripes, CellShape.Cross),
        new CellInfo(CellColor.Black, CellPattern.Empty, CellShape.Cross),
        new CellInfo(CellColor.DarkGray, CellPattern.Checkered, CellShape.Triangle),
        new CellInfo(CellColor.DarkGray, CellPattern.Stripes, CellShape.Square),
        new CellInfo(CellColor.DarkGray, CellPattern.Filled, CellShape.Circle),
        new CellInfo(CellColor.DarkGray, CellPattern.Stripes, CellShape.Cross),
    };

    void Activate()
    { //Shit that should happen when the bomb arrives (factory)/Lights turn on
        //string[] sentence = { "Yo wassup hello, sit yo pretty ass so long as you came in the door, I just wanna chill.", "Hello" };
        //int index2 = Rnd.Range(0, 2);
        //text.text = sentence[index2];
        StartCoroutine(Scroll());
    }

    IEnumerator Scroll()
    {
        text.transform.localPosition = new Vector3(text.transform.localPosition.x, 0.0352f, text.transform.localPosition.z);
        while (true)
        {
            text.transform.Translate(Vector3.left * Time.deltaTime * textSpeed);
            yield return null;
        }
    }

    void Start()
    {
        ModuleId = ModuleIdCounter++;
        GetComponent<KMBombModule>().OnActivate += Activate;

        var fontTexture = TextRenderer.sharedMaterial.mainTexture;
        var mr = manage.MakeMaterials();
        TextRenderer.material = mr.Text;
        TextRenderer.material.mainTexture = fontTexture;
        mask.sharedMaterial = mr.Mask;

        for (int i = 0; i < ColorButtons.Length; i++)
            ColorButtons[i].OnInteract += ColorButtonPress(i);
        for (int i = 0; i < ShapeButtons.Length; i++)
            ShapeButtons[i].OnInteract += ShapeButtonPress(i);
        for (int i = 0; i < PatternButtons.Length; i++)
            PatternButtons[i].OnInteract += PatternButtonPress(i);
        ResetButton.OnInteract += ResetButtonPress();

        _currentColor = (CellColor)Rnd.Range(0, Enum.GetValues(typeof(CellColor)).Length);
        _currentShape = (CellShape)Rnd.Range(0, Enum.GetValues(typeof(CellShape)).Length);
        _currentPattern = (CellPattern)Rnd.Range(0, Enum.GetValues(typeof(CellPattern)).Length);

        UpdateDisplayedShape();

        string[] beg = { "The chosen cell ", "The answer ", "The opposite of the incorrect answer " };

        StartFromScratch:
        _condition = beg[Rnd.Range(0, 3)];
        _possibleAnswers = _cellinfos.ToArray();
        bool usedCase1 = false;
        bool usedCase2 = false;

        NewPhrase:
        int maxCases = 5;
        int rndCase = Rnd.Range(0, maxCases);

        // TEMP
        // rndCase = 4;

        if (rndCase == 0) // e.g. "is a circle"
        {
            if (usedCase1)
                goto NewPhrase;
            usedCase1 = true;
            var rndProp = Rnd.Range(0, 4);
            string str = "";
            if (Rnd.Range(0, 3) == 0)
            {
                var prop = (CellColor)rndProp;
                _possibleAnswers = _possibleAnswers.Where(i => i.Color == prop).ToArray();
                str = string.Format("is on a {0} background",
                    (prop == CellColor.LightGray ? "light gray" : prop == CellColor.DarkGray ? "dark gray" : prop.ToString()).ToLowerInvariant());
            }
            else if (Rnd.Range(0, 3) == 1)
            {
                var prop = (CellPattern)rndProp;
                _possibleAnswers = _possibleAnswers.Where(i => i.Pattern == prop).ToArray();
                str = string.Format("is {0}", prop.ToString().ToLowerInvariant());
            }
            else
            {
                var prop = (CellShape)rndProp;
                _possibleAnswers = _possibleAnswers.Where(i => i.Shape == prop).ToArray();
                str = string.Format("is a {0}", prop.ToString().ToLowerInvariant());
            }
            _condition += str;
            goto CheckConditions;
        }
        if (rndCase == 1) // e.g. is either a square or a triangle
        {
            if (usedCase2)
                goto NewPhrase;
            usedCase2 = true;
            var rndProp = Enumerable.Range(0, 4).ToArray().Shuffle();
            string str = "";
            if (Rnd.Range(0, 3) == 0)
            {
                var prop0 = (CellColor)rndProp[0];
                var prop1 = (CellColor)rndProp[1];
                _possibleAnswers = _possibleAnswers.Where(i => i.Color == prop0 || i.Color == prop1).ToArray();
                str = string.Format("is either on a {0} background or a {1} background",
                    (prop0 == CellColor.LightGray ? "light gray" : prop0 == CellColor.DarkGray ? "dark gray" : prop0.ToString()).ToLowerInvariant(),
                    (prop1 == CellColor.LightGray ? "light gray" : prop1 == CellColor.DarkGray ? "dark gray" : prop1.ToString()).ToLowerInvariant());
            }
            else if (Rnd.Range(0, 3) == 1)
            {
                var prop0 = (CellPattern)rndProp[0];
                var prop1 = (CellPattern)rndProp[1];
                _possibleAnswers = _possibleAnswers.Where(i => i.Pattern == prop0 || i.Pattern == prop1).ToArray();
                str = string.Format("is either {0} or {1}", prop0.ToString().ToLowerInvariant(), prop1.ToString().ToLowerInvariant());
            }
            else
            {
                var prop0 = (CellShape)rndProp[0];
                var prop1 = (CellShape)rndProp[1];
                _possibleAnswers = _possibleAnswers.Where(i => i.Shape == prop0 || i.Shape == prop1).ToArray();
                str = string.Format("is either a {0} or a {1}", prop0.ToString().ToLowerInvariant(), prop1.ToString().ToLowerInvariant());
            }
            _condition += str;
            goto CheckConditions;
        }
        if (rndCase == 2) // e.g. is orthogonally adjacent to a white background
        {
            var rndProp = Rnd.Range(0, 4);
            string str = "";
            if (Rnd.Range(0, 3) == 0)
            {
                var prop = (CellColor)rndProp;
                _possibleAnswers = _possibleAnswers.Where(i => GetOrthogonal(Array.IndexOf(_cellinfos, i)).Any(j => _cellinfos[j].Color == prop)).ToArray();
                str = string.Format("is orthogonally adjacent to a cell with a {0} background",
                    (prop == CellColor.LightGray ? "light gray" : prop == CellColor.DarkGray ? "dark gray" : prop.ToString()).ToLowerInvariant());
            }
            else if (Rnd.Range(0, 3) == 1)
            {
                var prop = (CellPattern)rndProp;
                _possibleAnswers = _possibleAnswers.Where(i => GetOrthogonal(Array.IndexOf(_cellinfos, i)).Any(j => _cellinfos[j].Pattern == prop)).ToArray();
                str = string.Format("is orthogonally adjacent to a {0} cell", prop.ToString().ToLowerInvariant());
            }
            else
            {
                var prop = (CellShape)rndProp;
                _possibleAnswers = _possibleAnswers.Where(i => GetOrthogonal(Array.IndexOf(_cellinfos, i)).Any(j => _cellinfos[j].Shape == prop)).ToArray();
                str = string.Format("is orthogonally adjacent to a {0}", prop.ToString().ToLowerInvariant());
            }
            _condition += str;
            goto CheckConditions;
        }
        if (rndCase == 3) // e.g. is diagonally adjacent to a checkered cell
        {
            var rndProp = Rnd.Range(0, 4);
            string str = "";
            if (Rnd.Range(0, 3) == 0)
            {
                var prop = (CellColor)rndProp;
                _possibleAnswers = _possibleAnswers.Where(i => GetDiagonal(Array.IndexOf(_cellinfos, i)).Any(j => _cellinfos[j].Color == prop)).ToArray();
                str = string.Format("is diagonally adjacent to a cell with a {0} background",
                    (prop == CellColor.LightGray ? "light gray" : prop == CellColor.DarkGray ? "dark gray" : prop.ToString()).ToLowerInvariant());
            }
            else if (Rnd.Range(0, 3) == 1)
            {
                var prop = (CellPattern)rndProp;
                _possibleAnswers = _possibleAnswers.Where(i => GetDiagonal(Array.IndexOf(_cellinfos, i)).Any(j => _cellinfos[j].Pattern == prop)).ToArray();
                str = string.Format("is diagonally adjacent to a {0} cell", prop.ToString().ToLowerInvariant());
            }
            else
            {
                var prop = (CellShape)rndProp;
                _possibleAnswers = _possibleAnswers.Where(i => GetDiagonal(Array.IndexOf(_cellinfos, i)).Any(j => _cellinfos[j].Shape == prop)).ToArray();
                str = string.Format("is diagonally adjacent to a {0}", prop.ToString().ToLowerInvariant());
            }
            _condition += str;
            goto CheckConditions;
        }
        if (rndCase == 4) // e.g. is a knights move away from a square
        {
            var rndProp = Rnd.Range(0, 4);
            string str = "";
            if (Rnd.Range(0, 3) == 0)
            {
                var prop = (CellColor)rndProp;
                _possibleAnswers = _possibleAnswers.Where(i => GetKnightPositions(Array.IndexOf(_cellinfos, i)).Any(j => _cellinfos[j].Color == prop)).ToArray();
                str = string.Format("is a knight's move away from a cell with a {0} background",
                    (prop == CellColor.LightGray ? "light gray" : prop == CellColor.DarkGray ? "dark gray" : prop.ToString()).ToLowerInvariant());
            }
            else if (Rnd.Range(0, 3) == 1)
            {
                var prop = (CellPattern)rndProp;
                _possibleAnswers = _possibleAnswers.Where(i => GetKnightPositions(Array.IndexOf(_cellinfos, i)).Any(j => _cellinfos[j].Pattern == prop)).ToArray();
                str = string.Format("is a knight's move away from a {0} cell", prop.ToString().ToLowerInvariant());
            }
            else
            {
                var prop = (CellShape)rndProp;
                _possibleAnswers = _possibleAnswers.Where(i => GetKnightPositions(Array.IndexOf(_cellinfos, i)).Any(j => _cellinfos[j].Shape == prop)).ToArray();
                str = string.Format("is a knight's move away from a {0}", prop.ToString().ToLowerInvariant());
            }
            _condition += str;
            goto CheckConditions;
        }

        CheckConditions:
        if (_possibleAnswers.Length > 1)// we need more conditions
        {
            // goto TempDone;
            _condition += ", ";
            goto NewPhrase;
        }
        if (_possibleAnswers.Length < 1) // oops we done fucked up, start over
            goto StartFromScratch;

        // We're done
        TempDone:
        Debug.Log(_possibleAnswers.Length);
        Debug.Log(_possibleAnswers.Select(i => Array.IndexOf(_cellinfos, i)).Join(" "));
        _condition += ".";
        Debug.Log(_condition);
    }

    private IEnumerable<int> GetOrthogonal(int pos)
    {
        if (pos % 8 != 0)
            yield return pos - 1;
        if (pos % 8 != 7)
            yield return pos + 1;
        if (pos / 8 != 0)
            yield return pos - 8;
        if (pos / 8 != 7)
            yield return pos + 8;
    }

    private IEnumerable<int> GetDiagonal(int pos)
    {
        if (pos % 8 != 0 && pos / 8 != 0)
            yield return pos - 9;
        if (pos % 8 != 7 && pos / 8 != 0)
            yield return pos - 7;
        if (pos % 8 != 0 && pos / 8 != 7)
            yield return pos + 7;
        if (pos % 8 != 7 && pos / 8 != 7)
            yield return pos + 9;
    }

    private IEnumerable<int> GetKnightPositions(int pos)
    {
        if (pos % 8 > 0 && pos / 8 > 1)
            yield return pos - 17;
        if (pos % 8 > 1 && pos / 8 > 0)
            yield return pos - 10;
        if (pos % 8 > 0 && pos / 8 < 6)
            yield return pos + 15;
        if (pos % 8 > 1 && pos / 8 < 7)
            yield return pos + 6;
        if (pos % 8 < 7 && pos / 8 > 1)
            yield return pos - 15;
        if (pos % 8 < 6 && pos / 8 > 0)
            yield return pos - 6;
        if (pos % 8 < 7 && pos / 8 < 6)
            yield return pos + 17;
        if (pos % 8 < 6 && pos / 8 < 7)
            yield return pos + 10;
    }

    /*
    private string GenerateSimpleCond(List<int> possAnswers)
    {
        int type = Rnd.Range(0, 3);
        int specType = Rnd.Range(0, 4);
        if (type == 0)
        {
            char ans = _possiblePatterns[specType];
            for (int i = 0; i < 64; i++)
            {
                if (!(patternChart[i / 8, i % 8].Equals(ans)))
                {
                    possAnswers.Remove(i);
                }
            }
            return "is " + ans;
        }
        if (type == 1)
        {
            char ans2 = _possibleShapes[specType];
            for (int i = 0; i < 64; i++)
            {
                if (!(shapeChart[i / 8, i % 8].Equals(ans2)))
                {
                    possAnswers.Remove(i);
                }
            }
            return "has a " + ans2;
        }
        char ans3 = _possibleColors[specType];
        for (int i = 0; i < 64; i++)
        {
            if (!(colorChart[i / 8, i % 8].Equals(ans3)))
            {
                possAnswers.Remove(i);
            }
        }
        return "is on a " + ans3 + " background";
    }
    */
    private void GenerateCompCond(List<int> possAnswers)
    {

    }

    private KMSelectable.OnInteractHandler ColorButtonPress(int i)
    {
        return delegate ()
        {
            _currentColor = (CellColor)i;
            UpdateDisplayedShape();
            return false;
        };
    }

    private KMSelectable.OnInteractHandler ShapeButtonPress(int i)
    {
        return delegate ()
        {
            _currentShape = (CellShape)i;
            UpdateDisplayedShape();
            return false;
        };
    }

    private KMSelectable.OnInteractHandler PatternButtonPress(int i)
    {
        return delegate ()
        {
            _currentPattern = (CellPattern)i;
            UpdateDisplayedShape();
            return false;
        };
    }

    private KMSelectable.OnInteractHandler ResetButtonPress()
    {
        return delegate ()
        {
            text.transform.localPosition = new Vector3(text.transform.localPosition.x, 0.0352f, text.transform.localPosition.z);
            return false;
        };
    }

    private void UpdateDisplayedShape()
    {
        ScreenColorBkgd.material = ScreenColorMats[(int)_currentColor];
        ScreenShape.material.mainTexture = ScreenTextures[(int)_currentPattern + (4 * (int)_currentShape) + ((int)_currentColor > 1 ? 16 : 0)];
    }

    void Update()
    { //Shit that happens at any point after initialization

    }

    void Solve()
    {
        GetComponent<KMBombModule>().HandlePass();
    }

    void Strike()
    {
        GetComponent<KMBombModule>().HandleStrike();
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use !{0} to do something.";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string Command)
    {
        yield return null;
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        yield return null;
    }
}
