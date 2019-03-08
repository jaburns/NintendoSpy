**Keybindings**
=====
One of RetroSpy's great features allows you to configure a button or button combination to a keystroke or keystroke combination. This is called keybindings. Every release of RetroSpy includes a *keybindings.xml* file. This is where you can setup your keybindings settings. The default file has a nice example of what a keybind looks like:

![](https://raw.githubusercontent.com/sk84uhlivin/RetroSpy/master/docs/tutorial-images/keybindings-tutorial/example.png)

You'll see in the commented example that with this configuration, if *l* and *r* were pressed on the controller at the same time, the *Home* key on the keyboard would be pressed. The only thing stopping us from being able to use this is removing the comments and info text. Save the file, open RetroSpy and it's set. Remember that like any other .xml asset in RetroSpy, you must save the file with RetroSpy closed, or reboot RetroSpy for the changes to take place. 

![](https://raw.githubusercontent.com/sk84uhlivin/RetroSpy/master/docs/tutorial-images/keybindings-tutorial/1binding.png)

You can also set multiple bindings like so:

![](https://raw.githubusercontent.com/sk84uhlivin/RetroSpy/master/docs/tutorial-images/keybindings-tutorial/2binding.png)

To turn off a keybinding, you'll need to comment the inner part of the tree. Commenting the whole tree will result in an error in RetroSpy.

![](https://raw.githubusercontent.com/sk84uhlivin/RetroSpy/master/docs/tutorial-images/keybindings-tutorial/comment.png)

## input button
You can find the button names for the input buttons in the *skin.xml* that is paired with whichever controller mode you are using. Note that these are case sensitive.

![](https://raw.githubusercontent.com/sk84uhlivin/RetroSpy/master/docs/tutorial-images/keybindings-tutorial/buttonname.png)

## output-key
The following is a list of all possible keys to set as a keybind. Not that these are not case sensitive:
| Keyboard           | output-key                | 
|:------------------:|:-------------------------:|
|    Enter           |    enter                  |
|    Tab             |    tab                    |
|    Esc             |    esc, escape            |
|    Home            |    home                   |
|    End             |    end                    |
|    Left            |    left                   |
|    Right           |    right                  |
|    Up              |    up                     |
|    Down            |    down                   |
|    Page Up         |    pgup                   |
|    Page Down       |    pgdn                   |
|    Num Lock        |    numlock                |
|    Scroll Lock     |    scrolllock             |
|    Print Screen    |    prtsc                  |
|    Break/Pause     |    break                  |
|    Backspace       |    backspace, bksp, bs    |
|    Clear           |    clear                  |
|    Caps Lock       |    capslock               |
|    Insert          |    ins, insert            |
|    Delete          |    del, delete            |
|    Help            |    help                   |
|    F1              |    F1                     |
|    F2              |    F2                     |
|    F3              |    F3                     |
|    F4              |    F4                     |
|    F5              |    F5                     |
|    F6              |    F6                     |
|    F7              |    F7                     |
|    F8              |    F8                     |
|    F9              |    F9                     |
|    F10             |    F10                    |
|    F11             |    F11                    |
|    F12             |    F12                    |
|    F13             |    F13                    |
|    F14             |    F14                    |
|    F15             |    F15                    |
|    F16             |    F16                    |
|    Numpad 0        |    numpad0                |
|    Numpad 1        |    numpad1                |
|    Numpad 2        |    numpad2                |
|    Numpad 3        |    numpad3                |
|    Numpad 4        |    numpad4                |
|    Numpad 5        |    numpad5                |
|    Numpad 6        |    numpad6                |
|    Numpad 7        |    numpad7                |
|    Numpad 8        |    numpad8                |
|    Numpad 9        |    numpad9                |
|    Multiply        |    multiply, *            |
|    Add             |    add, +                 |
|    Subtract        |    subtract, -            |
|    Divide          |    divide, /              |
