using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets.Scripts.Config
{
    public enum MachineType
    {
        MIXCOATING_MACHINE,
        PRESS_MACHINE,
        STACK_MACHINE,
        TEST_MACHINE,
        SHIPPING_TRUCK,
        UNKNOWN_ERROR
    }
    public enum ProcessType
    {
        LOADING,
        CONVEYOR,
        MIXCOATING,
        PRESSING,
        STACKING,
        TESTING,
        SHIPPING
    }

    public enum IngredientType
    {
        ACTIVE_LIQUID,
        NMP,
        NEGATIVE_ELECTRODE,
        ELECTROLYTIC
    }

    public enum MachineStatus
    {
        WAITING,
        LOADING,
        PROCESSING,
        UNLOADING
    }

    public enum ProcessResultStatus
    {
        NONE = -1,
        SUCESS = 1,
        FAIL = 0
    }

}
