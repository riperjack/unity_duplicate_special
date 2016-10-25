using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class DuplicateSpecial : EditorWindow
{
    int count;
    Vector3 relOffset;
    Vector3 conOffset;

    // object to duplicate
    GameObject target;
    Renderer rend;

    // wrapper object for duplicates
    GameObject wrapper;

    [MenuItem("Plugins/Duplicate special")]
    static void Init()
    {
        DuplicateSpecial window = (DuplicateSpecial)GetWindow(typeof(DuplicateSpecial), true, "Duplicate special");
        window.Show();
    }

    void OnGUI()
    {
        target = Selection.activeGameObject;
        if (target == null)
        {
            ErrorMsg("Please select an object to duplicate from Hierarchy View");
            return;
        }

        rend = target.GetComponentInChildren<Renderer>();

        EditorGUI.BeginChangeCheck();

        count = EditorGUILayout.IntField("Count", count);
        relOffset = EditorGUILayout.Vector3Field("Relative offset", relOffset);
        conOffset = EditorGUILayout.Vector3Field("Constant offset", conOffset);

        if (EditorGUI.EndChangeCheck())
        {
            Rollback(); // destroy wrapper from previous changes check tick before creating a new one
            Apply();
        }

        if (GUILayout.Button("Ok")) Close();
        if (GUILayout.Button("Cancel"))
        {
            Rollback();
            Close();
        }
    }

    private void Apply()
    {
        wrapper = new GameObject(target.name + "_duplicates_" + count);

        for (int i = 1; i <= count; i++)
        {
            GameObject clone = Instantiate(target, target.transform.position, target.transform.rotation) as GameObject;

            Vector3 offset = new Vector3(rend.bounds.size.x * relOffset.x + conOffset.x,
                                        rend.bounds.size.y * relOffset.y + conOffset.y,
                                        rend.bounds.size.z * relOffset.z + conOffset.z)
                                        * i;
            clone.transform.Translate(offset);
            clone.transform.SetParent(wrapper.transform);
        }
    }

    private void Rollback()
    {
        DestroyImmediate(wrapper);
    }

    private void ErrorMsg(string msg)
    {
        EditorGUILayout.LabelField(msg);
    }

    void OnDestroy()
    {
        if (count == 0) Rollback();
    }
}
