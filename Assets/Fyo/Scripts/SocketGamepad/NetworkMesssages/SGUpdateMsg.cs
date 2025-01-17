﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fyo {
    /// <summary>
    /// Common update message sent to and from Controllers
    /// Inbound traffic from controllers defines the Input values
    /// 
    /// </summary>
    public class SGUpdateMsg : JSONObject {
        public int SGID = -1;
        public string MessageType = "";
        public JSONObject Data = new JSONObject();

        private void Setup() {
            AddField("SGID", -1);
            AddField("MessageType", "");
            AddField("data", Data);
        }

        public SGUpdateMsg() : base() {
            Setup();
        }

        public SGUpdateMsg(SocketGamepad gamepad) : base() {
            Setup();
            SGID = gamepad.SGID;
            MessageType = "input";
            Data = gamepad.InputData;
            Serialize();
        }

        public SGUpdateMsg(JSONObject clone) : base() {
            Setup();
            if (clone.HasField("SGID"))
                clone.GetField(ref SGID, "SGID");
            if (clone.HasField("MessageType"))
                clone.GetField(ref MessageType, "MessageType");
            if (clone.HasField("data"))
                Data = clone["data"];
            Serialize();
        }

        public void Serialize() {
            SetField("SGID", SGID);
            SetField("MessageType", CreateStringObject(MessageType));
            SetField("data", Data);
        }

        void Deserialize() {
            SGID = int.Parse(GetField("SGID").str);
            MessageType = GetField("MessageType").str;
            Data = this["data"];
        }
    }
}
