/* Manifest file keeps track of every active project, first line is total active projects.
 * Next lines are project name, total paragraphs in project and paragraphs finished. This then repeats until all projects are documented in the manifest. */

string configPath = ".\\config.db";
string? mainFolderPath = null;
string manifestPath;

void generateConfig(string configPath) { 
    Console.Clear();
    Console.Write("The main folder will be used for storing data from this application.\n" +
        "You will be asked to enter a full path to your desired folder. Ex. C:\\Users\\your_username\\..." + 
        "You may also leave this blank, and the current location of the program will be used.");
    Console.Write("\n\nEnter your full main folder path: ");
    string? mainFolder = Console.ReadLine();
    if (mainFolder == null) { mainFolder = ""; }
    mainFolder = mainFolder.Trim();
    if (mainFolder == null || mainFolder == "") {
        mainFolder = Directory.GetCurrentDirectory();
    } else {
        mainFolder = Path.GetFullPath(mainFolder); 
        Directory.CreateDirectory(mainFolder);
    }
    Console.Clear();
    Console.Write("Generating configuration file in the same folder as the program, this will only take a second.");
    var db = File.Create(configPath);
    db.Dispose();
    string[] configToWrite = {mainFolder};
    var dbt = File.WriteAllLinesAsync(configPath, configToWrite);
    dbt.Wait(500);
    dbt.Dispose();
    var m = File.Create(mainFolder + "\\manifest.db");
    m.Dispose();
    string[] manifestToWrite = {"0"};
    var mt = File.WriteAllLinesAsync(mainFolder + "\\manifest.db", manifestToWrite);
    Console.Write("\nCompleted, program description: ");
    Console.Write("\nThis program was created to help users get the most out of reading long, complex" +
                  "\ntexts. For this, we use a strategy created by Assistant Professor of Philosophy" +
                  "\nJeffrey Kaplan. Read a paragraph of text, summarize it in a single sentance." +
                  "\nAfter this, summarize all text before in a single sentance. Keeping only the" +
                  "\nmost critical parts/ideas is the key. This helps you retain the information you read." +
                  "\n----------------------------PRESS ENTER TO CONTINUE----------------------------\n");
    Console.ReadLine();

}
void fileWrite(string path, string[] content, char mode='w') {
    System.Threading.Tasks.Task fileTracker;
    switch (mode) {
        case 'w':
            fileTracker = File.WriteAllLinesAsync(path, content);
            break;
        case 'a':
            fileTracker = File.AppendAllLinesAsync(path, content);
            break;
        default:
            fileTracker = File.WriteAllLinesAsync(path, content);
            break;
    }
    fileTracker.Wait(1000);
    fileTracker.Dispose();
}
string inputValidator(string question, string rejectionResponse = "Input is invalid, try again.") {
    string? output = null;
while (output == null || output == "") { 
    Console.Write(question + "\n$> ");
    output = Console.ReadLine();
    if (output == null || output == "") { Console.WriteLine("\n" + rejectionResponse); } }
    return output.Trim();
}
void manifestUpdator(string manifestPath, string projectName, string newParagraphsFinsihed = "0", bool archive = false) {
    string[] manifestContent = File.ReadAllLines(manifestPath);
    string[] manifestFile = File.ReadAllLines(manifestPath); // Imports manifest data
    int i = 0;
    List<string> projectNames = new List<string>();
    List<string> projectParagraphs = new List<string>();
    List<string> projectFinsihedParagraphs = new List<string>();
    foreach (string line in manifestFile)
    {
        if (i == 0) {}
        else
        {
            switch (i % 3)
            {
                case 1:
                    projectNames.Add(line);
                    break;
                case 2:
                    projectParagraphs.Add(line);
                    break;
                case 0:
                    projectFinsihedParagraphs.Add(line);
                    break;
            }
        }
        i++;
    } // End of import
    int foundIndex = -1;
    for (i = 0; i < projectNames.Count(); i++) {
        if (projectNames[i].Trim() == projectName.Trim()) { foundIndex = i; break; }
}
    if (foundIndex == -1) {
        return;
    } else {
        projectFinsihedParagraphs[foundIndex] = newParagraphsFinsihed;
        int currentProjects = Int32.Parse(manifestFile[0].Trim());
        if (archive) {
            currentProjects -= 1;
            projectNames.RemoveAt(foundIndex);
            projectParagraphs.RemoveAt(foundIndex);
            projectFinsihedParagraphs.RemoveAt(foundIndex);
        }
        List<string> writeToFile = new List<string>();
        writeToFile.Add(currentProjects.ToString());
        for (int j = 0; j < projectNames.Count(); j++)
        {
            writeToFile.Add(projectNames[j]);
            writeToFile.Add(projectParagraphs[j]);
            writeToFile.Add(projectFinsihedParagraphs[j]);
        }
        fileWrite(manifestPath, writeToFile.ToArray());
    }
}
void createProject(string manifestPath, string mainFolderPath) {
    Console.Clear();
    Console.WriteLine("Enter a project name, which will be used in the list and file name.");
    Console.Write("\nProject Name: ");
    string? projectName = Console.ReadLine();
    if (projectName == null || projectName == "") {
        Console.WriteLine("\nProject Name can not be blank. Exiting project creation wizard.");
        Thread.Sleep(3000);
        Console.Clear();
        return; 
    }
    Console.Clear();
    Console.WriteLine("How many paragraphs are in the text?");
    Console.Write("\nParagraphs: ");
    string? totalParagraphs = Console.ReadLine();
    if (totalParagraphs == null || totalParagraphs == "") {
        Console.WriteLine("\nParagraphs can not be blank. Exiting project creation wizard.");
        Thread.Sleep(3000);
        Console.Clear();
        return;
    }
    string[] append = { projectName, totalParagraphs, "0" }; // projectName, totalParagraphs, paragraphs finished
    fileWrite(manifestPath, append.ToArray(), 'a');
    string[] manifestFile = File.ReadAllLines(manifestPath);
    manifestFile[0] = (Int32.Parse(manifestFile[0].Trim()) + 1).ToString(); // Iterates current number of projects
    fileWrite(manifestPath, manifestFile.ToArray());
    var projectFile = File.Create(mainFolderPath + $"\\{projectName.ToLower()}.txt");
    projectFile.Dispose();
    Console.Clear();
    Console.WriteLine($"Project {projectName} created with {totalParagraphs} paragraphs.");
}

