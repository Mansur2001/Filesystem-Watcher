# FileSystem Watcher Project

## Iteration 6
My partner and I struggled this week due to the difficulties of handling avalonia ui GUI. Mansur spent a lot of time just fixing things that didn’t want to work no matter how many times I thought I had it right. Avalonia’s binding system gave me endless issues—simple things like binding a ComboBox to a list felt like hitting a wall. The errors weren’t helpful either, they just looped with vague AVLN3000 messages until I figured out the underlying issue with how ItemsControl expects its data. It forced me to dig through their docs and check GitHub issues just to move forward. Tairan spent a chunk of his time correcting constructor mismatches in the ViewModel. What should’ve been a straightforward factory call turned into hours of refactoring because the argument list didn’t match. These small things keep stacking up and make it hard to gain momentum. It doesn’t help that testing was mostly absent until now, so I had to set up test classes from scratch for nearly every service and view model. That at least gave me more confidence moving forward.It ended up wiping some of the code for the export management and email service, so he had to rewrite parts from memory. That definitely slowed things down on his side, especially because the email feature was relying on some tricky formatting and SMTP logic. We’re regrouping now and syncing up again, especially since the presentation is coming up. I’ve started building the slide deck and plan to tie everything together visually soon, even though laying that out cleanly has been its own challenge. We both know we’re in the last stretch, but the technical debt has started to show. At this point, we’re just trying to make sure everything works and looks good enough to stand behind.




## Requirements
- C# + FileSystemWatcher + SQLite.NET

## Developer Info

Name: Mansur Yassin and Tairan Zhang 
Course: TCSS360 Software Design and Quality Assurance  
Instructor: tom capaul
Version: v1.0  
---


