using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadManager : MonoBehaviour
{
    private static readonly List<Action> executable_actions = new List<Action>();   //will store actions that need to be executed on main thread
    private static readonly List<Action> copied = new List<Action>();
    private static bool execute = false;

    private void FixedUpdate()
    {
        UpdateMainThread();
    }

    public static void UpdateMainThread()
    {
        if (execute)                                                                //if there is an action that needs to be executed
        {
            copied.Clear();                                                         //clear list in order to initialize it properly
            lock (executable_actions)
            {
                copied.AddRange(executable_actions);                                //add actions to the end of list (create a copy of actions that needs to be executed)
                executable_actions.Clear();
                execute = false;
            }

            for (int i = 0; i < copied.Count; i++)
            {
                copied[i]();                                                        //execute every action that has been stored
            }
        }
    }

    public static void ExecuteOnMainThread(Action action)                           //set given action to be executed on the main thread
    {
        if (action == null)
        {
            Console.WriteLine("No action to execute on main thread...");
            return;
        }

        lock (executable_actions)                                                   //execute above statement-block and then release the lock
        {
            executable_actions.Add(action);                                         //add the action, that needs to be executed, to the list
            execute = true;
        }
    }
}
