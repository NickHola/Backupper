﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static Main.Validations.Validation;
using Main;
using Main.Serializes;
using Main.Validations;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace Backupper
{
    [Serializable]
    public class RESTsettingM : INotifyPropertyChanged, IValidation
    {
        #region "IValidation"
        private bool isValid;
        public bool IsValid
        { //Implements IValidation.isValid
            get
            {
                return isValid;
            }

            private set
            {
                isValid = value;
                OnPropertyChanged();
            }
        }

        public ValidationResult ValidMySelf(string propName = "")
        {
            bool result = true;
            string errDesc = "";

            if (propName == "" || propName == nameof(RootAddress))
            {
                result = ValidateRootAddress(this.RootAddress, out errDesc);
            }

            if (propName == "" || propName == nameof(RoutePrefix))
            {
                result = ValidateRoutePrefix(this.RoutePrefix, out errDesc);
            }
            if (propName == "" || propName == nameof(RouteOfGetBackups))
            {
                result = ValidateRouteOfGetBackups(this.RouteOfGetBackups, out errDesc);
            }
            if (propName == "" || propName == nameof(RouteOfPutBackups))
            {
                result = ValidateRouteOfPutBackups(this.RouteOfPutBackups, out errDesc);
            }
            if (propName == "")
            {
                this.IsValid = result;
            }
            else
            {
                if (result == false) this.IsValid = result;
            }

            return new ValidationResult(result, errDesc);
        }
        #endregion

        private string rootAddress, routePrefix, routeOfGetBackups, routeOfPutBackups;

        public string RootAddress
        {
            get
            {
                return rootAddress;
            }
            set
            {
                CtrlValue(value, ctrlVoid: false);
                if (ValidateRootAddress(value, out string errMess) == false) throw new Exception(errMess);
                rootAddress = value;
                OnPropertyChanged();
            }
        }
        public string RoutePrefix
        {
            get
            {
                return routePrefix;
            }
            set
            {
                CtrlValue(value, ctrlVoid: false);
                if (ValidateRoutePrefix(value, out string errMess) == false) throw new Exception(errMess);
                routePrefix = value;
                OnPropertyChanged();
            }
        }
        public string RouteOfGetBackups
        {
            get
            {
                return routeOfGetBackups;
            }
            set
            {
                CtrlValue(value, ctrlVoid: false);
                if (ValidateRouteOfGetBackups(value, out string errMess) == false) throw new Exception(errMess);
                routeOfGetBackups = value;
                OnPropertyChanged();
            }
        }
        public string RouteOfPutBackups
        {
            get
            {
                return routeOfPutBackups;
            }
            set
            {
                CtrlValue(value, ctrlVoid: false);
                if (ValidateRouteOfPutBackups(value, out string errMess) == false) throw new Exception(errMess);
                routeOfPutBackups = value;
                OnPropertyChanged();
            }
        }

        public RESTsettingM()
        {
            rootAddress = "";
            routePrefix = "";
            routeOfGetBackups = "";
            routeOfPutBackups = "";
        }

        private bool ValidateRootAddress(string value, out string errMessage)
        {
            errMessage = "";
            if (value == "") return true;
            if (Regex.IsMatch(value, @"^http:\/\/[^:\/\\]{3,}[:]{1}?[0-9]{1,}\/$") == false)
            {
                errMessage = "Wrong syntax for Root Address";
                return false;
            }
            return true;
        }
        private bool ValidateRoutePrefix(string value, out string errMessage)
        {
            errMessage = "";
            if (value == "") return true;
            if (Regex.IsMatch(value, @"^[^\/][^:\/\\]{2,}\/$") == false)
            {
                errMessage = "Wrong syntax for Root Prefix";
                return false;
            }
            return true;
        }
        private bool ValidateRouteOfGetBackups(string value, out string errMessage)
        {
            errMessage = "";
            if (value == "") return true;
            if (Regex.IsMatch(value, @"^[^\/][^:\/\\]{2,}\/$") == false)
            {
                errMessage = "Wrong syntax for Route of Get Backups";
                return false;
            }
            return true;
        }
        private bool ValidateRouteOfPutBackups(string value, out string errMessage)
        {
            errMessage = "";
            if (value == "") return true;
            if (Regex.IsMatch(value, @"^[^\/][^:\/\\]{2,}\/$") == false)
            {
                errMessage = "Wrong syntax for Route of Put Backups";
                return false;
            }
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
