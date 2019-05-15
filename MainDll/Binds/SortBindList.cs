using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Main.DataOre;
using Main.Serializes;
using Main.Thrs;
using Main.Validations;
using Main.Logs;

namespace Main.Binds
{
    /// <summary>
    /// SortableBindingList è una lista che supporta l'ordinamento su 1 o più proprietà della classe
    /// Dependencies:
    ///  - .NET 3.5 or higher
    /// </summary>
    [Serializable]
    public class SortBindList<T> : BindList<T>
    {
        [JsonProperty] private List<ListSortDescription> listaOrdinamenti;
        public string defaultValueOfNewT;

        public new event EventHandler<BindListEventArgs> BeforeInsertItem;
        public new event EventHandler<BindListEventArgs> AfterInsertItem;
        public new event EventHandler<BindListEventArgs> BeforeSetItem;
        public new event EventHandler<BindListEventArgs> AfterSetItem;
        public new event EventHandler<BindListEventArgs> BeforeRemoveItem;
        public new event EventHandler<BindListEventArgs> AfterRemoveItem;

        public event EventHandler<PropertyChangedEventArgs> ProprietàOggTCambiata;

        //<JsonConstructor> Public Sub New() 'ATTENZIONE: serve solamente per lo xaml non aggiungere codice
        //    Me.New(Nothing)
        //End Sub

        [JsonConstructor]
        private SortBindList() : this(default)
        { }

        public SortBindList(List<Ordinamenti> ordinamenti = null, bool oggConValidazione = true) : base(oggConValidazione: oggConValidazione)
        {
            if (typeof(ISortBindObj).IsAssignableFrom(typeof(T)) == false)
            { //Verifico se il tipo degli oggetti ha l'interfaccia ISortBindObj
                throw new Exception(Excep.ScriviLogInEx(new Mess(Tipi.ERR, Log.main.errUserText, "T di tipo:<" + typeof(T).Name + "> non implementa l'interfaccia ISortBindObj")));
            }

            if (ordinamenti == null) ordinamenti = new List<Ordinamenti> { new Ordinamenti("IndiceOrd", ListSortDirection.Ascending) };
            GeneraListaOrdinamenti(ordinamenti);

            T oggTmp = (T)Activator.CreateInstance(typeof(T));
            Serialize.SerializeInText(oggTmp, ref defaultValueOfNewT);
        }

        //Public Sub New(list As IEnumerable(Of T))
        //    MyBase.New(list.ToList())
        //End Sub

        private void GeneraListaOrdinamenti(List<Ordinamenti> ordinamenti)
        {
            listaOrdinamenti = new List<ListSortDescription>();

            foreach (Ordinamenti ordinamento in ordinamenti)
            {
                var proprieta = TypeDescriptor.GetProperties(typeof(T))[ordinamento.nomeProprietà];
                if (proprieta == null)
                {
                    Log.main.Add(new Mess(Tipi.ERR, Log.main.errUserText, "ordinamento.nomeProprietà:<" + ordinamento.nomeProprietà + "> non trovato nel tipo T, t.GetType:<" + typeof(T).Name + ">"));
                    continue;
                }
                listaOrdinamenti.Add(new ListSortDescription(proprieta, ordinamento.direzione));
            }
        }

