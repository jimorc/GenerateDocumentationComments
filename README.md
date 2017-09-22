# GenerateDocumentationComments
A Visual Studio extension for generating C# documentation comments

At this time, boilerplate summary documentation comments are added to public, protected,
and internal class declarations when the Tools->Invoke GenerateDocumentationCommentsInEditor
menu item is selected.

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
4. Creating A Documentation Comment Generator For C#: DocumentCommentsRewriter Class (to be published) discusses how
each syntax node is processed for documentation comment generation, and how that translates into the structure of the 
class that generates that output.
5. Creating A Documentation Comment Generator For C#: Testing (to be published) discusses the various levels of testing
that are and can be performed on the documentation comments generator and how the extension is designed for testing.

Future posts will be used to extend the discussion.
