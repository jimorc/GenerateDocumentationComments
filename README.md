# GenerateDocumentationComments
A Visual Studio extension for generating C# documentation comments

At this time, boilerplate summary documentation comments are added to public, protected,
and internal class declarations when the Tools->Invoke GenerateDocumentationCommentsInEditor
menu item is selected. Summary documentation is also added to public, protected, and internal
class constructors.

I blog about this extension. See the following posts for information about this extension:

1. [I Was Doing It All Wrong, And How I Changed To Doing It Right!](
https://jaipblog.wordpress.com/2017/09/15/i-was-doing-it-all-wrong-and-how-i-changed-to-doing-it-right/)
defines an initial set of user stories and a set of 9 unit tests for adding summary documentation
comments for class declarations.
2. [Creating A Documentation Comment Generator For C#: High-Level Design Decisions](
https://jaipblog.wordpress.com/2017/09/22/creating-a-documentation-comment-generator-for-c-high-level-design-decisions/)
discusses various ways of triggering the generation of boilerplate documentation comments, and why I
chose to create this as a Visual Studio extension.
3. [Creating A Documentation Comment Generator For C#: Setting Up The Framework](
https://jaipblog.wordpress.com/2017/09/22/creating-a-documentation-comment-generator-for-c-setting-up-the-framework/) looks 
at creating a Visual Studio extension with a menu item that will generate documentation comments for
the active document (the document in the active editor window). It shows how to retrieve the source code
in the active document, at a high level how to modify the source code to add documentation comments, and
finally, how to write the modified source code back to the active document.
4. [Creating A Documentation Comment Generator For C#: DocumentCommentsRewriter Class](
https://jaipblog.wordpress.com/2017/09/29/creating-a-documentation-comment-generator-for-c-documentcommentsrewriter-class/) 
discusses how
each syntax node is processed for documentation comment generation, and how that translates into the structure of the 
class that generates that output.
5. [Creating A Documentation Comment Generator For C#: Unit Testing](
https://jaipblog.wordpress.com/2017/10/13/creating-a-documentation-comment-generator-for-c-unit-testing/) 
begins the discussion of
testing of the documentation comments generator by looking at the four levels of testing and then discussing
unit testing in some detail.
6. [Creating A Documentation Comment Generator For C#: Integration, System, And Acceptance Testing](
https://jaipblog.wordpress.com/2017/10/20/creating-a-documentation-comment-generator-for-c-integration-system-and-acceptance-testing/) discusses integration, system, and acceptance testing that I will perform.

Future posts will be used to extend the discussion.

# Requirements
1. Visual Studio 2017, with at a minimum, the following workloads:
   * .Net desktop development
   * Visual Studio extension development
2. .Net Compiler Platform SDK extension
3. DGML editor if you want to activate the View Directed Syntax Graph menu item in the Syntax Visualizer. See the
Syntax Visualizer section of Tools For Viewing And Creating Roslyn Syntax Trees (to be published).
