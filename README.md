# VetMedData.NET
A .NET Core class library for obtaining structured data on UK-licensed Veterinary Medicines

## Building
This library is provided as a Microsoft Visual Studio 2017 solution.

TODO: Build instructions for dotnet core.

## Using

### Getting the VMD PID
VMDPIDFactory is a factory class for the VMDPID class hierarchy. After a successful GET and parse of the xml feed from the Veterinary Medicines Directorate (VMD), the factory class will retain the instance of the VMDPID class