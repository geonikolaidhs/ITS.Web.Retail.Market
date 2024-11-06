using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Hardware.Wincor.Fiscal
{
    public class WincorFiscalPrinterStatus : FiscalPrinterStatus
    {
        public bool IsBlocked { get; set; }
        public bool InTrainingMode { get; set; }
        public bool DocumentOpen { get; set; }
        public bool InServiceReset { get; set; }
        public bool PrintoutInterrupted { get; set; }
        public bool CmdInterrupted { get; set; }
        public bool FiscalFileNearlyFull { get; set; }
        public bool EJDataPending { get; set; }
        //public int FiscalReceiptNumber { get; set; }
        //public string SerialNumber { get; set; }

        /// <summary>
        /// Fiscal commands are not available if the MFC is in the Non Fiscal Mode.
        /// </summary>
        public bool InFiscalMode { get; set; }
        //public bool DeviceBusy { get; set; }
        //public bool FatalError { get; set; }
        //public bool BatteryWarning { get; set; }
        //public bool PrinterOffline { get; set; }
        //public bool PrinterTimeout { get; set; }
        //public bool CutterError { get; set; }

        //public bool CashOutOpen { get; set; }
        //public bool CashInOpen { get; set; }
        //public bool DrawerOpen { get; set; }

        public static Dictionary<int, string> ErrorCodeDescriptionMapping = new Dictionary<int, string>() { 
            { 0x00,"None"},
            { 0x10 , "This error is transmitted with the first ASB message, which is send from the MFC to the Host after power on. The application must initialize the printer and the MFC if necessary."},
            { 0x11 , "During the power-on test a CPU error was detected. Replace the microprocessor on the MFC board or change the whole MFC board before executing a TKD reset."},
            { 0x12 , "During the power-on test a write/read error was detected in the battery buffered CMOS RAM. The power-on test does not destroy the RAM Data. Replace the CMOS RAM on the MFC board or change the whole MFC board before executing a TKD reset."},
            { 0x13 , "During the power-on test or during the TEST_SELFTEST command a checksum error was detected in the EPROM (program memory). Replace the EPROM on the MFC board before executing a TKD reset."},
            { 0x14 , "The battery, which supplies the CMOS RAM and the RTC during power-off, is empty. Replace the battery on the MF board and set the current time before executing a TKD reset."},
            { 0x15 , "The RAM redundancy test fails during the power-on test. The RAM content is lost due to a battery extraction or a wrong MFC board handling. Set the current time and execute a TKD reset."},
            { 0x16 , "During the power-on test a write/read error was detected in the battery buffered RAM of the RTC. Replace the RTC on the MFC board before executing a TKD reset."},
            { 0x17 , "The FW was changed while the Day was open (MFC Status Bit MFC_DAY_OPEN = 1). The Totalizers of the current day are lost. Execute a TKD reset."},
            { 0x21 , "The MFMEM print isn't connected to the MFC (this is detected by write/read to the shift register of the MFMEM board) or the memory chip isn't assembled (this is detected by reading only 0xff in ReadHwData ()) on the MFMEM print. Connect a fully assembled MFMEM board to the MFC before executing a TKD reset."},
            { 0x22 , "It wasn't possible to store an MFMEM byte. Check the MFMEM board, the MFC (especially the program voltage) and the data connection between the two boards. If the problem is solved, execute a TKD reset."},
            { 0x23 , "The maximum amount of TKD resets is reached. Replace the MFMEM board by a new one and start the initialisation procedure."},
            { 0x24 , "The MFMEM is defect which can be seen in Byte 0 of hw data. Replace the MFMEM board by a new one and start the initialisation procedure."},
            { 0x25 , "The checksum of one MFMEM block is wrong. Replace the MFMEM board by a new one and start the initialisation procedure."},
            { 0x26 , "The maximum amount of MFMEM blocks, which could be stored in the MFMEM, is reached. Replace the MFMEM board by a new one and start the initialisation procedure."},
            { 0x27 , "Before storing a MFMEM block, the FW detects that auiMFMEMPtr [NEXT_BL] doesn't point to an unused block or the last block isn't in use. Execute a TKD reset."},
            { 0x28 , "The serial number in the battery buffered RAM of the MFC is not the same as the serial number which is stored in the SNO block of the connected MFMEM board. Execute a TKD reset."},
            { 0x29 , "1. the checksum of the HWData, which are transmitted with the INIT_HWDATA command, is wrong. 2. After power-on the error will be set if the checksum of the HWData, which are stored in the MFMEM, is wrong. Send an INIT_HWDATA command with a correct checksum for the HWData and then replace the MFMEM board by a new one and start the initialisation procedure."},
            { 0x2A , "After 3 attempts it wasn't possible to strore a MFMEM block. Check the MFMEM board, the MFC (especially the program voltage) and the data connection between the two boards. Use the test command TEST_READ_MFMEM_BL to analyze the problem. If the problem is solved, execute a TKD reset."},
            { 0x2B , "The maximal amount of a special MFMEM block type, which could be stored in the MFMEM, is reached (e.g. in Greece changes of TH and VAT rates). If the storage of the block is absolut necessary (e.g. VAT change) the MFMEM must be replaced by a new one. If it's not absolutely necessary to store the block (e.g. TH change) it is possible to go on working."},
            { 0x2C , "After 3 attempts it wasn't possible to read a MFMEM block. Check the MFMEM board, the MFC and the data connection between the two boards. Use the test command TEST_READ_MFMEM_BL to analyze the problem. If the problem is solved, execute a TKD reset."},
            { 0x2D , "The EJ medium (CF or MMC) or the interface has a hardware defect. Replace the EJ medium or the MFC main board and execute a TKD reset."},
            { 0x2E , "The smart card medium has a hardware defect. Replace the smart card together with the MFMEM and execute a TKD reset."},
            { 0x2F , "EJMEM Data changed or manipulated. Execute a TKD reset."},
            { 0x31 , "The printer doesn't give an answer to a \"Transmit real time printer status\" (GS ENQ). Check the data connection between the MFC and the printer and execute the adequate error recovery procedure described in the ERRORHANDLING chapter for the Printer Recoverable Hardware Error event (PRT_RECOV_ERR)."},
            { 0x32 , "used only in BEETLE/50 and BEETLE/60"},
            { 0x33 , "The printer ASB message indicates a \"mechanical error\" (head temperature high) or a \"error which is impossible to recover\" (e.g.voltage of power supply wrong). A mechanical error could be caused by a paper jam. After eliminating the problem the printer could be recovered by sending a <DLE><ENQ> 02H. The \"unrecoverable error\" could only be eliminated by switching off the printer, solve the problem and switching on."},
            { 0x34 , "used only in BEETLE/50 and BEETLE/60"},
            { 0x35 , "The inserted CF generates an error with low severity during the formatting procedure. Replace the inserted CF with a new one."},
            { 0x36 , "Out of paper. Insert a new paper roll."}, //1. In a RETAIL_SALES_RECEIPT or a SERVICE_SALES_RECEIPT a paper end situation is detected. 2. Before executing a BEGIN_RECEIPT, DAY_END or a REPORT command a paper near end situation is detected. 
            { 0x37 , "used only in BEETLE/50 and BEETLE/60"},
            { 0x38 , "The EJMEM is removed from the controller or the EJMEM interface or EJMEM itself is wrong. Power off the controller, insert the EJMEM, power on the controller. If the Journal Period is started the EJMEM can not be changed."},
            { 0x39 , "The EJMEM was changed in a Journal Period started status. Power off the controller, reinsert the changed Compact Flash, power on the controller."},
            { 0x3A , "The CF was changed with a not formatted one in a Journal Period not started status. (Not used in Greece). Power off the controller, insert a CF formatted by another device, power on the controller. In this case the controller will format the CF and will create a new journal file with the incremented EOD block counter."},
            { 0x3B , "Not used."},
            { 0x40 , "The display configuration isn't or is wrong defined with the last MFC_OPEN command. One of the displays is as not connected defined. Send the corrected MFC_OPEN command."},
            { 0x41 , "The operator display isn't connected to the internal interface of the system main board (this check is only possible with a compact system). Connect the operator display to the internal interface of the system main board and send again the MFC_OPEN command or send a corrected MFC_OPEN command, if the operator display is connected to another interface."},
            { 0x42 , "The customer display isn't connected to the internal interface of the system main board (this check is only possible with a compact system). Connect the customer display to the internal interface of the system main board and send again the MFC_OPEN command or send a corrected MFC_OPEN command, if the customer display is connected to another interface."},
            { 0x43 , "The customer display isn't connected to the COM4 interface of the system main board (this check is only possible with a compact system). Connect the customer display to the COM4 interface of the system main board and send again the MFC_OPEN command or send a corrected MFC_OPEN command, if the customer display is connected to another interface."},
            { 0x44 , "The operator display isn't connected to the COM3 interface of the system main board (this check is only possible with a compact system). Connect the operator display to the COM3 interface of the system main board and send again the MFC_OPEN command or send a corrected MFC_OPEN command, if the operator display is connected to another interface."},
            { 0x45 , "The customer display isn't connected to the RS232 interface of the MFC. Connect the customer display to the RS232 interface of the MFC and send again the MFC_OPEN command or send a corrected MFC_OPEN command, if the customer display is connected to another interface."},
            { 0x46 , "The customer display isn't connected to the display interface of the modular printer. Connect the customer display to the display interface of the modular printer and send again the MFC_OPEN command or send a corrected MFC_OPEN command, if the customer display is connected to another interface."},
            { 0x50 , "The SNO is not stored in MFMEM. Fiscal commands are only allowed after SNO is present. Set SNO for fiscal command apply."},
            { 0x51 , "A fiscal command (ESC MFB ....) was interrupted by a MFC command (ESC "+" .....). Send a corrected command."},
            { 0x52 , "The command is at the current MFC status not available. The MFC status depends on the two MFC_STATUSBYTES (e.g. a DAY_BEGIN command is only available if the MFC_OPEN bit is cleared). Send a command which is allowed at the current MFC status."},
            { 0x53 , "The fiscal command (ESC MFB .... ESC MFE) is longer than 256 byte. Send a corrected command."},
            { 0x54 , "One of the parameter types, which is specified by the two ASCII values after the MFB1 inside the fiscal sequence, isn't transmitted with the command but is required. Send a corrected command."},
            { 0x55 , "DAY_BEGIN command will be rejected with this error message if no VAT rates stored in the MFMEM. Execute SET_VATRATES command."},
            { 0x56 , "DAY_BEGIN command will be rejected with this error message if no TH is stored in the MFC/MFMEM. Hint: A TKD reset erases the TH. The TH could be restored from the MFMEM. Execute command SET_HEADER"},
            { 0x57 , "One of the transmitted parameters <Date>, <Time> or <TimeDate> has a wrong format. Send a corrected command."},
            { 0x58 , "In the connected MFMEM are no hardware data stored. Send an INIT_HWDATA command."},
            { 0x59 , "The connected MFMEM isn't fully formatted. Send a INIT_FORMAT_MFMEM command for fully formatting."},
            { 0x5A , "The fiscal command, which is specified by the two ASCII values after the MFB inside the fiscal sequence, isn't available. Send a corrected command."},
            { 0x5B , "In the REPORT command is the parameter <FromDate> greater than the parameter <ToDate> or the parameter <FromBlock> is greater than the parameter <ToBlock>. Send a corrected command."},
            { 0x5C , "One of the parameter types, which is specified by the two ASCII values after the MFB1 or MFB2 inside the fiscal sequence, isn't required inside the transmitted command. Send a corrected command."},
            { 0x5D , "One of the parameters has the wrong contents, the wrong length or isn't transmitted. Send a corrected command."},
            { 0x5E , "One of the parameter types, which is specified by the two ASCII values after the MFB1 or MFB2 inside the fiscal sequence, isn't transmitted with the command but is required. Send a corrected command."},
            { 0x5F , "One of the parameter types, which are specified by the two ASCII values after the MFB1 or MFB2 inside the fiscal sequence, isn’t available. Send a corrected command."},
            { 0x60 , "The LRC computed by the MFC does not match with the LRC received. This error can be generated by a transmission error or a wrong LRC computation.(Not used in Greece). Resend the command with a correct LRC."},
            { 0x61 , "The date and time of the RTC isn't set. This will be checked during power-on, TKD Reset and before storing the SNO block in the MFMEM. Set the date and time of the RTC with the command SET_TIME."},
            { 0x62 , "The command SET_TIME is rejected, because the time is set before the last block date. Set the date and time of the RTC with the command SET_TIME, which is equal or greater than the date of the last block."},
            { 0x63 , "DAY_BEGIN command will be rejected with this error message if no BBD info is stored in the MF. Execute the command SET_CURRENCY_CHANGE_DATE."},
            { 0x64 , "The EJMEM free space is less then 6kB: See Flowchart chapter 7.13"},
            { 0x65 , "In the EJ_GET_EOD_ENTRY or EJ_GET_DATA commands the ' <FromBlock>' parameter points to a not found EJ file. Send a corrected command."},
            { 0x66 , "In the EJ_GET_DATA command the '<Offset>' parameter is higher then the EJ file size or is not a multiple of 512. Send a corrected command."},
            { 0x67 , "Used in Italy"},
            { 0x68 , "The Smart Card installed in the MFC isn't prepared for using in the field. (The Smart Card PERSO bit is not set). Prepare the Smart Card with the necessary tool for field usage."},
            { 0x69 , "Last smart card command generates an error. This error can be read by the EJ_GET_SMART_CARD_ERROR command. Reset the smart card and repeat the last command."},
            { 0x6A , "Last CF test command generates an error. This error can be read by the TEST_CF_GET_INTERFACE_ERROR test command. Execute the TEST_CF_GET_INTERFACE_ERROR command and analyse the returned error codes."},
            { 0x6B , "In the EJ_CONFIRM_EJMEM_DATA_TRANSFERRED command, the '<EJFileMessageDigest>' parameter is wrong. See chapter 7.5.5"},
            { 0x71 , "The last command is rejected because it will generate an overflow of one totalizer in the structure sReceipt.aulTot []. Close the current receipt and open a new one."},
            { 0x72 , "Used only in BEETLE/50 and BEETLE/60"},
            { 0x73 , "The last command is rejected because it will generate an underflow of one totalizer in the structure sReceipt.aulTot []. Send a corrected command."},
            { 0x74 , "The last command is rejected because it will generate an overflow of one totalizer in the structure sDay.aulTot []. Close the current day and open a new one."},
            { 0x75 , "During the execution of the TOTAL_LINE command it's detected that the current receipt totalizers will generate an overflow of one totalizer in the structure sGrand.aulVAT [] or sGrand.aelNetSale[]. The receipt will be cancelled and the day will be closed by MFC. Replace the MFMEM board by a new one and start the initialisation procedure."},
            { 0x79 , "The last command is rejected because it will generate an overflow of one counter in the structure sDay.auiCnt []. Close the current day and open a new one."},
            { 0x81 , "The parameter <TotalValue>, which is transmitted with the command TOTAL_LINE, isn't equal to the totalizer sReceipt.aulTot [BRUTTO_SALES]. The receipt will automatically canceled by the MFC. Open a new receipt and transmit the corrected parameter <TotalValue> with the command TOTAL_LINE."},
            { 0x82 , "The button 1 is pressed during the command TEST_SELFTEST was executed. If the button isn't pressed replace the button."},
            { 0x83 , "The button 2 is pressed during the command TEST_SELFTEST was executed. If the button isn't pressed replace the button."},
            { 0x84 , "The TKD jumper is in reset position during power on. Switch off the printer/system and set the TKD jumper to the \"NORM\" position."},
            { 0x85 , "Before / during a TKD Reset (See 15.3) or before SET_TIME command (See 15.2) a SPECIAL_TECHNICAL_INTERVENTION command execution is necessary. Execute the SPECIAL_TECHNICAL_INTERVENTION command."},
            { 0x86 , "At the RECEIPT_BEGIN command execution, the elapsed time from the first RECEIPT_OPEN command in this fiscal day is greather then 24 hours. Execute the DAY_END and DAY_BEGIN command sequence."},
            { 0x87 , "The password (SPECIAL_TECHNICAL_INTERVENTION command) is wrong. Send a correct password."},
            { 0x88 , "The SET_TIME command is executed when the MFC is in BLOCKADE status. Execute the TKD reset procedure."},
            { 0x8F , "Only for debugging purpose"},
            { 0x91 , "One of the transmitted parameters contains a restricted text, which is not allowed. Send a corrected command."}};

    }
}
