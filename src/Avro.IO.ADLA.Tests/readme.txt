The MSFT analytics unit test assembly is provided with the Azure Data Lake tools that come with Visual Studio.

However, there is an issue such that referencing and loading the Microsoft.Analytics.UnitTest.dll provided by MSFT
causes a FileLoadException.

https://github.com/Azure/usql/issues/99

To work around this issue I have decompiled the assembly to source with dotPeak, and added the project to our solution
until the issue is resolved by MSFT.  These projects should not be modified, as they do not enforce our coding standards.