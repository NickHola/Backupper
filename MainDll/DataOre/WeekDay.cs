using Main.Binds;
using System;
using System.ComponentModel;

namespace Main.DataOre
{
    //[TypeConverter(typeof(EnumToStringa))]
    public enum WeekDay
    { //ATTENZIONE: l'ordinamento deve essere lo stesso di DayOfWeek poichè così posso fare i confronti diretti con la prop.DayOfWeek dei Datetime, serve per fare i binding in Italiano
        Sunday = 0,
        Monday = 1,
        Tuesday = 2,
        Wednesday = 3,
        Thursday = 4,
        Friday = 5,
        Saturday = 6
    }
}