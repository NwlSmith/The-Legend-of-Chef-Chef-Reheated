using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour {

    public static MessageManager instance;

    /*
     * 00. no_energy
     * 01. rats_escaped
     * 02. found_key
     * 03. carrot_message
     *
     * 04. press_close
     * 05. press_open
     * 06. press_push
     * 07. press_unlock
     * 08. door_locked
     * 09. go_inside
     * 
     * 10. need_weapon
     * 11. kitchen_cleared
     * 12. get_to_exit

     * priorities:
     * play temporary story messages
     * if you enter an interractables collision area, unless the player is making a mistake,
     *      like being out of energy, then turn on the continuous message
     * otherwise, play constant story messages
     * 
     */

    private Message[] messages = new Message[13];
    private List<Message> messages_displaying = new List<Message>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        CompileMessages();
    }

    private void CompileMessages()
    {
        for (int i = 0; i < messages.Length; i++)
        {
            Message temp_message = transform.GetChild(i).GetComponent<Message>();
            if (temp_message == null)
            {
                Debug.Log("Error: This child of MessageUIdoes not exist, i = " + i);
                return;
            }
            temp_message.img_num = i;
            messages[i] = temp_message;
        }
    }

    public void StartDisplay(int img_num)
    {
        if (img_num >= messages.Length)
        {
            Debug.Log("Error: Message " + img_num + " does not exist. Aborting call to StartDisplay.");
            return;
        }

        // retrieve the new message
        Message new_message = messages[img_num];
        new_message.StartDisplay();

        if (messages_displaying.Count > 0)
            messages_displaying[0].Enable(false);

        // insert it in the priority list at proper place
        int new_index = InsertMessageInPriority(new_message);

        messages_displaying[0].Enable(true);

        if (new_index == 0)
        {
            // if it is now the new first element, then enable the image
            new_message.Enable(true);
            // if there were a message that was displaying before the new message, then disable it
            if (messages_displaying.Count > 1)
                messages_displaying[1].Enable(false);
        }

        /* 
         * 
         * Have messages only handle their ALPHA channel, the message manager will determine if they are enabled or not.
         * makes code in messages much simpler
         * Manager will recieve startdisplay, which will tell the messages to startdisplay.
         *      It will place it in the list. If it is now the first element in the list, it will enable it
         *      if there is something behind the new message and it is enabled, then disable it.
         * if manager gets stopdisplay, it will check if it is being told to stop the top message.
         *      If yes, it will disable its img, remove it from the front of the list, and, if there is a next message, it will enable that img.
         *      then it will tell the removed message to stopdisplay
         * 
         */
    }

    private int InsertMessageInPriority(Message new_message)
    {
        for (int i = 0; i < messages_displaying.Count; i++)
        {
            if (new_message.priority < messages_displaying[i].priority)
            {
                messages_displaying.Insert(i, new_message);
                return i;
            }
        }
        messages_displaying.Insert(messages_displaying.Count, new_message);
        return messages_displaying.Count;
    }

    public void StopDisplay(int img_num)
    {
        if (img_num >= messages.Length)
        {
            Debug.Log("Error: Message " + img_num + " does not exist. Aborting call to StopDisplay.");
            return;
        }

        // retrieve the new message and tell it to stop
        Message removed_message = messages[img_num];
        removed_message.StopDisplay();

        int rem_index = messages_displaying.IndexOf(removed_message);

        if (rem_index < 0)
        {
            Debug.Log("Error: Message " + img_num + " was not displaying. Aborting call to StopDisplay.");
            return;
        }
        // if the message is the first one, and there is a second one, enable the second one
        else if (rem_index == 0 && messages_displaying.Count > 1)
            messages_displaying[1].Enable(true);
        messages_displaying.RemoveAt(rem_index);
    }
}
