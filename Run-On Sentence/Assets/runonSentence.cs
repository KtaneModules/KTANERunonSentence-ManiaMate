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
    public KMBombModule Module;
    public KMAudio Audio;

    private int ModuleId;
    private static int ModuleIdCounter = 1;
    private bool ModuleSolved;

    public TextMesh text;
    public MeshRenderer TextRenderer;
    public MeshRenderer mask;
    public MaskShaderManager manage;
    private const float textSpeed = 0.075f;

    public MeshRenderer ScreenColorBkgd;
    public MeshRenderer ScreenShape;
    public Material[] ScreenColorMats;
    public Texture[] ScreenTextures;

    public KMSelectable[] ColorButtons;
    public KMSelectable[] ShapeButtons;
    public KMSelectable[] PatternButtons;
    public KMSelectable ResetButton;
    public KMSelectable SubmitButton;

    private CellColor _currentColor;
    private CellPattern _currentPattern;
    private CellShape _currentShape;

    private string _condition = "";
    private CellInfo[] _possibleAnswers;
    private CellInfo _goalAnswer;

    public enum CellColor { White, LightGray, DarkGray, Black }
    public enum CellPattern { Empty, Checkered, Striped, Filled }
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

        public override string ToString()
        {
            return string.Format("a{0} {1} {2} with a {3} background", Pattern == CellPattern.Empty ? "n" : "", Pattern.ToString().ToLowerInvariant(), Shape.ToString().ToLowerInvariant(), (Color == CellColor.LightGray ? "light gray" : Color == CellColor.DarkGray ? "dark gray" : Color.ToString()).ToLowerInvariant());
        }
    }

    private static readonly CellInfo[] _cellinfos = new CellInfo[]
    {
        new CellInfo(0, CellColor.White, CellPattern.Filled, CellShape.Cross),
        new CellInfo(1, CellColor.Black, CellPattern.Striped, CellShape.Cross),
        new CellInfo(2, CellColor.DarkGray, CellPattern.Empty, CellShape.Circle),
        new CellInfo(3, CellColor.Black, CellPattern.Checkered, CellShape.Circle),
        new CellInfo(4, CellColor.Black, CellPattern.Checkered, CellShape.Triangle),
        new CellInfo(5, CellColor.Black, CellPattern.Empty, CellShape.Square),
        new CellInfo(6, CellColor.DarkGray, CellPattern.Empty, CellShape.Cross),
        new CellInfo(7, CellColor.LightGray, CellPattern.Striped, CellShape.Square),
        new CellInfo(8, CellColor.White, CellPattern.Striped, CellShape.Circle),
        new CellInfo(9, CellColor.Black, CellPattern.Striped, CellShape.Triangle),
        new CellInfo(10, CellColor.DarkGray, CellPattern.Empty, CellShape.Square),
        new CellInfo(11, CellColor.LightGray, CellPattern.Empty, CellShape.Circle),
        new CellInfo(12, CellColor.White, CellPattern.Filled, CellShape.Triangle),
        new CellInfo(13, CellColor.White, CellPattern.Checkered, CellShape.Square),
        new CellInfo(14, CellColor.Black, CellPattern.Checkered, CellShape.Square),
        new CellInfo(15, CellColor.DarkGray, CellPattern.Filled, CellShape.Cross),
        new CellInfo(16, CellColor.White, CellPattern.Empty, CellShape.Square),
        new CellInfo(17, CellColor.LightGray, CellPattern.Filled, CellShape.Square),
        new CellInfo(18, CellColor.LightGray, CellPattern.Empty, CellShape.Cross),
        new CellInfo(19, CellColor.LightGray, CellPattern.Striped, CellShape.Triangle),
        new CellInfo(20, CellColor.White, CellPattern.Checkered, CellShape.Circle),
        new CellInfo(21, CellColor.LightGray, CellPattern.Filled, CellShape.Triangle),
        new CellInfo(22, CellColor.LightGray, CellPattern.Striped, CellShape.Circle),
        new CellInfo(23, CellColor.LightGray, CellPattern.Checkered, CellShape.Square),
        new CellInfo(24, CellColor.White, CellPattern.Empty, CellShape.Cross),
        new CellInfo(25, CellColor.Black, CellPattern.Striped, CellShape.Circle),
        new CellInfo(26, CellColor.DarkGray, CellPattern.Checkered, CellShape.Circle),
        new CellInfo(27, CellColor.LightGray, CellPattern.Filled, CellShape.Circle),
        new CellInfo(28, CellColor.White, CellPattern.Empty, CellShape.Triangle),
        new CellInfo(29, CellColor.Black, CellPattern.Filled, CellShape.Square),
        new CellInfo(30, CellColor.White, CellPattern.Striped, CellShape.Square),
        new CellInfo(31, CellColor.LightGray, CellPattern.Filled, CellShape.Cross),
        new CellInfo(32, CellColor.Black, CellPattern.Empty, CellShape.Triangle),
        new CellInfo(33, CellColor.White, CellPattern.Empty, CellShape.Circle),
        new CellInfo(34, CellColor.White, CellPattern.Checkered, CellShape.Triangle),
        new CellInfo(35, CellColor.DarkGray, CellPattern.Striped, CellShape.Triangle),
        new CellInfo(36, CellColor.White, CellPattern.Checkered, CellShape.Cross),
        new CellInfo(37, CellColor.LightGray, CellPattern.Striped, CellShape.Cross),
        new CellInfo(38, CellColor.LightGray, CellPattern.Checkered, CellShape.Cross),
        new CellInfo(39, CellColor.White, CellPattern.Striped, CellShape.Triangle),
        new CellInfo(40, CellColor.Black, CellPattern.Checkered, CellShape.Cross),
        new CellInfo(41, CellColor.DarkGray, CellPattern.Checkered, CellShape.Cross),
        new CellInfo(42, CellColor.LightGray, CellPattern.Empty, CellShape.Square),
        new CellInfo(43, CellColor.White, CellPattern.Filled, CellShape.Circle),
        new CellInfo(44, CellColor.DarkGray, CellPattern.Empty, CellShape.Triangle),
        new CellInfo(45, CellColor.Black, CellPattern.Filled, CellShape.Circle),
        new CellInfo(46, CellColor.LightGray, CellPattern.Checkered, CellShape.Circle),
        new CellInfo(47, CellColor.DarkGray, CellPattern.Filled, CellShape.Triangle),
        new CellInfo(48, CellColor.DarkGray, CellPattern.Filled, CellShape.Square),
        new CellInfo(49, CellColor.Black, CellPattern.Striped, CellShape.Square),
        new CellInfo(50, CellColor.LightGray, CellPattern.Checkered, CellShape.Triangle),
        new CellInfo(51, CellColor.DarkGray, CellPattern.Striped, CellShape.Circle),
        new CellInfo(52, CellColor.Black, CellPattern.Empty, CellShape.Circle),
        new CellInfo(53, CellColor.Black, CellPattern.Filled, CellShape.Triangle),
        new CellInfo(54, CellColor.White, CellPattern.Filled, CellShape.Square),
        new CellInfo(55, CellColor.DarkGray, CellPattern.Checkered, CellShape.Square),
        new CellInfo(56, CellColor.Black, CellPattern.Filled, CellShape.Cross),
        new CellInfo(57, CellColor.LightGray, CellPattern.Empty, CellShape.Triangle),
        new CellInfo(58, CellColor.White, CellPattern.Striped, CellShape.Cross),
        new CellInfo(59, CellColor.Black, CellPattern.Empty, CellShape.Cross),
        new CellInfo(60, CellColor.DarkGray, CellPattern.Checkered, CellShape.Triangle),
        new CellInfo(61, CellColor.DarkGray, CellPattern.Striped, CellShape.Square),
        new CellInfo(62, CellColor.DarkGray, CellPattern.Filled, CellShape.Circle),
        new CellInfo(63, CellColor.DarkGray, CellPattern.Striped, CellShape.Cross),
    };

    private static readonly string[] _fakes = new string[]
    {
        "is on a white background",
        "is on a light gray background",
        "is on a dark gray background",
        "is on a black background",
        "is empty",
        "is checkered",
        "is striped",
        "is filled",
        "is a circle",
        "is a square",
        "is a triangle",
        "is a cross",
        "is orthogonally adjacent to a cell with a white background",
        "is orthogonally adjacent to a cell with a light gray background",
        "is orthogonally adjacent to a cell with a dark gray background",
        "is orthogonally adjacent to a cell with a black background",
        "is orthogonally adjacent to an empty cell",
        "is orthogonally adjacent to a checkered cell",
        "is orthogonally adjacent to a striped cell",
        "is orthogonally adjacent to a filled cell",
        "is orthogonally adjacent to a circle",
        "is orthogonally adjacent to a square",
        "is orthogonally adjacent to a triangle",
        "is orthogonally adjacent to a cross",
        "is diagonally adjacent to a cell with a white background",
        "is diagonally adjacent to a cell with a light gray background",
        "is diagonally adjacent to a cell with a dark gray background",
        "is diagonally adjacent to a cell with a black background",
        "is diagonally adjacent to an empty cell",
        "is diagonally adjacent to a checkered cell",
        "is diagonally adjacent to a striped cell",
        "is diagonally adjacent to a filled cell",
        "is diagonally adjacent to a circle",
        "is diagonally adjacent to a square",
        "is diagonally adjacent to a triangle",
        "is diagonally adjacent to a cross",
        "is a knight's move away from a cell with a white background",
        "is a knight's move away from a cell with a light gray background",
        "is a knight's move away from a cell with a dark gray background",
        "is a knight's move away from a cell with a black background",
        "is a knight's move away from an empty cell",
        "is a knight's move away from a checkered cell",
        "is a knight's move away from a striped cell",
        "is a knight's move away from a filled cell",
        "is a knight's move away from a circle",
        "is a knight's move away from a square",
        "is a knight's move away from a triangle",
        "is a knight's move away from a cross",
        "has 2 distinct backgrounds orthogonally adjacent to it",
        "has 3 distinct backgrounds orthogonally adjacent to it",
        "has 4 distinct backgrounds orthogonally adjacent to it",
        "has 2 distinct patterns orthogonally adjacent to it",
        "has 3 distinct patterns orthogonally adjacent to it",
        "has 4 distinct patterns orthogonally adjacent to it",
        "has 2 distinct shapes orthogonally adjacent to it",
        "has 3 distinct shapes orthogonally adjacent to it",
        "has 4 distinct shapes orthogonally adjacent to it",
        "has 2 distinct backgrounds diagonally adjacent to it",
        "has 3 distinct backgrounds diagonally adjacent to it",
        "has 4 distinct backgrounds diagonally adjacent to it",
        "has 2 distinct patterns diagonally adjacent to it",
        "has 3 distinct patterns diagonally adjacent to it",
        "has 4 distinct patterns diagonally adjacent to it",
        "has 2 distinct shapes diagonally adjacent to it",
        "has 3 distinct shapes diagonally adjacent to it",
        "has 4 distinct shapes diagonally adjacent to it",
        "has 2 distinct backgrounds that are a knight's move away from it",
        "has 3 distinct backgrounds that are a knight's move away from it",
        "has 4 distinct backgrounds that are a knight's move away from it",
        "has 2 distinct patterns that are a knight's move away from it",
        "has 3 distinct patterns that are a knight's move away from it",
        "has 4 distinct patterns that are a knight's move away from it",
        "has 2 distinct shapes that are a knight's move away from it",
        "has 3 distinct shapes that are a knight's move away from it",
        "has 4 distinct shapes that are a knight's move away from it",
        "is on the left half of the grid",
        "is on the right half of the grid",
        "is on the top half of the grid",
        "is on the bottom half of the grid"
    };

    void Activate()
    {
        text.text = _condition;
        text.anchor = TextAnchor.MiddleLeft;
        text.alignment = TextAlignment.Left;
        text.fontSize = 340;
        StartCoroutine(Scroll());
    }

    IEnumerator Scroll()
    {
        text.transform.localPosition = new Vector3(text.transform.localPosition.x, 0.0352f, text.transform.localPosition.z);
        while (!ModuleSolved)
        {
            if (text.transform.localPosition.y < -15.0f)
            {
                text.transform.localPosition = new Vector3(text.transform.localPosition.x, 0.0352f, text.transform.localPosition.z);
            }
            var y = text.transform.localPosition.y;
            text.transform.localPosition = new Vector3(text.transform.localPosition.x, y - (textSpeed * Time.deltaTime), text.transform.localPosition.z);
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

        for (int i = 0; i < PatternButtons.Length; i++)
            PatternButtons[i].OnInteract += PatternButtonPress(i);
        for (int i = 0; i < ShapeButtons.Length; i++)
            ShapeButtons[i].OnInteract += ShapeButtonPress(i);
        for (int i = 0; i < ColorButtons.Length; i++)
            ColorButtons[i].OnInteract += ColorButtonPress(i);
        ResetButton.OnInteract += ResetButtonPress;
        SubmitButton.OnInteract += SubmitButtonPress;

        _currentColor = (CellColor)Rnd.Range(0, Enum.GetValues(typeof(CellColor)).Length);
        _currentShape = (CellShape)Rnd.Range(0, Enum.GetValues(typeof(CellShape)).Length);
        _currentPattern = (CellPattern)Rnd.Range(0, Enum.GetValues(typeof(CellPattern)).Length);

        UpdateDisplayedShape();

        string[] _startTexts = { "Hello world", "Deez", "I can't even fit on the screen", "Good luck!" };
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
        int maxCases = 9; // Number of cases possible
        bool[] usedCases = new bool[maxCases];

        NewPhrase:
        if (!usedCases.Contains(false))
            goto StartFromScratch;

        int rndCase = Rnd.Range(0, maxCases);

        if (rndCase == 0) // e.g. "is a circle"
        {
            if (usedCases[rndCase])
                goto NewPhrase;
            usedCases[rndCase] = true;
            var rndProp = Rnd.Range(0, 4);
            var rndVal = Rnd.Range(0, 3);
            string str = "";
            if (rndVal == 0)
            {
                var prop = (CellColor)rndProp;
                _possibleAnswers = _possibleAnswers.Where(i => i.Color == prop).ToArray();
                str = string.Format("is on a {0} background",
                    (prop == CellColor.LightGray ? "light gray" : prop == CellColor.DarkGray ? "dark gray" : prop.ToString()).ToLowerInvariant());
            }
            else if (rndVal == 1)
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
            var rndVal = Rnd.Range(0, 3);
            string str = "";
            if (rndVal == 0)
            {
                var prop0 = (CellColor)rndProp[0];
                var prop1 = (CellColor)rndProp[1];
                _possibleAnswers = _possibleAnswers.Where(i => i.Color == prop0 || i.Color == prop1).ToArray();
                str = string.Format("is either on a {0} background or a {1} background",
                    (prop0 == CellColor.LightGray ? "light gray" : prop0 == CellColor.DarkGray ? "dark gray" : prop0.ToString()).ToLowerInvariant(),
                    (prop1 == CellColor.LightGray ? "light gray" : prop1 == CellColor.DarkGray ? "dark gray" : prop1.ToString()).ToLowerInvariant());
            }
            else if (rndVal == 1)
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
            var rndVal = Rnd.Range(0, 3);
            string str = "";
            if (rndVal == 0)
            {
                var prop = (CellColor)rndProp;
                _possibleAnswers = _possibleAnswers.Where(i => GetOrthogonal(Array.IndexOf(_cellinfos, i)).Any(j => _cellinfos[j].Color == prop)).ToArray();
                str = string.Format("is orthogonally adjacent to a cell with a {0} background",
                    (prop == CellColor.LightGray ? "light gray" : prop == CellColor.DarkGray ? "dark gray" : prop.ToString()).ToLowerInvariant());
            }
            else if (rndVal == 1)
            {
                var prop = (CellPattern)rndProp;
                _possibleAnswers = _possibleAnswers.Where(i => GetOrthogonal(Array.IndexOf(_cellinfos, i)).Any(j => _cellinfos[j].Pattern == prop)).ToArray();
                str = string.Format("is orthogonally adjacent to a{1} {0} cell", prop.ToString().ToLowerInvariant(), prop == CellPattern.Empty ? "n" : "");
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
            var rndVal = Rnd.Range(0, 3);
            string str = "";
            if (rndVal == 0)
            {
                var prop = (CellColor)rndProp;
                _possibleAnswers = _possibleAnswers.Where(i => GetDiagonal(Array.IndexOf(_cellinfos, i)).Any(j => _cellinfos[j].Color == prop)).ToArray();
                str = string.Format("is diagonally adjacent to a cell with a {0} background",
                    (prop == CellColor.LightGray ? "light gray" : prop == CellColor.DarkGray ? "dark gray" : prop.ToString()).ToLowerInvariant());
            }
            else if (rndVal == 1)
            {
                var prop = (CellPattern)rndProp;
                _possibleAnswers = _possibleAnswers.Where(i => GetDiagonal(Array.IndexOf(_cellinfos, i)).Any(j => _cellinfos[j].Pattern == prop)).ToArray();
                str = string.Format("is diagonally adjacent to a{1} {0} cell", prop.ToString().ToLowerInvariant(), prop == CellPattern.Empty ? "n" : "");
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
            var rndVal = Rnd.Range(0, 3);
            string str = "";
            if (rndVal == 0)
            {
                var prop = (CellColor)rndProp;
                _possibleAnswers = _possibleAnswers.Where(i => GetKnight(Array.IndexOf(_cellinfos, i)).Any(j => _cellinfos[j].Color == prop)).ToArray();
                str = string.Format("is a knight's move away from a cell with a {0} background",
                    (prop == CellColor.LightGray ? "light gray" : prop == CellColor.DarkGray ? "dark gray" : prop.ToString()).ToLowerInvariant());
            }
            else if (rndVal == 1)
            {
                var prop = (CellPattern)rndProp;
                _possibleAnswers = _possibleAnswers.Where(i => GetKnight(Array.IndexOf(_cellinfos, i)).Any(j => _cellinfos[j].Pattern == prop)).ToArray();
                str = string.Format("is a knight's move away from a{1} {0} cell", prop.ToString().ToLowerInvariant(), prop == CellPattern.Empty ? "n" : "");
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
            var rndVal = Rnd.Range(0, 3);
            string str = "";
            if (rndVal == 0)
            {
                _possibleAnswers = _possibleAnswers.Where(i => GetOrthogonal(i.Index).Select(j => _cellinfos[j].Color).Distinct().Count() == rndCount).ToArray();
                str = string.Format("has {0} distinct backgrounds orthogonally adjacent to it", rndCount);
            }
            else if (rndVal == 1)
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
            var rndVal = Rnd.Range(0, 3);
            string str = "";
            if (rndVal == 0)
            {
                _possibleAnswers = _possibleAnswers.Where(i => GetDiagonal(i.Index).Select(j => _cellinfos[j].Color).Distinct().Count() == rndCount).ToArray();
                str = string.Format("has {0} distinct backgrounds diagonally adjacent to it", rndCount);
            }
            else if (rndVal == 1)
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
            var rndVal = Rnd.Range(0, 3);
            string str = "";
            if (rndVal == 0)
            {
                _possibleAnswers = _possibleAnswers.Where(i => GetKnight(i.Index).Select(j => _cellinfos[j].Color).Distinct().Count() == rndCount).ToArray();
                str = string.Format("has {0} distinct backgrounds that are a knight's move away from it", rndCount);
            }
            else if (rndVal == 1)
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
        if (rndCase == 8) // e.g. is in the bottom half of the grid
        {
            if (usedCases[rndCase])
                goto NewPhrase;
            usedCases[rndCase] = true;
            var rndVal = Rnd.Range(0, 4);
            string str;
            if (rndVal == 0)
            {
                _possibleAnswers = _possibleAnswers.Where(i => i.Index % 8 <= 3).ToArray();
                str = "is on the left half of the grid";
            }
            else if (rndVal == 1)
            {
                _possibleAnswers = _possibleAnswers.Where(i => i.Index % 8 >= 4).ToArray();
                str = "is on the right half of the grid";
            }
            else if (rndVal == 2)
            {
                _possibleAnswers = _possibleAnswers.Where(i => i.Index / 8 <= 3).ToArray();
                str = "is on the top half of the grid";
            }
            else
            {
                _possibleAnswers = _possibleAnswers.Where(i => i.Index / 8 >= 4).ToArray();
                str = "is on the bottom half of the grid";
            }
            _condition += str;
            goto CheckConditions;
        }

        CheckConditions:
        if (_possibleAnswers.Length > 1) // we need more conditions
        {
            _condition += " and ";
            goto NewPhrase;
        }
        if (_possibleAnswers.Length == 0) // oops we done fucked up, start over
            goto StartFromScratch;

        // We're done
        _condition += ".";
        _goalAnswer = _possibleAnswers.First();

        // Fuck it dummy time
        var conditionWithoutDummy = _condition;
        var ands = new List<int>();
        for (int i = 0; i < _condition.Length - 2; i++)
            if (_condition.Substring(i, 3) == "and")
                ands.Add(i + 3);
        var randomIxs = ands.ToArray().Shuffle().Take(2).OrderBy(x => x).ToArray();
        var stringA = _condition.Substring(0, randomIxs[1]);
        var stringB = _condition.Substring(randomIxs[1]);
        int ix = ands.IndexOf(randomIxs[0]);
        var ordinals = new[] { "first", "second", "third", "fourth", "fifth", "sixth", "seventh", "eighth", "ninth", "tenth" };
        _condition = stringA + " actually ignore the " + ordinals[ix + 1] + " thing I said and" + stringB;
        var fake = _fakes.PickRandom();
        var stringC = _condition.Substring(0, randomIxs[0]);
        var stringD = _condition.Substring(randomIxs[0]);
        _condition = stringC + " " + fake + " and" + stringD;

        Debug.LogFormat("[Run-On Sentence #{0}] The display reads:", ModuleId);
        Debug.LogFormat("[Run-On Sentence #{0}] \"{1}\"", ModuleId, _condition);
        Debug.LogFormat("[Run-On Sentence #{0}] The dummy phrase: \"{1}\".", ModuleId, fake);
        Debug.LogFormat("[Run-On Sentence #{0}] The display without the dummy reads:", ModuleId);
        Debug.LogFormat("[Run-On Sentence #{0}] \"{1}\"", ModuleId, conditionWithoutDummy);
        Debug.LogFormat("[Run-On Sentence #{0}] The cell to submit, located at {1}, is {2}.", ModuleId, GetCoord(_goalAnswer.Index), _goalAnswer.ToString());
    }

    private string GetCoord(int pos)
    {
        return "ABCDEFGH"[pos % 8].ToString() + ((pos / 8) + 1).ToString();
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

    private KMSelectable.OnInteractHandler PatternButtonPress(int i)
    {
        return delegate ()
        {
            PatternButtons[i].AddInteractionPunch(0.5f);
            if (ModuleSolved)
                return false;
            PlayRandomSound();
            _currentPattern = (CellPattern)i;
            UpdateDisplayedShape();
            return false;
        };
    }

    private KMSelectable.OnInteractHandler ShapeButtonPress(int i)
    {
        return delegate ()
        {
            ShapeButtons[i].AddInteractionPunch(0.5f);
            if (ModuleSolved)
                return false;
            PlayRandomSound();
            _currentShape = (CellShape)i;
            UpdateDisplayedShape();
            return false;
        };
    }

    private KMSelectable.OnInteractHandler ColorButtonPress(int i)
    {
        return delegate ()
        {
            ColorButtons[i].AddInteractionPunch(0.5f);
            if (ModuleSolved)
                return false;
            PlayRandomSound();
            _currentColor = (CellColor)i;
            UpdateDisplayedShape();
            return false;
        };
    }

    private void PlayRandomSound()
    {
        Audio.PlaySoundAtTransform("Punch" + Rnd.Range(1, 3), transform);
    }

    private bool ResetButtonPress()
    {
        ResetButton.AddInteractionPunch(0.5f);
        if (ModuleSolved)
            return false;
        text.transform.localPosition = new Vector3(text.transform.localPosition.x, 0.0352f, text.transform.localPosition.z);
        return false;
    }

    private bool SubmitButtonPress()
    {
        SubmitButton.AddInteractionPunch(0.5f);
        if (ModuleSolved)
            return false;
        var inputCell = new CellInfo(-1, _currentColor, _currentPattern, _currentShape);
        if (inputCell.Equals(_goalAnswer))
        {
            ModuleSolved = true;
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
            Module.HandlePass();
            string[] _solveTexts = { "Yippee!", "W Defuser", ":)", "Hope it was fun"};

            string solveText = _solveTexts[Rnd.Range(0, 4)];
            text.text = solveText;

            if (solveText.Equals("Yippee!"))
            {
                text.transform.localPosition = new Vector3(text.transform.localPosition.x, -0.0212f, text.transform.localPosition.z);
            }
            if (solveText.Equals("W Defuser"))
            {
                text.transform.localPosition = new Vector3(text.transform.localPosition.x, -0.031f, text.transform.localPosition.z);
            }
            if (solveText.Equals(":)"))
            {
                text.transform.localPosition = new Vector3(text.transform.localPosition.x, -0.0028f, text.transform.localPosition.z);
            }
            if (solveText.Equals("Hope it was fun"))
            {
                text.fontSize = 240;
                text.transform.localPosition = new Vector3(text.transform.localPosition.x, -0.031f, text.transform.localPosition.z);
            }
           
            Debug.LogFormat("[Run-On Sentence #{0}] You correctly submitted {1}. Module solved.", ModuleId, inputCell.ToString());
        }
        else
        {
            Module.HandleStrike();
            Debug.LogFormat("[Run-On Sentence #{0}] You incorrectly submitted {1}. Strike.", ModuleId, inputCell.ToString());
        }
        return false;
    }

    private void UpdateDisplayedShape()
    {
        ScreenColorBkgd.material = ScreenColorMats[(int)_currentColor];
        ScreenShape.material.mainTexture = ScreenTextures[(int)_currentPattern + (4 * (int)_currentShape) + ((int)_currentColor > 1 ? 16 : 0)];
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = "!{0} reset [Press the reset button.] | !{0} submit empty square white [Submit an empty square with a white background.]\nPatterns are empty, checkered, striped, filled. Shapes are circle, square, triangle, cross. Colors are white, light gray, dark gray, black.";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        var m = Regex.Match(command, @"^\s*reset\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (m.Success)
        {
            yield return null;
            ResetButton.OnInteract();
            yield break;
        }
        m = Regex.Match(command, @"^\s*submit\s+(?:(?<p0>empty)|(?<p1>checkered)|(?<p2>striped)|(?<p3>filled))\s+(?:(?<s0>circle)|(?<s1>square)|(?<s2>triangle)|(?<s3>cross))\s+(?:(?<c0>white)|(?<c1>light\s+gray)|(?<c2>dark\s+gray)|(?<c3>black))\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (!m.Success)
            yield break;

        yield return null;
        int patternIx = Enumerable.Range(0, 4).First(ix => m.Groups["p" + ix].Success);
        int shapeIx = Enumerable.Range(0, 4).First(ix => m.Groups["s" + ix].Success);
        int colorIx = Enumerable.Range(0, 4).First(ix => m.Groups["c" + ix].Success);

        PatternButtons[patternIx].OnInteract();
        yield return new WaitForSeconds(0.2f);
        ShapeButtons[shapeIx].OnInteract();
        yield return new WaitForSeconds(0.2f);
        ColorButtons[colorIx].OnInteract();
        yield return new WaitForSeconds(0.2f);
        SubmitButton.OnInteract();
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        PatternButtons[(int)_goalAnswer.Pattern].OnInteract();
        yield return new WaitForSeconds(0.2f);
        ShapeButtons[(int)_goalAnswer.Shape].OnInteract();
        yield return new WaitForSeconds(0.2f);
        ColorButtons[(int)_goalAnswer.Color].OnInteract();
        yield return new WaitForSeconds(0.2f);
        SubmitButton.OnInteract();
        yield break;
    }
}