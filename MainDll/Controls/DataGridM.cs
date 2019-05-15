using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Input;
using Main.Validations;
using Main.Logs;
using static Main.Validations.Validation;

namespace Main.Controls
{
    public class DataGridM : DataGrid
    {
        #region "DependencyProperty definition"
        public bool ValidaRighe
        {
            get { return (bool)this.GetValue(ValidaRigheProperty); }
            set { this.SetValue(ValidaRigheProperty, value); }
        }
        public static readonly DependencyProperty ValidaRigheProperty = DependencyProperty.Register("ValidaRighe", typeof(bool), typeof(DataGridM), new PropertyMetadata(true));

        public Type ClasseDiValidazioneCella
        {
            get { return (Type)this.GetValue(ClasseDiValidazioneCellaProperty); }
            set { this.SetValue(ClasseDiValidazioneCellaProperty, value); }
        }
        public static readonly DependencyProperty ClasseDiValidazioneCellaProperty = DependencyProperty.Register("ClasseDiValidazioneCella", typeof(Type), typeof(DataGridM), new PropertyMetadata(null));

        public Type ClasseDiValidazioneRiga
        {
            get { return (Type)this.GetValue(ClasseDiValidazioneRigaProperty); }
            set { this.SetValue(ClasseDiValidazioneRigaProperty, value); }
        }
        public static readonly DependencyProperty ClasseDiValidazioneRigaProperty = DependencyProperty.Register("ClasseDiValidazioneRiga", typeof(Type), typeof(DataGridM), new PropertyMetadata(null));

        public ValidationStep StepValidazione
        {
            get { return (ValidationStep)this.GetValue(StepValidazioneProperty); }
            set { this.SetValue(StepValidazioneProperty, value); }
        }
        public static readonly DependencyProperty StepValidazioneProperty = DependencyProperty.Register("StepValidazione", typeof(ValidationStep), typeof(DataGridM), new PropertyMetadata(ValidationStep.UpdatedValue));

        public bool MostraEliminaRiga
        {
            get { return (bool)this.GetValue(MostraEliminaRigaProperty); }
            set { this.SetValue(MostraEliminaRigaProperty, value); }
        }
        public static readonly DependencyProperty MostraEliminaRigaProperty = DependencyProperty.Register("MostraEliminaRiga", typeof(bool), typeof(DataGridM), new PropertyMetadata(false));

        public bool EliminaRigaInUltimaColonna
        {
            get { return (bool)this.GetValue(EliminaRigaInUltimaColonnaProperty); }
            set { this.SetValue(EliminaRigaInUltimaColonnaProperty, value); }
        }
        public static readonly DependencyProperty EliminaRigaInUltimaColonnaProperty = DependencyProperty.Register("EliminaRigaInUltimaColonna", typeof(bool), typeof(DataGridM), new PropertyMetadata(true));

        public double EliminaRigaLarghezza
        {
            get { return (double)this.GetValue(EliminaRigaLarghezzaProperty); }
            set { this.SetValue(EliminaRigaLarghezzaProperty, value); }
        }
        public static readonly DependencyProperty EliminaRigaLarghezzaProperty = DependencyProperty.Register("EliminaRigaLarghezza", typeof(double), typeof(DataGridM), new PropertyMetadata(Convert.ToDouble(35)));

        public SolidColorBrush ColoreSfondoPopupErr
        {
            get { return (SolidColorBrush)this.GetValue(ColoreSfondoPopupErrProperty); }
            set { this.SetValue(ColoreSfondoPopupErrProperty, value); }
        }
        public static readonly DependencyProperty ColoreSfondoPopupErrProperty = DependencyProperty.Register("ColoreSfondoPopupErr", typeof(SolidColorBrush), typeof(DataGridM), new PropertyMetadata(Util.DammiBrushDaEsadec("E52914")));

