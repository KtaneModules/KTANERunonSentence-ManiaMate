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
    private float textSpeed = 0.1f;
    int length;

    float textPosBegin = 0.092f;
    float boundaryTextEnd = -0.094f;
    bool isLooping = true;

    public MeshRenderer ScreenColorBkgd;
    public MeshRenderer ScreenShape;
    public Material[] ScreenColorMats;
    public Texture[] ScreenTextures;

    public KMSelectable[] ColorButtons;
    public KMSelectable[] ShapeButtons;
    public KMSelectable[] PatternButtons;

    private char _currentColor;
    private char _currentPattern;
    private char _currentShape;

    private static readonly string _possibleColors = "WLDK";
    private static readonly string _possiblePatterns = "DEFS";
    private static readonly string _possibleShapes = "OSTX";

    //W = white, K = black, D = darkgrey, L = lightgrey
    private char[,] colorChart = new char[,]
    {
       {'W', 'K', 'D', 'K', 'K', 'K', 'D', 'L'},
       {'W', 'K', 'D', 'L', 'W', 'W', 'K', 'D'},
       {'W', 'L', 'L', 'L', 'W', 'L', 'L', 'L'},
       {'W', 'K', 'D', 'L', 'W', 'K', 'W', 'L'},
       {'K', 'W', 'W', 'D', 'W', 'L', 'L', 'W'},
       {'K', 'D', 'L', 'W', 'D', 'K', 'L', 'D'},
       {'D', 'K', 'L', 'D', 'K', 'K', 'W', 'D'},
       {'K', 'L', 'W', 'K', 'D', 'D', 'D', 'D'}
    };

    //F = filled, E = empty, S = striped, D = diamonds
    private char[,] patternChart = new char[,]
    {
       {'F', 'S', 'E', 'D', 'D', 'E', 'E', 'S'},
       {'S', 'S', 'E', 'E', 'F', 'D', 'D', 'F'},
       {'E', 'F', 'E', 'S', 'D', 'F', 'S', 'D'},
       {'E', 'S', 'D', 'F', 'E', 'F', 'S', 'F'},
       {'E', 'E', 'D', 'S', 'D', 'S', 'D', 'S'},
       {'D', 'D', 'E', 'F', 'E', 'F', 'D', 'F'},
       {'F', 'S', 'D', 'S', 'E', 'F', 'F', 'D'},
       {'F', 'E', 'S', 'E', 'D', 'S', 'F', 'S'}
    };

    //X = X, O = O, S = Square, T = Triangle
    private char[,] shapeChart = new char[,]
    {
       {'X', 'X', 'O', 'O', 'T', 'S', 'X', 'S'},
       {'O', 'T', 'S', 'O', 'T', 'S', 'S', 'X'},
       {'S', 'S', 'X', 'T', 'O', 'T', 'O', 'S'},
       {'X', 'O', 'O', 'O', 'T', 'S', 'S', 'X'},
       {'T', 'O', 'T', 'T', 'X', 'X', 'X', 'T'},
       {'X', 'X', 'S', 'O', 'T', 'O', 'O', 'T'},
       {'S', 'S', 'T', 'O', 'O', 'T', 'S', 'S'},
       {'X', 'T', 'X', 'X', 'T', 'S', 'O', 'X'}
    };

    void Activate()
    { //Shit that should happen when the bomb arrives (factory)/Lights turn on
        string[] sentence = { "Yo wassup hello, sit yo pretty ass so long as you came in the door, I just wanna chill.", "Hello" };
        int index2 = Rnd.Range(0, 2);
        text.text = sentence[index2];
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
    { //Shit
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

        string sent = "The chosen cell is ";
        int index = Rnd.Range(0, 64);
        Debug.Log(index);
        int row = index / 8;
        int col = index % 8;
        char colorAnswer = colorChart[row, col];
        char shapeAnswer = shapeChart[row, col];
        char pattAnswer = patternChart[row, col];
        // Debug.Log(colorAnswer);
        // Debug.Log(shapeAnswer);
        // Debug.Log(pattAnswer);

        _currentColor = _possibleColors.PickRandom();
        _currentShape = _possibleShapes.PickRandom();
        _currentPattern = _possiblePatterns.PickRandom();
        Debug.Log(_currentColor + ", " + _currentShape + ", " + _currentPattern);

        UpdateDisplayedShape();
    }

    private KMSelectable.OnInteractHandler ColorButtonPress(int i)
    {
        return delegate ()
        {
            _currentColor = _possibleColors[i];
            UpdateDisplayedShape();
            return false;
        };
    }

    private KMSelectable.OnInteractHandler ShapeButtonPress(int i)
    {
        return delegate ()
        {
            _currentShape = _possibleShapes[i];
            UpdateDisplayedShape();
            return false;
        };
    }

    private KMSelectable.OnInteractHandler PatternButtonPress(int i)
    {
        return delegate ()
        {
            _currentPattern = _possiblePatterns[i];
            UpdateDisplayedShape();
            return false;
        };
    }

    private void UpdateDisplayedShape()
    {
        ScreenColorBkgd.material = ScreenColorMats[_possibleColors.IndexOf(_currentColor)];
        ScreenShape.material.mainTexture = ScreenTextures[_possiblePatterns.IndexOf(_currentPattern) + (4 * _possibleShapes.IndexOf(_currentShape)) + ((_possibleColors.IndexOf(_currentColor) > 1) ? 16 : 0)];
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
