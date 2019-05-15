using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Main.Binds
{
    [Serializable]
    public class BindList<T> : BindingList<T>
    {
        public bool oggConValidazione;

        public event EventHandler<BindListEventArgs> BeforeInsertItem;
        public event EventHandler<BindListEventArgs> AfterInsertItem;
        public event EventHandler<BindListEventArgs> BeforeSetItem;
        public event EventHandler<BindListEventArgs> AfterSetItem;
        public event EventHandler<BindListEventArgs> BeforeRemoveItem;
        public event EventHandler<BindListEventArgs> AfterRemoveItem;
        public event EventHandler<BindListEventArgs> ItemNumberChanged;

        public event EventHandler<PropertyChangedEventArgs> ObjectTPropertyChanged;

        [JsonConstructor]
        private BindList()
        { this.oggConValidazione = false; }

        public BindList(bool oggConValidazione = false)
        { this.oggConValidazione = oggConValidazione; }

        protected override void InsertItem(Int32 index, T item)
        {
            BindListEventArgs args = new BindListEventArgs(index, item);
            BeforeInsertItem?.Invoke(this, args); //?.Invoke invice della parentesi tonda diretta poichè se nessuno ha sottoscritto l'evento non va in eccezione
            if (args.interrompiOperazione == true) return;

            base.InsertItem(index, item);

            AfterInsertItem?.Invoke(this, args); //?.Invoke invice della parentesi tonda diretta poichè se nessuno ha sottoscritto l'evento non va in eccezione
            ItemNumberChanged?.Invoke(this, args);

            var PropertyChangedEH = new PropertyChangedEventHandler(CambioPropOggT); //Se il valore di una delle proprietà dei vari oggetti contenuti nella lista cambia, allora intercetto il cambiamento...
            typeof(T).GetEvent("PropertyChanged").AddEventHandler(item, PropertyChangedEH); //...e richiamo il metodo CambioPropOggT che a sua volta scatena l'evento CambioProprietàOggT
        }

        protected override void SetItem(Int32 index, T item)
        {
            var args = new BindListEventArgs(index, item);
            BeforeSetItem?.Invoke(this, args); //?.Invoke invice della parentesi tonda diretta poichè se nessuno ha sottoscritto l'evento non va in eccezione
            if (args.interrompiOperazione == true) return;

            base.SetItem(index, item);

            AfterSetItem?.Invoke(this, args); //?.Invoke invice della parentesi tonda diretta poichè se nessuno ha sottoscritto l'evento non va in eccezione
        }

        protected override void RemoveItem(Int32 index)
        {
            var args = new BindListEventArgs(index, Items[index]);
            BeforeRemoveItem?.Invoke(this, args); //?.Invoke invice della parentesi tonda diretta poichè se nessuno ha sottoscritto l'evento non va in eccezione
            if (args.interrompiOperazione == true) return;

            var PropChangedOggT = new PropertyChangedEventHandler(CambioPropOggT);
            typeof(T).GetEvent("PropertyChanged").RemoveEventHandler(Items[index], PropChangedOggT);

            base.RemoveItem(index);

            AfterRemoveItem?.Invoke(this, args); //?.Invoke invice della parentesi tonda diretta poichè se nessuno ha sottoscritto l'evento non va in eccezione
            ItemNumberChanged?.Invoke(this, args);
        }

        private void CambioPropOggT(object sender, PropertyChangedEventArgs args)
        {
            ObjectTPropertyChanged?.Invoke(this, args); //?.Invoke invice della parentesi tonda diretta poichè se nessuno ha sottoscritto l'evento non va in eccezione
        }

        public void AddRange(IEnumerable<T> collection)
        {
            foreach (T tItem in collection)
            {
                Items.Add(tItem);
            }
        }

        public Int32 Find<TKey>(string property, TKey key)
        {
            // Check the properties for a property with the specified name.
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            PropertyDescriptor prop = properties.Find(property, true);

            // If there is not a match, return -1 otherwise pass search to FindCore method.
            if (prop == null) return -1;
            return FindCore(prop, key);
        }

        protected override Int32 FindCore(PropertyDescriptor prop, object key)
        {
            //Get the property info for the specified property.
            PropertyInfo propInfo = typeof(T).GetProperty(prop.Name);
            T tItem;

            if (key != null)
            {
                //Loop through the items to see if the key
                // value matches the property value.
                for (int i = 0; i < Count - 1; i++)
                {
                    tItem = Items[i];
                    if (propInfo.GetValue(tItem, null).Equals(key)) return i;
                }
            }
            return -1;
        }

        protected override bool SupportsSearchingCore
        {
            get { return true; }
        }

    }
}
