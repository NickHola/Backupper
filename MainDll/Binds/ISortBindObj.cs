using Newtonsoft.Json;
using System;

namespace Main.Binds
{
    public interface ISortBindObj
    {
        [JsonIgnore] SincroValidazioneRiordino SincroValidazioneRiordino { get; set; }
        [JsonIgnore] UInt16 TimeOutValidazionMs { get; set; }

        UInt64 IndiceOrd { get; set; }
    }
    
    //<Serializable> Public Class ISortBindObj
    //    Implements INotifyPropertyChanged, IComparable

    //    Private padre_ As Object
    //    Private indiceOrd_ As UInt64
    //    Private sincroValidazioneRiordino_ As sincroValidazioneRiordino
    //    Private timeOutValidazionMs_ As UInt16

    //    <JsonIgnore> Public ReadOnly Property padre As Object
    //        Get
    //            Return padre_
    //        End Get
    //    End Property
    //    Public Property indiceOrd As UInt64
    //        Get
    //            Return indiceOrd_
    //        End Get
    //        Set(value As UInt64)
    //            indiceOrd_ = value
    //            NotifyPropertyChanged()
    //        End Set
    //    End Property
    //    <JsonIgnore> Property sincroValidazioneRiordino As sincroValidazioneRiordino
    //        Get
    //            Return sincroValidazioneRiordino_
    //        End Get
    //        Set(value As sincroValidazioneRiordino)
    //            sincroValidazioneRiordino_ = value
    //            NotifyPropertyChanged()
    //        End Set
    //    End Property
    //    <JsonIgnore> Property timeOutValidazionMs As UInt16
    //        Get
    //            Return timeOutValidazionMs_
    //        End Get
    //        Set(value As UInt16)
    //            timeOutValidazionMs_ = value
    //            NotifyPropertyChanged()
    //        End Set
    //    End Property

    //    Public Sub New(padre)
    //        Me.padre_ = padre
    //    End Sub

    //    Public Shared Function VerificaEsistanzaInOgg(tipoOgg As Type) As Boolean
    //        If IsNothing(tipoOgg.GetProperty("ISortBindObj")) = False AndAlso tipoOgg.GetProperty("ISortBindObj").PropertyType = GetType(ISortBindObj) Then Return True
    //        Return False
    //    End Function

    //    Public Function CompareTo(obj As Object) As Integer Implements IComparable.CompareTo
    //        Return Me.indiceOrd.CompareTo(obj.indiceOrd)
    //        'If Me.indiceOrd > obj.Balance Then Return -1
    //        'If Me.indiceOrd = obj.Balance Then Return 0
    //        'Return 1
    //    End Function

    //    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    //    Private Sub NotifyPropertyChanged(<CallerMemberName()> Optional ByVal propertyName As String = Nothing)
    //        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    //    End Sub
    //End Class
}