        protected override void InsertItem(int index, T item)
        {

            string serialItem = ""; bool esitoSerial;
            esitoSerial = Serialize.SerializeInText(item, ref serialItem);

            var args = new BindListEventArgs(index, item); //ATTENZIONE:scateno l'evento BeforeInsertItem prima del MyBase.InsertItem(index, item), che ha sua volta scatena lo stesso evento nella classe da cui derivo...
            BeforeInsertItem?.Invoke(this, args);          //in caso di sottoscrizione dell'evento, verrà comunque intercettato(correttamente) solomente 1 volta l'evento di questa classe e non quello da cui derivo
            if (args.interrompiOperazione == true) return;

            if (esitoSerial == false || defaultValueOfNewT == serialItem) //Si verifica quando il DataGrid aggiunge un item vuoto alla lista
            { //Aggiunto da dataGrid poichè ha valori di default (non è sicuro al 100% ma molto probabile), quindi non faccio il riordina altrimenti la nuova colonna...
                base.InsertItem(index, item); //                            ...non sarebbe più l'ultima e quindi il datagrid andrebbe in edit su un oggetto già esistente ovviamente sbagliando
                //OnListChanged(New ListChangedEventArgs(ListChangedType.ItemAdded, index))
            }
            else
            {
                Items.Add(item); //A differenza di "base.InsertItem(index, item)" Non scatena l'aggiornamento dei Listener che fanno il binding sulla lista, mi serve aggiugere l'elemento, ordinare e poi segnalare ai listener
                Ordina();
            }

            AfterInsertItem?.Invoke(this, args); //?.Invoke invice della parentesi tonda diretta poichè se nessuno ha sottoscritto l'evento non va in eccezione

            PropertyChangedEventHandler PropChangedOggT = new PropertyChangedEventHandler(CambioProprietaOggT); //Se il valore di una delle proprietà dei vari oggetti contenuti nella lista cambia, allora intercetto il cambiamento...
            typeof(T).GetEvent("PropertyChanged").AddEventHandler(item, PropChangedOggT); //...e verifico se la prop. concorre all'ordinamento degli items
        }

        protected override void SetItem(int index, T item)
        {
            //MyBase.SetItem(index, item) //Commentato poichè scatena l'aggiornamento dei Listener che fanno il binding sulla lista, mi serve aggiornare l'elemento, ordinare e poi segnalare ai listener
            var args = new BindListEventArgs(index, item);
            BeforeSetItem?.Invoke(this, args); //?.Invoke invice della parentesi tonda diretta poichè se nessuno ha sottoscritto l'evento non va in eccezione
            if (args.interrompiOperazione == true) return;

            Items[index] = item;
            Ordina();

            AfterSetItem?.Invoke(this, args); //?.Invoke invice della parentesi tonda diretta poichè se nessuno ha sottoscritto l'evento non va in eccezione
        }

        protected override void RemoveItem(int index)
        {
            var args = new BindListEventArgs(index, Items[index]);
            BeforeRemoveItem?.Invoke(this, args); //?.Invoke invice della parentesi tonda diretta poichè se nessuno ha sottoscritto l'evento non va in eccezione
            if (args.interrompiOperazione == true) return;

            var PropertyChangedEH = new PropertyChangedEventHandler(CambioProprietaOggT);
            typeof(T).GetEvent("PropertyChanged").RemoveEventHandler(Items[index], PropertyChangedEH);

            base.RemoveItem(index);

            AfterRemoveItem?.Invoke(this, args); //?.Invoke invice della parentesi tonda diretta poichè se nessuno ha sottoscritto l'evento non va in eccezione
        }

