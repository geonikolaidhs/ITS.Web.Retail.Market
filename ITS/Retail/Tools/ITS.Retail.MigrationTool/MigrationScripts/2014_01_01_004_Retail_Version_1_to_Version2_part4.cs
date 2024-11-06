using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;
using DevExpress.Xpo;
using System.Data;
using DevExpress.Xpo.DB;
using System.Data.SqlClient;
using ITS.Retail.MigrationTool.Classes;
using ITS.Retail.WebClient.Helpers;

namespace MigrationTool.MigrationScripts
{

    

    //[Migration(20130404)]
    [RetailMigration(author: "kdk", year: 2014, month: 1, day: 1, order: 4, version: "2.0.0.67")]
    public class Retail_Version_1_to_Version2_part4 : Migration
    {
        private readonly String[][] MinistryDocumentTypes = new string[][]{
             new String[] {"CF069D3D-40F6-41DA-B823-0357DE163CDC","-","225","ΤΙΜΟΛΟΓΙΟ (Αγοράς αγροτικών προϊόντων) - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ","Τ(ΑΓ.ΑΓΡ.)-Δ.Α.","2","1"},
             new String[] {"F0969209-4E34-495B-A6C7-064A5E19715E","13 ΠΑΡ 1","145","ΕΙΣΙΤΗΡΙΟ ΠΛΟΙΩΝ","ΕΙΣ.ΠΛΟΙΩΝ","1","1"},
             new String[] {"DC400774-29A4-4BCD-911C-06769798AC0E","ΠΟΛ 1039/09-03-2006","310","ΠΑΡΑΔΟΣΗ ΚΤΙΣΜΑΤΩΝ","ΠΑΡΑΔ.ΚΤΙΣΜΑΤΩΝ","1","1"},
             new String[] {"3FB08C25-C415-4B11-9504-06D4346EE059","Μονο για πλυντήρια","242","ΒΙΒΛΙΟ ΕΙΣΕΡΧΟΜΕΝΩΝ - ΑΠΟΔΕΙΞΗ ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ","Β.Ε.Α. - Α.Π.Υ.","1","1"},
             new String[] {"C9584460-7602-40B9-9086-072449E90E87","ιατροί, οδοντίατροι","259","ΒΙΒΛΙΟ ΕΠΙΣΚΕΨΗΣ ΑΣΘΕΝΩΝ-ΑΠΟΔ.ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ(ιατροί, οδον)","Β.ΕΠΙΣΚ.ΑΣΘ-ΑΠΥ","1","1"},
             new String[] {"0A8F550D-5D35-47A2-8480-0AF1EF8DD051","ΠΟΛ.1102/92","191","ΕΚΚΑΘΑΡΙΣΗ (Αεροπορικών εισιτηρίων)","ΕΚΚ.Α.ΕΙΣ","2","1"},
             new String[] {"C17A4A6C-B3D5-4C70-B302-0B5B0EF78FB3","-","329","ΤΙΜΟΛΟΓΙΟ(Παροχ.Υπηρ.)-ΤΙΜ.(Αγοράς Αγρ.Προϊόντων)-ΔΕΛ. ΑΠΟΣΤ","ΤΠ-Τ(ΑΓ.ΑΓΡ)-ΔΑ","4","0"},
             new String[] {"5479D9BE-4B75-47B1-854B-0C967C9B2C60","1167, 1271/02","180","ΦΟΡΤΩΤΙΚΗ","ΦΟΡΤΩΤΙΚΗ","1","1"},
             new String[] {"484ACF61-E46E-4E0B-B365-0D9B53596B92","-","227","ΤΙΜΟΛΟΓΙΟ(Παροχή Υπηρεσιων)-ΤΙΜΟΛΟΓΙΟ(Πώληση Αγαθών)-ΔΕΛ.ΑΠ.","ΤΠΥ.-Τ.Π-Δ.Α","1","1"},
             new String[] {"0C762D22-7626-422F-8F05-0EC1D8D9412D","11 παρ.1","158","ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ","Δ.Α","0","1"},
             new String[] {"8E8D002F-9031-4347-A867-121630BE6DD2","Συγκοινωνιακών μέσων εκτός πλοίων","139","ΕΙΣΙΤΗΡΙΟ","ΕΙΣ.Σ.ΜΕΣΩΝ","1","1"},
             new String[] {"D5C86F24-7345-4053-8B8F-14BA2B0C1B57","Αρθ.10 παρ.5δ & Αρθ 19παρ.4(Διαγνωστικά)","254","ΒΙΒΛΙΟ ΕΠΙΣΚΕΨΗΣ ΑΣΘΕΝΩΝ - ΑΠΟΔΕΙΞΗ ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ","Β.ΕΠΙΣΚ.ΑΣΘ-ΑΠΥ","1","1"},
             new String[] {"0E102A64-5667-47B2-9F7E-15951ECE064E","16 παρ.9","183","ΑΠΟΔΕΙΞΗ ΜΕΤΑΦΟΡΑΣ","ΑΠ.ΜΕΤΑΦ","1","1"},
             new String[] {"EB5C8469-A570-44BA-91CE-1A12018A4EBD","-","502","ΑΝΤΙΤΥΠΟ","-","0","1"},
             new String[] {"35C27CF2-CE41-413C-94CC-1B5D61CE585D","-","253","ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ-ΑΠΟΔ.ΛΙΑΝΙΚΗΣ ΠΩΛΗΣΗΣ-ΑΠΟΔ.ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩ","ΔΑ - ΑΛΠ - ΑΠΥ","1","1"},
             new String[] {"211A6B5A-DB2D-4CF4-83D7-212958605D89","-","231","ΑΠΟΔΕΙΞΗ ΛΙΑΝΙΚΗΣ ΠΩΛΗΣΗΣ - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ","Α.Λ.Π-Δ.Α","1","1"},
             new String[] {"A6B33C65-E8C0-4394-99FD-2179BF05D16F","-","500","ΔΙΑΦΟΡΑ ΒΙΒΛΙΑ","-","0","1"},
             new String[] {"064C10A6-3719-46E4-A418-21A87706F5E3","-","234","ΑΠΟΔΕΙΞΗ ΕΠΑΓΓΕΛΜΑΤΙΚΗΣ ΔΑΠΑΝΗΣ - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ","Α.Ε.Δ-Δ.Α","2","1"},
             new String[] {"F567B469-C4B6-4D26-BAFA-22AA410EE12A","ΠΟΛ.1233/93","193","ΕΚΚΑΘΑΡΙΣΗ (Εισιτήρια Αστικών Συγκοινωνιών)","ΕΚΚ.ΕΙΣ.Α.Σ.","2","1"},
             new String[] {"E3CAA4BA-8ACE-45E8-B2CD-25981D87F28F","αρθρο 10 παρ. 5 περ ζ","316","ΔΕΛΤΙΟ ΕΠΙΣΚΕΥΗΣ","ΔΕΛΤ. ΕΠΙΣΚΕΥΗΣ","0","1"},
             new String[] {"066DEE6E-3897-4D51-B5EB-2A782E9B2DBF","12 παρ.3","163","ΤΙΜΟΛΟΓΙΟ (Επιδοτ. Αποζημ. κ.λ.π.)","ΤΙΜ 12 παρ.3","1","1"},
             new String[] {"6D9C2836-A4EA-4764-8466-2BF3812D7166","-","235","ΑΠΟΔΕΙΞΗ ΕΠΙΣΤΡΟΦΗΣ - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ","Α.Π.ΕΠΙΣΤ-Δ.Α","2","1"},
             new String[] {"8FB0AC10-DC04-4234-93CF-2F9465D4F72F","16 παρ.6","181","ΚΑΤΑΣΤΑΣΗ ΑΠΟΣΤΟΛΗΣ","ΚΑΤ.ΑΠΟΣΤ","0","1"},
             new String[] {"5EB1A003-D929-4A8F-BF9A-30B075D21EDF","ΠΟΛ.1102/93","190","ΑΠΟΔΕΙΞΗ ΠΩΛΗΣΗΣ (Αεροπ.Εισιτηρίων)","Α.Π.Α.ΕΙΣ","1","1"},
             new String[] {"9027AD67-7D00-4E9A-91C4-3876E416480E","Ν.3870/2010 άρθρο 1§5","328","ΕΙΣΙΤΗΡΙΟ ΕΚΔΗΛΩΣΕΩΝ","ΕΙΣIΤ.ΕΚΔΗΛ","1","1"},
             new String[] {"49EB86C9-F3AA-4305-8A17-39BFE08088C8","-","239","ΕΙΣΙΤΗΡΙΟ - ΑΠΟΔΕΙΞΗ ΜΕΤΑΦΟΡΑΣ","ΕΙΣΙΤ.-ΑΠΟΔ.ΜΕΤ","1","1"},
             new String[] {"9CD7B603-5CF2-4DEF-89E9-3A68724C6020","-","211","ΕΚΚΑΘΑΡΙΣΗ (Ασφαλιστικών επιχειρήσεων)","ΕΚΚ.ΑΣΦ.ΕΠ","2","1"},
             new String[] {"EB12EF9C-19B4-4943-ACB5-3B39DC7938A2","12 ΠΑΡ 5,  6","58","ΤΙΜΟΛΟΓΙΟ ΑΓΟΡΑΣ (ΑΠΟ ΑΓΡΟΤΕΣ)","ΤΙΜ.ΑΓ.ΑΓΡ.","2","1"},
             new String[] {"45B15706-4BC0-4BA9-92D1-3C5A4112E964","1070526/496/0015/30-7-1999 (Ελαιουργεία)","268","ΑΠΟΔΕΙΞΗ ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ - ΔΕΛΤΙΟ ΠΟΣΟΤΙΚΗΣ ΠΑΡΑΛΑΒΗΣ","ΑΠΥ-ΔΕΛ.ΠΟΣ.ΠΑΡ","1","1"},
             new String[] {"F513F4A3-7787-4725-88D3-3C6F40C16810","-","251","ΑΠΟΔΕΙΞΗ ΑΥΤΟΠΑΡΑΔΟΣΗΣ - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ","ΑΠ.ΑΥΤΟΠΑΡ-Δ.Α.","1","1"},
             new String[] {"1EEE705A-5F83-4A9B-A6D1-3D2B18159421","Θεαμάτων","138","ΕΙΣΙΤΗΡΙΟ","ΕΙΣ.ΘΕΑΜ","1","1"},
             new String[] {"E5620A2A-D6F4-497F-8BFC-3D4C039E183C","Εισιτηρίων καλαθοσφ. - ΕΣΑΚ ΠΟΛ.1094/95","200","ΕΚΚΑΘΑΡΙΣΗ","ΕΚΚ.ΕΙΣ.ΚΑΛ","2","1"},
             new String[] {"6940CD3A-4F54-4C4D-9FA0-3EEFD1949A31","10 παρ. 5ηα","65","ΤΙΤΛΟΣ ΑΠΟΘΗΚΕΥΣΗΣ","ΤΙΤΛ. ΑΠΟΘ.","0","1"},
             new String[] {"0EEB37A2-01DA-442A-9239-3FC2B07E885F","ΠΟΛ 1151/06-06-2001 (ελαιοτριβεία)","279","ΑΠΟΔ ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ - ΔΕΛ ΠΟΣΟΤ ΠAΡΑΛΑΒΗΣ - ΔΕΛ ΑΠΟΣΤΟΛΗΣ","Α.Π.Υ-Δ.Π.Π-Δ.Α","1","1"},
             new String[] {"B2B3D46F-A0F4-446F-A052-3FF1DE3881B7","ΕΓΚ. 3 ΠΑΡ 13.2.5","147","ΕΙΣΙΤΗΡΙΟ ΥΔΡΟΘΕΡΑΠΕΙΑΣ","ΕΙΣ.ΥΔΡ.","1","1"},
             new String[] {"2FE0FDBF-7187-42F5-97CF-40FF1666243E","12 παρ. 1, 2","161","ΤΙΜΟΛΟΓΙΟ (Πώληση Αγαθών)","Τ.Π.","1","1"},
             new String[] {"D71C0235-C28B-42EE-9DD0-44423AAF0B6E","-","219","ΒΙΒΛΙΟ ΕΡΓΩΝ - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ","Β.ΕΡΓ.-Δ.Α.","0","1"},
             new String[] {"9FB6D3BD-F444-4145-8E63-4D9E4B1990B1","ΠΟΛ.176/77","56","ΑΠΟΔΕΙΞΗ ΕΠΙΣΤΡΟΦΗΣ ΑΣΦΑΛΙΣΤΡΩΝ","ΑΠΟΔ.ΕΠ.ΑΣΦ.","2","1"},
             new String[] {"F9FEF985-F441-4148-A3B0-4EF79248A54C","12 μικτή χρήση","165","ΤΙΜΟΛΟΓΙΟ","ΤΙΜ.","1","1"},
             new String[] {"49F8A2F4-E012-4902-86EB-50A417A3953A","13 παρ.1-3","174","ΑΠΟΔΕΙΞΗ ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ","Α.Π.Υ","1","1"},
             new String[] {"E02B4E2C-0662-4130-970C-55797067BA57","13α΄","177","ΔΕΛΤΙΟ ΚΙΝΗΣΗΣ ΤΟΥΡΙΣΤΙΚΩΝ ΛΕΩΦΟΡΕΙΩΝ","Δ.Κ.Τ.Λ","0","1"},
             new String[] {"1D76F86B-0C18-4E8D-A42C-57328A71F5A5","(Λοιπές Περιπτώσεις)","213","ΕΚΚΑΘΑΡΙΣΗ","ΕΚΚ.Λ.Π.","2","1"},
             new String[] {"06655D82-BE49-4243-8925-5B80A40736FD","ΠΟΛ 1063/13-04-05 και ΠΟΛ 1088/13-06-05","302","Δ. ΠΟΣΟΤΙΚΗΣ ΠΑΡΑΔΟΣΗΣ-ΑΠΟΔ. ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ (ΕΚΜ DVD κλπ)","ΔΠοσ Π/δ-ΑΠΥDVD","1","1"},
             new String[] {"7CDC08E5-AE0A-49DA-B92B-5BADB1B1CFE1","11 παρ.3","159","ΣΥΓΚΕΝΤΡΩΤΙΚΟ ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ","Σ.Δ.Α","0","1"},
             new String[] {"1C243458-8583-46BA-BA9B-5D157CEBC12D","-","233","ΑΠΟΔΕΙΞΗ ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ - ΑΠΟΔΕΙΞΗ ΛΙΑΝΙΚΗΣ ΠΩΛΗΣΗΣ","Α.Π.Υ-Α.Λ.Π","1","1"},
             new String[] {"E913FA9A-1A41-4313-8692-5ED7E42BA7BD","-","295","ΔΕΛΤΙΟ ΠΟΣΟΤΙΚΗΣ ΠΑΡΑΛΑΒΗΣ-ΤΙΜΟΛΟΓΙΟ ΑΓΟΡΑΣ","ΔΠΠ-ΤΙΜ ΑΓΟΡ","2","1"},
             new String[] {"F095E5A1-E37D-4AED-A2DF-5F7CA71D6A99","ΠΟΛ 1102/93","258","ΑΠΟΔΕΙΞΗ ΠΩΛΗΣΗΣ ΕΙΣΙΤΗΡΙΩΝ - ΑΠΟΔΕΙΞΗ ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ","ΑΠ.ΠΩΛ.ΕΙΣ.-ΑΠΥ","1","1"},
             new String[] {"AE0CA95C-5D92-4059-A9F5-5FA432320FEF","-","245","ΤΙΜΟΛΟΓΙΟ (Αγοράς Αγροτικών Προϊόντων) - ΕΚΚΑΘΑΡΙΣΗ","Τ(ΑΓ.ΑΓΡ.)-ΕΚΚ","2","1"},
             new String[] {"0FADD030-46DA-40D1-9210-603AA09E117A","ΠΟΛ. 66/88","210","ΕΚΚΑΘΑΡΙΣΗ (ΙΜΕ)","ΕΚΚ.ΙΜΕ","2","1"},
             new String[] {"B346F641-148A-45FF-97D9-60A9D85206C4","11 παρ.4","160","ΔΕΛΤΙΟ ΕΣΩΤΕΡΙΚΗΣ ΔΙΑΚΙΝΗΣΗΣ","Δ.Ε.Δ","0","1"},
             new String[] {"4F6DD00D-50B5-4EFD-BE80-63CC41BE52B9","Αρθρο 10 παρ. 5θ, 5ι","308","ΑΠΟΔ. ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ - Β. ΕΙΣΕΡΧΟΜΕΝΩΝ - Β. ΣΤΑΘΜΕΥΣΗΣ","ΑΠΥ-Β.ΣΤΑ-Β.ΕΙΣ","1","1"},
             new String[] {"6E8373BF-BB9F-415E-93D7-69BA85F11095","-","267","ΤΙΜΟΛΟΓΙΟ - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ -ΣΥΝΟΔΕΥΤΙΚΟ ΔΙΟΙΚΗΤΙΚΟ ΕΓΓΡΑΦΟ","Τ-Δ.Α.-ΣΥΝ.Δ.ΕΓ","1","1"},
             new String[] {"4560ACE7-09E2-48DF-8F80-6F67279C9AB9","13 παρ. 1-3","173","ΑΠΟΔΕΙΞΗ ΛΙΑΝΙΚΗΣ ΠΩΛΗΣΗΣ","Α.Λ.Π","1","1"},
             new String[] {"8B5F89FE-25D3-4EC1-A9DB-7474698159AB","12 παρ.13","169","ΠΙΣΤΩΤΙΚΟ ΤΙΜΟΛΟΓΙΟ","Π.Τ","2","1"},
             new String[] {"986B20FB-8035-4F14-B012-74D7732439BA","Αρθ.10παρ.5ιζ & Αρθ.19παρ.4 & ΠΟΛ1219/96","260","ΒΙΒΛΙΟ ΕΡΓΩΝ-ΤΙΜΟΛΟΓΙΟ-ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ","Β.ΕΡΓ-ΤΙΜ-ΔΑ","1","1"},
             new String[] {"E1354B6E-CA05-49EF-A115-798D8DA9F62E","αρθρο 29 παρ.3 του Ν 3522/06","317","ΣΥΓΚΕΝΤΡΩΤΙΚΟ ΔΕΛΤΙΟ ΕΠΙΣΤΡΟΦΗΣ","Σ.Δ.Ε.","0","1"},
             new String[] {"EBFE441E-4121-40D5-8BD0-7A0EACEBDEE8","-","257","ΑΠΟΔΕΙΞΗ ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ-ΤΙΜΟΛ(Πώλησης)-ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ","Α.Π.Υ-Τ.Π.-Δ.Α.","1","1"},
             new String[] {"365723C0-AD1B-4CF5-B28E-7ECA97516DF6","ΡΘΡΟ 23 ΠΑΡ. 5","215","ΕΙΔΙΚΟ ΑΚΥΡΩΤΙΚΟ ΣΤΟΙΧΕΙΟ","ΕΙΔ.ΑΚ.ΣΤ.","3","1"},
             new String[] {"2A0EC1B7-C527-4DEF-ADD8-7EFF22045A45","-","224","ΤΙΜΟΛΟΓΙΟ (Αγοράς από ιδιώτη) - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ","Τ(ΑΓ)-Δ.Α.","2","1"},
             new String[] {"3A402988-07CA-4616-BF43-7F2F4E5CC77D","-","501","ΛΟΙΠΑ ΣΤΟΙΧΕΙΑ","-","0","1"},
             new String[] {"7E2B7F3B-0D22-4627-B2A3-818C60F3B6DC","Αρθ. 14 Π.Δ 186/92","178","ΑΠΟΔΕΙΞΗ ΑΥΤΟΠΑΡΑΔΟΣΗΣ","Α.ΑΥΤΟΠΑΡ","1","1"},
             new String[] {"84165BE1-5F58-43D6-BDE4-823053426621","12 παρ. 5,  11 παρ. 1","272","ΤΙΜΟΛΟΓΙΟ ΑΓΟΡΑΣ - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ","ΤΙΜ.ΑΓ.-Δ.Α.","2","1"},
             new String[] {"A2D225CD-E507-423E-890B-8AD35BEDFAEB","Συνδρομών εφημερίδων και περιοδικών","176","ΑΠΟΔΕΙΞΗ ΕΙΣΠΡΑΞΗΣ","Α.ΕΙΣ.ΣΥΝΔ","1","1"},
             new String[] {"4C9D3CDF-E052-4894-A149-8F1AA8981A18","-","238","ΒΙΒΛΙΟ ΑΠΟΘΗΚΕΥΣΗΣ - ΤΙΤΛΟΣ ΑΠΟΘΗΚΕΥΣΗΣ","Β.ΑΠΟΘ-ΤΙΤ.ΑΠΟΘ","0","1"},
             new String[] {"0D63AB7B-E9EC-496C-B264-9320EB803778","-","221","ΤΙΜΟΛΟΓΙΟ -ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ","Τ-Δ.Α.","1","1"},
             new String[] {"89CF5539-EE34-4439-8EA7-94578676355F","-","222","ΤΙΜΟΛΟΓΙΟ (Πώληση Αγαθών) - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ","Τ.Π.-Δ.Α.","1","1"},
             new String[] {"1EAE92D4-EE7E-425C-8161-94C41FB32AE3","12 ΠΑΡ 5","60","ΤΙΜΟΛΟΓΙΟ ΑΓΟΡΑΣ (ΑΠΟ ΙΔΙΩΤΕΣ)","ΤΙΜ.ΑΓ.ΙΔΙΩΤ.","2","1"},
             new String[] {"BFFF0575-7865-46FD-A884-9AC79A61423F","12 ΠΑΡ. 5","62","ΤΙΜΟΛΟΓΙΟ ΑΓΟΡΑΣ (ΑΠΟ ΑΡΝΟΥΜΕΝΟΥΣ)","ΤΙΜ.ΑΓΟΡ.ΑΡΝ.","2","1"},
             new String[] {"AC63A669-5AD4-451B-B62E-9D126D07DDA3","-","223","ΤΙΜΟΛΟΓΙΟ (Παροχης Υπηρεσιων) - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ","Τ.Π.Υ.-Δ.Α.","1","1"},
             new String[] {"2578B628-8567-4E02-8EEE-9EFC4D8DA399","ΠΟΛ 1063/13-04-05 και ΠΟΛ 1088/13-06-05","298","ΔΕΛΤΙΟ ΠΟΣΟΤΙΚΗΣ ΠΑΡΑΔΟΣΗΣ (ΕΚΜ DVD κ.λ.π.)","ΔΠΠ/Δ ΕΚΜ DVD","0","1"},
             new String[] {"CCE3CAF2-559E-4A89-B5EA-9F3F21220DCD","-","229","ΔΕΛΤΙΟ ΠΟΣΟΤΙΚΗΣ ΠΑΡΑΛΑΒΗΣ - ΠΙΣΤΩΤΙΚΟ ΤΙΜΟΛΟΓΙΟ","ΔΕΛ.ΠΟΣ.ΠΑΡ-Π.Τ","2","1"},
             new String[] {"D688D02B-9FD2-426D-9518-9F41A6721C8A","-","226","ΤΙΜΟΛΟΓΙΟ (Παροχή Υπηρεσιων) - ΤΙΜΟΛΟΓΙΟ (Πώληση Αγαθών)","ΤΠ.Υ.-Τ.Π.","1","1"},
             new String[] {"08036F0E-2CBA-49CE-8112-A108BB08C432","ΠΟΛ.1104/94","197","ΕΚΚΑΘΑΡΙΣΗ (Καρτών σταθμ. δημ. χωρ.)","ΕΚΚ.Κ.ΣΤΑΘ","2","1"},
             new String[] {"3FDC78D9-6243-4834-B446-A127F886EDD2","12 παρ. 1, 2","162","ΤΙΜΟΛΟΓΙΟ (Παροχή Υπηρεσιών)","Τ.Π.Υ","1","1"},
             new String[] {"6574C197-296E-46FE-97EA-A26CB9088F5A","ΠΟΛ 1063/13-04-05 και ΠΟΛ 1088/13-06-05","301","Δ.ΠΟΣΟΤΙΚΗΣ ΠΑΡΑΛΑΒΗΣ-ΑΠΟΔ. ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ (ΕΚΜ DVD κλπ)","ΔΠοσ.Παρ-ΑΠΥDVD","1","1"},
             new String[] {"D35B41C5-6C2C-4D5E-BEA2-A317478FAC71","Κέντρων διασκέδασης κ.λ.π. ΠΟΛ.1159/94","137","ΕΙΣΙΤΗΡΙΟ","ΕΙΣ.Κ.ΔΙΑΣΚ","1","1"},
             new String[] {"F0DDD9A5-9FE7-4C9B-B1A2-A4D81AE101ED","10 παρ 5 ηα","63","ΔΕΛΤΙΟ ΕΙΣΑΓΩΓΗΣ","ΔΕΛ.ΕΙΣΑΓ.","0","1"},
             new String[] {"FD5BCEFA-7EAF-45A0-B1EF-A6BD04469879","Αρθ. 14 Π.Δ 186/92","179","ΑΠΟΔΕΙΞΗ ΔΑΠΑΝΗΣ","Α.ΔΑΠΑΝΗΣ","2","1"},
             new String[] {"6DA11F85-CE8E-49D6-9472-A7D248FE5805","Αθεώρητο απο 1-1-03 (ΠΟΛ 1166/2002)","252","ΑΠΟΔΕΙΞΗ ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ-ΜΙΣΘΩΤΗΡΙΟ ΣΥΜΒΟΛΑΙΟ ΑΥΤΟΚΙΝΗΤΩΝ","ΑΠΥ-ΜΙΣΘΩΤΗΡΙΟ","1","1"},
             new String[] {"CB942712-25F0-41B6-9CC9-AC0DA9427054","-","218","ΒΙΒΛΙΟ ΣΤΑΘΜΕΥΣΗΣ -ΦΥΛΑΞΗΣ ΣΚΑΦΩΝ -ΑΠΟΔΕΙΞΗ ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩ","Β.ΣΤ.ΣΚ-Α.Π.Υ.","1","1"},
             new String[] {"EF648710-B302-4F22-91E5-B141AD20F997","Πωλ.Ανθέων μέσω ΙΝΤΕΡΦΛΟΡΑ ΠΟΛ.1212/94","199","ΕΚΚΑΘΑΡΙΣΗ","ΕΚΚ.Π.ΑΝΘ","2","1"},
             new String[] {"4D5FBB78-833A-440E-A6FD-B733E6F68344","-","217","ΒΙΒΛΙΟ ΣΤΑΘΜΕΥΣΗΣ (οχηματων) - ΑΠΟΔΕΙΞΗ ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ","Β.ΣΤ.-Α.Π.Υ.","1","1"},
             new String[] {"2C88CA0F-37CA-47A5-A8C5-BBA488E432A9","αρθρο 10 παρ. 5ε","313","ΒΙΒΛΙΟ ΠΕΛΑΤΩΝ-ΑΠΟΔΕΙΞΗ ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ","Β.ΠΕΛ-ΑΠΥ","1","1"},
             new String[] {"3931ED74-23C0-4319-B863-BC1D77248628","ΠΟΛ 1063/13-04-05 και ΠΟΛ 1088/13-06-05","303","Δ. ΠΟΣΟΤΙΚΗΣ ΠΑΡΑΔΟΣΗΣ - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ (ΕΚΜ DVD κλπ)","ΔΠοσ.Παρ-ΔΑ DVD","0","1"},
             new String[] {"F78B0349-6345-4C77-9775-C4630CEBF537","ΠΟΛ.176/77","54","ΑΠΟΔΕΙΞΗ ΑΣΦΑΛΙΣΤΡΩΝ","ΑΠΟΔ.ΑΣΦ.","1","1"},
             new String[] {"042B7F43-6853-4FBE-99B6-C4B94A14AFAC","-","232","ΑΠΟΔΕΙΞΗ ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ","Α.Π.Υ-Δ.Α","1","1"},
             new String[] {"78E88F81-AF67-4426-B6D1-C6A75B139427","Αρθ.16","246","ΑΠΟΔΕΙΞΗ ΚΑΙ ΗΜΕΡΟΛΟΓΙΟ ΜΕΤΑΦΟΡΑΣ","ΑΠΟΔ & ΗΜ.ΜΕΤΑΦ","1","1"},
             new String[] {"5ADBA305-911F-43D8-9A89-C974DAF7FD1A","12 παρ.7, 8","168","ΕΚΚΑΘΑΡΙΣΗ","ΕΚΚΑΘ","2","1"},
             new String[] {"B270DC84-1725-4DA4-B450-CAC102C31141","12 παρ. 5","270","ΤΙΜΟΛΟΓΙΟ ΑΓΟΡΑΣ","ΤΙΜ.ΑΓΟΡ","2","1"},
             new String[] {"D2B7304F-325B-459D-BA57-D8243069DCA2","10§1 ΠΔ186/92 και2§11ν.3052/02 απο1-1-03","40","ΔΕΛΤΙΟ ΠΟΣΟΤΙΚΗΣ ΠΑΡΑΛΑΒΗΣ","ΔΕΛΤ.ΠΟΣ.ΠΑΡ.","0","1"},
             new String[] {"21253F71-85D2-480A-A703-DA12DF14F74A","Στάθμευσης δημοσίων χώρων ΠΟΛ.1104/94","196","ΑΠΟΔΕΙΞΗ ΠΑΡΑΔΟΘΕΙΣΩΝ ΚΑΡΤΩΝ","Α.ΠΑΡ.ΚΑΡΤ.","1","1"},
             new String[] {"9660195E-E1E5-4D9A-BFE5-E35C0EFC6286","ΠΟΛ.1292/93","194","ΕΚΚΑΘΑΡΙΣΗ (Εφημερίδων και περιοδικών)","ΕΚΚ.ΕΦ.ΠΕΡ.","2","1"},
             new String[] {"79C3DE12-DB95-4746-8E6B-E382403825A3","ΠΟΛ 1063/13-04-05 και ΠΟΛ 1088/13-06-05","299","ΔΕΛΤΙΟ ΠΟΣΟΤΙΚΗΣ ΠΑΡΑΛΑΒΗΣ (ΕΚΜ DVD κ.λ.π.)","ΔΠΠ/Λ ΕΚΜ DVD","0","1"},
             new String[] {"883030E5-31E9-499D-9470-E5A91BC18217","-","228","ΕΚΚΑΘΑΡΙΣΗ - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ","ΕΚΚ-Δ.Α.","2","1"},
             new String[] {"80E0132C-0630-4679-BAD0-E65C10A42184","ΠΟΛ 61/88","209","ΕΚΚΑΘΑΡΙΣΗ (ΚΤΕΛ)","ΕΚΚ.ΚΤΕΛ","2","1"},
             new String[] {"0D51F1EE-F7C2-499F-BF84-E9C55FD3AB62","ΠΟΛ 1063/13-04-05 και ΠΟΛ 1088/13-06-05","306","(ΕΚΜDVD) Δ.ΑΠΟΣΤΟΛΗΣ-Δ.ΠΟΣΟΤ ΠΑΡΑΛΑΒΗΣ-ΑΠΟΔ ΠΑΡ. ΥΠΗΡΕΣΙΩΝ","ΔΠΠ-ΔΑ-ΑΠΥ DVD","1","1"},
             new String[] {"B532CBBF-A920-459C-A9D2-EA1824C21ED4","16 παρ.10","182","ΔΙΟΡΘΩΤΙΚΟ ΣΗΜΕΙΩΜΑ","ΔΙ.ΣΗΜ","2","1"},
             new String[] {"A0BFC722-B3E0-4F11-B28B-EB321B82B045","Εισιτήρια Αστικών Συγκοινωνιών","192","ΑΠΟΔΕΙΞΗ ΠΑΡΑΔΟΘΕΝΤΩΝ ΕΙΣΙΤΗΡΙΩΝ","Α.ΠΑΡ.ΕΙΣ","1","1"},
             new String[] {"884C8FF0-FFB3-4382-BB7E-EC5F5324763D","-","236","ΠΙΣΤΩΤΙΚΟ ΤΙΜΟΛΟΓΙΟ - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ","ΠΙΣΤ.Τ-Δ.Α","2","1"},
             new String[] {"0F860213-7C9B-440E-86D5-ECBE646D4AFD","13 παρ.1","175","ΑΠΟΔΕΙΞΗ ΕΠΙΣΤΡΟΦΗΣ","Α.ΕΠΙΣΤΡ","2","1"},
             new String[] {"BD543ABD-F41E-445D-BD0C-F0936507B3B7","ΠΟΛ 1063/13-04-05 και ΠΟΛ 1088/13-06-05","304","Δ. ΠΟΣΟΤΙΚΗΣ ΠΑΡΑΛΑΒΗΣ - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ (ΕΚΜ DVD κλπ)","ΔΠοσ Παρ-ΔΑ DVD","0","1"},
             new String[] {"8DE707DD-4590-4E1A-A58C-F36110E3869C","10 παρ. 5β,  13,  19 παρ. 4","247","ΜΗΤΡΩΟ ΜΑΘΗΤΩΝ - ΑΠΟΔΕΙΞΗ ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ","ΜΗΤΡ.ΜΑΘ-Α.Π.Υ.","1","1"},
             new String[] {"23E8375E-569E-424B-8DA6-F3BDA30FE466","ΠΟΛ 1063/13-04-05 και ΠΟΛ 1088/13-06-05","307","(ΕΚΜDVD) Δ.ΑΠΟΣΤΟΛΗΣ-Δ.ΠΟΣΟΤ ΠΑΡΑΔΟΣΗΣ-ΑΠΟΔ ΠΑΡ. ΥΠΗΡΕΣΙΩΝ","ΔΠΠΔ-ΔΑ-ΑΠΥ DVD","1","1"},
             new String[] {"FC973870-8F44-4EB4-90D5-F6B0D41DADF4","1215/96","212","ΕΚΚΑΘΑΡΙΣΗ (Κοινοπραξιών πλοίων)","ΕΚΚ.Κ.ΠΛΟΙΩΝ","2","1"},
             new String[] {"71DD75B1-65A2-4213-8CFB-F8433022579B","Εστιατ.ζυθεστ.κ.λ.π. ΠΟΛ.1117/91, 1238/92","188","ΔΕΛΤΙΟ ΠΑΡΑΓΓΕΛΙΑΣ","ΔΕΛ.ΠΑΡΑΓ.","0","1"},
             new String[] {"9D2F2EEA-EBAC-45C6-80F0-F8E6F9F577D7","ΠΟΛ.1319/93","195","ΕΚΚΑΘΑΡΙΣΗ (Μεριδίων αμοιβ. Κεφαλαίων)","ΕΚΚ.Μ.Α.Κ","2","1"},
             new String[] {"6B37B033-5020-4257-AD63-FBBB2F94381F","-","237","ΦΟΡΤΩΤΙΚΗ - ΤΙΜΟΛΟΓΙΟ (Παροχή Υπηρεσιων)","ΦΟΡΤ-Τ.Π.Υ","1","1"},
             new String[] {"15B84F62-84AD-4DD0-806A-FFD93FB1BC34","άρθρο 16 παρ. 13","240","ΣΥΓΚΕΝΤΡΩΤΙΚΗ ΦΟΡΤΩΤΙΚΗ","ΣΥΓΚ. ΦΟΡΤ.","1","1"}
        };


