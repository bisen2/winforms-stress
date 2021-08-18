# Winforms Stress Testing

This repo contains code that I am using to stress test the UI update loop of WinForms applications.

In particular, I am:
- Running a form application
- Pinging UI updates via BeginInvoke from a different thread
- Marking ping rates at which the form stutters