        protected void Ordina(object oggT = null, bool updateListeners = true)
        {
            if (Thr.UIDispatcher.CheckAccess() == false)
            { //ATTENZIONE: deve stare fuori da Try poichè altrimenti all'esecuzione dell'Exit Sub eseguirebbe OnListChanged che è proprio quello che da problemi se eseguito con...
                Thr.UIDispatcher.Invoke(() => Ordina(oggT, updateListeners)); //... un thread diverso da quello della ui poichè scatena il riaggiornamento dei datagrid collegati alla SortBindList e da eccezione di crossThread.
                return;
            }

            try
            {
                if (listaOrdinamenti == null) return;
                if (oggT != null) return;               //&& oggT.saltaRiordino == true
                IOrderedEnumerable<T> orderedList = null;
                bool isPrimoOrdin = true;
                foreach (ListSortDescription ordinamento in listaOrdinamenti)
                {
                    PropertyDescriptor prop = ordinamento.PropertyDescriptor;
                    Type interfaceType = prop.PropertyType.GetInterface("IComparable");

                    if (interfaceType == null)
                    {
                        //Check if this is a Nullable(Of IComparable)
                        if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        { //Check if the type parameter implements IComparable
                            Type tType = prop.PropertyType.GetGenericArguments()[0];
                            interfaceType = tType.GetInterface("IComparable");
                        }
                    }

                    if (interfaceType == null)
                    {
                        //If the property type does not implement IComparable, let the user know.
                        throw new NotSupportedException(String.Format("Cannot sort by {0}. {1} does not implement IComparable.", prop.Name, prop.PropertyType.ToString()));
                    }

                    if (isPrimoOrdin)
                    {
                        orderedList = ordinamento.SortDirection == ListSortDirection.Ascending ? Items.OrderBy((x) => prop.GetValue(x)) : Items.OrderByDescending((x) => prop.GetValue(x));
                        isPrimoOrdin = false;
                    }
                    else
                    {
                        orderedList = ordinamento.SortDirection == ListSortDirection.Ascending ? orderedList.ThenBy((x) => prop.GetValue(x)) : orderedList.ThenByDescending((x) => prop.GetValue(x));
                    }
                }

                if (orderedList == null) return;

                //Sorting succeeded
                var result = orderedList.ToList();

                //Copy the sorted items back into the list.
                Items.Clear();
                foreach (T tItem in result)
                {
                    Items.Add(tItem);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(Excep.ScriviLogInEx(new Mess(Tipi.ERR, Log.main.errUserText, "Eccezione ex.mess:<" + ex.Message + ">")));
            }
            finally
            {
                OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1)); //Va comunque fatto il reset e non il ListChangedType.ItemAdded o altri poichè dopo il riordino, l'ultimo elemento non è detto...
                //                                                                   ...che sia il nuovo e quindi i listiner visualizzarebbero una riga in più duplicando l'ultimo elemento
            }
        }

        private void CambioProprietaOggT(object sender, PropertyChangedEventArgs args)
        {
            ProprietàOggTCambiata?.Invoke(this, args); //?.Invoke invice della parentesi tonda diretta poichè se nessuno ha sottoscritto l'evento non va in eccezione

            //se la proprietà dell'oggetto di tipo T che è cambiata è tra la lista delle prop.che concorrono all'ordinamento allora rieseguo l'ordinamento
            if ((from tmp in listaOrdinamenti where tmp.PropertyDescriptor.Name == args.PropertyName select tmp).Count() > 0)
            {
                if (this.oggConValidazione == true)  //ATTENZIONE: Se c'è la validazione di un item della lista da parte del datagrid, tale validazione viene richiamata solamente una volta terminati gli eventi richiamati...
                    Thr.AvviaNuovo(() => thrAttesaValidazioneEOrdina(ref sender)); //...dall'onPropertyChange della proprietà variata(quindi al termine di questa CambioProprietaOggettoT), quindi lancio un thread separato...
                else //                                                         ...che attenderà la fine della validazione per poi eseguire il riordino se l'oggetto risulterà valido
                    Ordina();
            }
        }

