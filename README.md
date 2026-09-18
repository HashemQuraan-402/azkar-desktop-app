# Azkar — Arabic Dhikr Desktop Application

![C#](https://img.shields.io/badge/C%23-.NET%20Framework%204.8-512BD4)
![UI](https://img.shields.io/badge/UI-Windows%20Forms-0078D4)
![Platform](https://img.shields.io/badge/Platform-Windows-0078D6)

Azkar is an Arabic Windows desktop application for reading and tracking morning and evening adhkar. It includes repetition counters, completion progress, a customizable tasbih counter, session history, and text-file export.

The project was developed as a practical learning application to demonstrate C#, object-oriented programming, event-driven user interfaces, collections, service classes, and file handling.

## Features

### Morning and Evening Adhkar

- Automatically displays morning adhkar from 07:00 through 15:59.
- Displays evening adhkar outside that time range.
- Provides a separate repetition counter for every dhikr.
- Disables a counter after its required repetitions are completed.
- Allows each counter to be reset after confirmation.
- Shows overall completion using a progress bar.
- Displays a Windows notification when the full list is completed.

### Tasbih Counter

- Accepts a custom dhikr name or phrase.
- Supports incrementing, decrementing, and directly adjusting the count.
- Includes optional start and stop time tracking.
- Calculates the duration of a saved tasbih session.
- Allows the active tasbih session to be reset or changed.

### Session History and Export

- Displays completed adhkar in a read-only table.
- Displays saved tasbih sessions with their count and timing information.
- Allows either history section to be cleared independently.
- Exports the current completion history to a `.txt` file selected by the user.

### Interface and Accessibility

- Provides an Arabic-oriented desktop interface.
- Supports keyboard navigation using `Tab`, `Shift + Tab`, `Up`, and `Down`.
- Displays the current time and date.
- Allows interface, panel, font, and text-color customization.

## Tech Stack

- C#
- .NET Framework 4.8
- Windows Forms
- Object-oriented programming
- Windows file dialogs and text-file I/O
- In-memory collections and data binding

## Project Structure

```text
azkar/
├── Models/
│   ├── Azkar.cs              # Adhkar session state
│   ├── Masbaha.cs            # Tasbih session data
│   └── Zeker.cs              # Individual dhikr data and counter state
├── Services/
│   ├── AzkarServices.cs      # Overall completion evaluation
│   └── ZekerService.cs       # Morning/evening content and item completion
├── Properties/               # Application resources and settings
├── Form1.cs                  # Main application behavior and event handling
├── Form1.Designer.cs         # Windows Forms UI definition
├── Program.cs                # Application entry point
├── App.config                # .NET Framework runtime configuration
├── اذكاري.csproj             # C# project file
└── اذكاري.sln                # Visual Studio solution
```

## Design Overview

The application separates its main responsibilities into three areas:

- **Models** represent an individual dhikr, an adhkar session, and a tasbih session.
- **Services** provide the morning and evening content and determine completion state.
- **Windows Forms UI** builds interactive controls, handles user actions, displays progress, and exports session results.

The current version stores completion and tasbih history in memory during the active application session. Use the export feature to save the displayed history before closing the application.

## Getting Started

### Requirements

- Windows 10 or Windows 11
- Visual Studio with the **.NET desktop development** workload
- .NET Framework 4.8 targeting pack
- Git, if cloning the repository

### 1. Clone the Repository

```bash
git clone https://github.com/HashemQuraan-402/azkar.git
cd azkar
```

You can also clone it through GitHub Desktop.

### 2. Open the Solution

Open the following file in Visual Studio:

```text
اذكاري.sln
```

### 3. Build the Application

In Visual Studio, select:

```text
Build > Build Solution
```

Or use the keyboard shortcut:

```text
Ctrl + Shift + B
```

A successful build should finish with `0 Failed`.

### 4. Run the Application

Press `F5` or select the green **Start** button in Visual Studio.

No database, API key, account, or external service is required.

## How to Use

### Complete Adhkar

1. Launch the application.
2. The appropriate morning or evening list is selected automatically.
3. Press the counter beside a dhikr after each repetition.
4. Continue until the counter reaches zero.
5. Use the reset button if a counter needs to be restarted.
6. Follow the progress indicator until the list is complete.
7. Use the completion action to add completed items to the current history.

### Record a Tasbih Session

1. Open the tasbih section.
2. Enter the dhikr name or phrase.
3. Start the session.
4. Use the increment and decrement controls to update the count.
5. Optionally start and stop the timer.
6. Save the session to display it in the current history.

### Export History

1. Complete at least one dhikr or save a tasbih session.
2. Open the history section.
3. Select the save/export action.
4. Choose a filename and location.
5. The application writes the visible records to a text file.

## Data and Privacy

- The application works locally and does not send information to an external service.
- It does not require personal information or authentication.
- History is stored only in memory unless the user explicitly exports it.
- Generated Visual Studio files and build output are excluded through `.gitignore`.

## Current Limitations

- History is not restored after the application closes.
- The morning/evening time range is currently defined directly in the application logic.
- The project does not currently contain automated tests.
- The application is designed for Windows and .NET Framework 4.8.

## Future Improvements

- Persist history locally using SQLite or JSON.
- Allow users to configure the morning and evening schedule.
- Add search, filtering, and favorites.
- Add automated tests for counters, completion rules, and services.
- Separate more UI logic into reusable components.
- Add application screenshots and a downloadable release.
- Review religious text against clearly cited, trusted sources.
- Document and verify the licenses of all included image assets.

## Author

**Hashem Quraan**

- GitHub: [HashemQuraan-402](https://github.com/HashemQuraan-402)
- LinkedIn: [hashem-quraan-b561453ab](https://www.linkedin.com/in/hashem-quraan-b561453ab)

