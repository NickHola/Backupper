using System;
using Newtonsoft.Json;
using Main.Logs;
using Main.Validations;

namespace Main.Www
{
    public class ConfigBase : Configs.ConfigBase
    {

        UInt16 maxParallelStreams, defaultTimeoutSec;
        UInt32 maxKBSec;
        int sleepAfterOneOperation;
        byte maxItemInStatisticsQueue, checkKBSecInterval;

        [JsonProperty]
        internal UInt16 MaxParallelStreams
        {

            get { return maxParallelStreams; }
            set
            {
                Validation.CtrlValue(value);
                if (value == 0)
                {
                    value = 1;
                    Log.main.Add(new Mess(LogType.Warn, Log.main.warnUserText, "ricevuto value a 0, valore minimo 1"));
                }
                maxParallelStreams = value;
            }
        }
        ///<summary> 0 significa velocità illimitata </summary>
        [JsonProperty]
        internal UInt32 MaxKBSec
        {
            get { return maxKBSec; }
            set
            {
                Validation.CtrlValue(value);
                maxKBSec = value;
            }
        }
        [JsonProperty]
        internal UInt16 DefaultTimeoutSec
        {
            get { return defaultTimeoutSec; }
            set
            {
                Validation.CtrlValue(value);
                if (value == 0)
                {
                    Log.main.Add(new Mess(LogType.Warn, Log.main.warnUserText, "ricevuto valore value:<" + value + ">, valore minimo 1 impostato"));
                    value = 1;
                }
                defaultTimeoutSec = value;
            }
        }
        [JsonProperty]
        internal byte MaxItemInStatisticsQueue
        {
            get { return maxItemInStatisticsQueue; }
            set
            {
                Validation.CtrlValue(value);
                if (value < 10)
                {
                    value = 10;
                    Log.main.Add(new Mess(LogType.Warn, Log.main.warnUserText, "ricevuto valore value:<" + value + ">, valore minimo 10 impostato"));
                }
                maxItemInStatisticsQueue = value;
            }
        }
        [JsonProperty]
        internal byte CheckKBSecInterval  //In seconds
        {
            get { return checkKBSecInterval; }
            set
            {
                Validation.CtrlValue(value);
                if (value < 8)
                {
                    value = 8;
                    Log.main.Add(new Mess(LogType.Warn, Log.main.warnUserText, "ricevuto valore value:<" + value + ">, valore minimo 8 impostato"));
                }
                checkKBSecInterval = value;
            }
        }
        [JsonProperty]
        internal int SleepAfterOneOperationMs  //In seconds
        {
            get { return sleepAfterOneOperation; }
            set
            {
                Validation.CtrlValue(value);
                sleepAfterOneOperation = value;
            }
        }

        [JsonConstructor]
        internal ConfigBase(string savableName) : base(savableName, distinguiUtente: false) //JsonConstructor: I parametri obbligatori devono avere lo stesso nome delle variabili/proprietà
        {
            MaxParallelStreams = 1;
            MaxKBSec = 0;
            DefaultTimeoutSec = 20;
            MaxItemInStatisticsQueue = 20;
            CheckKBSecInterval = 10;
            SleepAfterOneOperationMs = 0;
        }
    }
}
