namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal interface ISimulatedTask
{
    void OnEntry();

    void OnChildEntry(int index);

    void OnComplete();

    void OnChildComplete(int index);

    void Simulate(LevelSimulation simulation);

    string Serialize();

    void Deserialize(string data);
}