        private readonly String[] TablesWithOwner = new String[]{
            "AddressType",
            "BarcodeType",
            "Buyer",
            "CategoryNode",
            "DataFileRecordHeader",
            "DocumentStatus",
            "DeliveryType",
            "Offer",
            "PaymentMethod",
            "Phone",
            "PhoneType",
            "PriceCatalog",
            "Role",
            "Seasonality",
            "VATCategory",
            "VATLevel",
            "VATFactor"
        };
        
        public override void Up()
        {
            //Terminal
            //Terminal. Code, TerminalTYpe, Customer
            if (Schema.Table("Terminal").Column("Code").Exists())
            {
                Delete.Column("Code").FromTable("Terminal");
            }
            if (Schema.Table("Terminal").Column("TerminalType").Exists())
            {
                Delete.Column("TerminalType").FromTable("Terminal");
            }
            if (Schema.Table("Terminal").Column("Customer").Exists())
            {
                Delete.Column("Customer").FromTable("Terminal");
            }
            

            //Ownership

            foreach (String tableowner in this.TablesWithOwner)
            {
                if (Schema.Table(tableowner).Column("Owner").Exists() == false)
                {
                    Alter.Table(tableowner).AddColumn("Owner").AsGuid().Nullable();
                    Execute.Sql(String.Format("update {0} set Owner = (select top 1 Oid from CompanyNew)", tableowner));
                }
            }

            Execute.Sql(String.Format("update {0} set Owner = (select top 1 Oid from CompanyNew)", "Customer"));
          

            //UserTypeAccess

            Update.Table("UserTypeAccess").Set(new { EntityType = "ITS.Retail.Model.CompanyNew" }).Where(new { EntityType = "ITS.RetailModel.Supplier" });
            Update.Table("UserTypeAccess").Set(new { EntityType = "ITS.Retail.Model.Customer" }).Where(new { EntityType = "ITS.RetailModel.Customer" });
            Update.Table("UserTypeAccess").Set(new { EntityType = "ITS.Retail.Model.Store" }).Where(new { EntityType = "ITS.RetailModel.Store" });
            Update.Table("DataFileRecordHeader").Set(new { EntiyName = "SupplierNew" }).Where(new { EntiyName = "Supplier" });

            Execute.Sql("update XPObjectType set AssemblyName = 'ITS.Retail.Model'");
            Execute.Sql("update XPObjectType set TypeName=REPLACE(TypeName, 'ITS.RetailModel','ITS.Retail.Model')");

            //Store.PriceCatalog
            if (Schema.Table("Store").Column("DefaultPriceCatalog").Exists() == false)
            {
                Alter.Table("Store").AddColumn("DefaultPriceCatalog").AsGuid().Nullable();

                //Execute.Sql("update Store set DefaultPriceCatalog = (select top 1 oid from PriceCatalog where PriceCatalog.ParentCatalog is null)");
            }

            //DocumentPayment
            if (Schema.Table("DocumentPayment").Column("Periods").Exists())
            {
                Delete.Column("Periods").FromTable("DocumentPayment");
            }
            if (Schema.Table("DocumentPayment").Column("Ammount").Exists())
            {
                Delete.Column("Ammount").FromTable("DocumentPayment");
            }
            if (Schema.Table("DocumentPayment").Column("Total").Exists())
            {
                Delete.Column("Total").FromTable("DocumentPayment");
            }
            if (Schema.Table("DocumentPayment").Column("CardNumber").Exists())
            {
                Delete.Column("CardNumber").FromTable("DocumentPayment");
            }
            if (Schema.Table("DocumentPayment").Column("ExpiredDate").Exists())
            {
                Delete.Column("ExpiredDate").FromTable("DocumentPayment");
            }
            if (Schema.Table("DocumentPayment").Column("HolderName").Exists())
            {
                Delete.Column("HolderName").FromTable("DocumentPayment");
            }
            if (Schema.Table("DocumentPayment").Column("CouponNumber").Exists())
            {
                Delete.Column("CouponNumber").FromTable("DocumentPayment");
            }

            if (Schema.Table("DocumentHeader").Column("Terminal").Exists())
            {
                Rename.Column("Terminal").OnTable("DocumentHeader").To("POS");
            }

            //Oid, [Description], Code,Title, ShortTitle, DocumentValueFactor,IsSupported  from MinistryDocumentType
            if (Schema.Table("MinistryDocumentType").Exists() == false)
            {
                Create.Table("MinistryDocumentType")
                    .WithColumn("Oid").AsGuid().NotNullable().PrimaryKey()
                    .WithColumn("Description").AsString(1024)
                    .WithColumn("Code").AsString(100)
                    .WithColumn("Title").AsString(100)
                    .WithColumn("ShortTitle").AsString(100)
                    .WithColumn("DocumentValueFactor").AsInt32()
                    .WithColumn("IsSupported").AsBoolean().Nullable()
                    .WithColumn("CreatedOnTicks").AsCustom("bigint")
                    .WithColumn("UpdatedOnTicks").AsCustom("bigint");


                foreach (String[] mdoctype in this.MinistryDocumentTypes)
                {
                    Insert.IntoTable("MinistryDocumentType").Row(new
                    {
                        Oid = Guid.NewGuid(),
                        Description = mdoctype[1],
                        Code = mdoctype[2],
                        Title = mdoctype[3],
                        ShortTitle = mdoctype[4],
                        DocumentValueFactor = Int32.Parse(mdoctype[5]),
                        IsSupported = mdoctype[6] == "1",
                        CreatedOnTicks = DateTime.Now.Ticks,
                        UpdatedOnTicks = DateTime.Now.Ticks

                    });
                }
            }

        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
