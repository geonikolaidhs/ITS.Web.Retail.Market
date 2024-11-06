using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionShowBlinkingErrorParams : ActionParams
    {
        public bool Blink { get; set; }

        public override eActions ActionCode
        {
            get { return eActions.SHOW_BLINKING_ERROR; }
        }        

        public ActionShowBlinkingErrorParams(bool blink)
        {
            this.Blink = blink;
        }
    }
}
