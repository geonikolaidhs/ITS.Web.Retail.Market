Feature: DocumentTypeIsDefaultIsUnique
	In order to avoid duplicate default DocumentTypes
	As an administrative user
	I want to be notified with a message

@DocumentTypeIsDefaultIsUniqueFailure
Scenario: The administrative user tries to save a default DocumentType while there are other default DocumentTypes
	Given I have entered a default DocumentType
	And There are other default DocumentTypes
	When I press Save
	Then the result should an error message
	And The DocumentType must not be saved

@DocumentTypeIsDefaultIsUniqueSuccess
Scenario: The administrative user tries to save a default or non default DocumentType while there are no other default DocumentTypes
	Given I have entered a DocumentType
	And There are no other default DocumentTypes
	When I press Save
	Then the result should an success message
	And The DocumentType must be saved