        private void thrAttesaValidazioneEOrdina(ref object oggT)
        {
            (oggT as ISortBindObj).SincroValidazioneRiordino = SincroValidazioneRiordino.RichiamatoRiordino; //Deve stare 
            Thr.SbloccaThrPadre();
            (oggT as ISortBindObj).TimeOutValidazionMs = 1000; //Possibilità poi aggiornarlo da validazione
            DateTime oraAtt = DateTime.Now;

            while ((oggT as ISortBindObj).SincroValidazioneRiordino == SincroValidazioneRiordino.RichiamatoRiordino || (oggT as ISortBindObj).SincroValidazioneRiordino == SincroValidazioneRiordino.InValidazione)
            {
                //DataOra.SleepConDoEvents(1);
                Thread.Sleep(1);
                if (DataOra.AttesaTempo(ref oraAtt, (oggT as ISortBindObj).TimeOutValidazionMs) == true) break;
            }
            if ((oggT as ISortBindObj).SincroValidazioneRiordino != SincroValidazioneRiordino.ValidazioneTerminata)
            {
                Log.main.Add(new Mess(Tipi.ERR, Log.main.errUserText, "SincroValidazioneRiordino non è ancora ValidazioneTerminata dopo:<" + (oggT as ISortBindObj).TimeOutValidazionMs + "> ms, valore SincroValidazioneRiordino:<" + (oggT as ISortBindObj).SincroValidazioneRiordino.ToString() + ">"));
            }

            if ((oggT as IValidation).IsValid == false) return;
            (oggT as ISortBindObj).SincroValidazioneRiordino = SincroValidazioneRiordino.InRiordino;
            Ordina();
            (oggT as ISortBindObj).SincroValidazioneRiordino = SincroValidazioneRiordino.FineRiordino;
        }

        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        protected override PropertyDescriptor SortPropertyCore
        {
            get
            {
                if (listaOrdinamenti == null || listaOrdinamenti.Count == 0) return null;
                return listaOrdinamenti[0].PropertyDescriptor;
            }
        }
        protected override ListSortDirection SortDirectionCore
        {
            get
            {
                if (listaOrdinamenti == null || listaOrdinamenti.Count == 0) return default(ListSortDirection);
                return listaOrdinamenti[0].SortDirection;
            }
        }
    }

    #region "Codice Originale"

    //Imports System.Collections.Generic
    //Imports System.ComponentModel
    //Imports System.Reflection
    //Imports System.Linq
    //Imports System.Linq.Dynamic

    //// <summary>
    //// SortableBindingList is a list that supports sorting its items and filtering them.
    //// When binding a <see cref="System.Collections.Generic.List(Of T)"/> to a <see cref="System.Windows.Forms.DataGridView"/>, you can not sort by clicking on the columns
    //// or filter the list. With this list, you can.
    //// 
    //// Dependencies:
    ////  - .NET 3.5 or higher
    ////  - System.Linq.Dynamic (DynamicQuery NuGet package)
    //// </summary>
    //// <typeparam name="T">The data type represented by this SortableBindingList</typeparam>
    //// <remarks></remarks>
    //Public Class SortableBindingList(Of T)
    //    Inherits BindingList(Of T)
    //    Implements IBindingListView

    //    Private _isSorted As Boolean = False
    //    Private _filter As String = ""
    //    Private _listSortDescriptors As IEnumerable(Of ListSortDescription)
    //    Private ReadOnly _originalData As List(Of T)

    //    /// <summary>
    //    /// Creates a new instance of SortableBindingList and populates it with the contents of the given list.
    //    /// </summary>
    //    /// <param name="list"></param>
    //    /// <remarks></remarks>
    //    Public Sub New(list As IEnumerable(Of T))
    //        MyBase.New(list.ToList()) //BindingList(Of T) requires an IList(Of T)

    //        _originalData = New List(Of T)(list)
    //    End Sub

    //    Public Sub AddRange(collection As IEnumerable(Of T))
    //        For Each tItem In collection
    //            Items.Add(tItem)
    //        Next

    //        _originalData.AddRange(collection)
    //    End Sub

    //    Protected Overrides Sub ApplySortCore(ByVal prop As PropertyDescriptor, ByVal direction As ListSortDirection)
    //        // Check to see if the property type we are sorting by implements the IComparable interface.
    //        Dim interfaceType As Type = prop.PropertyType.GetInterface("IComparable")

    //        If interfaceType Is Nothing Then
    //            //Check if this is a Nullable(Of IComparable)
    //            If prop.PropertyType.IsGenericType AndAlso prop.PropertyType.GetGenericTypeDefinition() Is GetType(Nullable(Of )) Then
    //                //Check if the type parameter implements IComparable
    //                Dim tType As Type = prop.PropertyType.GetGenericArguments()(0)
    //                interfaceType = tType.GetInterface("IComparable")
    //            End If
    //        End If

    //        If interfaceType IsNot Nothing Then
    //            //If so, set the SortPropertyValue and SortDirectionValue.
    //            _listSortDescriptors = New List(Of ListSortDescription)({
    //          New ListSortDescription(prop, direction)
    //      })

    //            //Sort the list using LINQ (OrderBy).
    //            Dim tempList = Items
    //            If direction = ListSortDirection.Ascending Then
    //                tempList = tempList.OrderBy(Function(x) prop.GetValue(x)).ToList()
    //            Else
    //                tempList = tempList.OrderByDescending(Function(x) prop.GetValue(x)).ToList()
    //            End If

    //            //Copy the sorted items back into the list.
    //            Items.Clear()
    //            For Each tItem In tempList
    //                Items.Add(tItem)
    //            Next

    //            _isSorted = True

    //            //Raise the ListChanged event so bound controls refresh their values.
    //            OnListChanged(New ListChangedEventArgs(ListChangedType.Reset, -1))
    //        Else
    //            //If the property type does not implement IComparable, let the user know.
    //            Throw New NotSupportedException(String.Format("Cannot sort by {0}. {1} does not implement IComparable.", prop.Name,
    //          prop.PropertyType.ToString()))
    //        End If
    //    End Sub

    //    Protected Overloads Sub ApplySortCore(ByVal sorts As IEnumerable(Of ListSortDescription), Optional ByVal informListeners As Boolean = True)
    //        Try
    //            Dim orderedList As IOrderedEnumerable(Of T) = Nothing
    //            Dim bFirstOrder As Boolean = True
    //            For Each sortDescriptor As ListSortDescription In sorts
    //                Dim prop As PropertyDescriptor = sortDescriptor.PropertyDescriptor
    //                Dim interfaceType As Type = prop.PropertyType.GetInterface("IComparable")

    //                If interfaceType Is Nothing Then
    //                    //Check if this is a Nullable(Of IComparable)
    //                    If prop.PropertyType.IsGenericType AndAlso prop.PropertyType.GetGenericTypeDefinition() Is GetType(Nullable(Of )) Then
    //                        //Check if the type parameter implements IComparable
    //                        Dim tType As Type = prop.PropertyType.GetGenericArguments()(0)
    //                        interfaceType = tType.GetInterface("IComparable")
    //                    End If
    //                End If

    //                If interfaceType Is Nothing Then
    //                    //If the property type does not implement IComparable, let the user know.
    //                    Throw New NotSupportedException(String.Format("Cannot sort by {0}. {1} does not implement IComparable.", prop.Name,
    //              prop.PropertyType.ToString()))
    //                End If

    //                //Sort the list using LINQ (OrderBy/ThenBy). Remember if the sort operation is the first sort or not (OrderBy vs ThenBy).
    //                If bFirstOrder Then
    //                    bFirstOrder = False
    //                    If sortDescriptor.SortDirection = ListSortDirection.Ascending Then
    //                        orderedList = Items.OrderBy(Function(x) prop.GetValue(x))
    //                    Else
    //                        orderedList = Items.OrderByDescending(Function(x) prop.GetValue(x))
    //                    End If
    //                Else
    //                    If sortDescriptor.SortDirection = ListSortDirection.Ascending Then
    //                        orderedList = orderedList.ThenBy(Function(x) prop.GetValue(x))
    //                    Else
    //                        orderedList = orderedList.ThenByDescending(Function(x) prop.GetValue(x))
    //                    End If
    //                End If
    //            Next

    //            //Sorting succeeded
    //            Dim result = orderedList.ToList()

    //            //Copy the sorted items back into the list.
    //            Items.Clear()
    //            For Each tItem In result
    //                Items.Add(tItem)
    //            Next

    //            _isSorted = True
    //            _listSortDescriptors = sorts

    //            //Most of the times, informListeners will be true. In rare cases, this function is called from EndNew, and then the OnListChanged event should not be fired.
    //            If informListeners Then
    //                //Raise the ListChanged event so bound controls refresh their values.
    //                OnListChanged(New ListChangedEventArgs(ListChangedType.Reset, -1))
    //            End If
    //        Catch
    //            //Reset the list
    //            Items.Clear()
    //            For Each tItem As T In _originalData
    //                Items.Add(tItem)
    //            Next

    //            //Rethrow the error
    //            Throw
    //        End Try
    //    End Sub

    //    Protected Sub UpdateFilter()
    //        _isSorted = False //remove sort
    //        Try
    //            //We filter on the entire collection
    //            Dim filtered = _originalData.AsQueryable()
    //            If Not String.IsNullOrEmpty(_filter) Then filtered = filtered.Where(_filter)

    //            Dim filteredResult = filtered.ToList()
    //            Items.Clear()

    //            If filteredResult IsNot Nothing AndAlso filteredResult.Count > 0 Then
    //                For Each tItem As T In filtered
    //                    Items.Add(tItem)
    //                Next
    //            End If
    //        Catch
    //            //Reset the list
    //            Items.Clear()
    //            For Each tItem As T In _originalData
    //                Items.Add(tItem)
    //            Next

    //            //Rethrow the error
    //            Throw
    //        Finally
    //            OnListChanged(New ListChangedEventArgs(ListChangedType.Reset, -1))
    //        End Try
    //    End Sub

    //    Protected Overrides Sub RemoveSortCore()
    //        If Not _isSorted Then Return

    //        //Restore original order
    //        Items.Clear()
    //        For Each tItem As T In _originalData
    //            Items.Add(tItem)
    //        Next
    //        _isSorted = False
    //        OnListChanged(New ListChangedEventArgs(ListChangedType.Reset, -1))
    //    End Sub

    //    Public Sub RemoveSort()
    //        RemoveSortCore()
    //    End Sub

    //    Public Function Find(Of TKey)(ByVal [property] As String, ByVal key As TKey) As Integer
    //        // Check the properties for a property with the specified name.
    //        Dim properties As PropertyDescriptorCollection = TypeDescriptor.GetProperties(GetType(T))
    //        Dim prop As PropertyDescriptor = properties.Find([property], True)

    //        // If there is not a match, return -1 otherwise pass search to FindCore method.
    //        If prop Is Nothing Then Return -1
    //        Return FindCore(prop, key)
    //    End Function

    //    Protected Overrides Function FindCore(ByVal prop As PropertyDescriptor, ByVal key As Object) As Integer
    //        //Get the property info for the specified property.
    //        Dim propInfo As PropertyInfo = GetType(T).GetProperty(prop.Name)
    //        Dim tItem As T

    //        If key IsNot Nothing Then
    //            //Loop through the items to see if the key
    //            // value matches the property value.
    //            For i As Integer = 0 To Count - 1
    //                tItem = Items(i)
    //                If (propInfo.GetValue(tItem, Nothing).Equals(key)) Then Return i
    //            Next
    //        End If

    //        Return -1
    //    End Function

    //    Public Overrides Sub EndNew(itemIndex As Integer)
    //        Try
    //            //Check to see if the item is added to the end of the list,
    //            //and if so, re-sort the list.
    //            If _isSorted And itemIndex = Count - 1 Then
    //                //Reapply the sort, but do not inform listeners, because this would reset the position.
    //                ApplySortCore(_listSortDescriptors, False)
    //            End If

    //            If Filter <> "" Then
    //                //Reapply the filter
    //                UpdateFilter()
    //            End If
    //        Finally
    //            MyBase.EndNew(itemIndex)
    //        End Try
    //    End Sub

    //    Protected Overrides ReadOnly Property SupportsSearchingCore() As Boolean
    //        Get
    //            Return True
    //        End Get
    //    End Property

    //    Protected Overrides ReadOnly Property SupportsSortingCore() As Boolean
    //        Get
    //            Return True
    //        End Get
    //    End Property

    //    Protected Overrides ReadOnly Property IsSortedCore() As Boolean
    //        Get
    //            Return _isSorted
    //        End Get
    //    End Property

    //    Protected Overrides ReadOnly Property SortPropertyCore() As PropertyDescriptor
    //        Get
    //            If _listSortDescriptors Is Nothing OrElse _listSortDescriptors.Count = 0 Then Return Nothing
    //            Return _listSortDescriptors(0).PropertyDescriptor
    //        End Get
    //    End Property

    //    Protected Overrides ReadOnly Property SortDirectionCore() As ListSortDirection
    //        Get
    //            If _listSortDescriptors Is Nothing OrElse _listSortDescriptors.Count = 0 Then Return Nothing
    //            Return _listSortDescriptors(0).SortDirection
    //        End Get
    //    End Property

    //    /// <summary>
    //    /// You can filter this list by adding a LINQ Where-clause.
    //    /// </summary>
    //    /// <value>A string representing a LINQ Where-clause</value>
    //    /// <returns>The filter value</returns>
    //    /// <example>
    //    /// You can filter on any property of <typeparamref name="T"/> like you would do when writing a LINQ Where-clause:
    //    /// <code lang="VB">
    //    /// Public Class Customer
    //    ///     Public Property Name As String 
    //    ///     Public Property FirstName As String
    //    ///     Public Property CountryIsoCode As String
    //    /// End Class
    //    ///
    //    /// //...  
    //    /// 
    //    /// Dim list As New SortableBindingList(Of Customer)()
    //    /// 
    //    /// //... 
    //    /// 
    //    /// list.Filter = "CountryIsoCode.ToUpper() = ""BE""" //Only show Belgian customers
    //    /// </code>
    //    /// <code lang="C#">
    //    /// public class Customer
    //    /// {
    //    ///     public string Name { get; set; }
    //    ///     public string FirstName { get; set; }
    //    ///     public string CountryIsoCode { get; set; }
    //    /// }
    //    /// 
    //    /// //...
    //    /// 
    //    /// SortableBindingList&gt;Customer&lt; list = new SortableBindingList&gt;Customer&lt;();
    //    /// 
    //    /// //...
    //    /// 
    //    /// list.Filter = "CountryIsoCode.ToUpper() = \"BE\"" //Only show Belgian customers
    //    /// </code>
    //    /// </example>
    //    Public Property Filter() As String Implements IBindingListView.Filter
    //        Get
    //            Return _filter
    //        End Get
    //        Set(ByVal value As String)
    //            If _filter = value Then Exit Property
    //            _filter = value
    //            UpdateFilter()
    //        End Set
    //    End Property

    //    Public ReadOnly Property SortDescriptions() As ListSortDescriptionCollection Implements IBindingListView.SortDescriptions
    //        Get
    //            Return New ListSortDescriptionCollection(_listSortDescriptors.ToArray())
    //        End Get
    //    End Property

    //    Public ReadOnly Property SupportsAdvancedSorting() As Boolean Implements IBindingListView.SupportsAdvancedSorting
    //        Get
    //            Return True
    //        End Get
    //    End Property

    //    Public ReadOnly Property SupportsFiltering() As Boolean Implements IBindingListView.SupportsFiltering
    //        Get
    //            Return True
    //        End Get
    //    End Property

    //    Public Sub ApplySort(ByVal sorts As ListSortDescriptionCollection) Implements IBindingListView.ApplySort
    //        ApplySortCore(sorts.Cast(Of ListSortDescription)())
    //    End Sub

    //    Public Sub RemoveFilter() Implements IBindingListView.RemoveFilter
    //        Filter = ""
    //    End Sub
    //End Class
    #endregion
}
