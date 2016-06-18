using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(TerrainController))]
public class TerrainEditor : Editor {
    public override void OnInspectorGUI() {
        //base.OnInspectorGUI();
        TerrainController controller = (TerrainController) target;
        if (DrawDefaultInspector()) {
            if (controller.UpdateInRealTime) {
                controller.DebugChunk();
            }
        }
    }
}
