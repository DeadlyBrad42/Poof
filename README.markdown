#Poof
###Tagline

About
=====

Poof is a tool for organizing, uploading, and retrieving images from Imgur. It's useful when maintaining a folder of reaction gifs, or whenever you need to quickly copy and paste a link to an image.

Poof is currently CLI-only, though many more features (including a GUI!) are planned.

Set-Up
=====

Before you use Poof, you will need to set it up. Setting it up is as simple as setting your home directory, scanning it for images, and tagging some of the images.

1. Set your home directory - Setting your home directory is easy, since Poof asks you where you want it the first time it is run. You can always change this later by using the `poof -h` command.
2. Scan the home directory for images using `poof -s`. Poof will automatically add any new files it comes across to the database, and upload them to Imgur.
3. Tag photos using the `poof -t` command. Tags are represented in the database as all lowercase alpha-numeric, and cannot contain spaces.

Usage
=====

Using Poof is as simple as running `poof [list of space-separated tags]`. Based on the tags you specified, poof will return the best matched photo and copy its address to your clipboard. That's it!

Supported Commands
=====

Poof is currently CLI-only. Running it for the first time should prompt you for the directory it will use to store your pictures. Poof currently responds to the following flags:

* `poof -h [directory]` - Sets a new home directory. Omitting a directory will output the current home directory.
* `poof -s` - Rescans the current directory, uploading any pictures it finds that aren't already in the database.
* `poof -l` - Lists all the files in the database, along with their upload address and all the tags associated with each picture.
* `poof -t [filename] [list of space-separated tags]` - Tags the specified picture with the tags supplied

Misc.
=====

As with most GitHub projects, Poof is a continual work in progress. There's currently a lot of features I have in mind, and quite a bit more bugs in the code, but I'll be hacking on it for the near future, so stay tuned.