void listProjects(string manifestPath) {
    string[] manifestFile = File.ReadAllLines(manifestPath);
    int i = 0;
    List<string> projectNames = new List<string>();
    List<string> projectParagraphs = new List<string>();
    List<string> projectFinsihedParagraphs = new List<string>();
    foreach (string line in manifestFile)
    {
        if (i == 0) { }
        else
        {
            switch (i % 3)
            {
                case 1:
                    projectNames.Add(line);
                    break;
                case 2:
                    projectParagraphs.Add(line);
                    break;
                case 0:
                    projectFinsihedParagraphs.Add(line);
                    break;
            }
        }
        i++;
    }
    if (!(manifestFile[0] == "0"))
    { // Writes the projects only if there are projects that exist
        Console.WriteLine("\nCurrent list of active projects: ");
        for (int j = 0; j < projectNames.Count(); j++)
        {
            Console.WriteLine($"{projectNames[j]} - Paragraphs: {projectFinsihedParagraphs[j]}/{projectParagraphs[j]}");
        }
    }
    else
    {
        Console.Write("\nThere are currently 0 active projects. \n");
    }
}

void openProject(List<string> projectNames, List<string> projectParagraphs, List<string> projectFinishedParagraphs, string inputName) {
    int foundIndex = -1;
    for (int i = 0; i < projectNames.Count(); i++)
    {
        if (projectNames[i].Trim() == inputName.Trim()) { foundIndex = i; break; }
    }
    if (foundIndex == -1)
    {
        Console.WriteLine("\nProject could not be located. Create or manually repair a project to continue.");
    }
    else
    {
        string projectName = projectNames[foundIndex];
        int paragraphTotal = Int32.Parse(projectParagraphs[foundIndex].Trim());
        string projectPath = $"{mainFolderPath}\\{projectName.ToLower()}.txt";
        List<string> readableHeader = new List<string>();
        List<string> lastParagraphAnalysis = new List<string>();
        List<string> singleParagraphReflection = new List<string>();
        Console.Write($"\nOpening project {projectName}...\n\n");
        string[] projectContent = File.ReadAllLines(projectPath);
        for (int j = 0; j < projectContent.Length; j++)
        {
            switch (j % 3)
            {
                case 0:
                    readableHeader.Add(projectContent[j]);
                    break;
                case 1:
                    lastParagraphAnalysis.Add(projectContent[j]);
                    break;
                case 2:
                    singleParagraphReflection.Add(projectContent[j]);
                    break;
            }
        }
        bool earlyExit = false;
        string userIn;
        while (readableHeader.Count() < paragraphTotal)
        {
            if (readableHeader.Count() < 2)
            {
                readableHeader.Add($"Paragraph {readableHeader.Count() + 1}/{paragraphTotal}");
                lastParagraphAnalysis.Add("No Analysis Required (for first 2 paragraphs)");
                userIn = inputValidator($"(Paragraph {readableHeader.Count}) Summarize the paragraph in a single sentence. ('save' to exit)");
                if (userIn == "save") { earlyExit = true; break; }
                singleParagraphReflection.Add(userIn);
                Console.WriteLine();

            }
            else
            {
                readableHeader.Add($"Paragraph {readableHeader.Count() + 1}/{paragraphTotal}");
                userIn = inputValidator($"(Paragraph {readableHeader.Count}) Summarize all text before your current paragraph in a single sentence. ('save' to exit)");
                if (userIn == "save") { earlyExit = true; break; }
                lastParagraphAnalysis.Add(userIn);
                Console.WriteLine();
                userIn = inputValidator($"(Paragraph {readableHeader.Count}) Summarize the paragraph in a single sentence. ('save' to exit)");
                if (userIn == "save") { earlyExit = true; break; }
                singleParagraphReflection.Add(userIn);
                Console.WriteLine();
            }

        }
        if (earlyExit)
        {
            if (!(readableHeader.Count == singleParagraphReflection.Count))
            {
                if ((lastParagraphAnalysis.Count() == readableHeader.Count()))
                {
                    lastParagraphAnalysis.RemoveAt(lastParagraphAnalysis.Count() - 1);
                }
                readableHeader.RemoveAt(readableHeader.Count() - 1);
            }
        }
        List<string> writeToFile = new List<string>();
        for (int j = 0; j < readableHeader.Count(); j++)
        {
            writeToFile.Add(readableHeader[j]);
            writeToFile.Add(lastParagraphAnalysis[j]);
            writeToFile.Add(singleParagraphReflection[j]);
        }
        var p = File.WriteAllLinesAsync(projectPath, writeToFile);
        p.Wait(1000);
        p.Dispose();
        if (earlyExit)
        {
            manifestUpdator(manifestPath, projectName, readableHeader.Count().ToString(), false);
        }
        else
        {
            string archive = inputValidator("Congratulations, you finished this project. Would you like to archive it? (y/n)").ToLower();
            bool a = false;
            if (archive == "y") { a = true; }
            else { a = false; }
            manifestUpdator(manifestPath, projectName, paragraphTotal.ToString(), a);
        }
    }
}
void projectUnarchive(string mainFolderPath, string manifestPath, string projectName) {
    string projectPath = $"{mainFolderPath}\\{projectName.ToLower()}.txt";
    if (!File.Exists(projectPath)) { // If file can not be found, return with error.
        Console.WriteLine("\nProject could not be located, make sure it's in the main folder and the name is correct.");
        return;
    }
    string[] projectContent = File.ReadAllLines(projectPath);
    if (projectContent.Length < 2) {
        string input = inputValidator("Project is empty and can not be recoved. Remove this file and start creation wizard? (y/n)").ToLower();
        if (input == "y") {
            File.Delete(projectPath);
            createProject(manifestPath, mainFolderPath);
            return;
        } else {
            return;
        }
    }
    string lastReadableHeader = projectContent[projectContent.Length - 3];
    string[] temp = lastReadableHeader.Split(" ");
    string[] desiredContent = temp[1].Split("/");
    string finishedParagraphs = desiredContent[0];
    string totalParagraphs = desiredContent[1];
    string[] append = { projectName, totalParagraphs, finishedParagraphs }; // projectName, totalParagraphs, paragraphs finished
    fileWrite(manifestPath, append.ToArray(), 'a');
    string[] manifestFile = File.ReadAllLines(manifestPath);
    manifestFile[0] = (Int32.Parse(manifestFile[0].Trim()) + 1).ToString(); // Iterates current number of projects
    fileWrite(manifestPath, manifestFile.ToArray());
    return;

}

