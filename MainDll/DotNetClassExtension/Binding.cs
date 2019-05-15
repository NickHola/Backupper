using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Main
{
    public static partial class DotNetClassExtension
    {
        public static Binding Clona(this Binding bindDaClonare)
        {
            Binding BindClonato = new Binding()
            {
                AsyncState = bindDaClonare.AsyncState,
                BindingGroupName = bindDaClonare.BindingGroupName,
                BindsDirectlyToSource = bindDaClonare.BindsDirectlyToSource,
                Converter = bindDaClonare.Converter,
                ConverterCulture = bindDaClonare.ConverterCulture,
                ConverterParameter = bindDaClonare.ConverterParameter,
                Delay = bindDaClonare.Delay,
                FallbackValue = bindDaClonare.FallbackValue,
                IsAsync = bindDaClonare.IsAsync,
                Mode = bindDaClonare.Mode,
                NotifyOnSourceUpdated = bindDaClonare.NotifyOnSourceUpdated,
                NotifyOnTargetUpdated = bindDaClonare.NotifyOnTargetUpdated,
                NotifyOnValidationError = bindDaClonare.NotifyOnValidationError,
                Path = bindDaClonare.Path,
                StringFormat = bindDaClonare.StringFormat,
                TargetNullValue = bindDaClonare.TargetNullValue,
                UpdateSourceExceptionFilter = bindDaClonare.UpdateSourceExceptionFilter,
                UpdateSourceTrigger = bindDaClonare.UpdateSourceTrigger,
                ValidatesOnDataErrors = bindDaClonare.ValidatesOnDataErrors,
                ValidatesOnExceptions = bindDaClonare.ValidatesOnExceptions,
                ValidatesOnNotifyDataErrors = bindDaClonare.ValidatesOnNotifyDataErrors,
                XPath = bindDaClonare.XPath
            };

            if (bindDaClonare.RelativeSource != null)
                BindClonato.RelativeSource = bindDaClonare.RelativeSource;
            else if (bindDaClonare.Source != null)
                BindClonato.Source = bindDaClonare.Source;
            else if (bindDaClonare.ElementName != null)
                BindClonato.ElementName = bindDaClonare.ElementName;

            foreach (var validationRule in bindDaClonare.ValidationRules)
                BindClonato.ValidationRules.Add(validationRule);

            return BindClonato;
        }
    }
}
