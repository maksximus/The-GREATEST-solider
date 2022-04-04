using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
    [SerializeField] DialogConfig DialogConfig;
    [SerializeField] ChrisDialogButton[] DialogButtons;
    [SerializeField] OscarGalaController GalaController;
    [SerializeField] CanvasGroup MenuVisuals;


    List<int> AlreadyDrawedPositiveDialogues = new List<int>();
    List<int> AlreadyDrawedNegativeDialogues = new List<int>();
    List<int> AlreadyDrawedNeutralDialogues = new List<int>();
    List<int> ButtonIndexAlreadyDrawedThisRound = new List<int>();
    const int ButtonCount = 3;


    void Start()
    {
        Assert.IsNotNull(DialogConfig, "Dialog Data not set");
        Assert.IsNotNull(DialogButtons, "DialogButtons not set");

        //Setup for advanced button manipulation
        foreach (var chDButton in DialogButtons)
        {
            chDButton.Setup(this);
            chDButton.AdvancedClick.AddListener(ButtonClicked);
        }

        //Hook to Gala Controller Events
        GalaController.OnNextRound.AddListener(GalaRequestedNextRound);
        GalaController.OnGameEnded.AddListener(GalaEnded);

        ShowDialogues();
        NextQuestion();
    }


    public void NextQuestion()
    {
        ButtonIndexAlreadyDrawedThisRound.Clear();

        //DrawPositive
        DialogItem drawedPositiveData;
        Draw(ref AlreadyDrawedPositiveDialogues, DialogConfig.Positive, out drawedPositiveData);
        var positiveButton = DrawButton(ref ButtonIndexAlreadyDrawedThisRound, DialogButtons);
        positiveButton.SetDialog(drawedPositiveData);

        //DrawNegative
        DialogItem drawedNegativeData;
        Draw(ref AlreadyDrawedNegativeDialogues, DialogConfig.Negative, out drawedNegativeData);
        var negativeButton = DrawButton(ref ButtonIndexAlreadyDrawedThisRound, DialogButtons);
        negativeButton.SetDialog(drawedNegativeData);

        //DrawNegative
        DialogItem drawedNeutralData;
        Draw(ref AlreadyDrawedNeutralDialogues, DialogConfig.Neutral, out drawedNeutralData);
        var neutralButton = DrawButton(ref ButtonIndexAlreadyDrawedThisRound, DialogButtons);
        neutralButton.SetDialog(drawedNeutralData);

        ButtonIndexAlreadyDrawedThisRound.Clear();
    }

    int Draw(ref List<int> alreadyDrawed, DialogItem[] allDialoguePool, out DialogItem item)
    {
        //Check if all positives already draw and we need to reshuffle dialogues
        if (alreadyDrawed.Count >= allDialoguePool.Length)
        {
            //reset memory
            alreadyDrawed.Clear();
        }

        int drawedIndex = -1;
        do
        {
            drawedIndex = Random.Range(0, allDialoguePool.Length);
        }
        while (alreadyDrawed.Contains(drawedIndex));
        alreadyDrawed.Add(drawedIndex);

        item = allDialoguePool[drawedIndex];

        return drawedIndex;
    }

    ChrisDialogButton DrawButton(ref List<int> alreadyDrawed, ChrisDialogButton[] buttons)
    {
        //Check if can draw any more buttons
        if (alreadyDrawed.Count >= buttons.Length)
        {
            //reset memory
            alreadyDrawed.Clear();
        }

        int drawedIndex = -1;
        do
        {
            drawedIndex = Random.Range(0, buttons.Length);
        }
        while (alreadyDrawed.Contains(drawedIndex));
        alreadyDrawed.Add(drawedIndex);

        return buttons[drawedIndex];
    }

    void GalaRequestedNextRound()
    {
        ShowDialogues();
        NextQuestion();
    }
    void GalaEnded(int score)
    {
        HideDialogues();

        //TODO:Show score and button to restart game
    }

    void ShowDialogues()
    {
        //TODO:fade in
        MenuVisuals.alpha = 1;   
    }

    void HideDialogues()
    {
        //TODO:fade out
        MenuVisuals.alpha = 0;
    }

    void ButtonClicked(DialogItem item)
    {
        HideDialogues();
        GalaController.SlapProgress(item.Value);
    }
}