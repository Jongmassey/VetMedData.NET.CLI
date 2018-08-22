# VetMedData.NET
A .NET Core class library for obtaining structured data on UK-licensed Veterinary Medicines

[![Build status](https://jongmassey.visualstudio.com/VetMedData.NET%20Build/_apis/build/status/VetMedData.NET%20Build-CI)](https://jongmassey.visualstudio.com/VetMedData.NET%20Build/_build/latest?definitionId=1)

## Building
This library is provided as a Microsoft Visual Studio 2017 solution and can be build from within this IDE.

For other platforms, the following command within the VetMedData.NET folder will build the project
```dotnet build -c Release```

## Using
### Getting the VMD PID
VMDPIDFactory is a factory class for the VMDPID class hierarchy. After a successful GET and parse of the xml feed from the Veterinary Medicines Directorate (VMD), the factory class will retain the instance of the VMDPID class

### EPAR Tools
If the EPARTools are to be used, this requires a Google Custom Search API Key. This is expected to be stored in a user secrets file with the ID ```3ed32012-d532-4d6f-9a0a-9a5ba6c7c636```.

This can be created from by 
```dotnet user-secrets set "GoogleAPISecrets:GoogleCustomSearchAPIKey" "{api key}" --project "{path to project folder containing VetMedData.NET.csproj}" ```

A Google Custom Search for the site ```http://www.ema.europa.eu/docs/en_gb/document_library/epar_-_product_information/veterinary/``` must also be created. 
The CX id of the custom search must then be saved to the secrets file:
```dotnet user-secrets set "GoogleAPISecrets:GoogleCustomSearchCX" "{CX id}" --project "{path to project folder containing VetMedData.NET.csproj}" ```



