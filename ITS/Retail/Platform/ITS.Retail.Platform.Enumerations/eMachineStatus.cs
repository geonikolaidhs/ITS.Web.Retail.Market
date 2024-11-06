using ITS.Retail.ResourcesLib;
using System;
using System.ComponentModel.DataAnnotations;

namespace ITS.Retail.Platform.Enumerations
{
    [Flags]
    public enum eMachineStatus
    {
        [Display(Name = "UNKNOWN_MACHINE_STATUS", ResourceType = typeof(Resources))]
        UNKNOWN = 1,                    //Άγνωστη κατάσταση μηχανής
        [Display(Name = "SALE", ResourceType = typeof(Resources))]
        SALE = 2,                       //Η μηχανή έχει κάνει εναρξη ημέρας, κ βάρδιας, αλλά δεν έχει ανοιχτο παραστατικό
        [Display(Name = "OPENDOCUMENT", ResourceType = typeof(Resources))]
        OPENDOCUMENT = 4,               //Η μηχανή έχει κάνει εναρξη ημέρας, κ βάρδιας, αλλά έχει ανοιχτο παραστατικό κ χτυπάει είδη
        [Display(Name = "OPENDOCUMENT_PAYMENT", ResourceType = typeof(Resources))]
        OPENDOCUMENT_PAYMENT = 8,       //Η μηχανή έχει κάνει εναρξη ημέρας, κ βάρδιας, αλλά έχει ανοιχτο παραστατικό κ έχει πατήσει σύνολο
        [Display(Name = "CLOSED_MACHINE_STATUS", ResourceType = typeof(Resources))]
        CLOSED = 16,                     //Η μηχανή ΔΕΝ έχει κάνει εναρξη ημέρας
        [Display(Name = "PAUSE", ResourceType = typeof(Resources))]
        PAUSE = 32,                      //Η μηχανή ειναι κλειδωμένη
        [Display(Name = "DAYSTARTED", ResourceType = typeof(Resources))]
        DAYSTARTED = 64//,                 //Η μηχανή έχει κάνει εναρξη ημέρας, αλλά όχι βάρδιας
        //[Display(Name = "OPENDRAWER", ResourceType = typeof(Resources))]
        //OPENDRAWER=128                  //Tο συρτάρι είναι ανοιχτό
    }
}
