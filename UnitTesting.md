# Unit Testing Guidelines #

# Table of Contents
1. [Testing Tools](#testing-tools)
2. [Test Method Naming Conventions](#test-method-naming-conventions)
3. [Test Method Body Conventions](#test-method-body-conventions)
4. [Visual Studio Code Snippet](#visual-studio-code-snippet)
5. [CI integration](#cI-integration)
6. [Writing Testable Code](#writing-testable-code)

## Testing Tools

**NUnit:** A popular C# testing framework. Install with nuget.

**Moq:** A popular C# mocking framework. Install with nuget.

**NUnit Test Adapter:** Visual Studio extension to have NUnit tests apear in the test explorer. Install by going to Tools -> Extensions and Updates -> Online

## Test Method Body Conventions



*Inspired [from](http://osherove.com/blog/2005/4/3/naming-standards-for-unit-tests.html)*

The following naming convension is used for the test methods: 
**[Method_Scenario_Expectation]**
**Method:** The method that is being tested
**Scenario:** The scenario under which the test is executed
**Expectation:** The expected result behaviour

Examples:


    public void Sum_NegativeNumberAs1stParam_ExceptionThrown()
    
    public void Sum_NegativeNumberAs2ndParam_ExceptionThrown ()
    
    public void Sum_SimpleValues_Calculated ()


## Test Method Body Conventions

*Inspired [from](http://www.typemock.com/unit-test-patterns-for-net)*

The **Arrange, Act, Assert** pattern is used for the test method body:

**Arrange**: Setup everything needed for the running the tested code. This includes any initialization of dependencies, mocks and data needed for the test to run.

**Act**: Invoke the code under test.

**Assert**: Specify the pass criteria for the test, which fails it if not met.

**Example:**

    public void ExtractActivationKeyInfo_ActivationKeyIsInvalidLength_OperationReturnsFailure()
	{
	    //Arrange
	    ActivationKeyInfoExtractor activationKeyInfoExtractor = CreateDefault();
	    string activationKey = "this is an invalid activation key";
	    byte[] clientKey = new byte[] { 47, 228, 156, 246, 66, 164, 80, 57, 197,
	                                    132, 85, 199, 129, 191, 12, 207, 152, 196,
	                                    6, 39, 170, 234, 87, 23, 75, 182, 234, 10,
	                                    165, 208, 19, 167 };
	
	    //Act
	    OperationResult<ExtractActivationKeyInfoResult> result =
	        activationKeyInfoExtractor.ExtractActivationKeyInfo(activationKey, clientKey);
	
	    //Assert
	    Assert.That(result.Failure);
	}


## Visual Studio Code Snippet

You can install the following code snippet by going to Tools -> Code Snippets Manager -> Language C# -> Import

Then in a test class, type aaa and press tab to make the following snippet appear.

    [Test]
	public void MethodName_Scenario_Expectation()
	{
	    //Arrange
	
	    //Act
	
	    //Assert
	    Assert.Fail();
	}


*TestMethodCodeSnipper.snippet*

    <?xml version="1.0" encoding="utf-8"?>
		<CodeSnippets
		    xmlns="http://schemas.microsoft.com/VisualStudio/2005/CodeSnippet">
		  <CodeSnippet Format="1.0.0">
		    <Header>
		      <Title>NUnit unit test snippet</Title>
		      <Author>Dave Lowe</Author>
		      <Description>Adds a unit test stub method for NUnit</Description>
		      <Shortcut>aaa</Shortcut>
		    </Header>
		    <Snippet>
		      <Declarations>
		        <Literal Editable="true">
		          <ID>Method</ID>
		          <ToolTip>name of method under test</ToolTip>
		          <Default>MethodName</Default>
		          <Type>string</Type>
		        </Literal>
		        <Literal Editable="true">
		          <ID>Scenario</ID>
		          <ToolTip>scenario under test</ToolTip>
		          <Default>Scenario</Default>
		          <Type>string</Type>
		        </Literal>
		        <Literal Editable="true">
		          <ID>Expectation</ID>
		          <ToolTip>expected behaviour</ToolTip>
		          <Default>Expectation</Default>
		          <Type>string</Type>
		        </Literal>
		      </Declarations>
		      <Code Language="CSharp">
		        <![CDATA[
		        [Test]
		        public void $Method$_$Scenario$_$Expectation$(){
		          //Arrange
				  
		          //Act
				  
		          //Assert
				  Assert.Fail();
		        }]]>
		      </Code>
		    </Snippet>
		  </CodeSnippet>
		</CodeSnippets>


## CI Integration

One way to integrate your unit tests with the build server, is that you can add a console application project to your solution and edit the .csproj file with a task like the following (ex. see ITS.Retail.Platform):

    <?xml version="1.0" encoding="utf-8"?>
	  <Project ToolsVersion="4.0" DefaultTargets="Test" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
	    <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
	    <Target Name="Test">
	       <Exec Command="nunit-console-x86.exe &quot;$(SolutionDir)\Tests\Test Assemblies\ITS.Licensing.Client.Tests.dll&quot; /xml:&quot;$(SolutionDir)Tests\Test Results\$(ConfigurationName) - Client Test Results.xml&quot;" WorkingDirectory="$(SolutionDir)\packages\NUnit.Runners.2.6.4\tools" />
	    </Target>
	...

Set that project to build only on configurations that run on the build server (e.g. Release). The above task will use the nunit console runner to execute all the tests defined in the given assembly and output the results as xml files in the given "Test Results" directory. That xml result file is then parsed by the build server. For ccnet, a merge publisher like the following is required in the configuration of the project:

    <project name="ITS.Licensing" queue="Custom" queuePriority="1">
	...
	   <publishers>
	      <merge>
	          <files>
	              <file>D:\CCNetBuildArea\ITS.Licensing\WC\Tests\Test Results\*.xml</file>
	          </files>
	      </merge>
	...

## Writing Testable Code

To be able to properly unit test your code, you must be following some basic coding practices. If not you, will struggle to write unit tests and even if you manage to write some, they will either break down easily (be brittle) or produce invalid results (be unreliable). In the end you will either spend a lot of time maintaining your tests or stop carring altogether.

One solution is to follow the the [SOLID principles of object-oriented programming](https://en.wikipedia.org/wiki/SOLID_%28object-oriented_design%29) . One of the many benefits of following these principles is that you code is extremelly easy to unit test.
Bellow is a summary of some basic flaws that directly affect the quality and brittleness of your unit tests.([source](http://misko.hevery.com/attachments/Guide-Writing%20Testable%20Code.pdf)):

[Flaw #1: Constructor does Real Work](misko.hevery.com/code-reviewers-guide/flaw-constructor-does-real-work/)

**Warning Signs**

- new keyword in a constructor or at field declaration
- Static method calls in a constructor or at field declaration
- Anything more than field assignment in constructors
- Object not fully initialized after the constructor finishes (watch out for initialize methods)
- Control flow (conditional or looping logic) in a constructor
- Code does complex object graph construction inside a constructor rather than using a factory or builder
- Adding or using an initialization block

[Flaw #2: Digging into Collaborators](http://misko.hevery.com/code-reviewers-guide/flaw-digging-into-collaborators/)

**Warning Signs**

- Objects are passed in but never used directly (only used to get access to other objects)
- Law of Demeter violation: method call chain walks an object graph with more than one dot (.)
- Suspicious names: context, environment, principal, container, or manager

[Flaw #3: Brittle Global State & Singletons](http://misko.hevery.com/code-reviewers-guide/flaw-brittle-global-state-singletons/)

**Warning Signs**

- Adding or using singletons
- Adding or using static fields or static methods
- Adding or using static initialization blocks
- Adding or using registries
- Adding or using service locators

[Flaw #4: Class Does Too Much](http://misko.hevery.com/code-reviewers-guide/flaw-class-does-too-much/)

**Warning Signs**

- Summing up what the class does includes the word “and”
- Class would be challenging for new team members to read and quickly “get it”
- Class has fields that are only used in some methods
- Class has static methods that only operate on parameters