        public SolidColorBrush ColoreScrittaPopupErr
        {
            get { return (SolidColorBrush)this.GetValue(ColoreScrittaPopupErrProperty); }
            set { this.SetValue(ColoreScrittaPopupErrProperty, value); }
        }
        public static readonly DependencyProperty ColoreScrittaPopupErrProperty = DependencyProperty.Register("ColoreScrittaPopupErr", typeof(SolidColorBrush), typeof(DataGridM), new PropertyMetadata(Brushes.White));

        public SolidColorBrush ColoreCellaInErr
        {
            get { return (SolidColorBrush)this.GetValue(ColoreCellaInErrProperty); }
            set { this.SetValue(ColoreCellaInErrProperty, value); }
        }
        public static readonly DependencyProperty ColoreCellaInErrProperty = DependencyProperty.Register("ColoreCellaInErr", typeof(SolidColorBrush), typeof(DataGridM), new PropertyMetadata(Util.DammiBrushDaEsadec("FC8600")));

        public SolidColorBrush ColoreRigaInErr
        {
            get { return (SolidColorBrush)this.GetValue(ColoreRigaInErrProperty); }
            set { this.SetValue(ColoreRigaInErrProperty, value); }
        }
        public static readonly DependencyProperty ColoreRigaInErrProperty = DependencyProperty.Register("ColoreRigaInErr", typeof(SolidColorBrush), typeof(DataGridM), new PropertyMetadata(Util.DammiBrushDaEsadec("FFBF87")));
        #endregion

        public bool conRicerca;
        private bool loaded, isInEdit, validazioneRigheImpostata;

        public bool IsInEdit
        {
            get { return isInEdit; }
        }

        public DataGridM() : this(conRicerca: false)
        { //Serve per lo xaml per non dover passare parametri
        }

        public DataGridM(bool conRicerca = true)
        {
            this.conRicerca = conRicerca;
            this.Style = (Style)App.UIResource["stlDtgMain"];
            this.ColumnHeaderStyle = (Style)App.UIResource["stlDtgClmHeader"];
            this.AutoGenerateColumns = false;
            this.Initialized += new EventHandler(MyInitialized);
            this.Loaded += new RoutedEventHandler(MyLoaded);
            //this.PreparingCellForEdit += new EventHandler<DataGridPreparingCellForEditEventArgs>(MyPreparingCellForEdit); //Spostata nel controllo DtgTextClm insieme a MyPreparingCellForEdit e TxtCell_TextChanged
            this.BeginningEdit += new EventHandler<DataGridBeginningEditEventArgs>(MyBeginningEdit);
            this.RowEditEnding += new EventHandler<DataGridRowEditEndingEventArgs>(MyRowEditEnding);
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(MyIsEnabledChanged);
            //this.CellEditEnding += new EventHandler<DataGridCellEditEndingEventArgs>(Me_CellEditEnding);
            //this.CurrentCellChanged += new EventHandler<EventArgs>(Me_CurrentCellChanged);
        }

        private void MyInitialized(object sender, EventArgs e)
        {
            //Attenzione: non tutti i Dtg hanno impostata la prop ItemsSource con un oggetto che non sia null, dato che ItemsSource mi serve per sapere se la classe...
            //ha un validatore e settare lo style per la presentazione deglie errori, allora intercetto il cambiamento della property
            var dpd = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(DataGrid));
            if (dpd != null) { dpd.AddValueChanged(this, new EventHandler(ItemsSourceChanged)); }

            //Se non si setta TargetNullValue si ha la seguente eccezione e viene visualizzato un rettangolo rosso intorno al datagrid
            //ConvertBack cannot convert value '{NewItemPlaceholder}' (type 'NamedObject'). BindingExpression:Path=cartellaCriteriSelezionata; DataItem='srcSelettoreFile' (Name='srcSelettore1'); target element is 'DataGridM' (Name='dtgCartelle'); target property is 'SelectedItem' (type 'Object') NotSupportedException:'System.NotSupportedException: TypeConverter non può effettuare una conversione da MS.Internal.NamedObject.
            BindingExpression BindingExpression = this.GetBindingExpression(DataGridM.SelectedItemProperty);

