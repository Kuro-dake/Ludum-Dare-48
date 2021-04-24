using System.Collections;
using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

[System.Serializable]
public class EntityList<T> : List<T> where T : Entity, new()
{

    public EntityList() : base() { }

    public EntityList(YamlSequenceNode sequence, YamlMappingNode parent = null)
    {
        AddEntitiesFromNode(sequence, parent);
    }

    public void AddEntitiesFromNode(YamlSequenceNode sequence, YamlMappingNode parent = null)
    {
        foreach (YamlMappingNode n in sequence)
        {
            T to_add = Entity.Create<T>(n, parent);
            to_add.CheckIntegrity();
            Add(to_add);
        }
    }

}
