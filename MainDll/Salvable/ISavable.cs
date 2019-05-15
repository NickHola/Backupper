using Main.Configs;
using Newtonsoft.Json;

namespace Main.Salvable
{
    public interface ISavable
    {
        [JsonIgnore] bool SavableMarkUser { get; }
        [JsonIgnore] bool SavableMarkPcName { get; }

        //[JsonIgnore] SaveLocation SavableSaveLocation { get; set; }  
        //[JsonIgnore] ISavable SavableParent { get; set; }

        string SavableName { get; set; }
        string SavableParentName { get; set; }
    }

    
}
