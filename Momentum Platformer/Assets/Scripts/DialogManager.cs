using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Events;
using Yarn.Unity;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance;
    public DialogueRunner dialogueRunner;
    public static UnityAction DialogStart, DialogOver;

    public bool dialogReady, dialogStarted;
    public bool hasDiamond = false;

    public int timeLeft = 10;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Update()
    {
        
    }

    public void LoadDialog(DialogTrigger dTrigger)
    {
        Debug.Log(dTrigger);
        //Set the start node for the dialog runner
        dialogueRunner.startNode = dTrigger.startNode;
        //Put the portrait in the dialog box
        Debug.Log(dTrigger.startNode);
        //the dialog is read to view
        dialogReady = true;
    }

    public void StartDialog()
    {
        Debug.Log("Dialogue Ready: " + dialogReady + " dialogueStarted: " + dialogueRunner.IsDialogueRunning);
        if (dialogReady && !dialogueRunner.IsDialogueRunning)
        {

            // just to be careful make sure the runner is stopped
            dialogueRunner.Stop();

            dialogueRunner.StartDialogue("NPC_Red");
            if (DialogStart != null)
                DialogStart();

            dialogStarted = true;
        }
    }
    public void OnDialogOver()
    {
        if (DialogStart != null)
            DialogOver();
        dialogStarted = false;

    }
    public void GotDiamond()
    {
        //Set the Diamond variable in Yarnspinner
        hasDiamond = true;
        dialogueRunner.VariableStorage.SetValue("$hasDiamond", true);
    }


}
