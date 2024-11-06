using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace ITS.Retail.WebClient.Helpers
{
    public class Wizard
    {
        public Guid ID { get; protected set; }
        public int CurrentStepIndex { get; protected set; }
        public WizardStep CurrentStep { get; protected set; }
        //public string Name { get; set; }
        public Unit Width { get; set; }
        public Unit Height { get; set; }
        public string HeaderText { get; set; }
        public string CloseUpEvent { get; set; }
        public string OnShownEvent { get; set; }
        public object WizardModel { get;  protected set; }
        public List<string> Arguements { get; protected set; }

        public Wizard(WizardStep firstStep, object viewModel, List<string> arguements)
        {
            this.ID = Guid.NewGuid();
            this.CurrentStepIndex = 0;
            this.CurrentStep = firstStep;
            this.WizardModel = viewModel;
            Arguements = (arguements == null) ? new List<string>() : arguements;
        }

        /// <summary>
        /// Moves the Wizard to the next step. Returns true if it's the final step
        /// </summary>
        /// <returns>Returns true if its the final step</returns>
        public bool MoveNext()
        {
            if (CurrentStep != null)
            {
                if (CurrentStep.NextStep != null)
                {
                    CurrentStepIndex++;
                    CurrentStep = CurrentStep.NextStep;
                    if (CurrentStep.NextStep == null)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                throw new Exception("Current Step is null");
            }

            return false;
        }

        /// <summary>
        /// Moves the Wizard to the previous step. Returns true if it's the first step
        /// </summary>
        /// <returns>Returns true if its the first step</returns>
        public bool GoBack()
        {
            if (CurrentStep != null)
            {
                if (CurrentStep.PreviousStep != null)
                {
                    CurrentStepIndex--;
                    CurrentStep = CurrentStep.PreviousStep;
                    if (CurrentStep.PreviousStep == null)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                throw new Exception("Current Step is null");
            }

            return false;
        }


        public WizardStep GetStepByIndex(int index)
        {
            return null;
        }
    }
}
