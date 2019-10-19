# Get the UNC path from a network drive via application which is in the Windows shell context menu

## Basic informations
- Windows Forms App (.NET Framework)

## Integrated development environment (IDE)
- Microsoft Visual Studio Community 2019

## Requirementes 
- .NET Framework 4.7.2


## Description
##### 0. Setup a new "Windows Forms App"-project
##### 1. Create Setup-Project to add entries to Windows Registry
Add a setup project to the solution:
- Add → New project → Setup Project (Visual Studio Installer)

It could be that you have to install an extension:
- Please click → Extensions and install the Extension "Microsoft Visual Studio Installer Projects".

[![](images/installer.png)](https://github.com/LuminiCode/windows-unc-path/blob/master/images/installer.png)

Add a primary output of your main application to the setup project:
- Right click on the "Setup Project" in the solution explorer and Add → Project output
- Select the project from combo box and "Primary output" from list box
Add keys to the registry:
- Right click on the "Setup Project" in the solution explorer and View → Registry

Add the following keys and values in the setup registry editor window:
- Right click on HKEY_CLASSES_ROOT → New Key → Set the name to "Directory"
- Right click on "Directory" → New → Key → Set the name to "shell"
- Right click on "shell" → New → Key → Set the name to "Get_UNC_Path"
- Right click on "Get_UNC_Path" → New → String Value
  - Delete the name and press Enter
  - Set value to "Get UNC Path" (Name which is shown in the context menu) in the properties window
- Right click on "Get_UNC_Path" → New →  String Value (Set the name to Icon)
  - Set the name to "Icon"
  - Set the value to "[TARGETDIR]windows-unc-path.exe" (name of the exe file)
- Right click on "Get_UNC_Path" → New → Key → Set the name to "command"
- Right click on "command" → New → String Value 
  - Delete the name and press Enter
  -	Set the value to “[TARGETDIR]windows-unc-path.exe” “%V"
  
  [![](images/registry_1.png)](https://github.com/LuminiCode/windows-unc-path/blob/master/images/registry_1.png)
  
    [![](images/registry_2.png)](https://github.com/LuminiCode/windows-unc-path/blob/master/images/registry_2.png)

##### 2. Add C# Code

Get path from selected directory:
```csharp
static void Main()
{
    string directoryName = null;

    //Get path from directory
    if (Environment.GetCommandLineArgs().Length > 1)

        for (int i = 1; i < Environment.GetCommandLineArgs().Length; i++)
        {
            if (i == 1)
            {
                directoryName = Environment.GetCommandLineArgs()[i];
            }
            else
            {
                directoryName = directoryName + " " + Environment.GetCommandLineArgs()[i];
            }
        }

    if (string.IsNullOrEmpty(directoryName))
        MessageBox.Show("After installing the application, " +
            "Right click on a folder an select 'Get UNC Path'");
    else

        Clipboard.SetText(GetUNCPath(@directoryName));
}
```

Get UNC-path from directory path:

```csharp
//Get UNC path from directory path
public static string GetUNCPath(string originalPath)
{
    StringBuilder sb = new StringBuilder(512);
    int size = sb.Capacity;


    char b = originalPath[1];
    if ((b >= 'a' && b <= 'z') || (b >= 'A' && b <= 'Z'))
    {
        originalPath = originalPath.Substring(1, originalPath.Length - 1);
    }

    // look for the {LETTER}: combination ...
    if (originalPath.Length > 2 && originalPath[1] == ':')
    {
        // the only valid drive letters are a-z && A-Z.
        char c = originalPath[0];


        if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
        {
            int error = WNetGetConnection(originalPath.Substring(0, 2),
                sb, ref size);

            if (error == 0)
            {
                string path = Path.GetFullPath(originalPath)
                    .Substring(Path.GetPathRoot(originalPath).Length);
                return Path.Combine(sb.ToString().TrimEnd(), path);
            }
        }
    }

    return originalPath;
}
```
##### 2. Build and install

- Right click on the "Setup Project" in the solution explorer and Build
- Right click on the "Setup Project" in the solution explorer and Install
[![](images/install.png)](https://github.com/LuminiCode/windows-unc-path/blob/master/images/install.png)
