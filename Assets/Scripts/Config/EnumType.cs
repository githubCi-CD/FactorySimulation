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
        SHIPPING,
        NONE
    }

    public enum IngredientType
    {
        ACTIVE_LIQUID = 1,
        NMP = 2,
        NEGATIVE_ELECTRODE = 3,
        ELECTROLYTIC = 4
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

    public enum CCTVBtn_type
    {
        LEFT,
        RIGHT,
        UP,
        DOWN,
        ZOOM_IN,
        ZOOM_OUT
    }

    public enum statisticType
    {
        NONE,
        PRODUCT_START_COUNT,
        PRODUCT_TEST_SUCCESS_COUNT,
        PRODUCT_TEST_FAIL_COUNT,
        MATERIAL_USAGE_ACTIVE_LIQUID,
        MATERIAL_USAGE_NMP,
        MATERIAL_USAGE_NEGATIVE_ELECTRODE,
        MATERIAL_USAGE_ELECTROLYTIC
    }

}