            if (BindingExpression != null)
            {
                Binding nuovoBind = BindingExpression.ParentBinding.Clona();
                nuovoBind.TargetNullValue = CollectionView.NewItemPlaceholder;
                this.SetBinding(DataGridM.SelectedItemProperty, nuovoBind);
            }
        }

        private void MyLoaded(Object sender, RoutedEventArgs e)
        {
            //If IsNothing(Me.ItemsSource) = False Then
            //    ImpostaValidazioneRighe() 'Sull'evento Initialized l'item source non è ancora valorizzato
            //Else
            //    Dim dpd = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, GetType(DataGrid))
            //    If IsNothing(dpd) = False Then dpd.AddValueChanged(Me, AddressOf ItemsSourceChanged)
            //End If

            if (loaded == true) return;
            try
            {
                if (MostraEliminaRiga == true) { InserisciColonnaElimanaRiga(); }
                if (Util.IsDesignTime == false) SetColumnStyle();
            }
            finally
            { loaded = true; }

        }

        private void ItemsSourceChanged(Object sender, EventArgs e)
        {
            if (this.ItemsSource != null && validazioneRigheImpostata == false)
            {
                validazioneRigheImpostata = true;
                ImpostaValidazioneRighe();
            }
        }

        private void MyBeginningEdit(Object sender, DataGridBeginningEditEventArgs e)
        { isInEdit = true; }

        private void MyRowEditEnding(Object sender, DataGridRowEditEndingEventArgs e)
        { isInEdit = false; }

        private void MyIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        { //Utile nel caso in cui il contenitore padre del dataGrid setta questultimo in disabled ma ho ancora delle celle in edit si genere una eccezione
            if (this.IsEnabled == false)
            {
                while (this.IsInEdit)
                    this.CommitEdit();
            }
        }

        //private void Me_CellEditEnding(Object sender, DataGridCellEditEndingEventArgs e)
        //{   string ciao = "";   }

        //private void Me_CurrentCellChanged(Object sender, EventArgs e)
        //{   string ciao = "";   }

        private void SetColumnStyle()
        {
            foreach (DataGridColumn colonna in this.Columns)
            {
                if (colonna.GetType() == typeof(DataGridTextColumn) || colonna.GetType().IsSubclassOf(typeof(DataGridTextColumn)))
                {
                    DataGridTextColumn dtgTxtClm = (DataGridTextColumn)colonna;
                    dtgTxtClm.ElementStyle = Util.AddStylesToAnExistingOne(dtgTxtClm.ElementStyle, new Style[] { }, typeof(TextBlock), (Style)App.UIResource["stlDtgCllTxt"]);
                    dtgTxtClm.EditingElementStyle = Util.AddStylesToAnExistingOne(dtgTxtClm.EditingElementStyle, new Style[] { }, typeof(TextBox), (Style)App.UIResource["stlDtgCllTxtEditing"]);
                }
            }
        }

