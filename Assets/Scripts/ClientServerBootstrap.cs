[UnityEngine.Scripting.Preserve]
public class ClientServerBootstrap : Unity.NetCode.ClientServerBootstrap
{
        public override bool Initialize(string defaultWorldName)
        {
                CreateLocalWorld(defaultWorldName);
                return true;
        }
}