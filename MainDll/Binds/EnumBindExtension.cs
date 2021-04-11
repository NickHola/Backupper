using Main.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Main.Binds
{
    public class EnumBindExtension : MarkupExtension
    {

        Type _enumType;

        public Type EnumType
        {
            get { return this._enumType; }
            set
            {
                if (value != this._enumType)
                {
                    if (value != null)
                    {
                        Type enumType = Nullable.GetUnderlyingType(value) ?? value;
                        if (!enumType.IsEnum) throw new ArgumentException(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, "Type must be for an Enum.")));
                    }
                    this._enumType = value;
                }
            }
        }

        //Utilizzo custom property  ItemsSource="{Binding Source={local:EnumBinding {x:Type local:NOME_ENUM}, Maiuscola=True}}"
        //Private _maiuscola As Boolean  
        //Public Property Maiuscola As Boolean
        //    Get
        //        Return Me._maiuscola
        //    End Get

        //    Set(ByVal value As Boolean)
        //        If value <> Me._maiuscola Then
        //            Me._maiuscola = value
        //        End If
        //    End Set
        //End Property

        public EnumBindExtension() { }

        public EnumBindExtension(Type enumType)
        {
            this.EnumType = enumType;
        }

        public EnumBindExtension(Type enumType, bool nullable)
        {
            this.EnumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (this._enumType == null) throw new InvalidOperationException(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, "The EnumType must be specified.")));
            Type actualEnumType = Nullable.GetUnderlyingType(this._enumType) ?? this._enumType;
            Array enumValues = Enum.GetValues(actualEnumType);

            if (actualEnumType == this._enumType) return enumValues;

            Array tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
            enumValues.CopyTo(tempArray, 1);
            return tempArray;
        }
    }
}