        #region "Colonna elimina righe"
        private void InserisciColonnaElimanaRiga()
        {
            var eliminaRiga = new DataGridTemplateColumn { Header = "Del.", MinWidth = EliminaRigaLarghezza, MaxWidth = EliminaRigaLarghezza }; //usato Min e max al posto di width così l'utente non la può modificare
                                                                                                                                                //Dim xaml As String = "<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                                                                                                                                                //                        <ContentPresenter ContentTemplate=""{StaticResource vctImgDeleteDtgRow}"" />
                                                                                                                                                //                    </DataTemplate>"
            var dataTempl = new DataTemplate(); //= Markup.XamlReader.Parse(xaml);
            var fefContentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            fefContentPresenter.AddHandler(ContentPresenter.PreviewMouseLeftButtonUpEvent, new MouseButtonEventHandler(EliminaRiga_MouseLeftButtonDown)); //New dlgEliminaRiga_MouseUp(AddressOf dgc_MouseLeftButtonDown))
            fefContentPresenter.SetValue(ContentPresenter.ContentTemplateProperty, App.UIResource["vctImgDeleteDtgRow"]);
            dataTempl.VisualTree = fefContentPresenter;

            eliminaRiga.CellTemplate = dataTempl;

            this.Columns.Add(eliminaRiga);
        }
        public void EliminaRiga_MouseLeftButtonDown(Object sender, MouseButtonEventArgs e)
        {
            IList lista = (IList)this.ItemsSource;
            //MessageBox.Show("TODO vedere il tipo di sender, Indice:" + this.Items.IndexOf((sender as ContentPresenter).Content));
            lista.Remove((sender as ContentPresenter).Content);

            //ATTENZIONE BUG del Dtg: in pratica se si rimuove dall'ItemsSource l'elemento che rende il Dtg nello stato di invalido (righe non editabili tranne quella non valida), quest'ultimo non ritorna valido e quindi...
            //...non si possono fare ulteriori modifiche alle righe (almeno l'elemento eliminato sparisce dal Dtg), se l'elemento era nuovo (quindi inserito nella riga vuota in fondo) neanche compare un nuova riga vuota....
            //...anche se nel Me.Items all'ultima posizione è correttamente presente il CollectionView.NewItemPlaceholder

            if (this.CanUserAddRows == true)
            { //Codice per far comparire una nuova riga vuota alla fine del Dtg
                this.CanUserAddRows = false;
                this.CanUserAddRows = true;
            }

            //***Codice vecchio per far ritornare il Dtg nello stato di normalità***********************************************
            //var bindingFlags = System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            //var cellErrorInfo = typeof(DataGrid).GetProperty("HasCellValidationError", bindingFlags);
            //var rowErrorInfo = typeof(DataGrid).GetProperty("HasRowValidationError", bindingFlags);
            //if (cellErrorInfo != null) { cellErrorInfo.SetValue(this, false, null); }
            //if (rowErrorInfo != null) { rowErrorInfo.SetValue(this, false, null); }
            //***Fine codice vecchio per far ritornare il Dtg nello stato di normalità***********************************************

            this.Items.Refresh();
        }
        #endregion

        #region  "Validazione righe"
        private void ImpostaValidazioneRighe()
        {
            if (this.ValidaRighe == false)
            { //Se nello xaml ho valorizzato ValidaRighe a False significa che se anche la lista ha una classe di validazione non la userò
                IVR_ImpostaListaSeSpeciale(false);
                return;
            }

            ValidationRule validatoreCelle, validatoreRighe;

            if (IVR_RecuperoValidatore(out validatoreCelle, out validatoreRighe) == false)
            {
                IVR_ImpostaListaSeSpeciale(false);
                return;
            }

            foreach (DataGridColumn colonna in this.Columns)
            {
                if (colonna.GetType().IsSubclassOf(typeof(DataGridBoundColumn)) == false) continue;

                DataGridBoundColumn colonnaBind = (DataGridBoundColumn)colonna;
                Binding bind = (Binding)colonnaBind.Binding;

                if (bind == null) continue;

                bind.ValidationRules.Add(validatoreCelle);

            }

            this.RowValidationRules.Add(validatoreRighe);
            //Me.RowValidationErrorTemplate = App.risorseUI("ctrTplDtgRowValidErr") 'Non sono riuscito ad ottenere il comportamente desiderato con il popup come fatto sullo stile della cella stlDtgCllTxtEditing



            Style styleToAdd = (Style)Util.GetResourceFromDictionary("stlDtgRowValidErr", "pack://application:,,,/MainDll;component/UIRes.xaml");

            this.RowStyle = Util.AddStylesToAnExistingOne(this.RowStyle, new Style[] { styleToAdd }, typeof(DataGridRow));

            //stileTmp = App.risorseUI("stlDtgRowValidErr")
            //If IsNothing(Me.RowStyle) = False Then stileTmp.BasedOn = Me.RowStyle
            //Me.RowStyle = stileTmp

            IVR_ImpostaListaSeSpeciale(true);

        }

