using UnityEngine;

public class Sample : MonoBehaviour {
    
    public string publicField;
    
    [ReadOnly]
    public string readOnlyField;

    [ReadOnly(EReadOnlyType.FULLY_DISABLED)]
    public bool fullReadOnly;
    
    [ReadOnly(EReadOnlyType.EDITABLE_RUNTIME)]
    public bool editableRuntime;
    
    [ReadOnly(EReadOnlyType.EDITABLE_EDITOR)]
    public bool editableEditor;
}
