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

    private string _condition = "";
    private CellInfo[] _possibleAnswers;

    public enum CellColor { White, LightGray, DarkGray, Black }
    public enum CellPattern { Empty, Checkered, Stripes, Filled }
    public enum CellShape { Circle, Square, Triangle, Cross }

    public class CellInfo : IEquatable<CellInfo>
    {
        public int Index;
        public CellColor Color;
        public CellPattern Pattern;
        public CellShape Shape;

        public CellInfo(int index, CellColor color, CellPattern pattern, CellShape shape)
        {
            Index = index;
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
        new CellInfo(0, CellColor.White, CellPattern.Filled, CellShape.Cross),
        new CellInfo(1, CellColor.Black, CellPattern.Stripes, CellShape.Cross),
        new CellInfo(2, CellColor.DarkGray, CellPattern.Empty, CellShape.Circle),
        new CellInfo(3, CellColor.Black, CellPattern.Checkered, CellShape.Circle),
        new CellInfo(4, CellColor.Black, CellPattern.Checkered, CellShape.Triangle),
        new CellInfo(5, CellColor.Black, CellPattern.Empty, CellShape.Square),
        new CellInfo(6, CellColor.DarkGray, CellPattern.Empty, CellShape.Cross),
        new CellInfo(7, CellColor.LightGray, CellPattern.Stripes, CellShape.Square),
        new CellInfo(8, CellColor.White, CellPattern.Stripes, CellShape.Circle),
        new CellInfo(9, CellColor.Black, CellPattern.Stripes, CellShape.Triangle),
        new CellInfo(10, CellColor.DarkGray, CellPattern.Empty, CellShape.Square),
        new CellInfo(11, CellColor.LightGray, CellPattern.Empty, CellShape.Circle),
        new CellInfo(12, CellColor.White, CellPattern.Filled, CellShape.Triangle),
        new CellInfo(13, CellColor.White, CellPattern.Checkered, CellShape.Square),
        new CellInfo(14, CellColor.Black, CellPattern.Checkered, CellShape.Square),
        new CellInfo(15, CellColor.DarkGray, CellPattern.Filled, CellShape.Cross),
        new CellInfo(16, CellColor.White, CellPattern.Empty, CellShape.Square),
        new CellInfo(17, CellColor.LightGray, CellPattern.Filled, CellShape.Square),
        new CellInfo(18, CellColor.LightGray, CellPattern.Empty, CellShape.Cross),
        new CellInfo(19, CellColor.LightGray, CellPattern.Stripes, CellShape.Triangle),
        new CellInfo(20, CellColor.White, CellPattern.Checkered, CellShape.Circle),
        new CellInfo(21, CellColor.LightGray, CellPattern.Filled, CellShape.Triangle),
        new CellInfo(22, CellColor.LightGray, CellPattern.Stripes, CellShape.Circle),
        new CellInfo(23, CellColor.LightGray, CellPattern.Checkered, CellShape.Square),
        new CellInfo(24, CellColor.White, CellPattern.Empty, CellShape.Cross),
        new CellInfo(25, CellColor.Black, CellPattern.Stripes, CellShape.Circle),
        new CellInfo(26, CellColor.DarkGray, CellPattern.Checkered, CellShape.Circle),
        new CellInfo(27, CellColor.LightGray, CellPattern.Filled, CellShape.Circle),
        new CellInfo(28, CellColor.White, CellPattern.Empty, CellShape.Triangle),
        new CellInfo(29, CellColor.Black, CellPattern.Filled, CellShape.Square),
        new CellInfo(30, CellColor.White, CellPattern.Stripes, CellShape.Square),
        new CellInfo(31, CellColor.LightGray, CellPattern.Filled, CellShape.Cross),
        new CellInfo(32, CellColor.Black, CellPattern.Empty, CellShape.Triangle),
        new CellInfo(33, CellColor.White, CellPattern.Empty, CellShape.Circle),
        new CellInfo(34, CellColor.White, CellPattern.Checkered, CellShape.Triangle),
        new CellInfo(35, CellColor.DarkGray, CellPattern.Stripes, CellShape.Triangle),
        new CellInfo(36, CellColor.White, CellPattern.Checkered, CellShape.Cross),
        new CellInfo(37, CellColor.LightGray, CellPattern.Stripes, CellShape.Cross),
        new CellInfo(38, CellColor.LightGray, CellPattern.Checkered, CellShape.Cross),
        new CellInfo(39, CellColor.White, CellPattern.Stripes, CellShape.Triangle),
        new CellInfo(40, CellColor.Black, CellPattern.Checkered, CellShape.Cross),
        new CellInfo(41, CellColor.DarkGray, CellPattern.Checkered, CellShape.Cross),
        new CellInfo(42, CellColor.LightGray, CellPattern.Empty, CellShape.Square),
        new CellInfo(43, CellColor.White, CellPattern.Filled, CellShape.Circle),
        new CellInfo(44, CellColor.DarkGray, CellPattern.Empty, CellShape.Triangle),
        new CellInfo(45, CellColor.Black, CellPattern.Filled, CellShape.Circle),
        new CellInfo(46, CellColor.LightGray, CellPattern.Checkered, CellShape.Circle),
        new CellInfo(47, CellColor.DarkGray, CellPattern.Filled, CellShape.Triangle),
        new CellInfo(48, CellColor.DarkGray, CellPattern.Filled, CellShape.Square),
        new CellInfo(49, CellColor.Black, CellPattern.Stripes, CellShape.Square),
        new CellInfo(50, CellColor.LightGray, CellPattern.Checkered, CellShape.Triangle),
        new CellInfo(51, CellColor.DarkGray, CellPattern.Stripes, CellShape.Circle),
        new CellInfo(52, CellColor.Black, CellPattern.Empty, CellShape.Circle),
        new CellInfo(53, CellColor.Black, CellPattern.Filled, CellShape.Triangle),
        new CellInfo(54, CellColor.White, CellPattern.Filled, CellShape.Square),
        new CellInfo(55, CellColor.DarkGray, CellPattern.Checkered, CellShape.Square),
        new CellInfo(56, CellColor.Black, CellPattern.Filled, CellShape.Cross),
        new CellInfo(57, CellColor.LightGray, CellPattern.Empty, CellShape.Triangle),
        new CellInfo(58, CellColor.White, CellPattern.Stripes, CellShape.Cross),
        new CellInfo(59, CellColor.Black, CellPattern.Empty, CellShape.Cross),
        new CellInfo(60, CellColor.DarkGray, CellPattern.Checkered, CellShape.Triangle),
        new CellInfo(61, CellColor.DarkGray, CellPattern.Stripes, CellShape.Square),
        new CellInfo(62, CellColor.DarkGray, CellPattern.Filled, CellShape.Circle),
        new CellInfo(63, CellColor.DarkGray, CellPattern.Stripes, CellShape.Cross),
    };

    void Activate()
    { //Shit that should happen when the bomb arrives (factory)/Lights turn on
        //string[] sentence = { "Yo wassup hello, sit yo pretty ass so long as you came in the door, I just wanna chill.", "Hello" };
        //int index2 = Rnd.Range(0, 2);
        //text.text = sentence[index2];
        text.text = _condition;
        text.anchor = TextAnchor.MiddleLeft;
        text.alignment = TextAlignment.Left;
        text.fontSize = 340;
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
            ColorButtons[i].OnInteract += ColorButtonPress(i, ColorButtons[i]);
        for (int i = 0; i < ShapeButtons.Length; i++)
            ShapeButtons[i].OnInteract += ShapeButtonPress(i, ShapeButtons[i]);
        for (int i = 0; i < PatternButtons.Length; i++)
            PatternButtons[i].OnInteract += PatternButtonPress(i, PatternButtons[i]);
        ResetButton.OnInteract += ResetButtonPress(ResetButton);

        _currentColor = (CellColor)Rnd.Range(0, Enum.GetValues(typeof(CellColor)).Length);
        _currentShape = (CellShape)Rnd.Range(0, Enum.GetValues(typeof(CellShape)).Length);
        _currentPattern = (CellPattern)Rnd.Range(0, Enum.GetValues(typeof(CellPattern)).Length);

        UpdateDisplayedShape();

        string[] _startTexts = {"Hello world", "Deez", "I can't even fit on the screen", "Good luck!"};
        string startText = _startTexts[Rnd.Range(0, 4)];
        text.text = startText;
        if (startText.Equals("Good luck!"))
        {
            text.transform.localPosition = new Vector3(text.transform.localPosition.x, -0.0305f, text.transform.localPosition.z);
        }
        if (startText.Equals("Deez"))
        {
            text.transform.localPosition = new Vector3(text.transform.localPosition.x, -0.0131f, text.transform.localPosition.z);
        }
        if (startText.Equals("I can't even fit on the screen"))
        {
            text.fontSize = 200;
            text.transform.localPosition = new Vector3(text.transform.localPosition.x, -0.0313f, text.transform.localPosition.z);
        }

        string[] beg = { "The chosen cell ", "The answer ", "The opposite of the incorrect answer " };

        StartFromScratch:
        _condition = beg[Rnd.Range(0, 3)];
        _possibleAnswers = _cellinfos.ToArray();
        int maxCases = 8;
        bool[] usedCases = new bool[maxCases];

        NewPhrase:
        int rndCase = Rnd.Range(0, maxCases);

        // TEMP
        // rndCase = 5;

        if (rndCase == 0) // e.g. "is a circle"
        {
            if (usedCases[rndCase])
                goto NewPhrase;
            usedCases[rndCase] = true;
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
            if (usedCases[rndCase])
                goto NewPhrase;
            usedCases[rndCase] = true;
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
            if (usedCases[rndCase])
                goto NewPhrase;
            usedCases[rndCase] = true;
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
            if (usedCases[rndCase])
                goto NewPhrase;
            usedCases[rndCase] = true;
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
            if (usedCases[rndCase])
                goto NewPhrase;
            usedCases[rndCase] = true;
            var rndProp = Rnd.Range(0, 4);
            string str = "";
            if (Rnd.Range(0, 3) == 0)
            {
                var prop = (CellColor)rndProp;
                _possibleAnswers = _possibleAnswers.Where(i => GetKnight(Array.IndexOf(_cellinfos, i)).Any(j => _cellinfos[j].Color == prop)).ToArray();
                str = string.Format("is a knight's move away from a cell with a {0} background",
                    (prop == CellColor.LightGray ? "light gray" : prop == CellColor.DarkGray ? "dark gray" : prop.ToString()).ToLowerInvariant());
            }
            else if (Rnd.Range(0, 3) == 1)
            {
                var prop = (CellPattern)rndProp;
                _possibleAnswers = _possibleAnswers.Where(i => GetKnight(Array.IndexOf(_cellinfos, i)).Any(j => _cellinfos[j].Pattern == prop)).ToArray();
                str = string.Format("is a knight's move away from a {0} cell", prop.ToString().ToLowerInvariant());
            }
            else
            {
                var prop = (CellShape)rndProp;
                _possibleAnswers = _possibleAnswers.Where(i => GetKnight(Array.IndexOf(_cellinfos, i)).Any(j => _cellinfos[j].Shape == prop)).ToArray();
                str = string.Format("is a knight's move away from a {0}", prop.ToString().ToLowerInvariant());
            }
            _condition += str;
            goto CheckConditions;
        }
        if (rndCase == 5) // e.g. has three distinct shapes orthogonally adjacent to it
        {
            if (usedCases[rndCase])
                goto NewPhrase;
            usedCases[rndCase] = true;
            var rndCount = Rnd.Range(2, 4);
            string str = "";
            if (Rnd.Range(0, 3) == 0)
            {
                _possibleAnswers = _possibleAnswers.Where(i => GetOrthogonal(i.Index).Select(j => _cellinfos[j].Color).Distinct().Count() == rndCount).ToArray();
                str = string.Format("has {0} distinct backgrounds orthogonally adjacent to it", rndCount);
            }
            else if (Rnd.Range(0, 3) == 1)
            {
                _possibleAnswers = _possibleAnswers.Where(i => GetOrthogonal(i.Index).Select(j => _cellinfos[j].Pattern).Distinct().Count() == rndCount).ToArray();
                str = string.Format("has {0} distinct patterns orthogonally adjacent to it", rndCount);
            }
            else
            {
                _possibleAnswers = _possibleAnswers.Where(i => GetOrthogonal(i.Index).Select(j => _cellinfos[j].Shape).Distinct().Count() == rndCount).ToArray();
                str = string.Format("has {0} distinct shapes orthogonally adjacent to it", rndCount);
            }
            _condition += str;
            goto CheckConditions;
        }
        if (rndCase == 6) // e.g. has three distinct shapes diagonally adjacent to it
        {
            if (usedCases[rndCase])
                goto NewPhrase;
            usedCases[rndCase] = true;
            var rndCount = Rnd.Range(2, 4);
            string str = "";
            if (Rnd.Range(0, 3) == 0)
            {
                _possibleAnswers = _possibleAnswers.Where(i => GetDiagonal(i.Index).Select(j => _cellinfos[j].Color).Distinct().Count() == rndCount).ToArray();
                str = string.Format("has {0} distinct backgrounds diagonally adjacent to it", rndCount);
            }
            else if (Rnd.Range(0, 3) == 1)
            {
                _possibleAnswers = _possibleAnswers.Where(i => GetDiagonal(i.Index).Select(j => _cellinfos[j].Pattern).Distinct().Count() == rndCount).ToArray();
                str = string.Format("has {0} distinct patterns diagonally adjacent to it", rndCount);
            }
            else
            {
                _possibleAnswers = _possibleAnswers.Where(i => GetDiagonal(i.Index).Select(j => _cellinfos[j].Shape).Distinct().Count() == rndCount).ToArray();
                str = string.Format("has {0} distinct shapes diagonally adjacent to it", rndCount);
            }
            _condition += str;
            goto CheckConditions;
        }
        if (rndCase == 7) // e.g. has three distinct shapes that are a knight's move away from it
        {
            if (usedCases[rndCase])
                goto NewPhrase;
            usedCases[rndCase] = true;
            var rndCount = Rnd.Range(2, 5);
            string str = "";
            if (Rnd.Range(0, 3) == 0)
            {
                _possibleAnswers = _possibleAnswers.Where(i => GetKnight(i.Index).Select(j => _cellinfos[j].Color).Distinct().Count() == rndCount).ToArray();
                str = string.Format("has {0} distinct backgrounds that are a knight's move away from it", rndCount);
            }
            else if (Rnd.Range(0, 3) == 1)
            {
                _possibleAnswers = _possibleAnswers.Where(i => GetKnight(i.Index).Select(j => _cellinfos[j].Pattern).Distinct().Count() == rndCount).ToArray();
                str = string.Format("has {0} distinct patterns that are a knight's move away from it", rndCount);
            }
            else
            {
                _possibleAnswers = _possibleAnswers.Where(i => GetKnight(i.Index).Select(j => _cellinfos[j].Shape).Distinct().Count() == rndCount).ToArray();
                str = string.Format("has {0} distinct shapes that are a knight's move away from it", rndCount);
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
        //TempDone:
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

    private IEnumerable<int> GetKnight(int pos)
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

    private KMSelectable.OnInteractHandler ColorButtonPress(int i, KMSelectable button)
    {
        return delegate ()
        {
            button.AddInteractionPunch(0.5f);
            _currentColor = (CellColor)i;
            UpdateDisplayedShape();
            return false;
        };
    }

    private KMSelectable.OnInteractHandler ShapeButtonPress(int i, KMSelectable button)
    {
        return delegate ()
        {
            button.AddInteractionPunch(0.5f);
            _currentShape = (CellShape)i;
            UpdateDisplayedShape();
            return false;
        };
    }

    private KMSelectable.OnInteractHandler PatternButtonPress(int i, KMSelectable button)
    {
        return delegate ()
        {
            button.AddInteractionPunch(0.5f);
            _currentPattern = (CellPattern)i;
            UpdateDisplayedShape();
            return false;
        };
    }

    private KMSelectable.OnInteractHandler ResetButtonPress(KMSelectable button)
    {
        return delegate ()
        {
            button.AddInteractionPunch();
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
