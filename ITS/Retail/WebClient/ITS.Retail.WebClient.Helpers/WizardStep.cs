using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.WebClient.Helpers
{
    public class WizardStep
    {
        /// <summary>
        /// Gets or sets the WizardStep Options
        /// </summary>
        public WizardStepOptions Options { get; set; }

        /// <summary>
        /// Gets or sets the next step of the wizard
        /// </summary>
        public WizardStep NextStep 
        {
            get
            {
                return nextStep;
            }
            set
            {
                nextStep = value;
                if (nextStep != null && nextStep.PreviousStep != this)
                {
                    nextStep.PreviousStep = this;
                }
                this.Options.NextButton.Visible = nextStep != null;
            }
        }

        /// <summary>
        /// Gets or sets the previous step of the wizard
        /// </summary>
        public WizardStep PreviousStep
        {
            get
            {
                return previousStep;
            }
            set
            {
                previousStep = value;
                if (previousStep != null && previousStep.NextStep != this)
                {
                    previousStep.NextStep = this;
                }
                this.Options.PreviousButton.Visible = previousStep != null;
            }
        }

        /// <summary>
        /// Initializes a new instance of WizardStep
        /// </summary>
        /// <param name="name"></param>
        public WizardStep(string name)
        {
            this.Options = new WizardStepOptions();
            this.Options.Name = name;
            this.Options.PreviousButton.Visible = false;
            this.Options.NextButton.Visible = false;
        }


        private WizardStep nextStep;
        private WizardStep previousStep;
    }
}
