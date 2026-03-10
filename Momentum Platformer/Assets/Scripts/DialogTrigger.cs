using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public string startNode;
    // Start is called before the first frame update

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            DialogManager.instance.LoadDialog(this);
            Debug.Log(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        DialogManager.instance.dialogReady = false;
    }

}

