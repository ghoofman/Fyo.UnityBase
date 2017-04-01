﻿using System;
using System.Collections.Generic;

namespace UnityEngine.EventSystems {
    [AddComponentMenu("Socket Gamepad Input Module")]
    public class SocketGamepadInputModule : BaseInputModule {
        public SocketGamepad Gamepad;
        public Dictionary<string, float> InputMap = new Dictionary<string, float>();

        private float m_NextAction = 0.0f;
        
        protected SocketGamepadInputModule() : base() {
        }

        [SerializeField]
        private float m_InputActionsPerSecond = 4;

        [SerializeField]
        private bool m_AllowActivationOnMobileDevice;

        public bool allowActivationOnMobileDevice {
            get {
                return m_AllowActivationOnMobileDevice;
            }
            set {
                m_AllowActivationOnMobileDevice = value;
            }
        }

        public float inputActionsPerSecond {
            get {
                return m_InputActionsPerSecond;
            }
            set {
                m_InputActionsPerSecond = value;
            }
        }

        public override void UpdateModule() {
        }

        public override bool IsModuleSupported() {
            return true;
        }

        public override bool ShouldActivateModule() {
            if (Gamepad == null)
                Gamepad = GameObject.FindObjectOfType<SocketGamepad>();

            return (base.ShouldActivateModule() || Gamepad != null);
        }

        public override void ActivateModule() {
            base.ActivateModule();

            var toSelect = eventSystem.currentSelectedGameObject;
            if (toSelect == null)
                toSelect = eventSystem.firstSelectedGameObject;

            eventSystem.SetSelectedGameObject(toSelect, GetBaseEventData());
        }

        public override void DeactivateModule() {
            base.DeactivateModule();
        }


        public override void Process() {
            bool usedEvent = SendUpdateEventToSelectedObject();

            if (eventSystem.sendNavigationEvents) {
                if (!usedEvent)
                    usedEvent |= SendMoveEventToSelectedObject();

                if (!usedEvent)
                    SendSubmitEventToSelectedObject();
            }
        }

        /// <summary>
        /// Process submit keys.
        /// </summary>
        private bool SendSubmitEventToSelectedObject() {
            if (eventSystem.currentSelectedGameObject == null)
                return false;

            var data = GetBaseEventData();
            if (Gamepad.InputData.GetField("Horizonal").n > 0.0f)
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.submitHandler);
            
            if (Gamepad.InputData.GetField("Vertical").n > 0.0f)
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.cancelHandler);
            return data.used;
        }
                
        /// <summary>
        /// Process keyboard events.
        /// </summary>
        private bool SendMoveEventToSelectedObject() {
            float time = Time.unscaledTime;

            if (Mathf.Approximately(Gamepad.InputData.GetField("Horizontal").f, 0.0f) && 
                Mathf.Approximately(Gamepad.InputData.GetField("Vertical").f, 0.0f) && 
                !Gamepad.InputData.GetField("Select").b && 
                !Gamepad.InputData.GetField("Cancel").b) {
                //Clear wait time if everything is released
                m_NextAction = 0.0f;
            }

            if (!(time >= m_NextAction))
                return false;
            
            // Debug.Log(m_ProcessingEvent.rawType + " axis:" + m_AllowAxisEvents + " value:" + "(" + x + "," + y + ")");
            var axisEventData = GetAxisEventData(Gamepad.InputData.GetField("Horizontal").f, Gamepad.InputData.GetField("Vertical").f, 0.0f);

            if (!Mathf.Approximately(axisEventData.moveVector.x, 0f)
                || !Mathf.Approximately(axisEventData.moveVector.y, 0f)) {
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.updateSelectedHandler);
            }
            m_NextAction = time + (1f / m_InputActionsPerSecond);
            return axisEventData.used;
        }
        
        private bool SendUpdateEventToSelectedObject() {
            if (eventSystem.currentSelectedGameObject == null)
                return false;

            var data = GetBaseEventData();
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
            return data.used;
        }
    }
}