        private bool IVR_RecuperoValidatore(out ValidationRule validatoreCelle, out ValidationRule validatoreRighe)
        {
            validatoreCelle = null;
            validatoreRighe = null;

            if (this.ClasseDiValidazioneCella == null || this.ClasseDiValidazioneRiga == null)
            { //Se il datagrid non indica una precisa classe di validazione per le righe allora prendo quella della lista se indicata
                if (IVR_RV_DalTipoTDellaLista(ref validatoreCelle, ref validatoreRighe) == false) return false;
            }

            if (this.ClasseDiValidazioneCella != null) { validatoreCelle = (ValidationRule)Activator.CreateInstance(this.ClasseDiValidazioneCella); }
            if (this.ClasseDiValidazioneRiga != null) { validatoreRighe = (ValidationRule)Activator.CreateInstance(this.ClasseDiValidazioneRiga); }

            validatoreCelle.ValidationStep = this.StepValidazione;
            validatoreRighe.ValidationStep = this.StepValidazione;

            return true;
        }

        private bool IVR_RV_DalTipoTDellaLista(ref ValidationRule validatoreCelle, ref ValidationRule validatoreRighe)
        {
            var listaOggT = this.ItemsSource;
            Type tipoDiT;

            if (listaOggT.GetType().GetGenericArguments().Count() == 0) return false; //Il tipo non ha argomenti generici, es. list(of XXX)

            tipoDiT = listaOggT.GetType().GetGenericArguments()[0];

            if (typeof(IValidation).IsAssignableFrom(tipoDiT) == false) return false; //Verifico se il tipo degli oggetti T implementa l'interfaccia Validation

            if (typeof(Binds.ISortBindObj).IsAssignableFrom(tipoDiT) == false)
            { //Verifico se il tipo degli oggetti T implementa l'interfaccia SortBindObj
                validatoreCelle = (ValidationRule)Activator.CreateInstance(typeof(Validations.ValidationOfObject));
                validatoreRighe = (ValidationRule)Activator.CreateInstance(typeof(Validations.ValidationOfDataGridRow));
            }
            else
            {
                validatoreCelle = (Validations.ValidationOfSortableObject)Activator.CreateInstance(typeof(Validations.ValidationOfSortableObject));
                validatoreRighe = (Validations.ValidationOfDataGridRowOfSortableObj)Activator.CreateInstance(typeof(Validations.ValidationOfDataGridRowOfSortableObj));
            }

            return true;
            //Dim propClasseDiValidazione = tipoDiT.GetProperty("classeDiValidazione")
            //If IsNothing(propClasseDiValidazione) = True Then Return Nothing
            //If IsNothing(propClasseDiValidazione.GetValue(Nothing)) = True Then Return Nothing
            //Return Activator.CreateInstance(propClasseDiValidazione.GetValue(Nothing))
        }

        private void IVR_ImpostaListaSeSpeciale(bool validaRighe)
        {
            var listaOggT = this.ItemsSource;
            if (listaOggT.GetType().IsGenericType == true && listaOggT.GetType().GetGenericTypeDefinition() == typeof(Binds.SortBindList<>))
            {
                //(listaOggT as Binds.SortBindList<object>).oggConValidazione = validaRighe;    //TODO under review because validaRighe is System.Collections.IEnumerable{Binds.SortBindList<FSes.StringMatchPathFilterM>} type.
            }

        }
        #endregion

        public static bool DammiDataGridDaRow(DataGridRow dtgRow, ref DataGrid dtg)
        {
            if (dtgRow == null)
            {
                Log.main.Add(new Mess(Tipi.ERR, Log.main.errUserText, "Ricevuto dtgRow a null"));
                return false;
            }
            try
            {
                DataGridRowsPresenter dtgRowPrt = (DataGridRowsPresenter)VisualTreeHelper.GetParent(dtgRow);
                ItemsPresenter itemPrt = (ItemsPresenter)dtgRowPrt.TemplatedParent;
                dtg = (DataGrid)itemPrt.TemplatedParent;
            }
            catch (Exception ex)
            {
                Log.main.Add(new Mess(Tipi.ERR, Log.main.errUserText, "Eccezione ex.mess:<" + ex.Message + ">"));
                return false;
            }
            return true;
        }
    }
}
