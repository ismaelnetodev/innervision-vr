using UnityEngine;

[CreateAssetMenu(fileName = "ArtifactObject", menuName = "Scriptable Objects/ArtifactObject")]
public class ArtifactObject : ScriptableObject
{

    public string descricao = string.Empty;
    public Mesh mesh;

}
