# FileSystem Watcher Project

## Iteration 3

During this sprint, Tairan and I focused on finalizing the core system logic for the File Watcher project. I implemented the FileWatcherViewModel, wired up real-time file monitoring, database saving, CSV exporting, and email sending. Tairan handled the SQLite integration, email service, and added XML documentation. We connected everything through the MainWindow view, binding all controls to the ViewModel. The main challenge was managing thread safety when updating the UI from file system events, which we resolved using the dispatcher. Naming consistency and keeping the ViewModel clean were also priorities. Overall, the system is now functional, modular, and ready for testing.

## Requirements
- C# + FileSystemWatcher + SQLite.NET

## Developer Info

Name: Mansur Yassin and Tairan Zhang 
Course: TCSS360 Software Design and Quality Assurance  
Instructor: tom capaul
Version: v1.0  
---


