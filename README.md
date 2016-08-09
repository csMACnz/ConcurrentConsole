# ConcurrentConsole
A wrapper for concurrent reading and writing to the command-line.

Do you want to have a command prompt to interact with an application, but have  your application  push out messages whenever it likes?

So do I. Which is why I wrote ConcurrentConsole. This library wraps System.Console to allow you to read from the Console, while
Also writing to it in other threads. But the key to it's usefulness is that It doesn't distrupt your flow as you type.