if(File.Exists(configPath)) {
    string[] configContent = File.ReadAllLines(configPath);
    mainFolderPath = configContent[0];
} else { 
    generateConfig(configPath);
    string[] configContent = File.ReadAllLines(configPath);
    mainFolderPath = configContent[0];
    Thread.Sleep(1000);
}
string time;
int timeNow = DateTime.Now.Hour;
if (timeNow < 12) {
    time = "Morning";
} else if (timeNow < 18) {
    time = "Afternoon";
} else {
    time = "Evening";
};
manifestPath = mainFolderPath + "\\manifest.db";

if (!File.Exists(manifestPath)) {
    var m = File.Create(mainFolderPath + "\\manifest.db");
    m.Dispose();
    string[] manifestToWrite = { "0" };
    var mt = File.WriteAllLinesAsync(mainFolderPath + "\\manifest.db", manifestToWrite);
    Console.Write("\nCompleted, starting program.");

}
string[] manifestContent = File.ReadAllLines(manifestPath);
string[] manifestFile = File.ReadAllLines(manifestPath); // Imports manifest data
int i = 0;
List<string> projectNames = new List<string>();
List<string> projectParagraphs = new List<string>();
List<string> projectFinsihedParagraphs = new List<string>();
foreach (string line in manifestFile)
{
    if (i == 0) {}
    else {
        switch (i % 3) {
            case 1:
                projectNames.Add(line);
                break;
            case 2:
                projectParagraphs.Add(line);
                break;
            case 0:
                projectFinsihedParagraphs.Add(line);
                break;
        }
    }
    i++;
} // End of import
Console.Clear();
Console.Write($"\nGood {time.ToLower()}, there are {manifestContent[0]} active readings to work on.");
Console.WriteLine("\nEnter 'help' to view current commands.");
bool noExit = true;
while (noExit == true) {
//Console.Write("\n\n$>> ");
string? input = inputValidator("");
if (input == null) { input = "N/A"; }
string[] inputArray = input.Split(" ");
switch(inputArray[0]) {
    case "help":
        Console.WriteLine("\nCurrently available commands: ");
        Console.WriteLine("help - displays list of currently available commands");
        Console.WriteLine("create - creates a new reading project");
        Console.WriteLine("open - opens a current reading project");
        Console.WriteLine("archive - marks a current reading project as inactive (removes from list)");
        Console.WriteLine("unarchive - marks an archived project as active");
        Console.WriteLine("list - lists current reading projects");
        Console.WriteLine("exit - exits the program");
        break;
    case "create":
        createProject(manifestPath, mainFolderPath);
        manifestContent = File.ReadAllLines(manifestPath);
        manifestFile = File.ReadAllLines(manifestPath); // Imports manifest data
        i = 0;
        projectNames = new List<string>();
        projectParagraphs = new List<string>();
        projectFinsihedParagraphs = new List<string>();
        foreach (string line in manifestFile)
        {
            if (i == 0) { }
            else
            {
                switch (i % 3)
                {
                    case 1:
                        projectNames.Add(line);
                        break;
                    case 2:
                        projectParagraphs.Add(line);
                        break;
                    case 0:
                        projectFinsihedParagraphs.Add(line);
                        break;
                }
            }
            i++;
        } // End of import
            break;
    case "list":
            listProjects(manifestPath);
            break;
            case "open":
            if (inputArray.Length < 2) {
                Console.WriteLine("\nOops, you need to specify a project name after open. Example: open project");
            } else {
            openProject(projectNames, projectParagraphs, projectFinsihedParagraphs, inputArray[1]);
            }
            break;
    case "archive":
            manifestUpdator(manifestPath, inputArray[1].Trim(), archive: true);
            break;
    case "unarchive":
            if (inputArray.Length < 2) {
                Console.WriteLine("\nOops, you need to specify a project name after open. Example: unarchive project");
            }
            else {
                projectUnarchive(mainFolderPath, manifestPath, inputArray[1].Trim());
            }
            break;

    case "exit":
        noExit = false;
        break;
} }