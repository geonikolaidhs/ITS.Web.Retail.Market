using System;
using TechTalk.SpecFlow;

namespace ITS.Retail.Platform.Specs.Steps
{
    [Binding]
    public class DocumentTypeIsDefaultIsUniqueSteps
    {
        [Given(@"I have entered a default DocumentType")]
        public void GivenIHaveEnteredADefaultDocumentType()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Given(@"There are other default DocumentTypes")]
        public void GivenThereAreOtherDefaultDocumentTypes()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Given(@"I have entered a DocumentType")]
        public void GivenIHaveEnteredADocumentType()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Given(@"There are no other default DocumentTypes")]
        public void GivenThereAreNoOtherDefaultDocumentTypes()
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"I press Save")]
        public void WhenIPressSave()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"the result should an error message")]
        public void ThenTheResultShouldAnErrorMessage()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"The DocumentType must not be saved")]
        public void ThenTheDocumentTypeMustNotBeSaved()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"the result should an success message")]
        public void ThenTheResultShouldAnSuccessMessage()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"The DocumentType must be saved")]
        public void ThenTheDocumentTypeMustBeSaved()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
