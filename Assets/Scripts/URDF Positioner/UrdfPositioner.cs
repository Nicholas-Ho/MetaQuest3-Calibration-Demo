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
        public UrdfGizmoPositioner gizmoPositioner = new UrdfGizmoPositioner();
        private PositionState state = PositionState.Ray;

        // Start is called before the first frame update
        void Start()
        {
            ArticulationBodiesEnabled bodiesEnabled;
            if (urdfModel.TryGetComponent(out bodiesEnabled)) {
                bodiesEnabled.SetEnabled(false);
            }
            rayPositioner.Initialise(urdfModel, StartGizmoState);
        }

        // Update is called once per frame
        void Update()
        {
            if (state == PositionState.Ray) {
                rayPositioner.Update();
            } else if (state == PositionState.Gizmo) {
                gizmoPositioner.Update();
            }
        }

        public void StartGizmoState(TransformData data) {
            state = PositionState.Gizmo;
            gizmoPositioner.Initialise(urdfModel, data, StartFixedState);
        }

        public void StartFixedState(TransformData data) {
            state = PositionState.Fixed;
            urdfModel.SetActive(true);
            urdfModel.transform.position = data.position;
            urdfModel.transform.rotation = data.rotation;

            ArticulationBodiesEnabled bodiesEnabled;
            if (urdfModel.TryGetComponent(out bodiesEnabled)) {
                bodiesEnabled.SetEnabled(true);
            }
        }
    }
}
