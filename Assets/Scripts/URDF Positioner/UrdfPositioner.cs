using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrdfPositioning {
    public delegate void TransformDataCallback(TransformData data);
    enum PositionState {
        Ray,
        Gizmo,
        Fixed
    }

    public class UrdfPositioner : MonoBehaviour
    {
        public GameObject urdfModel;
        public UrdfRayPositioner rayPositioner = new UrdfRayPositioner();
        private PositionState state = PositionState.Ray;

        // Start is called before the first frame update
        void Start()
        {
            rayPositioner.Initialise(urdfModel, StartGizmoState);
        }

        // Update is called once per frame
        void Update()
        {
            if (state == PositionState.Ray) {
                rayPositioner.Update();
            }
        }

        public void StartGizmoState(TransformData data) {
            StartFixedState(data);
        }

        public void StartFixedState(TransformData data) {
            state = PositionState.Fixed;
            urdfModel.SetActive(true);
            urdfModel.transform.position = data.position;
            urdfModel.transform.rotation = data.rotation;
        }
    }
}
