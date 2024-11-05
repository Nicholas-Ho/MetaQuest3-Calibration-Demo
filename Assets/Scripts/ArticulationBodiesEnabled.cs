using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction.Body.Input;
using UnityEngine;

public class ArticulationBodiesEnabled : MonoBehaviour
{
    private ArticulationBody[] bodies;
    private bool bodiesRetrieved = false;

    public void SetEnabled(bool setEnabled)
    {
        if (!bodiesRetrieved) {
            bodies = gameObject.GetComponentsInChildren<ArticulationBody>(true);
            bodiesRetrieved = true;
        }
        foreach (ArticulationBody body in bodies) {
            body.enabled = setEnabled;
        }
    }
}
