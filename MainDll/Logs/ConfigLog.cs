﻿using System;
using Newtonsoft.Json;
using Main.Logs;
using Main.FSes;
using Main.Validations;

namespace Main.Logs2
{
    [Serializable]
    internal class ConfigLog : Configs.ConfigBase
    {
        private string percorsoFile, nomeFile, estensioneFile;
        private byte numFile;
        private UInt32 dimFile;

        [JsonProperty]
        internal string PercorsoFile
        {
            get { return percorsoFile; }
            set
            {

                Validation.CtrlValue(value);
                if (FS. ValidaPercorsoFile(value, false, out _) == false)
                    throw new Exception(Excep.ScriviLogInEx(new Mess(LogType._Nothing, ""))); //Viene già loggato in ValidaPercorsoFile

                percorsoFile = value;
            }
        }
        [JsonProperty]
        internal string NomeFile
        {
            get { return nomeFile; }
            set
            {
                Validation.CtrlValue(value);
                nomeFile = value;
            }
        }
        [JsonProperty]
        internal string EstensioneFile
        {
            get { return estensioneFile; }
            set
            {
                Validation.CtrlValue(value);
                if (value.Left(1) != ".") value = "." + value;
                estensioneFile = value;
            }
        }
        [JsonProperty]
        internal byte NumFile
        {
            get { return numFile; }
            set
            {
                Validation.CtrlValue(value, ctrlVoid: false);

                if (value < 2)
                { numFile = 2; }
                else
                { numFile = value; }
            }
        }
        [JsonProperty]
        internal UInt32 DimFile
        {
            get { return dimFile; }
            set
            {
                Validation.CtrlValue(value, ctrlVoid: false);
                if (value < 300000)
                    dimFile = 300000;
                else if (value > 5000000)
                    dimFile = 5000000;
                else
                    dimFile = value;
            }
        }

        //Friend Sub New() 'ATTENZIONE deve essere usato solo dal deserializzatore
        //    MyBase.New("")
        //End Sub

        [JsonConstructor]
        internal ConfigLog(string savableName) : base(savableName, distinguiUtente: false)
        { //JsonConstructor: I parametri obbligatori devono avere lo stesso nome delle variabili/proprietà
            PercorsoFile = Str.relativo; //Reflection.Assembly.GetExecutingAssembly().Location & "\"
            NomeFile = Str.relativo;
            EstensioneFile = ".log";
            NumFile = 2;
            DimFile = 2097152;  //2MB
        }

        internal string DammiPercNomeFile(bool conPerc = true, bool conNome = true, bool conEsten = true)
        {
            string percEnome, tmpStr;
            percEnome = tmpStr = "";

            if (conPerc == true)
            {
                if (PercorsoFile == "") return "";

                if (PercorsoFile == Str.relativo)
                {
                    App.GetAppFullPath(out _, path: out tmpStr, name: out _);
                    percEnome += tmpStr;
                }
                else
                {
                    percEnome += this.PercorsoFile;
                }
            }

            if (conNome == true)
            {
                if (NomeFile == "") return "";
                if (NomeFile == Str.relativo)
                {
                    App.GetAppFullPath(out _, out _, name: out tmpStr, removeExe: true);
                    //percEnome += tmpStr.RemoveFinal(".exe");
                    percEnome += tmpStr;
                }
                else
                {
                    percEnome += this.NomeFile;
                }
                if (conEsten == true) percEnome += this.EstensioneFile;
            }
            return percEnome;
        }
    }
}
