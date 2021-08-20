# Winforms Stress Testing

## Stress Test Breakdown
This repo contains code that I am using to stress test the UI update loop of WinForms applications.

This project contains three types of tests:
- Invoke tests
- BeginInvoke tests
- Irregular BeginInvoke tests

### Invoke Tests
These tests call `Invoke(delegate)` on a form repeatedly from a different thread. This should not actually provide much stress to the UI thread as `Invoke(...)` is a blocking call, meaning that the non-UI thread is unable to call `Invoke(...)` again until the UI has finished the work specified in the first call.

### BeginInvoke Tests
These tests call `BeginInvoke(delegate)` on a form repeatedly from a different thread. Depending on the frequency of `BeginInvoke(...)` calls, this can provide a lot of stress to the UI thread. The initial round of testing has shown that the UI thread can handle up to 1,000 calls/second, but beyond that freezes up and becomes unresponsive.

### Irregular BeginInvoke Tests
These tests call `BeginInvoke(delegate)` in a similar manner to the BeginInvoke tests, but occasionally skips the normal pause between `BeginInvoke(...)` calls. This is done to test how the UI thread responds to occasionally quick `BeginInvoke(...)` calls when the average speed is still slow. The initial round of testing has shown that the UI thread can handle occasional `BeginInvoke(...)` calls that come in with less than 1ms delays, so long as the overall rate of incoming calls does not exceed 1,000 per second.

## Project Structure
This project is composed of three source files: `Program.fs`, `TestRunner.fs`, and `Types.fs`.

### `Types.fs`
This file contains the low level implementation of the tests. The `Forms` module contains a type `MyForm`, which defines the form that invoke requests are made against. This type provides the methods for updating UI elements that will be called during tests. This file also contains the `Stressor` module, which provides functions that accept a form and delegate as input and set up the non-UI thread to ping invoke requests against the form. Each function in this module represents a different way in which invoke requests can be pushed onto the UI thread.

### `TestRunner.fs`
This file provides the suite of tests that can be run using the types defined in `Types.fs`. The `GenericTest` module provides a higher level abstraction over the `Stressor` module by setting up the form and the stressor, running them in parallel, and then asking the user to report the visual experience of the form. The `Tests` module builds on top of this abstraction by providing functions that define a specific form and delegate to the tests. The `Tests` module is simply specified implementations of the `GenericTest` functions that define the scope of the available test suite. Finally, the `TestRunner` module provides a function that runs the entire test suite sequentially and builds a chart of the collected data.

### `Program.fs`
This is the entry point of the application. It sets the test parameters, uses the `TestRunner` module to run the entire test suite, and then displays the chart for the user.
