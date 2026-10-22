# SimpleRDP

A simple, portable Remote Desktop manager for Windows.

![SimpleRDP Screenshot](docs/screenshot.png)

## Overview

Windows comes with a built-in Remote Desktop app, but it makes managing multiple computers and backing up saved passwords difficult. **SimpleRDP** gives you a clean, easy-to-use list of all your remote computers in one place. Everything is saved in the app folder, so you can easily back it up or copy it to another PC.

## Features

- **Portable**: Just one `.exe` file—no installation required. Copy the folder to a USB drive or another PC and all your connections and passwords come with you.
- **Saved Passwords**: Save your login details once and connect with a double-click.
- **Native Windows Remote Desktop**: Uses the standard Windows Remote Desktop client under the hood, so your sessions work and feel exactly the same.
- **Quick Cloning**: Duplicate an existing connection with a single click using the **Clone** button.
- **Remembers Window Size**: Automatically saves your window height when you close the app.
- **Classic Warning Dialog Option**: One-click checkbox to switch back to the classic Windows Remote Desktop connection warning prompt.

---

## Short Manual

### 1. Add or Edit a Connection
1. Click **New** (or select a connection and click **Edit**).
2. Type a **Name** for the connection at the top right, then fill in the **Computer** address, **User name**, **Password**, and **Domain** (optional).
3. Click **Save**.

### 2. Clone a Connection
1. Click on a connection in the list.
2. Click **Clone** to make a copy of it, change the name or address, and click **Save**.

### 3. Connect
- Double-click any connection in the list, or select it and click **Connect**.
- Check **Keep opening SimpleRDP** if you want SimpleRDP to stay open after you close your remote session.

### 4. Change Screen Resolution or More Settings
1. Select a connection and click **Options**.
2. The standard Windows Remote Desktop settings window will open.
3. Change any settings you want (such as display size, sound, or shared folders), then click **Save** inside that window and close it.

### 5. Backup Your Connections
- Simply copy the entire SimpleRDP folder (`SimpleRDP.exe`, `config.json`, and the `rdp-files` folder) to your backup drive or another computer.

---

## License

- Modified 2026 by Alex under the MIT License.
- Originally based on RDP Portal by Louis Lam (MIT License).
- MIT License

Copyright (c) 2025 Louis Lam

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
