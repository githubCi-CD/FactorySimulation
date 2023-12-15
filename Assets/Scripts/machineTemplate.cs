using Assets.Scripts.Config;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public interface machineInterface
    {
        public bool LoadingMachine();
        public bool ProcessMachine();
        public bool CompleteMachine();
    }
}
